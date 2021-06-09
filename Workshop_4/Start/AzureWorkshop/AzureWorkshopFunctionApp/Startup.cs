using AzureWorkshopFunctionApp.Interfaces;
using AzureWorkshopFunctionApp.Services;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(AzureWorkshopFunctionApp.Startup))]


namespace AzureWorkshopFunctionApp
{

    public class Startup : FunctionsStartup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddScoped<IBlobStorageService>((s) => 
            { 
                return new BlobStorageService(Configuration.GetConnectionString("StorageAccountConnectionString")); 
            });
        }
    }
}
