// <copyright file="SqlDatastore.cs" company="Jedi Contributors">
// Copyright (c) Jedi Contributors. All rights reserved.
//  
// This software is licensed under the MIT license. Read the LICENSE file in the 
// repository for more information.
// </copyright>

using Jedi.Datastore.Contracts;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Jedi.Datastore.Services.Providers
{
    /// <summary>
    /// Proxy for Sql datastore connections.
    /// </summary>
    public class SqlDatastore : IDatastoreProvider
    {
        private readonly ILogger<SqlDatastore> _logger;
        private readonly IOptionsMonitor<DatastoreConfiguration> _datastoreConfiguration;

        /// <summary>
        /// Create a new <see cref="SqlDatastore"/>.
        /// </summary>
        /// <param name="logger">The logger for the datastore.</param>
        /// <param name="datastoreConfiguration">Datastore configuration.</param>
        public SqlDatastore(ILogger<SqlDatastore> logger, IOptionsMonitor<DatastoreConfiguration> datastoreConfiguration)
        {
            _logger = logger;
            _datastoreConfiguration = datastoreConfiguration;
        }
    }
}
