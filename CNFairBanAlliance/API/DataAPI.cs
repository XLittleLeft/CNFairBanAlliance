using System;
using System.IO;
using System.Net;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Newtonsoft.Json;
using LabApi.Features.Console;

namespace CNFairBanAlliance.API
{
    public class BanEntry
    {
        public string Nickname { get; set; }
        public string User_ID { get; set; }
        public string IP { get; set; }
        public string Reason { get; set; }
        public string Time { get; set; }
    }
    public enum BanType
    {
        None,
        UserID,
        IP
    }

    public static class DataAPI
    {
        private const string CryptoSalt = "CFBA_Network_Salt_2026";
        private const string DownloadUrl = "https://gitee.com/XLittleLeft/cfba-public-data/raw/master/banlist.enc";

        private static readonly object _lock = new();
        private static List<BanEntry> _cachedList = [];
        private static HashSet<string> _cachedUserIds = new(StringComparer.OrdinalIgnoreCase);
        private static HashSet<string> _cachedIps = new(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// 获取当前内存中缓存的所有封禁记录拷贝
        /// </summary>
        public static List<BanEntry> CachedList
        {
            get
            {
                lock (_lock) return [.. _cachedList];
            }
        }

        /// <summary>
        /// 异步刷新封禁名单（在后台线程中执行网络下载、异或对冲与自适应解密，绝对不卡死主线程）
        /// </summary>
        /// <param name="serverName">当前服务器的标识</param>
        /// <param name="subKeyHex">分发的64位专属子私钥</param>
        /// <returns>异步任务结果，返回是否成功刷新</returns>
        public static async Task<bool> UpdateCacheAsync(string serverName, string subKeyHex)
        {
            if (string.IsNullOrEmpty(serverName) || string.IsNullOrEmpty(subKeyHex) || subKeyHex.Length != 64)
            {
                Logger.Error("[CFBA] 无效的服务器标识或子私钥，无法更新缓存");
                return false;
            }

            try
            {
                byte[] encryptedData;

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                using (WebClient webClient = new())
                {
                    encryptedData = await Task.Run(() => webClient.DownloadData(DownloadUrl));
                }

                if (encryptedData == null || encryptedData.Length <= 16) return false;

                string jsonResult = await Task.Run(() => InternalDecryptCbc(encryptedData, serverName, subKeyHex));

                if (string.IsNullOrEmpty(jsonResult)) return false;

                var incomingList = JsonConvert.DeserializeObject<List<BanEntry>>(jsonResult);
                incomingList ??= [];

                lock (_lock)
                {
                    _cachedList = incomingList;
                    _cachedUserIds = new HashSet<string>(
                        _cachedList.Select(x => x.User_ID),
                        StringComparer.OrdinalIgnoreCase
                    );
                    _cachedIps = new HashSet<string>(
                        _cachedList.Select(x => x.IP).Where(ip => !string.IsNullOrEmpty(ip)),
                        StringComparer.OrdinalIgnoreCase
                    );
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static string InternalDecryptCbc(byte[] fileBytes, string name, string keyHex)
        {
            byte[] subKeyBytes = HexToBytes(keyHex);
            byte[] saltBytes = Encoding.UTF8.GetBytes(CryptoSalt);
            byte[] nameBytes = Encoding.UTF8.GetBytes(name);
            byte[] masterKey;

            using (HMACSHA256 hmac = new(saltBytes))
            {
                byte[] factorBytes = hmac.ComputeHash(nameBytes);
                masterKey = new byte[32];
                for (int i = 0; i < 32; i++) masterKey[i] = (byte)(subKeyBytes[i] ^ factorBytes[i]);
            }

            try
            {
                byte[] iv = new byte[16];
                byte[] ciphertext = new byte[fileBytes.Length - 16];
                Buffer.BlockCopy(fileBytes, 0, iv, 0, 16);
                Buffer.BlockCopy(fileBytes, 16, ciphertext, 0, ciphertext.Length);

                using (AesManaged aes = new())
                {
                    aes.KeySize = 256;
                    aes.BlockSize = 128;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;
                    aes.Key = masterKey;
                    aes.IV = iv;

                    using (ICryptoTransform decryptor = aes.CreateDecryptor())
                    {
                        using (MemoryStream ms = new(ciphertext))
                        {
                            using (CryptoStream cs = new(ms, decryptor, CryptoStreamMode.Read))
                            {
                                using (StreamReader sr = new(cs, Encoding.UTF8))
                                {
                                    return sr.ReadToEnd();
                                }
                            }
                        }
                    }
                }
            }
            finally
            {
                Array.Clear(masterKey, 0, masterKey.Length);
            }
        }

        private static byte[] HexToBytes(string hex)
        {
            hex = hex.Replace(" ", "").Replace("\n", "").Replace("\r", "").ToLower();
            byte[] bytes = new byte[hex.Length / 2];
            for (int i = 0; i < bytes.Length; i++) bytes[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            return bytes;
        }

        /// <summary>
        /// 扩展方法：直接检查某个 User_ID 字符串是否处于联Ban中
        /// </summary>
        public static bool IsBanned(string userId , string IP , out BanType banType)
        {
            if (string.IsNullOrEmpty(userId))
            {
                banType = BanType.None;
                return false;
            }
            lock (_lock)
            {
                if (_cachedUserIds.Contains(userId))
                {
                    banType = BanType.UserID;
                    return true;
                }
                else if (CustomEvents.Config.CheckPlayerIP && _cachedIps.Contains(IP))
                {
                    banType = BanType.IP;
                    return true;
                }

                banType = BanType.None;
                return false;
            }
        }

        /// <summary>
        /// 扩展方法：直接获取某个 User_ID 被封禁的原因
        /// </summary>
        public static string GetBannedReason(string userId)
        {
            if (string.IsNullOrEmpty(userId)) return "未发现异常状态";
            lock (_lock)
            {
                var entry = _cachedList.FirstOrDefault(x => x.User_ID.Equals(userId, StringComparison.OrdinalIgnoreCase));
                return entry != null ? entry.Reason : "未在 CFBA 联合名单中发现对应封禁原因";
            }
        }

        /// <summary>
        /// 扩展方法：直接获取某个 User_ID 被封禁的时间
        /// </summary>
        public static string GetBannedTime(string userId)
        {
            if (string.IsNullOrEmpty(userId)) return "未发现异常状态";
            lock (_lock)
            {
                var entry = _cachedList.FirstOrDefault(x => x.User_ID.Equals(userId, StringComparison.OrdinalIgnoreCase));
                return entry != null ? entry.Time : "未在 CFBA 联合名单中发现对应封禁时间";
            }
        }

        /// <summary>
        /// 扩展方法：直接反查某个 User_ID 被封禁时的游戏内昵称
        /// </summary>
        public static string GetBannedNickname(this string userId)
        {
            if (string.IsNullOrEmpty(userId)) return "未知玩家";
            lock (_lock)
            {
                var entry = _cachedList.FirstOrDefault(x => x.User_ID.Equals(userId, StringComparison.OrdinalIgnoreCase));
                return entry != null ? entry.Nickname : "未知";
            }
        }
    }
}