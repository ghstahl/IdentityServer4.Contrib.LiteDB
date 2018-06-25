using System;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Stores;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace IdentityServer4.OperationalStore.CoreTests.Stores
{
    public abstract class PersistedGrantStoreTests
    {
        private IOperationalTestContext _operationalTestContext;
        private IPersistedGrantStore _persistedGrantStore;

        public PersistedGrantStoreTests(
            IOperationalTestContext operationalTestContext,
            IPersistedGrantStore persistedGrantStore)
        {
            _operationalTestContext = operationalTestContext;
            _persistedGrantStore = persistedGrantStore;
        }

        private static Models.PersistedGrant UniquePersistedGrant => new Models.PersistedGrant
        {
            Key = Unique.S,
            Type = "authorization_code",
            ClientId = Unique.S,
            SubjectId = Unique.S,
            CreationTime = new DateTime(2016, 08, 01),
            Expiration = new DateTime(2016, 08, 31),
            Data = Unique.S
        };

        [TestMethod]
        public async Task DI_Valid()
        {
            _operationalTestContext.ShouldNotBeNull();
            _persistedGrantStore.ShouldNotBeNull();
        }


        [TestMethod]
        public async Task StoreAsync_WhenPersistedGrantStored_ExpectSuccess()
        {
            // Arrange
            var persistedGrant = UniquePersistedGrant;

            // Act
            await _persistedGrantStore.StoreAsync(persistedGrant);


            // Assert
            var foundPersistedGrant = await _persistedGrantStore.GetAsync(persistedGrant.Key);
            // Assert
            Assert.IsNotNull(foundPersistedGrant);

        }

        [TestMethod]
        public async Task StoreAsync_WhenPersistedGrantStored_ExpectUpdateSuccess()
        {
            // Arrange
            var persistedGrant = UniquePersistedGrant;
            // Act
            await _persistedGrantStore.StoreAsync(persistedGrant);

            var newDate = DateTime.Now.AddDays(2);
            persistedGrant.Expiration = newDate;

            // Act
            await _persistedGrantStore.StoreAsync(persistedGrant);


            // Assert
            var foundPersistedGrant = await _persistedGrantStore.GetAsync(persistedGrant.Key);
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
            var persistedGrant = UniquePersistedGrant;
            // Act
            await _persistedGrantStore.StoreAsync(persistedGrant);

           

            // Assert
            var foundPersistedGrant = await _persistedGrantStore.GetAsync(persistedGrant.Key);
            // Assert
            Assert.IsNotNull(foundPersistedGrant);

        }
        [TestMethod]
        public async Task GetAsync_WithSubAndTypeAndPersistedGrantExists_ExpectPersistedGrantReturned()
        {
            // Arrange
            var persistedGrant = UniquePersistedGrant;

            // Act
            await _persistedGrantStore.StoreAsync(persistedGrant);



            // Assert
            var foundPersistedGrants = await _persistedGrantStore.GetAllAsync(persistedGrant.SubjectId);
            // Assert
            Assert.IsNotNull(foundPersistedGrants);
            Assert.IsTrue(foundPersistedGrants.Any());

        }
        [TestMethod]
        public async Task RemoveAsync_WhenKeyOfExistingReceived_ExpectGrantDeleted()
        {
            // Arrange
            var persistedGrant = UniquePersistedGrant;

            // Act
            await _persistedGrantStore.StoreAsync(persistedGrant);
            var foundPersistedGrant = await _persistedGrantStore.GetAsync(persistedGrant.Key);
            Assert.IsNotNull(foundPersistedGrant);

            await _persistedGrantStore.RemoveAsync(persistedGrant.Key);

            foundPersistedGrant = await _persistedGrantStore.GetAsync(persistedGrant.Key);


            // Assert
            Assert.IsNull(foundPersistedGrant);

        }

        [TestMethod]
        public async Task RemoveAllAsync_WhenSubIdAndClientIdOfExistingReceived_ExpectGrantDeleted()
        {
            // Arrange
            var persistedGrant = UniquePersistedGrant;

            // Act
            await _persistedGrantStore.StoreAsync(persistedGrant);
            var foundPersistedGrant = await _persistedGrantStore.GetAsync(persistedGrant.Key);
            Assert.IsNotNull(foundPersistedGrant);

            await _persistedGrantStore.RemoveAllAsync(persistedGrant.SubjectId, persistedGrant.ClientId);

            foundPersistedGrant = await _persistedGrantStore.GetAsync(persistedGrant.Key);


            // Assert
            Assert.IsNull(foundPersistedGrant);

        }

        [TestMethod]
        public async Task RemoveAllAsync_WhenSubIdClientIdAndTypeOfExistingReceived_ExpectGrantDeleted()
        {
            // Arrange
            var persistedGrant = UniquePersistedGrant;

            // Act
            await _persistedGrantStore.StoreAsync(persistedGrant);
            var foundPersistedGrant = await _persistedGrantStore.GetAsync(persistedGrant.Key);
            Assert.IsNotNull(foundPersistedGrant);

            await _persistedGrantStore.RemoveAllAsync(persistedGrant.SubjectId, persistedGrant.ClientId, persistedGrant.Type);


            foundPersistedGrant = await _persistedGrantStore.GetAsync(persistedGrant.Key);


            // Assert
            Assert.IsNull(foundPersistedGrant);

        }
    }
}