using System;
using Azure.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.Hosting;

namespace AppConfigurationDemo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) => 
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                    webBuilder.ConfigureAppConfiguration((hostingContext, config) =>
                    {
                        var settings = config.Build();
                        var credentials = new DefaultAzureCredential(
                            new DefaultAzureCredentialOptions
                            {
                                ExcludeVisualStudioCredential = true
                            });
                        config.AddAzureAppConfiguration(options =>
                        {
                            options.Connect(new Uri(settings["AppConfig:Endpoint"]), credentials)
                                .ConfigureKeyVault(kv =>
                                {
                                    kv.SetCredential(credentials);
                                })
                                .Select(KeyFilter.Any, "dev");
                        });
                    }).UseStartup<Startup>());
    }
}
