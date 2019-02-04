using System;
using System.Collections.Generic;
using System.Linq;

namespace _04_dsa.Modules
{
    public static class Helper
    {
        private static string discordToken;
        private static string heldenToken;
        private static PostgresData postgresData = new PostgresData();

        public static string DiscordToken { get => discordToken; set => discordToken = value; }
        public static string HeldenToken { get => heldenToken; set => heldenToken = value; }
        public static PostgresData PostgresData { get => postgresData; set => postgresData = value; }

        public static void LoadConfig()
        {
            DiscordToken = System.IO.File.ReadLines(@"token.cfg").ElementAtOrDefault(0);
            HeldenToken = System.IO.File.ReadLines(@"token.cfg").ElementAtOrDefault(1);
            PostgresData.Address = System.IO.File.ReadLines(@"token.cfg").ElementAtOrDefault(2);
            PostgresData.Username = System.IO.File.ReadLines(@"token.cfg").ElementAtOrDefault(3);
            PostgresData.Password = System.IO.File.ReadLines(@"token.cfg").ElementAtOrDefault(4);
            PostgresData.Database = System.IO.File.ReadLines(@"token.cfg").ElementAtOrDefault(5);
        }

        public static KeyValuePair<int, int> FormatRoll(string countXboundary)
        {
            int c = - 1, b = - 1;
            countXboundary = countXboundary.ToLower();
            if (countXboundary.Contains('d'))
            {
                c = Convert.ToInt32(countXboundary.Split('d')[0]);
                b = Convert.ToInt32(countXboundary.Split('d')[1]);
                if (c < 1)
                    c *= -1;
                if (b < 1)
                    b *= -1;
            }
            else if (countXboundary.Contains('w'))
            {
                c = Convert.ToInt32(countXboundary.Split('w')[0]);
                b = Convert.ToInt32(countXboundary.Split('w')[1]);
                if (c < 1)
                    c *= -1;
                if (b < 1)
                    b *= -1;
            }
            return new KeyValuePair<int, int>(c, b);
        }
    }

    public class PostgresData
    {
        private string address;
        private string username;
        private string password;
        private string database;

        public string Address { get => address; set => address = value; }
        public string Username { get => username; set => username = value; }
        public string Password { get => password; set => password = value; }
        public string Database { get => database; set => database = value; }
    }
    
}
