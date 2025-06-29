using CNFairBanAlliance.API;
using LabApi.Events.CustomHandlers;
using LabApi.Features;
using LabApi.Loader;
using LabApi.Loader.Features.Plugins;
using System.Timers;

namespace CNFairBanAlliance
{
    public class Plugins : Plugin
    {
        Timer timer = new();

        public CustomEventHandler Events { get; } = new();

        public override void LoadConfigs()
        {
            base.LoadConfigs();

            CustomEventHandler.Config = this.LoadConfig<Config>("config.yml");
        }

        public override string Name => "中国公平封禁联盟系统";

        public override string Description => "中国公平封禁联盟系统(CFBA)";

        public override string Author => "X小左";

        public override System.Version Version => new(1, 0, 5);

        public override System.Version RequiredApiVersion => new(LabApiProperties.CompiledVersion);

        public override void Enable()
        {
            CustomHandlersManager.RegisterEventsHandler(Events);

            MySQLAPI.SaveDatabaseToTxtFile();
            timer.Interval = CustomEventHandler.Config.CheckInterval * 60000;
            timer.Elapsed += MySQLAPI.CheckDatabaseUpdates;
            timer.Start();
        }

        public override void Disable()
        {
            CustomHandlersManager.UnregisterEventsHandler(Events);

            timer.Stop();
            timer.Close();
        }
    }
}
