// <copyright file="DatastoreController.cs" company="Jedi Contributors">
// Copyright (c) Jedi Contributors. All rights reserved.
//  
// This software is licensed under the MIT license. Read the LICENSE file in the 
// repository for more information.
// </copyright>

using Jedi.Common.Api;
using Jedi.Common.Api.Sessions;
using Jedi.Common.Contracts.Protocols;
using Jedi.Common.Contracts.Protocols.Miscellaneous;

namespace Jedi.Datastore.EntryPoint.Controllers
{
    /// <summary>
    /// Handles datastore business logic.
    /// </summary>
    public class DatastoreController : Controller
    {
        private readonly ILogger<DatastoreController> _logger;
        private readonly ISessionFactory _sessionFactory;

        /// <summary>
        /// Create a new <see cref="DatastoreController"/>.
        /// </summary>
        /// <param name="logger">Logger for the controller to use.</param>
        /// <param name="sessionFactory">Factory used to get sessions.</param>
        public DatastoreController(
            ILogger<DatastoreController> logger,
            ISessionFactory sessionFactory)
        {
            _logger = logger;
            _sessionFactory = sessionFactory;
        }

        /// <summary>
        /// Request a new seed.
        /// </summary>
        /// <param name="sessionId">The session requesting a seed.</param>
        /// <param name="protocol">The protocol that was sent.</param>
        [Protocol(ProtocolCommand.MISC_SEED_REQ)]
        public PROTO_MISC_SEED_ACK? MISC_SEED_REQ(Guid sessionId, [FromBody] PROTO_MISC_SEED_REQ protocol)
        {
            _logger.LogInformation("DatastoreController: Session requested a new seed; Session: {Session}", sessionId);

            if (!_sessionFactory.GetSession(sessionId, out var session) || session == null)
            {
                _logger.LogError("DatastoreController: Failed to get session; Session: {Session}", sessionId);
                return null;
            }

            var seed = session.Seed();
            if (!seed.HasValue)
            {
                _logger.LogError("DatastoreController: Failed to set seed, destroying session; Session: {Session}, Seed: {Seed}", sessionId, seed);
                session.Destroy();
                return null;
            }

            _logger.LogInformation("DatastoreController: Set session seed; Session: {Session}, Seed: {Seed}", session.Id, seed);

            return new PROTO_MISC_SEED_ACK
            {
                Seed = seed.Value
            };
        }
    }
}
