using AzureWorkshopApp.Models;
using AzureWorkshopApp.Services;
// TODO: Kommenter inn usings
//using Microsoft.AspNetCore.Authentication;
//using Microsoft.AspNetCore.Authentication.AzureAD.UI;
//using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
// TODO: Kommenter inn usings
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Authorization;
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

            // TODO: Kommenter inn cookie policy
            //services.Configure<CookiePolicyOptions>(options =>
            //{
            //    // This lambda determines whether user consent for non-essential cookies is needed for a given request.
            //    options.CheckConsentNeeded = context => true;
            //    options.MinimumSameSitePolicy = SameSiteMode.None;
            //});

            // TODO: Kommenter inn som legger til autentiseserings-service for Azure AD
            //services.AddAuthentication(AzureADDefaults.AuthenticationScheme)
            //    .AddAzureAD(options => Configuration.Bind("AzureAd", options));

            // TODO: Kommenter ut denne.
            services.AddMvc();

            // TODO: Kommenter inn denne som legger til MVC service med policy som krever autentisert bruker
            //services.AddMvc(options =>
            //    {
            //        var policy = new AuthorizationPolicyBuilder()
            //            .RequireAuthenticatedUser()
            //            .Build();
            //        options.Filters.Add(new AuthorizeFilter(policy));
            //    })

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

            // TODO: Kommenter inn som legger til cookie policy og autentisering i http-pipeline
            //app.UseCookiePolicy();
            //app.UseAuthentication();

            app.UseRouting();
            // TODO: Kommenter inn denne for å legge til autorisering
            //app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapDefaultControllerRoute();
            });

        }
    }
}
