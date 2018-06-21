using System.Collections.Generic;
using FakeItEasy;
using IdentityServer4.LiteDB.Configuration;
using IdentityServer4.LiteDB.DbContexts;
using IdentityServer4.LiteDB.Entities;
using IdentityServer4.LiteDB.Interfaces;
using IdentityServer4.LiteDB.Stores;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Client = IdentityServer4.LiteDB.Entities.Client;

namespace IdentityServer4.LiteDB.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var f = new TempFile();

            var fileName = f.Filename;
            var settings = A.Fake<IOptions<LiteDBConfiguration>>();
            A.CallTo(() => settings.Value).Returns(new LiteDBConfiguration()
            {
                ConnectionString = f.Filename
            });

            var serviceProvider = new ServiceCollection()
                .AddLogging()
                .AddTransient<IConfigurationDbContext, ConfigurationDbContext>()
                .AddSingleton(settings)
                .AddTransient<IClientStore, ClientStore>()
                .BuildServiceProvider();
            var configurationDbContext = serviceProvider.GetService<IConfigurationDbContext>();
            var client = new Client()
            {
                ClientId = "PagesWebAppClient",
                ClientName = "PagesWebAppClient Client",
                AllowedGrantTypes = new List<ClientGrantType>()
                {
                    new ClientGrantType(){GrantType = GrantType.Implicit}
                },
                RequireConsent = true,
                RedirectUris = new List<ClientRedirectUri>()
                {
                    new ClientRedirectUri(){RedirectUri = "https://localhost:44307/signin-oidc-pages-webapp-client"}
                },
                PostLogoutRedirectUris = new List<ClientPostLogoutRedirectUri>()
                {
                    new ClientPostLogoutRedirectUri()
                    {
                        PostLogoutRedirectUri = "https://localhost:44307/Identity/Account/SignoutCallbackOidc"
                    }
                },
                FrontChannelLogoutSessionRequired = true,
                FrontChannelLogoutUri = "https://localhost:44307/Identity/Account/SignoutFrontChannel",
                AllowedScopes = new List<ClientScope>()
                {
                    new ClientScope(){Scope = IdentityServerConstants.StandardScopes.OpenId},
                    new ClientScope(){Scope = IdentityServerConstants.StandardScopes.Profile}
                }
            };

            configurationDbContext.AddClient(client);

            var clientStore = serviceProvider.GetService<IClientStore>();





        }
    }
}
