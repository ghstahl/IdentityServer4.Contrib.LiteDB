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
}