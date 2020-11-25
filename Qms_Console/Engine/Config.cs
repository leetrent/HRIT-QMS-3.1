using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace QmsCore.Engine
{
 /// <summary>
    /// The config class provides a wrapper for the appsettings.json file. It's done as a singletone so that it can be statically called
    /// with no need to instantiate anyting
    /// 
    /// Example:  string dir = Config.Settings.BaseDirectory;
    /// </summary>
    public sealed class Config
    {
        private static readonly Config settings = new Config();

        public static Config Settings
        {
            get{return settings;}
            
        }
        private static IConfiguration AppSettings {get;set;}
        /// <summary>
        /// The base directory is where the OLU files will be picked up from to begin the loadig process.
        /// </summary>
        public string CSVDirectory = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string LogDirectory = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public string ArchiveDirectory = string.Empty;
        public string JsonDirectory = string.Empty;
        public string DllDirectory = string.Empty;

        public string ConnectionString = string.Empty;
        static Config()
        {}

        private Config()
        {
            string env = Program.Environment;
            var builder = new ConfigurationBuilder()
                             .SetBasePath(Environment.GetEnvironmentVariable("APPSETTINGS_DIRECTORY"))
                             .AddJsonFile("qms_appsettings." + env + ".json", optional: false, reloadOnChange: true);

            AppSettings = builder.Build();
            CSVDirectory = AppSettings.GetSection("AppSettings")["CSVDirectory"];
            JsonDirectory = AppSettings.GetSection("AppSettings")["JsonDirectory"];
            LogDirectory = AppSettings.GetSection("AppSettings")["LogDirectory"];
            ArchiveDirectory = AppSettings.GetSection("AppSettings")["ArchiveDirectory"];
            DllDirectory = AppSettings.GetSection("AppSettings")["DllDirectory"];
            ConnectionString = AppSettings.GetValue<string>("DatabaseConnection");

            QmsCore.Model.Config.Settings.Rebuild("qms_appsettings." + env + ".json");

        }
    }//end class    
}