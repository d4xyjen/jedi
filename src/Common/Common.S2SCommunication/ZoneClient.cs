// <copyright file="ZoneClient.cs" company="Jedi Contributors">
// Copyright (c) Jedi Contributors. All rights reserved.
//  
// This software is licensed under the MIT license. Read the LICENSE file in the 
// repository for more information.
// </copyright>

using Jedi.Common.Api.Clients;
using Jedi.Zone.Contracts;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Jedi.Common.S2SCommunication
{
    /// <summary>
    /// Client that allows you to communicate with the zone service
    /// using protocols.
    /// </summary>
    public interface IZoneClient
    {
    }

    /// <summary>
    /// Client that allows you to communicate with the zone service
    /// using protocols.
    /// </summary>
    public class ZoneClient : IZoneClient
    {
        private readonly ILogger<ZoneClient> _logger;
        private readonly IServiceClientFactory<IZoneController> _serviceClientFactory;
        private readonly IOptionsMonitor<ZoneConfiguration> _zoneConfiguration;

        /// <summary>
        /// Create a new <see cref="ZoneClient"/>.
        /// </summary>
        /// <param name="logger">Logger for the client to use.</param>
        /// <param name="serviceClientFactory">Service client factory for the client to use.</param>
        /// <param name="zoneConfiguration">Zone service configuration.</param>
        public ZoneClient(ILogger<ZoneClient> logger, IServiceClientFactory<IZoneController> serviceClientFactory, IOptionsMonitor<ZoneConfiguration> zoneConfiguration)
        {
            _logger = logger;
            _serviceClientFactory = serviceClientFactory;
            _zoneConfiguration = zoneConfiguration;
        }
    }
}