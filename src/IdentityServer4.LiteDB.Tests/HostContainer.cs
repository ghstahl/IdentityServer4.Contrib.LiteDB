using FakeItEasy;
using IdentityServer4.LiteDB.Configuration;
using IdentityServer4.LiteDB.DbContexts;
using IdentityServer4.LiteDB.Interfaces;
using IdentityServer4.LiteDB.Stores;
using IdentityServer4.Stores;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Client = IdentityServer4.LiteDB.Entities.Client;

namespace IdentityServer4.LiteDB.Tests
{
    public class HostContainer
    {
        private TempFile TempFile { get; }

        public HostContainer()
        {
            TempFile = new TempFile();
            var liteDbConfiguration = new LiteDBConfiguration()
            {
                ConnectionString = TempFile.Filename
            };
            var settings = A.Fake<IOptions<LiteDBConfiguration>>();
            A.CallTo(() => settings.Value).Returns(liteDbConfiguration);

            var serviceCollection = new ServiceCollection()
                .AddLogging()
                .AddTransient<IConfigurationDbContext, ConfigurationDbContext>()
                .AddSingleton(settings);

            var identityServerBuilder = serviceCollection.AddIdentityServer();
           

            identityServerBuilder.AddConfigurationStore(setupAction =>
            {
                setupAction.ConnectionString = liteDbConfiguration.ConnectionString;
            });

            identityServerBuilder.AddOperationalStore(setupAction =>
            {
                setupAction.ConnectionString = liteDbConfiguration.ConnectionString;
            });

            ServiceProvider = serviceCollection.BuildServiceProvider();
        }

        public ServiceProvider ServiceProvider { get; }
    }
}
