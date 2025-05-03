using AzureWorkshopFunctionApp.Interfaces;
using AzureWorkshopFunctionApp.Services;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();


builder.Services.
    AddScoped<IBlobStorageService, BlobStorageService>(_ =>
                new BlobStorageService(Environment.GetEnvironmentVariable("AzureWebJobsStorage"))).
    AddScoped<IImageService, ImageService>();


var host = builder.Build();

host.Run();