// <copyright file="GameClient.cs" company="Jedi Contributors">
// Copyright (c) Jedi Contributors. All rights reserved.
//  
// This software is licensed under the MIT license. Read the LICENSE file in the 
// repository for more information.
// </copyright>

using Jedi.Common.Api.Clients;
using Jedi.Game.Contracts;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Jedi.Common.S2SCommunication
{
    /// <summary>
    /// Client that allows you to communicate with the game service
    /// using protocols.
    /// </summary>
    public interface IGameClient
    {
    }

    /// <summary>
    /// Client that allows you to communicate with the game service
    /// using protocols.
    /// </summary>
    public class GameClient : IGameClient
    {
        private readonly ILogger<GameClient> _logger;
        private readonly IServiceClientFactory<IGameController> _serviceClientFactory;
        private readonly IOptionsMonitor<GameConfiguration> _gameConfiguration;

        /// <summary>
        /// Create a new <see cref="GameClient"/>.
        /// </summary>
        /// <param name="logger">Logger for the client to use.</param>
        /// <param name="serviceClientFactory">Service client factory for the client to use.</param>
        /// <param name="gameConfiguration">Game service configuration.</param>
        public GameClient(ILogger<GameClient> logger, IServiceClientFactory<IGameController> serviceClientFactory, IOptionsMonitor<GameConfiguration> gameConfiguration)
        {
            _logger = logger;
            _serviceClientFactory = serviceClientFactory;
            _gameConfiguration = gameConfiguration;
        }
    }
}