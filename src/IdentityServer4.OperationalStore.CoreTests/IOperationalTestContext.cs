using System;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;

namespace IdentityServer4.OperationalStore.CoreTests
{
    public interface IOperationalTestContext : IDisposable
    {
        IQueryable<Client> Clients { get; }
        IQueryable<IdentityResource> IdentityResources { get; }
        IQueryable<ApiResource> ApiResources { get; }

        Task AddClientAsync(Client client);

        Task AddIdentityResourceAsync(IdentityResource resource);

        Task AddApiResourceAsync(ApiResource resource);
    }
}