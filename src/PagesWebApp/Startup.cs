using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.LiteDB.Interfaces;
using IdentityServer4.LiteDB.Mappers;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PagesWebApp.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PagesWebApp.Services;

namespace PagesWebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            HostingEnvironment = hostingEnvironment;
            /*
             "MongoDB": {
                "ConnectionString": "mongodb://localhost:27017",
                "Database": "IS4"
              },

             */
            var liteDBPath = Path.Combine(HostingEnvironment.ContentRootPath, @"LiteDB");
            if (!Directory.Exists(liteDBPath))
            {
                Directory.CreateDirectory(liteDBPath);
            }

            Configuration["liteDb:ConnectionString"] = Path.Combine(liteDBPath, "IdentityServer4.liteDB");
        }

        public IHostingEnvironment HostingEnvironment { get; set; }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
           
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddTransient<IEmailSender, EmailSender>();

            // configure identity server with in-memory stores, keys, clients and scopes
            services.AddIdentityServer(options =>
                {
                    options.UserInteraction.LoginUrl = "/identity/account/login";
                    options.UserInteraction.LogoutUrl = "/identity/account/logout";
                    options.UserInteraction.ConsentUrl = "/identity/consent";
                    options.Authentication.CheckSessionCookieName = $".idsrv.session.{Configuration["appName"]}";
                })
                .AddDeveloperSigningCredential()
                .AddConfigurationStore(setupAction =>
                {
                    setupAction.ConnectionString = Configuration["liteDb:ConnectionString"];
                })
                .AddOperationalStore(setupAction =>
                {
                    setupAction.ConnectionString = Configuration["liteDb:ConnectionString"];
                })
                .AddAspNetIdentity<IdentityUser>();

            var authenticationBuilder = services.AddAuthentication();
            var googleClientId = Configuration["Google-ClientId"];
            var googleClientSecret = Configuration["Google-ClientSecret"];
            if (!string.IsNullOrEmpty(googleClientId) && !string.IsNullOrEmpty(googleClientSecret))
            {
                authenticationBuilder.AddGoogle(options =>
                {
                    options.ClientId = googleClientId;
                    options.ClientSecret = googleClientSecret;
                    options.Events.OnRemoteFailure = context =>
                    {
                        context.Response.Redirect("/");
                        context.HandleResponse();
                        return Task.CompletedTask;
                    };
                });
            }
                

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {

            // Setup Databases
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                EnsureSeedData(serviceScope.ServiceProvider.GetService<IConfigurationDbContext>());
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            // app.UseAuthentication(); // not needed, since UseIdentityServer adds the authentication middleware
            app.UseIdentityServer();

            app.UseMvc();
        }
        private static void EnsureSeedData(IConfigurationDbContext context)
        {
            if (!context.Clients.Any())
            {
                foreach (var client in Config.GetClients().ToList())
                {
                    context.AddClient(client.ToEntity());
                }
            }

            if (!context.IdentityResources.Any())
            {
                foreach (var resource in Config.GetIdentityResources().ToList())
                {
                    context.AddIdentityResource(resource.ToEntity());
                }
            }

            if (!context.ApiResources.Any())
            {
                foreach (var resource in Config.GetApiResources().ToList())
                {
                    context.AddApiResource(resource.ToEntity());
                }
            }
        }
    }
}

