using System;
using System.Collections.Generic;
using System.IO;

namespace CityExplorerServer
{
    public class ServerConfig
    {
        private const string ConfigName = "serverConfig.txt";

        private string configPath;
        private Dictionary<string, string> configEntries;

        public string this[string key] => configEntries[key];
        
        public ServerConfig()
        {
            configPath = AppDomain.CurrentDomain.BaseDirectory + ConfigName;
            ReadConfig();
        }

        private void ReadConfig()
        {
            if (!File.Exists(configPath))
                throw new FileNotFoundException($"Config at {configPath} not found!");

            configEntries = new Dictionary<string, string>();
            foreach (string line in File.ReadAllLines(configPath))
            {
                if (line.StartsWith("//") || !line.Contains("="))
                    continue;

                string[] splitted = line.Split('=');
                configEntries.Add(splitted[0], splitted[1]);
            }
        }
    }
}