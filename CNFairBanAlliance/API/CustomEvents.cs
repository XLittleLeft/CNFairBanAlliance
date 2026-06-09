using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.CustomHandlers;
using System;

using Log = LabApi.Features.Console.Logger;

namespace CNFairBanAlliance.API
{
    public class CustomEvents : CustomEventsHandler
    {
        public static Config Config;

        public override void OnPlayerPreAuthenticating(PlayerPreAuthenticatingEventArgs ev)
        {
            var UserID = ev.UserId;
            var IpAddress = ev.IpAddress;

            try
            {
                if (DataAPI.IsBanned(UserID, IpAddress, out BanType banType))
                {
                    string reason = DataAPI.GetBannedReason(UserID);
                    string date = DataAPI.GetBannedTime(UserID);
                    if (Config.Log)
                    {
                        Log.Info($"封禁UserID :({UserID})尝试加入服务器被处理");
                    }
                    if (banType == BanType.UserID)
                    {
                        ev.RejectCustom($"\n[中国公平封禁联盟系统(CFBA)] 你的UserID在黑名单里\n你已被踢出由于:{reason}\n封禁时间:{date} 如有异议请联系服主");
                    }
                    else if (banType == BanType.IP)
                    {
                        ev.RejectCustom($"\n[中国公平封禁联盟系统(CFBA)] 你的IP在黑名单里\n你已被踢出由于:{reason}\n封禁时间:{date} 如有异议请联系服主");
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                Log.Error("检查玩家出错，请检查本地数据文件中是否有异常");
            }
        }
    }
}
