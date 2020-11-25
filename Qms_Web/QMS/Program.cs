using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace QMS
{
    // public class Program
    // {
    //     public static void Main(string[] args)
    //     {
    //         CreateWebHostBuilder(args).Build().Run();
    //     }

    //     public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
    //         WebHost.CreateDefaultBuilder(args)
    //             .UseStartup<Startup>();
    // }

    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("[Program][Main] =>");
            Console.WriteLine("----------------------------------------------------------------");
            Console.WriteLine("APPSETTINGS_DIRECTORY:");
            Console.WriteLine(Environment.GetEnvironmentVariable("APPSETTINGS_DIRECTORY"));
            Console.WriteLine("----------------------------------------------------------------");
            Console.WriteLine("<= [Program][Main]");
            
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                config.SetBasePath(Environment.GetEnvironmentVariable("APPSETTINGS_DIRECTORY"));
                config.AddJsonFile("qms_appsettings.json", optional: false, reloadOnChange: true);
            })
            .UseStartup<Startup>();
    }
    
}
