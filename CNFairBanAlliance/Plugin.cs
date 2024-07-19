using CNFairBanAlliance.API;
using GameCore;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Events;
using Log = PluginAPI.Core.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MEC;
using CNFairBanAlliance.Helper;
using LiteNetLib;

namespace CNFairBanAlliance
{
    public class Plugin
    {
        [PluginEntryPoint("中国公平封禁联盟系统","1.0.1", "中国公平封禁联盟系统(CFBA)","X小左")]
        void OnLoad()
        {
            EventManager.RegisterEvents(this);
        }

        [PluginConfig] public static Config Config;

        [PluginEvent]
        void OnPlayerPreauth(PlayerPreauthEvent ev)
        {
            var UserID = ev.UserId;
            var IpAddress = ev.IpAddress;
            var ConnectionRequest = ev.ConnectionRequest;

            if (MySQLAPI.CheckPlayerUserID(UserID, out var Reason, out var BannedDate))
            {
                if (Config.Log)
                {
                    Log.Info($"封禁UserID :({UserID})尝试加入服务器被处理");
                }
                CustomLiteNetLib4MirrorTransport.ProcessCancellationData(ConnectionRequest, PreauthCancellationData.Reject($"\n[中国公平封禁联盟系统(CFBA)] 你的UserID在黑名单里\n你已被封禁由于:{Reason}\n封禁时间:{BannedDate} 如有异议请联系服主或访问网站ban.jiubian.net\n如果你并不存在于网站中说明此服主恶意篡改了本地储存的数据", true));
            }
            else if (Config.CheckPlayerIP)
            {
                if (MySQLAPI.CheckPlayerIP(IpAddress, out var _Reason, out var _BannedDate))
                {
                    if (Config.Log)
                    {
                        Log.Info($"封禁IP :({IpAddress})尝试加入服务器被处理");
                    }
                    CustomLiteNetLib4MirrorTransport.ProcessCancellationData(ConnectionRequest, PreauthCancellationData.Reject($"\n[中国公平封禁联盟系统(CFBA)] 你的IP在黑名单里\n你已被封禁由于:{Reason}\n封禁时间:{BannedDate} 如有异议请联系服主或访问网站ban.jiubian.net\n如果你并不存在于网站中说明此服主恶意篡改了本地储存的数据", true));
                }
            }
        }

        [PluginEvent]
        void OnRoundStart(RoundStartEvent ev)
        {
            UpdateHandle = Timing.RunCoroutine(UpdateHelper.UpdateDateBase());
        }

        [PluginEvent]
        void OnRoundEnd(RoundEndEvent ev)
        {
            Timing.KillCoroutines(UpdateHandle);
        }

        private CoroutineHandle UpdateHandle;
    }
}
