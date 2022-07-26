// <copyright file="UserController.cs" company="Jedi Contributors">
// Copyright (c) Jedi Contributors. All rights reserved.
//  
// This software is licensed under the MIT license. Read the LICENSE file in the 
// repository for more information.
// </copyright>

using Jedi.Authorization.Contracts;
using Jedi.Common.Api;
using Jedi.Common.Api.Sessions;
using Jedi.Common.Contracts.Protocols;
using Jedi.Common.Contracts.Protocols.User;
using Jedi.Common.S2SCommunication;
using Microsoft.Extensions.Options;

namespace Jedi.Authorization.EntryPoint.Controllers
{
    /// <summary>
    /// Handles user business logic.
    /// </summary>
    public class UserController : Controller
    {
        private static readonly byte[] XTrapKey = { 0x33, 0x33, 0x42, 0x35, 0x34, 0x33, 0x42, 0x30, 0x43, 0x41, 0x36, 0x45, 0x37, 0x43, 0x34, 0x31, 0x45, 0x35, 0x44, 0x31, 0x44, 0x30, 0x36, 0x35, 0x31, 0x33, 0x30, 0x37, 0x0 };

        private readonly ILogger<UserController> _logger;
        private readonly ISessionFactory _sessionFactory;
        private readonly IOptionsMonitor<AuthorizationConfiguration> _authorizationConfiguration;
        private readonly IDatastoreClient _datastoreClient;

        /// <summary>
        /// Create a new <see cref="UserController"/>.
        /// </summary>
        /// <param name="logger">Logger for the controller to use.</param>
        /// <param name="sessionFactory">Factory used to get sessions.</param>
        /// <param name="authorizationConfiguration">Configuration for the authorization service.</param>
        /// <param name="datastoreClient">Datastore service proxy.</param>
        public UserController(
            ILogger<UserController> logger,
            ISessionFactory sessionFactory,
            IOptionsMonitor<AuthorizationConfiguration> authorizationConfiguration,
            IDatastoreClient datastoreClient)
        {
            _logger = logger;
            _sessionFactory = sessionFactory;
            _authorizationConfiguration = authorizationConfiguration;
            _datastoreClient = datastoreClient;
        }

        /// <summary>
        /// Request client version validation.
        /// </summary>
        /// <param name="sessionId">The session requesting client version validation.</param>
        /// <param name="protocol">The protocol that was sent.</param>
        [Protocol(ProtocolCommand.USER_CLIENT_VERSION_CHECK_REQ)]
        public void CheckVersion(Guid sessionId, [FromBody] PROTO_USER_CLIENT_VERSION_CHECK_REQ protocol)
        {
            _logger.LogInformation("AuthorizationController: User requesting client version check; Session: {Session}, Version: {Version}", sessionId, protocol.Version);

            if (!_sessionFactory.GetSession(sessionId, out var session) || session == null)
            {
                _logger.LogError("AuthorizationController: Failed to get session; Session: {Session}", sessionId);
                return;
            }

            var supportedClientVersions = _authorizationConfiguration.CurrentValue.SupportedGameVersions;
            if (supportedClientVersions == null)
            {
                _logger.LogError("AuthorizationController: No supported client versions were configured; Session: {Session}", session.Id);
                return;
            }

            for (var i = 0; i < supportedClientVersions.Length; i++)
            {
                var supportedVersion = supportedClientVersions[i];
                if (protocol.Version.Equals(supportedVersion))
                {
                    _logger.LogInformation("AuthorizationController: User client version supported; Session: {Session}, Version: {Version}", session.Id, protocol.Version);
                    session.Send(ProtocolCommand.USER_CLIENT_RIGHTVERSION_CHECK_ACK, new PROTO_USER_CLIENT_RIGHTVERSION_CHECK_ACK {XTrapKey = XTrapKey});
                    return;
                }
            }

            _logger.LogWarning("AuthorizationController: User client version not supported; Session: {Session}, Version: {Version}", session.Id, protocol.Version);
            session.Send(ProtocolCommand.USER_CLIENT_WRONGVERSION_CHECK_ACK, new PROTO_USER_CLIENT_WRONGVERSION_CHECK_ACK());
        }

        /// <summary>
        /// Request authorization.
        /// </summary>
        /// <param name="sessionId">The session requesting authorization.</param>
        /// <param name="protocol">The protocol that was sent.</param>
        [Protocol(ProtocolCommand.USER_US_LOGIN_REQ)]
        public async void Login(Guid sessionId, [FromBody] PROTO_USER_US_LOGIN_REQ protocol)
        {
            _logger.LogInformation("AuthorizationController: User logging in; Session: {Session}, Username: {Username}, Spawnapps: {Spawnapps}", sessionId, protocol.Username, protocol.Spawnapps);

            // example of an asynchronous s2s operation

            var response = await _datastoreClient.CheckPasswordAsync(protocol.Username, protocol.Password);
        }

        /// <summary>
        /// Request XTrap key validation.
        /// </summary>
        /// <param name="sessionId">The session requesting XTrap key validation.</param>
        /// <param name="protocol">The protocol that was sent.</param>
        [Protocol(ProtocolCommand.USER_XTRAP_REQ)]
        public void CheckXTrapKey(Guid sessionId, [FromBody] PROTO_USER_XTRAP_REQ protocol)
        {
        }
    }
}
