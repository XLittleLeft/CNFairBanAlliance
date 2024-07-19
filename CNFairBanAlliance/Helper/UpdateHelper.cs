using CNFairBanAlliance.API;
using MEC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNFairBanAlliance.Helper
{
    public static class UpdateHelper
    {
        public static IEnumerator<float> UpdateDateBase()
        {
            MySQLAPI.CheckDatabaseUpdates();
            yield return Timing.WaitForSeconds(Plugin.Config.CheckInterval * 60);
        }
    }
}
