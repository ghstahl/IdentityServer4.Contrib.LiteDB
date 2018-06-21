using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using IdentityServer4.LiteDB.Mappers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IdentityServer4.LiteDB.Tests.Mappers
{
    [TestClass]
    public class ApiResourceMapperProfileTests
    {
        [TestMethod]
        public void Map()
        { 
            var model = new Models.ApiResource(nameof(ApiResourceMapperProfileTests));
            var entity = ApiResourceMappers.ToEntity(model);
            model = ApiResourceMappers.ToModel(entity);
 
            Assert.IsNotNull(entity);
            Assert.IsNotNull(model);
        }
        [TestMethod]
        public void Map_Properties()
        {
 
            var model = new Models.ApiResource
            {
                Description = "description",
                DisplayName = "displayname",
                Name = "foo",
                Scopes = { new Models.Scope("foo1"), new Models.Scope("foo2") },
                Enabled = false
            };

            // Act
            var mappedEntity = ApiResourceMappers.ToEntity(model);
            var mappedModel = ApiResourceMappers.ToModel(mappedEntity);


            // Assert
            Assert.IsNotNull(mappedEntity);
            Assert.AreEqual(2, mappedEntity.Scopes.Count);
            Assert.IsNotNull(mappedEntity.Scopes.FirstOrDefault(x => x.Name == "foo1"));
            Assert.IsNotNull(mappedEntity.Scopes.FirstOrDefault(x => x.Name == "foo2"));

            Assert.IsNotNull(model);
            Assert.AreEqual("description", mappedModel.Description);
            Assert.AreEqual("displayname", mappedModel.DisplayName);
            Assert.IsFalse(mappedModel.Enabled);
            Assert.AreEqual("foo", mappedModel.Name);
        }
        [TestMethod]
        public void Map_Use_Defaults()
        {
            // Arrange
 
            var entity = new Entities.ApiResource
            {
                Name = Guid.NewGuid().ToString(),
                DisplayName = Guid.NewGuid().ToString(),

                Secrets = new System.Collections.Generic.List<Entities.ApiSecret>
                {
                    new Entities.ApiSecret
                    {
                       
                    }
                }
            };

            var def = new Models.ApiResource
            {
                ApiSecrets = { new Models.Secret("foo") }
            };

            // Act

            var mappedModel = ApiResourceMappers.ToModel(entity);

            // Assert
            Assert.AreEqual(def.ApiSecrets.First().Type, mappedModel.ApiSecrets.First().Type);
        }
        [TestMethod]
        public void Map_Use_Defaults_Missing_ThrowsException()
        {
            // Arrange

            var entity = new Entities.ApiResource
            {
             //   Name = Guid.NewGuid().ToString(),
                DisplayName = Guid.NewGuid().ToString(),

                Secrets = new System.Collections.Generic.List<Entities.ApiSecret>
                {
                    new Entities.ApiSecret
                    {

                    }
                }
            };

            var def = new Models.ApiResource
            {
                ApiSecrets = { new Models.Secret("foo") }
            };

           
            // Act & Assert
            Assert.ThrowsException<ArgumentNullException>(() => ApiResourceMappers.ToModel(entity));
            
        }
    }
}
