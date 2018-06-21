using System.Linq;
using AutoMapper;
using IdentityServer4.LiteDB.Mappers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IdentityServer4.LiteDB.Tests.Mappers
{
    [TestClass]
    public class ClientMapperProfileTests
    {
        [TestMethod]
        public void Map()
        {
            var model = new Models.Client();
            var entity = ClientMappers.ToEntity(model);
            model = ClientMappers.ToModel(entity);

            // Assert
            Assert.IsNotNull(entity);
            Assert.IsNotNull(model);
        }
        [TestMethod]
        public void Map_Properties()
        {
            var model = new Models.Client
            {
                Properties =
                {
                    {"foo1", "bar1"},
                    {"foo2", "bar2"}
                }
            };

            // Act
            var mappedEntity = ClientMappers.ToEntity(model);
            var mappedModel = ClientMappers.ToModel(mappedEntity);


            // Assert
            Assert.IsNotNull(mappedEntity);
            Assert.AreEqual(2, mappedEntity.Properties.Count);
            var foo1 = mappedEntity.Properties.FirstOrDefault(x => x.Key == "foo1");
            Assert.IsNotNull(foo1);
            Assert.AreEqual("bar1", foo1.Value);
            var foo2 = mappedEntity.Properties.FirstOrDefault(x => x.Key == "foo2");
            Assert.IsNotNull(foo2);
            Assert.AreEqual("bar2", foo2.Value);

            Assert.IsNotNull(mappedModel);
            Assert.AreEqual(2, mappedModel.Properties.Count);
            Assert.IsTrue(mappedModel.Properties.ContainsKey("foo1"));
            Assert.IsTrue(mappedModel.Properties.ContainsKey("foo2"));
            Assert.AreEqual("bar1", mappedModel.Properties["foo1"]);
            Assert.AreEqual("bar2", mappedModel.Properties["foo2"]);
        }
        [TestMethod]
        public void Map_Duplicated_Properties_ThrowsException()
        {
            var entity = new Entities.Client
            {
                Properties = new System.Collections.Generic.List<Entities.ClientProperty>
                {
                    new Entities.ClientProperty {Key = "foo1", Value = "bar1"},
                    new Entities.ClientProperty {Key = "foo1", Value = "bar2"},
                }
            };

            // Act & Assert
            Assert.ThrowsException<AutoMapperMappingException>(() => ClientMappers.ToModel(entity));
        }
    }
}