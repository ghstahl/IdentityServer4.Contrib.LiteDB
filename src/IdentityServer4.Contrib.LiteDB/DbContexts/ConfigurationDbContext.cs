// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.LiteDB.Configuration;
using IdentityServer4.LiteDB.Entities;
using IdentityServer4.LiteDB.Interfaces;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;

namespace IdentityServer4.LiteDB.DbContexts
{
    public class ConfigurationDbContext : LiteDBContextBase, IConfigurationDbContext
    {
        private LiteCollection<Client> _clients;
        private LiteCollection<IdentityResource> _identityResources;
        private LiteCollection<ApiResource> _apiResources;

        public ConfigurationDbContext(IOptions<LiteDBConfiguration> settings)
            : base(settings)
        {
            _clients = Database.GetCollection<Client>(Constants.TableNames.Client);
            _identityResources = Database.GetCollection<IdentityResource>(Constants.TableNames.IdentityResource);
            _apiResources = Database.GetCollection<ApiResource>(Constants.TableNames.ApiResource);
        }

        public IQueryable<Client> Clients
        {
            get { return _clients.FindAll().AsQueryable(); }
        }

        public IQueryable<IdentityResource> IdentityResources
        {
            get { return _identityResources.FindAll().AsQueryable(); }
        }

        public IQueryable<ApiResource> ApiResources
        {
            get { return _apiResources.FindAll().AsQueryable(); }
        }

        public async Task AddClient(Client entity)
        {
            _clients.Insert(entity);
        }

        public async Task AddIdentityResource(IdentityResource entity)
        {
            _identityResources.Insert(entity);
        }

        public async Task AddApiResource(ApiResource entity)
        {
            _apiResources.Insert(entity);
        }

    }
}