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

namespace CNFairBanAlliance
{
    public class Plugin
    {
        [PluginEntryPoint("中国公平封禁联盟系统","1.0.1", "中国公平封禁联盟系统(CFBA)","X小左")]
        void OnLoad()
        {
            EventManager.RegisterEvents(this);
        }

        [PluginConfig] public Config Config;

        [PluginEvent]
        void OnPlayerJoined(PlayerJoinedEvent ev)
        {
            var Player = ev.Player;

            if (Player == null) return;

            if (Player.CheckPlayerUserID(out var Reason, out var BannedDate))
            {
                if (Config.Log)
                {
                    Log.Info($"封禁玩家{Player.Nickname}({Player.UserId})尝试加入服务器被处理");
                }
                switch (Config.CheckAction)
                {
                    case CheckAction.Kick:
                        Player.Kick($"\n[中国公平封禁联盟系统(CFBA)] 你的UserID在黑名单里\n你已被踢出由于:{Reason}\n封禁时间:{BannedDate} 如有异议请联系服主或访问网站ban.jiubian.net");
                        break;
                    case CheckAction.Ban:
                        Player.Ban($"\n[中国公平封禁联盟系统(CFBA)] 你的UserID在黑名单里\n你已被封禁由于:{Reason}\n封禁时间:{BannedDate} 如有异议请联系服主或访问网站ban.jiubian.net" , Config.BanTime);
                        break;
                }
            }
            else if (Config.CheckPlayerIP)
            {
                if (Config.Log)
                {
                    Log.Info($"封禁玩家{Player.Nickname}({Player.IpAddress})尝试加入服务器被处理");
                }
                if (Player.CheckPlayerIP(out var _Reason, out var _BannedDate))
                {
                    switch (Config.CheckAction)
                    {
                        case CheckAction.Kick:
                            Player.Kick($"\n[中国公平封禁联盟系统(CFBA)] 你的IP在黑名单里\n你已被封禁由于:{_Reason}\n封禁时间:{_BannedDate} 如有异议请联系服主或访问网站ban.jiubian.net");
                            break;
                        case CheckAction.Ban:
                            Player.Ban($"\n[中国公平封禁联盟系统(CFBA)] 你的IP在黑名单里\n你已被封禁由于:{_Reason}\n封禁时间:{_BannedDate} 如有异议请联系服主或访问网站ban.jiubian.net", Config.BanTime);
                            break;
                    }
                }
            }
        }
    }
}
