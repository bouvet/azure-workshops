using AzureWorkshopFunctionApp.Interfaces;
using AzureWorkshopFunctionApp.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services => {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
                // Register services (same as before, but adapted for isolated process)
        services.AddScoped<IBlobStorageService, BlobStorageService>(_ =>
            new BlobStorageService(Environment.GetEnvironmentVariable("AzureWebJobsStorage")));

        services.AddScoped<IImageService, ImageService>();
    })
    .Build();

host.Run();
