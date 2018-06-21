<meta name='keywords' content='IdentityServer4, LiteDB'>
[![Build status](https://ci.appveyor.com/api/projects/status/04tdsxp1k964qhgv?svg=true)](https://ci.appveyor.com/project/ghstahl/identityserver4-contrib-litedb)
# IdentityServer4.Contrib.LiteDB
## Reference
[IdentityServer4.Contrib.MongoDB](https://github.com/diogodamiani/IdentityServer4.Contrib.MongoDB)  
This project is about 99% the same as the MongoDB variant.  LiteDB is MongoDB friendly, so it was basically a search and replace "MongoDB" => "LiteDB"  


## Usage  
[Startup](src/PagesWebApp/Startup.cs)  

```
public Startup(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
{
    Configuration = configuration;
    HostingEnvironment = hostingEnvironment;
   
    var liteDBPath = Path.Combine(HostingEnvironment.ContentRootPath, @"LiteDB");
    if (!Directory.Exists(liteDBPath))
    {
        Directory.CreateDirectory(liteDBPath);
    }

    Configuration["liteDb:ConnectionString"] = Path.Combine(liteDBPath, "IdentityServer4.liteDB");
}
```

```
public void ConfigureServices(IServiceCollection services)
{
...
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
  
  ...
}
```
```
public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{
    // Setup Databases
    using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
    {
        EnsureSeedData(serviceScope.ServiceProvider.GetService<IConfigurationDbContext>());
    }
    ...
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
```
