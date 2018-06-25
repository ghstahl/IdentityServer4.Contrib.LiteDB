using FakeItEasy;
using IdentityServer4.LiteDB.Configuration;
using IdentityServer4.LiteDB.DbContexts;
using IdentityServer4.LiteDB.Interfaces;
using IdentityServer4.LiteDB.Stores;
using IdentityServer4.OperationalStore.CoreTests;
using IdentityServer4.Stores;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Client = IdentityServer4.LiteDB.Entities.Client;

namespace IdentityServer4.LiteDB.Tests
{
    public static class HostContainer
    {
        private static TempFile _tempFile;
        private static ServiceProvider _serviceProvider;

        public static ServiceProvider ServiceProvider
        {
            get
            {
                if (_serviceProvider == null)
                {

                    _tempFile = new TempFile();
                    var liteDbConfiguration = new LiteDBConfiguration()
                    {
                        ConnectionString = _tempFile.Filename
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
                    serviceCollection.AddSingleton<IOperationalTestContext,OperationalTestContext>();
                    _serviceProvider = serviceCollection.BuildServiceProvider();
                }

                return _serviceProvider;
            }
        }
    }
}
