using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.LiteDB.Interfaces;
using IdentityServer4.LiteDB.Mappers;
using IdentityServer4.OperationalStore.CoreTests;
using IdentityServer4.Stores;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IdentityServer4.LiteDB.Tests.Stores
{
    [TestClass]
    public class ResourceStoreTests: IdentityServer4.OperationalStore.CoreTests.Stores.ResourceStoreTests
    {
        public ResourceStoreTests() :
            base(
                HostContainer.ServiceProvider.GetService<IOperationalTestContext>(),
                HostContainer.ServiceProvider.GetService<IResourceStore>())
        {
        }
    }
}