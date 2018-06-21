using IdentityServer4.LiteDB.Mappers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IdentityServer4.LiteDB.Tests.Mappers
{
    [TestClass]
    public class IdentityResourceMapperProfileTests
    {
        [TestMethod]
        public void Map()
        {
            var model = new Models.IdentityResource();
            var entity = IdentityResourceMappers.ToEntity(model);
            model = IdentityResourceMappers.ToModel(entity);

            // Assert
            Assert.IsNotNull(entity);
            Assert.IsNotNull(model);
        }
    }
    [TestClass]
    public class PersistedGrantMapperProfileTests
    {
        [TestMethod]
        public void Map()
        {
            var model = new Models.PersistedGrant();
            var entity = PersistedGrantMappers.ToEntity(model);
            model = PersistedGrantMappers.ToModel(entity);

            // Assert
            Assert.IsNotNull(entity);
            Assert.IsNotNull(model);
        }
    }
}