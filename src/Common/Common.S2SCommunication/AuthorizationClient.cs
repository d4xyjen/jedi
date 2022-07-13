// <copyright file="AuthorizationClient.cs" company="Jedi Contributors">
// Copyright (c) Jedi Contributors. All rights reserved.
//  
// This software is licensed under the MIT license. Read the LICENSE file in the 
// repository for more information.
// </copyright>

using Jedi.Authorization.Contracts;
using Jedi.Common.Api.Clients;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Jedi.Common.S2SCommunication
{
    /// <summary>
    /// Client that allows you to communicate with the authorization service
    /// using protocols.
    /// </summary>
    public interface IAuthorizationClient
    {
    }

    /// <summary>
    /// Client that allows you to communicate with the authorization service
    /// using protocols.
    /// </summary>
    public class AuthorizationClient : IAuthorizationClient
    {
        private readonly ILogger<AuthorizationClient> _logger;
        private readonly IServiceClientFactory<IAuthorizationController> _serviceClientFactory;
        private readonly IOptionsMonitor<AuthorizationConfiguration> _authorizationConfiguration;

        /// <summary>
        /// Create a new <see cref="AuthorizationClient"/>.
        /// </summary>
        /// <param name="logger">Logger for the client to use.</param>
        /// <param name="serviceClientFactory">Service client factory for the client to use.</param>
        /// <param name="authorizationConfiguration">Authorization service configuration.</param>
        public AuthorizationClient(ILogger<AuthorizationClient> logger, IServiceClientFactory<IAuthorizationController> serviceClientFactory, IOptionsMonitor<AuthorizationConfiguration> authorizationConfiguration)
        {
            _logger = logger;
            _serviceClientFactory = serviceClientFactory;
            _authorizationConfiguration = authorizationConfiguration;
        }
    }
}
