using GameCore;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.CustomHandlers;
using System;

using Log = LabApi.Features.Console.Logger;

namespace CNFairBanAlliance.API
{
    public class CustomEventHandler : CustomEventsHandler
    {
        public static Config Config;

        public override void OnPlayerPreAuthenticating(PlayerPreAuthenticatingEventArgs ev)
        {
            var UserID = ev.UserId;
            var IpAddress = ev.IpAddress;

            try
            {
                if (MySQLAPI.CheckPlayerUserID(UserID, out var Reason, out var BannedDate))
                {
                    if (Config.Log)
                    {
                        Log.Info($"封禁UserID :({UserID})尝试加入服务器被处理");
                    }
                    ev.RejectCustom($"\n[中国公平封禁联盟系统(CFBA)] 你的UserID在黑名单里\n你已被踢出由于:{Reason}\n封禁时间:{BannedDate} 如有异议请联系服主或访问网站www.cnfba.top\n如果你并不存在于网站中说明此服主恶意篡改了本地储存的数据");
                }
                else if (Config.CheckPlayerIP)
                {
                    if (MySQLAPI.CheckPlayerIP(IpAddress, out var _Reason, out var _BannedDate))
                    {
                        if (Config.Log)
                        {
                            Log.Info($"封禁IP :({IpAddress})尝试加入服务器被处理");
                        }
                        ev.RejectCustom($"\n[中国公平封禁联盟系统(CFBA)] 你的IP在黑名单里\n你已被踢出由于:{_Reason}\n封禁时间:{_BannedDate} 如有异议请联系服主或访问网站www.cnfba.top\n如果你并不存在于网站中说明此服主恶意篡改了本地储存的数据");
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
