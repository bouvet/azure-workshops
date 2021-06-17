using AzureWorkshopFunctionApp.Interfaces;
using AzureWorkshopFunctionApp.Services;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;

[assembly: FunctionsStartup(typeof(AzureWorkshopFunctionApp.Startup))]


namespace AzureWorkshopFunctionApp
{

    public class Startup : FunctionsStartup
    {

        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddScoped<IBlobStorageService, BlobStorageService>((s) => { return new BlobStorageService(Environment.GetEnvironmentVariable("AzureWebJobsStorage")); });
            builder.Services.AddScoped<IImageService, ImageService>();
        }
    }
}
