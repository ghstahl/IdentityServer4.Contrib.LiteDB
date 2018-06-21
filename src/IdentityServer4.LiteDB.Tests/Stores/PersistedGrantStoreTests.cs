using System;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.LiteDB.Interfaces;
using IdentityServer4.LiteDB.Mappers;
using IdentityServer4.Stores;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IdentityServer4.LiteDB.Tests.Stores
{
    [TestClass]
    public class PersistedGrantStoreTests
    {
        public PersistedGrantStoreTests()
        {
            HostContainer = new HostContainer();
        }

        public HostContainer HostContainer { get; }

        public ServiceProvider ServiceProvider
        {
            get { return HostContainer.ServiceProvider; }
        }

        private static Models.PersistedGrant CreateTestObject()
        {
            return new Models.PersistedGrant
            {
                Key = Guid.NewGuid().ToString(),
                Type = "authorization_code",
                ClientId = Guid.NewGuid().ToString(),
                SubjectId = Guid.NewGuid().ToString(),
                CreationTime = new DateTime(2016, 08, 01),
                Expiration = new DateTime(2016, 08, 31),
                Data = Guid.NewGuid().ToString()
            };
        }

        [TestMethod]
        public async Task StoreAsync_WhenPersistedGrantStored_ExpectSuccess()
        {
            // Arrange
            var persistedGrant = CreateTestObject();

            // Act
            var persistedGrantStore = ServiceProvider.GetService<IPersistedGrantStore>();
            await persistedGrantStore.StoreAsync(persistedGrant);


            // Assert
            var foundPersistedGrant = await persistedGrantStore.GetAsync(persistedGrant.Key);
            // Assert
            Assert.IsNotNull(foundPersistedGrant);

        }

        [TestMethod]
        public async Task StoreAsync_WhenPersistedGrantStored_ExpectUpdateSuccess()
        {
            // Arrange
            var persistedGrant = CreateTestObject();
            var persistedGrantStore = ServiceProvider.GetService<IPersistedGrantStore>();

            // Act
            await persistedGrantStore.StoreAsync(persistedGrant);

            var newDate = DateTime.Now.AddDays(2);
            persistedGrant.Expiration = newDate;

            // Act
            await persistedGrantStore.StoreAsync(persistedGrant);


            // Assert
            var foundPersistedGrant = await persistedGrantStore.GetAsync(persistedGrant.Key);
            // Assert
            Assert.IsNotNull(foundPersistedGrant);
            
            Assert.AreEqual(newDate.Date, foundPersistedGrant.Expiration.Value.Date);
            Assert.AreEqual(newDate.Hour, foundPersistedGrant.Expiration.Value.Hour);
            Assert.AreEqual(newDate.Minute, foundPersistedGrant.Expiration.Value.Minute);
            Assert.AreEqual(newDate.Second, foundPersistedGrant.Expiration.Value.Second);
        }

        [TestMethod]
        public async Task GetAsync_WithKeyAndPersistedGrantExists_ExpectPersistedGrantReturned()
        {
            // Arrange
            var persistedGrant = CreateTestObject();
            var persistedGrantStore = ServiceProvider.GetService<IPersistedGrantStore>();

            // Act
            await persistedGrantStore.StoreAsync(persistedGrant);

           

            // Assert
            var foundPersistedGrant = await persistedGrantStore.GetAsync(persistedGrant.Key);
            // Assert
            Assert.IsNotNull(foundPersistedGrant);

        }
        [TestMethod]
        public async Task GetAsync_WithSubAndTypeAndPersistedGrantExists_ExpectPersistedGrantReturned()
        {
            // Arrange
            var persistedGrant = CreateTestObject();
            var persistedGrantStore = ServiceProvider.GetService<IPersistedGrantStore>();

            // Act
            await persistedGrantStore.StoreAsync(persistedGrant);



            // Assert
            var foundPersistedGrants = await persistedGrantStore.GetAllAsync(persistedGrant.SubjectId);
            // Assert
            Assert.IsNotNull(foundPersistedGrants);
            Assert.IsTrue(foundPersistedGrants.Any());

        }
        [TestMethod]
        public async Task RemoveAsync_WhenKeyOfExistingReceived_ExpectGrantDeleted()
        {
            // Arrange
            var persistedGrant = CreateTestObject();
            var persistedGrantStore = ServiceProvider.GetService<IPersistedGrantStore>();

            // Act
            await persistedGrantStore.StoreAsync(persistedGrant);
            var foundPersistedGrant = await persistedGrantStore.GetAsync(persistedGrant.Key);
            Assert.IsNotNull(foundPersistedGrant);

            await persistedGrantStore.RemoveAsync(persistedGrant.Key);

            foundPersistedGrant = await persistedGrantStore.GetAsync(persistedGrant.Key);


            // Assert
            Assert.IsNull(foundPersistedGrant);

        }

        [TestMethod]
        public async Task RemoveAllAsync_WhenSubIdAndClientIdOfExistingReceived_ExpectGrantDeleted()
        {
            // Arrange
            var persistedGrant = CreateTestObject();
            var persistedGrantStore = ServiceProvider.GetService<IPersistedGrantStore>();

            // Act
            await persistedGrantStore.StoreAsync(persistedGrant);
            var foundPersistedGrant = await persistedGrantStore.GetAsync(persistedGrant.Key);
            Assert.IsNotNull(foundPersistedGrant);

            await persistedGrantStore.RemoveAllAsync(persistedGrant.SubjectId, persistedGrant.ClientId);

            foundPersistedGrant = await persistedGrantStore.GetAsync(persistedGrant.Key);


            // Assert
            Assert.IsNull(foundPersistedGrant);

        }

        [TestMethod]
        public async Task RemoveAllAsync_WhenSubIdClientIdAndTypeOfExistingReceived_ExpectGrantDeleted()
        {
            // Arrange
            var persistedGrant = CreateTestObject();
            var persistedGrantStore = ServiceProvider.GetService<IPersistedGrantStore>();

            // Act
            await persistedGrantStore.StoreAsync(persistedGrant);
            var foundPersistedGrant = await persistedGrantStore.GetAsync(persistedGrant.Key);
            Assert.IsNotNull(foundPersistedGrant);

            await persistedGrantStore.RemoveAllAsync(persistedGrant.SubjectId, persistedGrant.ClientId, persistedGrant.Type);


            foundPersistedGrant = await persistedGrantStore.GetAsync(persistedGrant.Key);


            // Assert
            Assert.IsNull(foundPersistedGrant);

        }
    }
}