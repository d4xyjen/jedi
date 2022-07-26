// <copyright file="UserController.cs" company="Jedi Contributors">
// Copyright (c) Jedi Contributors. All rights reserved.
//  
// This software is licensed under the MIT license. Read the LICENSE file in the 
// repository for more information.
// </copyright>

using Jedi.Common.Api;
using Jedi.Common.Api.Messaging;
using Jedi.Common.Api.Sessions;
using Jedi.Common.Contracts.Protocols;
using Jedi.Common.Contracts.Protocols.User;

namespace Jedi.Datastore.EntryPoint.Controllers
{
    /// <summary>
    /// Handles user business logic.
    /// </summary>
    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;
        private readonly ISessionFactory _sessionFactory;

        /// <summary>
        /// Create a new <see cref="UserController"/>.
        /// </summary>
        /// <param name="logger">Logger for the controller to use.</param>
        /// <param name="sessionFactory">Factory used to get sessions.</param>
        public UserController(
            ILogger<UserController> logger,
            ISessionFactory sessionFactory)
        {
            _logger = logger;
            _sessionFactory = sessionFactory;
        }

        [Protocol(ProtocolCommand.USER_PASSWORD_CHECK_REQ)]
        public PROTO_USER_PASSWORD_CHECK_ACK USER_PASSWORD_CHECK_REQ(Guid sessionId, [FromBody] PROTO_USER_PASSWORD_CHECK_REQ protocol)
        {
            return new PROTO_USER_PASSWORD_CHECK_ACK()
            {
                OperationId = protocol.OperationId
            };
        }
    }
}
