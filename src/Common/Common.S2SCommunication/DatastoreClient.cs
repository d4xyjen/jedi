// <copyright file="DatastoreClient.cs" company="Jedi Contributors">
// Copyright (c) Jedi Contributors. All rights reserved.
//  
// This software is licensed under the MIT license. Read the LICENSE file in the 
// repository for more information.
// </copyright>

using Jedi.Common.Api.Clients;
using Jedi.Common.Contracts.Protocols.User;
using Jedi.Datastore.Contracts;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Jedi.Common.S2SCommunication
{
    /// <summary>
    /// Client that allows you to communicate with the datastore service
    /// using protocols.
    /// </summary>
    public interface IDatastoreClient
    {
        /// <summary>
        /// Attempt a password for an account asynchronously.
        /// </summary>
        /// <param name="username">The username of the account.</param>
        /// <param name="password">The attempted password.</param>
        /// <returns></returns>
        public Task<PROTO_USER_PASSWORD_CHECK_ACK> CheckPasswordAsync(string username, string password);
    }

    /// <summary>
    /// Client that allows you to communicate with the datastore service
    /// using protocols.
    /// </summary>
    public class DatastoreClient : IDatastoreClient
    {
        private readonly ILogger<DatastoreClient> _logger;
        private readonly IServiceClientFactory<IDatastoreController> _serviceClientFactory;
        private readonly IOptionsMonitor<DatastoreConfiguration> _datastoreConfiguration;

        /// <summary>
        /// Create a new <see cref="DatastoreClient"/>.
        /// </summary>
        /// <param name="logger">Logger for the client to use.</param>
        /// <param name="serviceClientFactory">Service client factory for the client to use.</param>
        /// <param name="datastoreConfiguration">Datastore service configuration.</param>
        public DatastoreClient(ILogger<DatastoreClient> logger, IServiceClientFactory<IDatastoreController> serviceClientFactory, IOptionsMonitor<DatastoreConfiguration> datastoreConfiguration)
        {
            _logger = logger;
            _serviceClientFactory = serviceClientFactory;
            _datastoreConfiguration = datastoreConfiguration;
        }

        /// <summary>
        /// Attempt a password for an account asynchronously.
        /// </summary>
        /// <param name="username">The username of the account.</param>
        /// <param name="password">The attempted password.</param>
        /// <returns></returns>
        public async Task<PROTO_USER_PASSWORD_CHECK_ACK> CheckPasswordAsync(string username, string password)
        {
            var protocol = new PROTO_USER_PASSWORD_CHECK_REQ
            {
                OperationId = Guid.NewGuid(),
                Username = username,
                Password = password
            };

            return await _serviceClientFactory
                .CreateClient(_datastoreConfiguration.CurrentValue.Client.ServiceEndpoint)
                .CheckPasswordAsync(protocol);
        }
    }
}