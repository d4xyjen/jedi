// <copyright file="SessionAcceptor.cs" company="Jedi Contributors">
// Copyright (c) Jedi Contributors. All rights reserved.
//  
// This software is licensed under the MIT license. Read the LICENSE file in the 
// repository for more information.
// </copyright>

using System.Net;
using System.Net.Sockets;
using Jedi.Common.Contracts;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Jedi.Common.Api.Sessions
{
    /// <summary>
    /// TCP session acceptor.
    /// </summary>
    public class SessionAcceptor : BackgroundService
    {
        private readonly ILogger<SessionAcceptor> _logger;
        private readonly ISessionFactory _sessionFactory;
        private readonly IOptionsMonitor<SessionConfiguration> _sessionConfiguration;
        private TcpListener? _listener;
        private EndPoint? _serviceEndpoint;

        /// <summary>
        /// Create a new <see cref="SessionAcceptor"/>
        /// </summary>
        /// <param name="logger">The session acceptor's logger.</param>
        /// <param name="sessionFactory">Factory used to create sessions.</param>
        /// <param name="sessionConfiguration">The session configuration.</param>
        public SessionAcceptor(ILogger<SessionAcceptor> logger, ISessionFactory sessionFactory, IOptionsMonitor<SessionConfiguration> sessionConfiguration)
        {
            _logger = logger;
            _sessionFactory = sessionFactory;
            _sessionConfiguration = sessionConfiguration;
        }

        /// <summary>
        /// Initialize the session acceptor.
        /// </summary>
        /// <param name="cancellationToken">The host's cancellation token.</param>
        /// <returns>The start task.</returns>
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("SessionAcceptor: Starting session acceptor");

            // providing a base address in network settings will override all
            // default base address behavior, like dynamic ports and IP loopback
            IPEndPoint? serviceEndpoint = null;
            if (!string.IsNullOrEmpty(_sessionConfiguration.CurrentValue.ServiceEndpoint) && !IPEndPoint.TryParse(_sessionConfiguration.CurrentValue.ServiceEndpoint, out serviceEndpoint))
            {
                // no need for this to be an error, just warn that we're ignoring
                // and fallback to the default settings later
                _logger.LogWarning("SessionAcceptor: Invalid endpoint specified for service, ignoring; Endpoint: {ServiceEndpoint}", _sessionConfiguration.CurrentValue.ServiceEndpoint);
            }

            // start the listener on all IP addresses, the loopback address, or a
            // specified base network address
            _listener = new TcpListener(serviceEndpoint ?? new IPEndPoint(_sessionConfiguration.CurrentValue.Loopback ? IPAddress.Loopback : IPAddress.Any, 0))
            {
                Server =
                {
                    NoDelay = _sessionConfiguration.CurrentValue.NoDelay
                }
            };

            _listener.Start(_sessionConfiguration.CurrentValue.AcceptorBacklog);
            _serviceEndpoint = _listener.LocalEndpoint; // get the actual base address after starting the listener

            _logger.LogInformation("SessionAcceptor: Session acceptor started; Endpoint: {ServiceEndpoint}", _serviceEndpoint);
            return base.StartAsync(cancellationToken);
        }

        /// <summary>
        /// Called when the session acceptor starts.
        /// This method executes the session acceptor's work.
        /// This mainly includes handling session events in the receive pipe.
        /// </summary>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> used to stop the service.</param>
        /// <returns>A <see cref="Task"/>.</returns>
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            // since this core loop is also being ran in the background
            // we still want to catch exceptions

            try
            {
                // typically, this should never happen, since we initialize the listener
                // in the StartAsync method above, but just to be safe
                if (_listener == null)
                {
                    _logger.LogError("SessionAcceptor: Listener was not initialized properly");
                    return;
                }

                // start of session lifecycle
                while (!cancellationToken.IsCancellationRequested)
                {
                    _sessionFactory.CreateSession(await _listener.AcceptTcpClientAsync(cancellationToken));
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "SessionAcceptor: An exception occurred while executing the acceptor");
            }
        }

        /// <summary>
        /// Stop the session acceptor.
        /// </summary>
        /// <param name="cancellationToken">The host's cancellation token.</param>
        /// <returns>The stop task.</returns>
        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("SessionAcceptor: Stopping session acceptor");

            await base.StopAsync(cancellationToken); // wait for the execution thread to finish first

            // stop listening for and accepting new sessions
            _listener?.Stop();

            _logger.LogInformation("SessionAcceptor: Stopped session acceptor");
        }
    }
}
