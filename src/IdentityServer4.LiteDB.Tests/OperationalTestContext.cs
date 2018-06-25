using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.LiteDB.Interfaces;
using IdentityServer4.LiteDB.Mappers;
using IdentityServer4.Models;
using IdentityServer4.OperationalStore.CoreTests;

namespace IdentityServer4.LiteDB.Tests
{
    public class OperationalTestContext : IOperationalTestContext
    {
        private IConfigurationDbContext _configurationDbContext;

        public OperationalTestContext(IConfigurationDbContext configurationDbContext)
        {
            _configurationDbContext = configurationDbContext;
        }
        public void Dispose()
        {
        }

        public IQueryable<Client> Clients
        {
            get
            {
                var query = from item in _configurationDbContext.Clients
                    select item.ToModel();
                return query;
            }
        }
        public IQueryable<IdentityResource> IdentityResources
        {
            get
            {
                var query = from item in _configurationDbContext.IdentityResources
                    select item.ToModel();
                return query;
            }
        }

        public IQueryable<ApiResource> ApiResources
        {
            get
            {
                var query = from item in _configurationDbContext.ApiResources
                    select item.ToModel();
                return query;
            }
        }

        public async Task AddClientAsync(Client client)
        {
            await _configurationDbContext.AddClient(client.ToEntity());
        }

        public async Task AddIdentityResourceAsync(IdentityResource resource)
        {
            await _configurationDbContext.AddIdentityResource(resource.ToEntity());
        }

        public async Task AddApiResourceAsync(ApiResource resource)
        {
            await _configurationDbContext.AddApiResource(resource.ToEntity());
        }
    }
}