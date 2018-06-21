// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.LiteDB;
using IdentityServer4.LiteDB.Configuration;
using IdentityServer4.LiteDB.DbContexts;
using IdentityServer4.LiteDB.Interfaces;
using IdentityServer4.LiteDB.Options;
using IdentityServer4.LiteDB.Services;
using IdentityServer4.LiteDB.Stores;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IdentityServerLiteDBBuilderExtensions
    {
        public static IIdentityServerBuilder AddConfigurationStore(
           this IIdentityServerBuilder builder, Action<LiteDBConfiguration> setupAction)
        {
            builder.Services.Configure(setupAction);

            return builder.AddConfigurationStore();
        }

        public static IIdentityServerBuilder AddConfigurationStore(
            this IIdentityServerBuilder builder, IConfiguration configuration)
        {
            builder.Services.Configure<LiteDBConfiguration>(configuration);

            return builder.AddConfigurationStore();
        }

        public static IIdentityServerBuilder AddOperationalStore(
           this IIdentityServerBuilder builder, 
           Action<LiteDBConfiguration> setupAction, 
           Action<TokenCleanupOptions> tokenCleanUpOptions = null)
        {
            builder.Services.Configure(setupAction);

            return builder.AddOperationalStore(tokenCleanUpOptions);
        }

        public static IIdentityServerBuilder AddOperationalStore(
            this IIdentityServerBuilder builder, 
            IConfiguration configuration,
            Action<TokenCleanupOptions> tokenCleanUpOptions = null)
        {
            builder.Services.Configure<LiteDBConfiguration>(configuration);

            return builder.AddOperationalStore(tokenCleanUpOptions);
        }

        private static IIdentityServerBuilder AddConfigurationStore(
            this IIdentityServerBuilder builder)
        {
            builder.Services.AddScoped<IConfigurationDbContext, ConfigurationDbContext>();

            builder.Services.AddTransient<IClientStore, ClientStore>();
            builder.Services.AddTransient<IResourceStore, ResourceStore>();
            builder.Services.AddTransient<ICorsPolicyService, CorsPolicyService>();

            return builder;
        }

        private static IIdentityServerBuilder AddOperationalStore(
            this IIdentityServerBuilder builder, 
            Action<TokenCleanupOptions> tokenCleanUpOptions = null)
        {
            builder.Services.AddScoped<IPersistedGrantDbContext, PersistedGrantDbContext>();

            builder.Services.AddTransient<IPersistedGrantStore, PersistedGrantStore>();

            var tokenCleanupOptions = new TokenCleanupOptions();
            tokenCleanUpOptions?.Invoke(tokenCleanupOptions);
            builder.Services.AddSingleton(tokenCleanupOptions);
            builder.Services.AddSingleton<TokenCleanup>();

            return builder;
        }

        public static IApplicationBuilder UseIdentityServerLiteDBTokenCleanup(this IApplicationBuilder app, IApplicationLifetime applicationLifetime)
        {
            var tokenCleanup = app.ApplicationServices.GetService<TokenCleanup>();
            if (tokenCleanup == null)
            {
                throw new InvalidOperationException("AddOperationalStore must be called on the service collection.");
            }
            applicationLifetime.ApplicationStarted.Register(tokenCleanup.Start);
            applicationLifetime.ApplicationStopping.Register(tokenCleanup.Stop);

            return app;
        }
    }
}