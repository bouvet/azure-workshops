using AzureWorkshopFunctionApp.Interfaces;
using AzureWorkshopFunctionApp.Services;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

[assembly: FunctionsStartup(typeof(AzureWorkshopFunctionApp.Startup))]


namespace AzureWorkshopFunctionApp
{

    public class Startup : FunctionsStartup
    {

        public IConfiguration Configuration { get; set; }
        public override void Configure(IFunctionsHostBuilder builder)
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("local.settings.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            builder.Services.AddScoped<IBlobStorageService, BlobStorageService>((s) => { return new BlobStorageService(Configuration.GetConnectionString("AzureWebJobsStorage")); });
            builder.Services.AddScoped<IImageService, ImageService>();
        }
    }
}
