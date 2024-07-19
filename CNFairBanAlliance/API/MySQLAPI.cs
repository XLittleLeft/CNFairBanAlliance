using MySql.Data.MySqlClient;
using PluginAPI.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace CNFairBanAlliance.API
{
    public static class MySQLAPI
    {
        public static DateTime LastUpdateTime = DateTime.Now;
        public static string FilePath = Plugin.Config.Path + "CFBA.txt";
        public static string connectstring = "server=103.40.13.87;port=33066;user=Plugin;password=FairnessandJustice;database=playerlist";
        //看到这并且兴奋的人，你好，我不会傻到那样，这个Plugin账户只有小范围读取权限和执行存储过程权限，什么也干不了，你一点数据都修改不了
        public static bool CheckPlayerUserID(string UserID , out string Reason , out string BannedDate)
        {
            using (StreamReader sr = new StreamReader(FilePath))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    string[] databases = line.Split(new char[] {';'});

                    string CurUserID = databases[0];
                    Reason = databases[3];
                    BannedDate = databases[4];

                    if (CurUserID == UserID)
                        return true;
                }
            }
            Reason = string.Empty;
            BannedDate = string.Empty;
            return false;
        }
        public static bool CheckPlayerIP(string IP, out string Reason, out string BannedDate)
        {
            using (StreamReader sr = new StreamReader(FilePath))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    string[] databases = line.Split(new char[] { ';' });

                    string CurIP = databases[1];
                    Reason = databases[3];
                    BannedDate = databases[4];

                    if (CurIP == IP)
                        return true;
                }
            }
            Reason = string.Empty;
            BannedDate = string.Empty;
            return false;
        }
        public static void SaveDataBaseToTxtFile()
        {
            LastUpdateTime = DateTime.Now;
            HashSet<string> list = new HashSet<string>();
            using (MySqlConnection connection = new MySqlConnection(connectstring))
            {
                connection.Open();
                string commandstring = "CALL GetDataBase()";
                using (MySqlCommand command = new MySqlCommand(commandstring, connection))
                {
                    using (MySqlDataReader dr = command.ExecuteReader())
                    {
                        using (StreamWriter sw = File.CreateText(FilePath))
                        {
                            while (dr.Read())
                            {
                                sw.WriteLine($"{dr.GetString(0)};{dr.GetString(1)};{dr.GetString(2)};{dr.GetString(3)};{dr.GetDateTime(4).ToString()}");
                            }
                        }
                    }
                }
            }
        }

        public static void CheckDatabaseUpdates(object sender, ElapsedEventArgs e)
        {
            using (MySqlConnection connection = new MySqlConnection(connectstring))
            {
                connection.Open();
                string commandstring = "CALL GetLastUpdateTime()";
                using (MySqlCommand command = new MySqlCommand(commandstring, connection))
                {
                    using (MySqlDataReader dr = command.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            if (dr.GetDateTime(0) > LastUpdateTime)
                            {
                                SaveDataBaseToTxtFile();
                            }
                        }
                    }
                }
            }
        }
    }
}
