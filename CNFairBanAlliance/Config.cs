using CNFairBanAlliance.API;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNFairBanAlliance
{
    public class Config
    {
        [Description("联Ban玩家尝试加入Log输出")]
        public bool Log { get; set; } = true;
        [Description("检查玩家IP")]
        public bool CheckPlayerIP { get; set; } = false;
        [Description("对联Ban玩家进服的操作(Kick或Ban)")]
        public CheckAction CheckAction { get; set; } = CheckAction.Kick;
        [Description("如果选择了Ban，服内封禁时间")]
        public long BanTime { get; set; } = 99999999;
    }
}
