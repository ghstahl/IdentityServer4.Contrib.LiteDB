using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace IdentityServer4.OperationalStore.CoreTests.Stores
{
    public abstract class ClientStoreUnitTests
    {
      
        private IOperationalTestContext _operationalTestContext;
        private IClientStore _clientStore;

        public ClientStoreUnitTests(
            IOperationalTestContext operationalTestContext,
            IClientStore clientStore)
        {
            _operationalTestContext = operationalTestContext;
            _clientStore = clientStore;
            
        }

        private Client UniqueClient
        {
            get
            {
                var client = new Client()
                {
                    ClientId = Unique.S,
                    ClientName = Unique.S,
                    AllowedGrantTypes = new List<string>()
                    {
                        Unique.S,
                        Unique.S
                    },
                    RequireConsent = true,
                    RedirectUris = new List<string>()
                    {
                        Unique.S,
                        Unique.S
                    },
                    PostLogoutRedirectUris = new List<string>()
                    {
                        Unique.S,
                        Unique.S
                    },
                    FrontChannelLogoutSessionRequired = true,
                    FrontChannelLogoutUri = Unique.Url,
                    AllowedScopes = new List<string>()
                    {
                        Unique.S,
                        Unique.S
                    }
                };
                return client;
            }
        }

        [TestMethod]
        public async Task DI_Valid()
        {
            _operationalTestContext.ShouldNotBeNull();
            _clientStore.ShouldNotBeNull();
        }

        [TestMethod]
        public async Task Add_Client_Find_Client_Success()
        {
            var client = UniqueClient;
            await _operationalTestContext.AddClientAsync(client);

            var idsrvClient = await _clientStore.FindClientByIdAsync(client.ClientId);
            idsrvClient.ShouldNotBeNull();
            idsrvClient.ClientId.ShouldMatch(client.ClientId);
        }

        [TestMethod]
        public async Task Find_Client_Fail()
        {
            var idsrvClient = await _clientStore.FindClientByIdAsync(Unique.S);
            idsrvClient.ShouldBeNull();
        }
    }
}