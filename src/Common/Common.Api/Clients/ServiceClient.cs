// <copyright file="ServiceClient.cs" company="Jedi Contributors">
// Copyright (c) Jedi Contributors. All rights reserved.
//  
// This software is licensed under the MIT license. Read the LICENSE file in the 
// repository for more information.
// </copyright>

using Jedi.Common.Api.Messaging;
using Jedi.Common.Api.Sessions;
using Jedi.Common.Contracts.Protocols;
using Jedi.Common.Core.Exceptions;

namespace Jedi.Common.Api.Clients
{
    /// <summary>
    /// Base class for service clients.
    /// </summary>
    public class ServiceClient
    {
        /// <summary>
        /// Create a new <see cref="ServiceClient"/>.
        /// </summary>
        /// <param name="session">The session used to communicate with the service.</param>
        public ServiceClient(Session session)
        {
            Session = session;
        }

        /// <summary>
        /// The session that the client uses to communicate with the service.
        /// </summary>
        public Session Session { get; set; }
        
        /// <summary>
        /// This method is used by generated API's to send messages to
        /// a client.
        /// </summary>
        protected void BuildAndSend(ProtocolType command, params Protocol[] args)
        {
            if (!Session.Send(command, args))
            {
                throw new SystemError($"Service client failed to send message of type {command} to session {Session.Id}");
            }
        }
    }
}
