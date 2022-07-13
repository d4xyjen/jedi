// <copyright file="WorldClient.cs" company="Jedi Contributors">
// Copyright (c) Jedi Contributors. All rights reserved.
//  
// This software is licensed under the MIT license. Read the LICENSE file in the 
// repository for more information.
// </copyright>

using Jedi.Common.Api.Clients;
using Jedi.World.Contracts;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Jedi.Common.S2SCommunication
{
    /// <summary>
    /// Client that allows you to communicate with the world service
    /// using protocols.
    /// </summary>
    public interface IWorldClient
    {
    }

    /// <summary>
    /// Client that allows you to communicate with the world service
    /// using protocols.
    /// </summary>
    public class WorldClient : IWorldClient
    {
        private readonly ILogger<WorldClient> _logger;
        private readonly IServiceClientFactory<IWorldController> _serviceClientFactory;
        private readonly IOptionsMonitor<WorldConfiguration> _worldConfiguration;

        /// <summary>
        /// Create a new <see cref="WorldClient"/>.
        /// </summary>
        /// <param name="logger">Logger for the client to use.</param>
        /// <param name="serviceClientFactory">Service client factory for the client to use.</param>
        /// <param name="worldConfiguration">World service configuration.</param>
        public WorldClient(ILogger<WorldClient> logger, IServiceClientFactory<IWorldController> serviceClientFactory, IOptionsMonitor<WorldConfiguration> worldConfiguration)
        {
            _logger = logger;
            _serviceClientFactory = serviceClientFactory;
            _worldConfiguration = worldConfiguration;
        }
    }
}