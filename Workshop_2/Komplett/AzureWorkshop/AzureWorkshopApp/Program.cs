using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AzureWorkshopApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, builder) =>
                {
                    var config = builder.Build();
                    if (!string.IsNullOrEmpty(config["KeyVaultUri"]))
                    {
                        var secretClient = new SecretClient(new Uri(config["KeyVaultUri"]), new DefaultAzureCredential());
                        builder.AddAzureKeyVault(secretClient, new KeyVaultSecretManager());
                    }
                })
                .UseStartup<Startup>();
    }
}
