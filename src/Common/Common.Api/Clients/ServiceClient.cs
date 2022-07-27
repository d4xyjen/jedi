// <copyright file="ServiceClient.cs" company="Jedi Contributors">
// Copyright (c) Jedi Contributors. All rights reserved.
//  
// This software is licensed under the MIT license. Read the LICENSE file in the 
// repository for more information.
// </copyright>

using Jedi.Common.Api.Constants;
using Jedi.Common.Api.Sessions;
using Jedi.Common.Contracts.Protocols;

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
        /// This method is used by compiled API's to send messages to
        /// a client.
        /// </summary>
        protected void Send(ProtocolCommand command, Protocol protocol)
        {
            Session.Send(command, protocol);
        }

        /// <summary>
        /// This method is used by compiled API's to send messages to
        /// a client asynchronously.
        /// </summary>
        protected async Task SendAsync(ProtocolCommand command, Protocol protocol)
        {
            await Session.SendAsync(command, protocol);
        }

        /// <summary>
        /// This method is used by compiled API's to send messages to
        /// a client asynchronously.
        /// </summary>
        protected async Task<TResponse?> SendAsync<TResponse>(ProtocolCommand command, Dto protocol) where TResponse : Dto
        {
            return await SendAsync<TResponse>(command, new CancellationTokenSource(TimeSpan.FromMilliseconds(AsyncConstants.AsyncSendTimeout)).Token, protocol);
        }

        /// <summary>
        /// This method is used by compiled API's to send messages to
        /// a client asynchronously with cancellation support.
        /// </summary>
        protected async Task<TResponse?> SendAsync<TResponse>(ProtocolCommand command, CancellationToken cancellationToken, Dto protocol) where TResponse : Dto
        {
            try
            {
                return await Session.SendAsync<TResponse>(command, cancellationToken, protocol);
            }
            catch (OperationCanceledException)
            {
                return default;
            }
        }
    }
}
