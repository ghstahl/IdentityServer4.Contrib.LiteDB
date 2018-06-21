using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer4.LiteDB.Entities;
using IdentityServer4.LiteDB.Interfaces;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IdentityServer4.LiteDB.Tests
{
    [TestClass]
    public class ClientStoreUnitTests
    {
        public Entities.Client GenerateEntityClient()
        {
            var client = new Entities.Client()
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
            return client;
        }

        [TestMethod]
        public async Task Add_Client_Find_Client_Success()
        {
            var hostContainer = new HostContainer();

            var configurationDbContext = hostContainer.ServiceProvider.GetService<IConfigurationDbContext>();

            var entityClient = GenerateEntityClient();
            configurationDbContext.AddClient(entityClient);

            var clientStore = hostContainer.ServiceProvider.GetService<IClientStore>();
            var idsrvClient = await clientStore.FindClientByIdAsync(entityClient.ClientId);

            Assert.IsNotNull(idsrvClient);
            Assert.AreEqual(idsrvClient.ClientId, entityClient.ClientId);

        }
        [TestMethod]
        public async Task Find_Client_Fail()
        {
            var hostContainer = new HostContainer();

            var configurationDbContext = hostContainer.ServiceProvider.GetService<IConfigurationDbContext>();

            var clientStore = hostContainer.ServiceProvider.GetService<IClientStore>();
            var idsrvClient = await clientStore.FindClientByIdAsync(Guid.NewGuid().ToString());

            Assert.IsNull(idsrvClient);

        }
    }
}