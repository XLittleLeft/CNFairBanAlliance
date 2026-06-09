using System.ComponentModel;

namespace CNFairBanAlliance
{
    public class Config
    {
        [Description("服务器标识")]
        public string ServerName { get; set; } = "Server";
        [Description("服务器秘钥")]
        public string ServerKey { get; set; } = "DefaultKey";
        [Description("联Ban玩家尝试加入Log输出")]
        public bool Log { get; set; } = true;
        [Description("检查数据更新时间（分钟）")]
        public float CheckInterval { get; set; } = 5;
        [Description("检查玩家IP")]
        public bool CheckPlayerIP { get; set; } = false;
    }
}
