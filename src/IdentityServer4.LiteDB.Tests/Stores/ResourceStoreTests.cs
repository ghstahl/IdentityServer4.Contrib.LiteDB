using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.LiteDB.Interfaces;
using IdentityServer4.LiteDB.Mappers;
using IdentityServer4.Stores;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IdentityServer4.LiteDB.Tests.Stores
{
    [TestClass]
    public class ResourceStoreTests
    {
        public ResourceStoreTests()
        {
            HostContainer = new HostContainer();
        }

        public HostContainer HostContainer { get; }

        public ServiceProvider ServiceProvider
        {
            get { return HostContainer.ServiceProvider; }
        }

        private static Models.IdentityResource CreateIdentityTestResource()
        {
            return new Models.IdentityResource
            {
                Name = Guid.NewGuid().ToString(),
                DisplayName = Guid.NewGuid().ToString(),
                Description = Guid.NewGuid().ToString(),
                ShowInDiscoveryDocument = true,
                UserClaims =
                {
                    JwtClaimTypes.Subject,
                    JwtClaimTypes.Name
                }
            };
        }

        private static Models.ApiResource CreateApiTestResource()
        {
            return new Models.ApiResource
            {
                Name = Guid.NewGuid().ToString(),
                ApiSecrets = new List<Models.Secret> {new Models.Secret(Models.HashExtensions.Sha256("secret"))},
                Scopes =
                    new List<Models.Scope>
                    {
                        new Models.Scope
                        {
                            Name = Guid.NewGuid().ToString(),
                            UserClaims = {Guid.NewGuid().ToString()}
                        }
                    },
                UserClaims =
                {
                    Guid.NewGuid().ToString(),
                    Guid.NewGuid().ToString(),
                }
            };
        }


        [TestMethod]
        public async Task FindResourcesAsync_WhenResourcesExist_ExpectResourcesReturned()
        {
            // Arrange
            var testIdentityResource = CreateIdentityTestResource();
            var testApiResource = CreateApiTestResource();

            var resourceStore = ServiceProvider.GetService<IResourceStore>();
            var configurationDbContext = ServiceProvider.GetService<IConfigurationDbContext>();

            configurationDbContext.AddApiResource(testApiResource.ToEntity());
            configurationDbContext.AddIdentityResource(testIdentityResource.ToEntity());

            var resources = await resourceStore.FindResourcesByScopeAsync(new List<string>
            {
                testIdentityResource.Name,
                testApiResource.Scopes.First().Name
            });

            // Assert
            Assert.IsNotNull(resources);
            Assert.IsNotNull(resources.IdentityResources);
            Assert.IsTrue(resources.IdentityResources.Any());
            Assert.IsNotNull(resources.ApiResources);
            Assert.IsTrue(resources.ApiResources.Any());
            Assert.IsNotNull(resources.IdentityResources.FirstOrDefault(x => x.Name == testIdentityResource.Name));
            Assert.IsNotNull(resources.ApiResources.FirstOrDefault(x => x.Name == testApiResource.Name));

        }

        [TestMethod]
        public async Task FindResourcesAsync_WhenResourcesExist_ExpectOnlyResourcesRequestedReturned()
        {
            // Arrange
            var testIdentityResource = CreateIdentityTestResource();
            var testIdentityResource2 = CreateIdentityTestResource();
            var testApiResource = CreateApiTestResource();
            var testApiResource2 = CreateApiTestResource();

            var resourceStore = ServiceProvider.GetService<IResourceStore>();
            var configurationDbContext = ServiceProvider.GetService<IConfigurationDbContext>();

            configurationDbContext.AddApiResource(testApiResource.ToEntity());
            configurationDbContext.AddIdentityResource(testIdentityResource.ToEntity());

            configurationDbContext.AddApiResource(testApiResource2.ToEntity());
            configurationDbContext.AddIdentityResource(testIdentityResource2.ToEntity());


            var resources = await resourceStore.FindResourcesByScopeAsync(new List<string>
            {
                testIdentityResource.Name,
                testApiResource.Scopes.First().Name
            });

            // Assert
            Assert.IsNotNull(resources);
            Assert.IsNotNull(resources.IdentityResources);
            Assert.IsTrue(resources.IdentityResources.Any());
            Assert.IsNotNull(resources.ApiResources);
            Assert.IsTrue(resources.ApiResources.Any());
            Assert.AreEqual(1, resources.IdentityResources.Count);
            Assert.AreEqual(1, resources.ApiResources.Count);
        }

        [TestMethod]
        public async Task GetAllResources_WhenAllResourcesRequested_ExpectAllResourcesIncludingHidden()
        {
            // Arrange
            var visibleIdentityResource = CreateIdentityTestResource();
            var visibleApiResource = CreateApiTestResource();
            var hiddenIdentityResource =
                new Models.IdentityResource {Name = Guid.NewGuid().ToString(), ShowInDiscoveryDocument = false};
            var hiddenApiResource = new Models.ApiResource
            {
                Name = Guid.NewGuid().ToString(),
                Scopes = new List<Models.Scope>
                {
                    new Models.Scope {Name = Guid.NewGuid().ToString(), ShowInDiscoveryDocument = false}
                }
            };


            var resourceStore = ServiceProvider.GetService<IResourceStore>();
            var configurationDbContext = ServiceProvider.GetService<IConfigurationDbContext>();

            configurationDbContext.AddApiResource(visibleApiResource.ToEntity());
            configurationDbContext.AddIdentityResource(visibleIdentityResource.ToEntity());

            configurationDbContext.AddApiResource(hiddenApiResource.ToEntity());
            configurationDbContext.AddIdentityResource(hiddenIdentityResource.ToEntity());


            var resources = await resourceStore.GetAllResourcesAsync();

            // Assert
            Assert.IsNotNull(resources);
            Assert.IsTrue(resources.IdentityResources.Any());
            Assert.IsTrue(resources.ApiResources.Any());

            var anyDoNotShowIdentityResources = (from identityResource in resources.IdentityResources
                where !identityResource.ShowInDiscoveryDocument
                select identityResource).Any();
            var anyDoNotShowApiResources = (from apiResource in resources.ApiResources
                from scope in apiResource.Scopes
                where !scope.ShowInDiscoveryDocument
                select apiResource).Any();

            Assert.IsTrue(anyDoNotShowIdentityResources);
            Assert.IsTrue(anyDoNotShowApiResources);
        }

        [TestMethod]
        public async Task FindIdentityResourcesByScopeAsync_WhenResourceExists_ExpectResourceAndCollectionsReturned()
        {
            // Arrange
            var resource = CreateIdentityTestResource();
            var resourceStore = ServiceProvider.GetService<IResourceStore>();
            var configurationDbContext = ServiceProvider.GetService<IConfigurationDbContext>();


            configurationDbContext.AddIdentityResource(resource.ToEntity());

            // Act
            var resources = await resourceStore
                .FindIdentityResourcesByScopeAsync(new List<string>
                {
                    resource.Name
                });

            // Assert
            Assert.IsNotNull(resources);
            Assert.IsTrue(resources.Any());

            var foundScope = resources.Single();
            Assert.AreEqual(resource.Name, foundScope.Name);
            Assert.IsNotNull(foundScope.UserClaims);
            Assert.IsTrue(foundScope.UserClaims.Any());

        }

        [TestMethod]
        public async Task FindIdentityResourcesByScopeAsync_WhenResourcesExist_ExpectOnlyRequestedReturned()
        {
            // Arrange
            var resourceStore = ServiceProvider.GetService<IResourceStore>();
            var configurationDbContext = ServiceProvider.GetService<IConfigurationDbContext>();

            var testIdentityResource = CreateIdentityTestResource();
            var testApiResource = CreateApiTestResource();

            configurationDbContext.AddIdentityResource(testIdentityResource.ToEntity());
            configurationDbContext.AddApiResource(testApiResource.ToEntity());

            // Act
            var resources = await resourceStore
                .FindIdentityResourcesByScopeAsync(new List<string>
                {
                    testIdentityResource.Name
                });

            // Assert
            Assert.IsNotNull(resources);
            Assert.IsTrue(resources.Any());
            Assert.AreEqual(resources.Count(),1);

        }

        [TestMethod]
        public async Task FindApiResourceAsync_WhenResourceExists_ExpectResourceAndCollectionsReturned()
        {
            // Arrange
            var resourceStore = ServiceProvider.GetService<IResourceStore>();
            var configurationDbContext = ServiceProvider.GetService<IConfigurationDbContext>();

            var testApiResource = CreateApiTestResource();

            configurationDbContext.AddApiResource(testApiResource.ToEntity());

            // Act
            var foundResource = await resourceStore
                .FindApiResourceAsync(testApiResource.Name).ConfigureAwait(false);

            // Assert
            Assert.IsNotNull(foundResource);
            Assert.IsNotNull(foundResource.UserClaims);
            Assert.IsTrue(foundResource.UserClaims.Any());
            Assert.IsNotNull(foundResource.ApiSecrets);
            Assert.IsTrue(foundResource.ApiSecrets.Any());
            Assert.IsNotNull(foundResource.Scopes);
            Assert.IsTrue(foundResource.Scopes.Any());

            var anyScopeWithUserClaims = (from scope in foundResource.Scopes
                where scope.UserClaims.Any()
                select scope).Any();

            Assert.IsTrue(anyScopeWithUserClaims);

        }

        [TestMethod]
        public async Task FindApiResourcesByScopeAsync_WhenResourceExists_ExpectResourceAndCollectionsReturned()
        {
            // Arrange
            var resourceStore = ServiceProvider.GetService<IResourceStore>();
            var configurationDbContext = ServiceProvider.GetService<IConfigurationDbContext>();

            var testApiResource = CreateApiTestResource();

            configurationDbContext.AddApiResource(testApiResource.ToEntity());

            // Act
            var resources = await resourceStore
                .FindApiResourcesByScopeAsync(new List<string>
            {
                testApiResource.Scopes.First().Name
            });


            // Assert
            Assert.IsTrue(resources.Any());
            Assert.IsNotNull(resources);

            Assert.IsNotNull(resources.First().UserClaims);
            Assert.IsTrue(resources.First().UserClaims.Any());
            Assert.IsNotNull(resources.First().ApiSecrets);
            Assert.IsTrue(resources.First().ApiSecrets.Any());
            Assert.IsNotNull(resources.First().Scopes);
            Assert.IsTrue(resources.First().Scopes.Any());

            var anyScopeWithUserClaims = (from scope in resources.First().Scopes
                where scope.UserClaims.Any()
                select scope).Any();

            Assert.IsTrue(anyScopeWithUserClaims);

        }

        [TestMethod]
        public async Task FindApiResourcesByScopeAsync_WhenMultipleResourcesExist_ExpectOnlyRequestedResourcesReturned()
        {
            // Arrange
            var resourceStore = ServiceProvider.GetService<IResourceStore>();
            var configurationDbContext = ServiceProvider.GetService<IConfigurationDbContext>();

            var testApiResource = CreateApiTestResource();

            configurationDbContext.AddApiResource(testApiResource.ToEntity());
            configurationDbContext.AddApiResource(CreateApiTestResource().ToEntity());
            configurationDbContext.AddApiResource(CreateApiTestResource().ToEntity());

            // Act
            var resources = await resourceStore
                .FindApiResourcesByScopeAsync(new List<string>
                {
                    testApiResource.Scopes.First().Name
                });

            // Assert
            Assert.IsNotNull(resources);
            Assert.IsTrue(resources.Any());
            Assert.AreEqual(1,resources.Count());

        }
    }
}