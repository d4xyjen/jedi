// <copyright file="AuthorizationController.cs" company="Jedi Contributors">
// Copyright (c) Jedi Contributors. All rights reserved.
//  
// This software is licensed under the MIT license. Read the LICENSE file in the 
// repository for more information.
// </copyright>

using Jedi.Authorization.Contracts;
using Jedi.Common.Api;
using Jedi.Common.Api.Messaging;
using Jedi.Common.Api.Sessions;
using Jedi.Common.Contracts.Protocols.Miscellaneous;
using Jedi.Common.Contracts.Protocols.User;
using Microsoft.Extensions.Options;

namespace Jedi.Authorization.EntryPoint.Controllers
{
    /// <summary>
    /// Handles authorization business logic.
    /// </summary>
    public class AuthorizationController : Controller
    {
        public static readonly byte[] XTrapKey = { 0x33, 0x33, 0x42, 0x35, 0x34, 0x33, 0x42, 0x30, 0x43, 0x41, 0x36, 0x45, 0x37, 0x43, 0x34, 0x31, 0x45, 0x35, 0x44, 0x31, 0x44, 0x30, 0x36, 0x35, 0x31, 0x33, 0x30, 0x37, 0x0 };

        private readonly ILogger<AuthorizationController> _logger;
        private readonly ISessionFactory _sessionFactory;
        private readonly IOptionsMonitor<AuthorizationConfiguration> _authorizationConfiguration;

        /// <summary>
        /// Create a new <see cref="AuthorizationController"/>.
        /// </summary>
        /// <param name="logger">Logger for the controller to use.</param>
        /// <param name="sessionFactory">Factory used to get sessions.</param>
        /// <param name="authorizationConfiguration">Configuration for the authorization service.</param>
        public AuthorizationController(ILogger<AuthorizationController> logger, ISessionFactory sessionFactory, IOptionsMonitor<AuthorizationConfiguration> authorizationConfiguration)
        {
            _logger = logger;
            _sessionFactory = sessionFactory;
            _authorizationConfiguration = authorizationConfiguration;
        }

        /// <summary>
        /// Request a new seed.
        /// </summary>
        /// <param name="sessionId">The session requesting a seed.</param>
        /// <param name="protocol">The protocol that was sent.</param>
        [ProtocolHandler(ProtocolType.MISC_SEED_REQ)]
        public void MISC_SEED_REQ(Guid sessionId, [FromBody] PROTO_MISC_SEED_REQ protocol)
        {
            _logger.LogInformation("AuthorizationController: Session requested a new seed; Session: {Session}", sessionId);

            if (!_sessionFactory.GetSession(sessionId, out var session) || session == null)
            {
                _logger.LogError("AuthorizationController: Failed to get session; Session: {Session}", sessionId);
                return;
            }

            session.GetNewSeed();

            if (!session.Seed.HasValue)
            {
                _logger.LogError("AuthorizationController: Failed to get new seed for session; Session: {Session}", sessionId);
                return;
            }

            session.Send(ProtocolType.MISC_SEED_ACK, new PROTO_MISC_SEED_ACK(session.Seed.Value));
            _logger.LogInformation("AuthorizationController: Assigned new seed to session; Session: {Session}, Seed: {Seed}", session.Id, session.Seed);
        }

        /// <summary>
        /// Request that the service check the client version.
        /// </summary>
        /// <param name="sessionId">The session requesting the version check.</param>
        /// <param name="protocol">The protocol that was sent.</param>
        [ProtocolHandler(ProtocolType.USER_CLIENT_VERSION_CHECK_REQ)]
        public void USER_CLIENT_VERSION_CHECK_REQ(Guid sessionId, [FromBody] PROTO_USER_CLIENT_VERSION_CHECK_REQ protocol)
        {
            _logger.LogInformation("AuthorizationController: Session requesting client version check; Session: {Session}, Version: {Version}", sessionId, protocol.Version);

            if (!_sessionFactory.GetSession(sessionId, out var session) || session == null)
            {
                _logger.LogError("AuthorizationController: Failed to get session; Session: {Session}", sessionId);
                return;
            }

            var supportedVersions = _authorizationConfiguration.CurrentValue.SupportedGameClientVersions;
            if (supportedVersions == null)
            {
                _logger.LogError("AuthorizationController: No supported versions were configured; Session: {Session}", session.Id);
                return;
            }

            for (var i = 0; i < supportedVersions.Length; i++)
            {
                var supportedVersion = supportedVersions[i];
                if (protocol.Version.Equals(supportedVersion))
                {
                    _logger.LogInformation("AuthorizationController: Client version is supported; Session: {Session}, Version: {Version}", session.Id, protocol.Version);
                    session.Send(ProtocolType.USER_CLIENT_RIGHTVERSION_CHECK_ACK, new PROTO_USER_CLIENT_RIGHTVERSION_CHECK_ACK((byte) XTrapKey.Length, XTrapKey));
                    return;
                }
            }

            _logger.LogWarning("AuthorizationController: Client version not supported; Session: {Session}, Version: {Version}", session.Id, protocol.Version);
            session.Send(ProtocolType.USER_CLIENT_WRONGVERSION_CHECK_ACK, new PROTO_USER_CLIENT_WRONGVERSION_CHECK_ACK());
        }
    }
}
