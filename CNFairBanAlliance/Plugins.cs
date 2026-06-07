using CNFairBanAlliance.API;
using LabApi.Events.CustomHandlers;
using LabApi.Features;
using LabApi.Features.Console;
using LabApi.Loader;
using LabApi.Loader.Features.Plugins;
using Org.BouncyCastle.Tls;
using System.Threading.Tasks;
using System.Timers;
using static Mono.Security.X509.X520;

namespace CNFairBanAlliance
{
    public class Plugins : Plugin
    {
        Timer Timer = new();

        public CustomEvents Events { get; } = new();

        public override void LoadConfigs()
        {
            CustomEvents.Config = this.LoadConfig<Config>("config.yml");
            base.LoadConfigs();
        }

        public override string Name => "中国公平封禁联盟系统";

        public override string Description => "中国公平封禁联盟系统(CFBA)";

        public override string Author => "X小左";

        public override System.Version Version => new(2, 0, 0);

        public override System.Version RequiredApiVersion => new(LabApiProperties.CompiledVersion);

        public override void Enable()
        {
            CustomHandlersManager.RegisterEventsHandler(Events);

            Logger.Info("正在后台热更新联Ban名单...");

            TriggerCacheUpdate();

            Timer.Interval = CustomEvents.Config.CheckInterval * 60000;
            Timer.Elapsed += (sender, e) => TriggerCacheUpdate();
            Timer.AutoReset = true;
            Timer.Start();
        }

        public override void Disable()
        {
            CustomHandlersManager.UnregisterEventsHandler(Events);

            if (Timer != null)
            {
                Timer.Stop();
                Timer.Dispose();
                Timer = null;
            }
        }

        private void TriggerCacheUpdate()
        {
            Task.Run(async () =>
            {
                bool success = await DataAPI.UpdateCacheAsync(CustomEvents.Config.ServerName, CustomEvents.Config.ServerKey);

                if (success)
                {
                    Logger.Info($"[+] 联Ban名单定时同步完成！当前总人数 {DataAPI.CachedList.Count}");
                }
                else
                {
                    Logger.Warn("[CFBA] 联Ban名单定时同步失败，将等待下一次轮询。");
                }
            });
        }
    }
}
