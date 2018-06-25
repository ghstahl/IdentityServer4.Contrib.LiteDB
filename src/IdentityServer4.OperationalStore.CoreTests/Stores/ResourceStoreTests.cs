using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Stores;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace IdentityServer4.OperationalStore.CoreTests.Stores
{

    public abstract class ResourceStoreTests
    {
        private IOperationalTestContext _operationalTestContext;
        private IResourceStore _resourceStore;

        public ResourceStoreTests(
            IOperationalTestContext operationalTestContext,
            IResourceStore resourceStore)
        {
            _operationalTestContext = operationalTestContext;
            _resourceStore = resourceStore;
        }

        private static Models.IdentityResource UniqueIdentityResource => new Models.IdentityResource
        {
            Name = Unique.S,
            DisplayName = Unique.S,
            Description = Unique.S,
            ShowInDiscoveryDocument = true,
            UserClaims =
            {
                JwtClaimTypes.Subject,
                JwtClaimTypes.Name
            }
        };

        private static Models.ApiResource UniqueApiResource => new Models.ApiResource
        {
            Name = Unique.S,
            ApiSecrets = new List<Models.Secret> {new Models.Secret(Models.HashExtensions.Sha256("secret"))},
            Scopes =
                new List<Models.Scope>
                {
                    new Models.Scope
                    {
                        Name = Unique.S,
                        UserClaims = {Unique.S}
                    }
                },
            UserClaims =
            {
                Unique.S,
                Unique.S,
            }
        };

        [TestMethod]
        public async Task DI_Valid()
        {
            _operationalTestContext.ShouldNotBeNull();
            _resourceStore.ShouldNotBeNull();
        }

        [TestMethod]
        public async Task FindResourcesAsync_WhenResourcesExist_ExpectResourcesReturned()
        {
            // Arrange
            var testIdentityResource = UniqueIdentityResource;
            var testApiResource = UniqueApiResource;


            await _operationalTestContext.AddApiResourceAsync(testApiResource);
            await _operationalTestContext.AddIdentityResourceAsync(testIdentityResource);

            var resources = await _resourceStore.FindResourcesByScopeAsync(new List<string>
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
            var testIdentityResource = UniqueIdentityResource;
            var testIdentityResource2 = UniqueIdentityResource;
            var testApiResource = UniqueApiResource;
            var testApiResource2 = UniqueApiResource;

            await _operationalTestContext.AddApiResourceAsync(testApiResource);
            await _operationalTestContext.AddIdentityResourceAsync(testIdentityResource);

            await _operationalTestContext.AddApiResourceAsync(testApiResource2);
            await _operationalTestContext.AddIdentityResourceAsync(testIdentityResource2);


            var resources = await _resourceStore.FindResourcesByScopeAsync(new List<string>
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
            var visibleIdentityResource = UniqueIdentityResource;
            var visibleApiResource = UniqueApiResource;
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



            await _operationalTestContext.AddApiResourceAsync(visibleApiResource);
            await _operationalTestContext.AddIdentityResourceAsync(visibleIdentityResource);

            await _operationalTestContext.AddApiResourceAsync(hiddenApiResource);
            await _operationalTestContext.AddIdentityResourceAsync(hiddenIdentityResource);


            var resources = await _resourceStore.GetAllResourcesAsync();

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
            var resource = UniqueIdentityResource;


            await _operationalTestContext.AddIdentityResourceAsync(resource);

            // Act
            var resources = await _resourceStore
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

            var testIdentityResource = UniqueIdentityResource;
            var testApiResource = UniqueApiResource;

            await _operationalTestContext.AddIdentityResourceAsync(testIdentityResource);
            await _operationalTestContext.AddApiResourceAsync(testApiResource);

            // Act
            var resources = await _resourceStore
                .FindIdentityResourcesByScopeAsync(new List<string>
                {
                    testIdentityResource.Name
                });

            // Assert
            Assert.IsNotNull(resources);
            Assert.IsTrue(resources.Any());
            Assert.AreEqual(resources.Count(), 1);

        }

        [TestMethod]
        public async Task FindApiResourceAsync_WhenResourceExists_ExpectResourceAndCollectionsReturned()
        {
            // Arrange
            var testApiResource = UniqueApiResource;

            await _operationalTestContext.AddApiResourceAsync(testApiResource);

            // Act
            var foundResource = await _resourceStore
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

            var testApiResource = UniqueApiResource;

            await _operationalTestContext.AddApiResourceAsync(testApiResource);

            // Act
            var resources = await _resourceStore
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

            var testApiResource = UniqueApiResource;

            await _operationalTestContext.AddApiResourceAsync(testApiResource);
            await _operationalTestContext.AddApiResourceAsync(UniqueApiResource);
            await _operationalTestContext.AddApiResourceAsync(UniqueApiResource);

            // Act
            var resources = await _resourceStore
                .FindApiResourcesByScopeAsync(new List<string>
                {
                    testApiResource.Scopes.First().Name
                });

            // Assert
            Assert.IsNotNull(resources);
            Assert.IsTrue(resources.Any());
            Assert.AreEqual(1, resources.Count());

        }
    }
}