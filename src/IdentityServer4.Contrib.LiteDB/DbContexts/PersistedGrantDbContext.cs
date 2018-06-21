// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.LiteDB.Configuration;
using IdentityServer4.LiteDB.Entities;
using IdentityServer4.LiteDB.Interfaces;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LiteDB;

namespace IdentityServer4.LiteDB.DbContexts
{
    public class PersistedGrantDbContext : LiteDBContextBase, IPersistedGrantDbContext
    {
        private LiteCollection<PersistedGrant> _persistedGrants;

        public PersistedGrantDbContext(IOptions<LiteDBConfiguration> settings) 
            : base(settings)
        {
            _persistedGrants = Database.GetCollection<PersistedGrant>(Constants.TableNames.PersistedGrant);
        }

        public IQueryable<PersistedGrant> PersistedGrants
        {
            get { return _persistedGrants.FindAll().AsQueryable(); }
        }

        public async Task Update(Expression<Func<PersistedGrant, bool>> filter, PersistedGrant entity)
        {
            _persistedGrants.Upsert(entity);
        }

        public async Task Add(PersistedGrant entity)
        {
            _persistedGrants.Insert(entity);
        }
        
        public async Task Remove(Expression<Func<PersistedGrant, bool>> filter)
        {
             _persistedGrants.Delete(filter);
        }

        public async Task RemoveExpired()
        {
           await Remove(x => x.Expiration < DateTime.UtcNow);
        }
    }
}