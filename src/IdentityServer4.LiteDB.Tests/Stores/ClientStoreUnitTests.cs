using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer4.LiteDB.Entities;
using IdentityServer4.LiteDB.Interfaces;
using IdentityServer4.Models;
using IdentityServer4.OperationalStore.CoreTests;
using IdentityServer4.Stores;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IdentityServer4.LiteDB.Tests.Stores
{

    [TestClass]
    public class ClientStoreUnitTests: IdentityServer4.OperationalStore.CoreTests.Stores.ClientStoreUnitTests
    {
        public ClientStoreUnitTests() :
            base(
                HostContainer.ServiceProvider.GetService<IOperationalTestContext>(),
                HostContainer.ServiceProvider.GetService<IClientStore>())
        {
        }
    }
}