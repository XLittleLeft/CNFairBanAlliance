using MySql.Data.MySqlClient;
using PluginAPI.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNFairBanAlliance.API
{
    public static class MySQLAPI
    {
        public static string connectstring = "server=103.40.13.87;port=33066;user=Plugin;password=FairnessandJustice;database=playerlist";

        public static bool CheckPlayerUserID(this Player Player , out string Reason , out string BannedDate)
        {
            using (MySqlConnection connection = new MySqlConnection(connectstring))
            {
                connection.Open();
                string commandstring = $"CALL CheckUserID('{Player.UserId}')";
                using (MySqlCommand command = new MySqlCommand(commandstring, connection))
                {
                    using (MySqlDataReader dr = command.ExecuteReader())
                    {
                        string _reason = string.Empty;
                        string _bannedDate = string.Empty;
                        while (dr.Read())
                        {
                            _reason = dr.GetString(0);
                            _bannedDate = dr.GetDateTime(1).ToString();
                        }
                        Reason = _reason;
                        BannedDate = _bannedDate;
                        if (dr.HasRows) return true;
                    }
                }
            }
            return false;
        }
        public static bool CheckPlayerIP(this Player Player, out string Reason, out string BannedDate)
        {
            using (MySqlConnection connection = new MySqlConnection(connectstring))
            {
                connection.Open();
                string commandstring = $"CALL CheckIP('{Player.IpAddress}')";
                using (MySqlCommand command = new MySqlCommand(commandstring, connection))
                {
                    using (MySqlDataReader dr = command.ExecuteReader())
                    {
                        string _reason = string.Empty;
                        string _bannedDate = string.Empty;
                        while (dr.Read())
                        {
                            _reason = dr.GetString(0);
                            _bannedDate = dr.GetDateTime(1).ToString();
                        }
                        Reason = _reason;
                        BannedDate = _bannedDate;
                        if (dr.HasRows) return true;
                    }
                }
            }
            return false;
        }
    }
}
