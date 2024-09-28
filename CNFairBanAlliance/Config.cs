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
        [Description("本地列表缓存路径")]
        public string Path { get; set; } = $"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\\CFBA.txt";
        [Description("检查数据更新时间（分钟）")]
        public float CheckInterval { get; set; } = 5;
        [Description("检查玩家IP")]
        public bool CheckPlayerIP { get; set; } = false;
    }
}
