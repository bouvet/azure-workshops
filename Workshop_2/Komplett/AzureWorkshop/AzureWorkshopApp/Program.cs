using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace AzureWorkshopApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Run();
        }

        public static IWebHost CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseApplicationInsights().Build();
    }
}
