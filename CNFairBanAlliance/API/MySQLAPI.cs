using GameCore;
using MySql.Data.MySqlClient;
using System;
using System.IO;
using System.Timers;

using Log = LabApi.Features.Console.Logger;

namespace CNFairBanAlliance.API
{
    public static class MySQLAPI
    {
        public static DateTime LastUpdateTime = DateTime.Now;
        public static string FilePath = CustomEventHandler.Config.Path;
        public static string connectionString = "server=sql.cnfba.top;port=33066;user=Plugin;password=FairnessandJustice;database=playerlist;SslMode=None";

        public static bool CheckPlayerUserID(string UserID, out string Reason, out string BannedDate)
        {
            using (StreamReader sr = new(FilePath))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    string[] databases = line.Split([';']);

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
            using (StreamReader sr = new(FilePath))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    string[] databases = line.Split([';']);

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
        public static void SaveDatabaseToTxtFile()
        {
            LastUpdateTime = DateTime.Now;

            using MySqlConnection connection = new(connectionString);
            connection.Open();
            string commandstring = "CALL GetDataBase()";
            using MySqlCommand command = new(commandstring, connection);
            using MySqlDataReader dr = command.ExecuteReader();
            using StreamWriter sw = File.CreateText(FilePath);
            while (dr.Read())
            {
                sw.WriteLine($"{dr.GetString(0)};{dr.GetString(1)};{dr.GetString(2)};{dr.GetString(3)};{dr.GetDateTime(4).ToString()}");
            }
        }



        public static void CheckDatabaseUpdates(object sender, ElapsedEventArgs e)
        {
            using MySqlConnection connection = new(connectionString);
            connection.Open();
            string commandstring = "CALL GetLastUpdateTime()";
            using MySqlCommand command = new(commandstring, connection);
            using MySqlDataReader dr = command.ExecuteReader();
            while (dr.Read())
            {
                if (dr.GetDateTime(0) > LastUpdateTime)
                {
                    SaveDatabaseToTxtFile();
                }
            }
        }
    }
}
