// <copyright file="Startup.cs" company="Jedi Contributors">
// Copyright (c) Jedi Contributors. All rights reserved.
//  
// This software is licensed under the MIT license. Read the LICENSE file in the 
// repository for more information.
// </copyright>

using Jedi.Authorization.Contracts;
using Jedi.Common.Api.Clients;
using Jedi.Common.Contracts;
using Jedi.Common.S2SCommunication;
using Jedi.Common.Startup;
using Jedi.Datastore.Contracts;

namespace Jedi.Authorization.EntryPoint
{
    /// <summary>
    /// Startup logic for the authorization service.
    /// </summary>
    public class Startup : IAppStartup
    {
        private readonly IConfiguration _configuration;
        private readonly IHostEnvironment _environment;

        /// <summary>
        /// Create a new <see cref="Startup"/> object to initialize the service.
        /// </summary>
        /// <param name="configuration">The application configuration.</param>
        /// <param name="environment">The hosting environment.</param>
        public Startup(IConfiguration configuration, IHostEnvironment environment)
        {
            _configuration = configuration;
            _environment = environment;
        }
        
        /// <summary>
        /// Configure the application's services.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> from the host.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<AuthorizationConfiguration>(_configuration);
            services.Configure<DatastoreConfiguration>(_configuration.GetSection("Datastore"));
            services.Configure<SessionConfiguration>(_configuration.GetSection("Sessions"));

            services.AddSingleton<IServiceClientFactory<IDatastoreController>, ServiceClientFactory<IDatastoreController>>();

            services.AddSingleton<IDatastoreClient, DatastoreClient>();

            services.AddHostedService<ServiceWorker>();
        }
    }
}