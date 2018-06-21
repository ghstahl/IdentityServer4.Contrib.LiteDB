using IdentityServer4.LiteDB.Configuration;
using Microsoft.Extensions.Options;
using System;
using LiteDB;

namespace IdentityServer4.LiteDB.DbContexts
{
    public class LiteDBContextBase : IDisposable
    {
        private readonly LiteDatabase _database;
        
        public LiteDBContextBase(IOptions<LiteDBConfiguration> settings)
        {
            if (settings.Value == null)
                throw new ArgumentNullException(nameof(settings), "LiteDBConfiguration cannot be null.");

            if (settings.Value.ConnectionString == null)
                throw new ArgumentNullException(nameof(settings), "LiteDBConfiguration.ConnectionString cannot be null.");

            _database = new LiteDatabase(settings.Value.ConnectionString);
        }

        protected LiteDatabase Database { get { return _database; } }

        public void Dispose()
        { 
            // TODO
        }
    }
}
