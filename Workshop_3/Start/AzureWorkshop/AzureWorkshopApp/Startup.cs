using AzureWorkshopApp.Models;
using AzureWorkshopApp.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
// TODO: Kommenter inn usings
// using Microsoft.Identity.Web;
// using Microsoft.Identity.Web.UI;
// using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AzureWorkshopApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            //TODO:Kommenter inn linja som legger til autentisering med Azure AD
            // services.AddMicrosoftIdentityWebAppAuthentication(Configuration, "AzureAd");

            // TODO: Kommenter ut denne.
            services.AddMvc();

            // TODO: Kommenter inn denne som legger til MVC service med policy som krever autentisert bruker
            // og UIfor Microsoft Identity
            // services.AddMvc(options =>
            //    {
            //        var policy = new AuthorizationPolicyBuilder()
            //            .RequireAuthenticatedUser()
            //            .Build();
            //        options.Filters.Add(new AuthorizeFilter(policy));
            //    }).AddMicrosoftIdentityUI();

            services.AddOptions();

            services.AddScoped<IStorageService, StorageService>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.Configure<AzureStorageConfig>(Configuration.GetSection("AzureStorageConfig"));
            services.AddApplicationInsightsTelemetry(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            // TODO: Kommenter inn som legger til autentisering i http-pipeline
            // app.UseAuthentication();

            app.UseRouting();
            // TODO: Kommenter inn denne for å legge til autorisering
            // app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapDefaultControllerRoute();
            });

        }
    }
}
