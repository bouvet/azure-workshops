using AzureWorkshopFunctionApp.Interfaces;
using AzureWorkshopFunctionApp.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()  // Required for Azure Functions
    .ConfigureServices(services =>
    {
        // Register services (same as before, but adapted for isolated process)
        services.AddScoped<IBlobStorageService, BlobStorageService>(_ =>
            new BlobStorageService(Environment.GetEnvironmentVariable("AzureWebJobsStorage")));

        services.AddScoped<IImageService, ImageService>();
    })
    .Build();

await host.RunAsync();  // Run the host