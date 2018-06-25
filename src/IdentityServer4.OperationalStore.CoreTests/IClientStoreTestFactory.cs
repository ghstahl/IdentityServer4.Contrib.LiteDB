using IdentityServer4.Models;
using IdentityServer4.Stores;

namespace IdentityServer4.OperationalStore.CoreTests
{
    public interface IClientStoreTestFactory
    {
        Client CreateClient();
        IClientStore ClientStore { get; }
    }
}
