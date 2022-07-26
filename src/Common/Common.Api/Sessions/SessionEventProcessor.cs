// <copyright file="SessionEventProcessor.cs" company="Jedi Contributors">
// Copyright (c) Jedi Contributors. All rights reserved.
//  
// This software is licensed under the MIT license. Read the LICENSE file in the 
// repository for more information.
// </copyright>

using Jedi.Common.Api.Messaging;
using Jedi.Common.Contracts;
using Jedi.Common.Contracts.Protocols;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Jedi.Common.Api.Sessions
{
    /// <summary>
    /// Background event processor for sessions.
    /// </summary>
    public class SessionEventProcessor : BackgroundService
    {
        private readonly ILogger<SessionEventProcessor> _logger;
        private readonly IOptionsMonitor<SessionConfiguration> _sessionConfiguration;
        private readonly ISessionEventQueue _eventQueue;
        private readonly IProtocolHandlerFactory _protocolHandlerFactory;
        private readonly ISessionFactory _sessionFactory;

        /// <summary>
        /// Create a new <see cref="SessionEventProcessor"/>
        /// </summary>
        /// <param name="logger">The session event handler's logger.</param>
        /// <param name="sessionConfiguration">The session configuration.</param>
        /// <param name="eventQueue">The global event queue.</param>
        /// <param name="protocolHandlerFactory">Handler factory for messages.</param>
        /// <param name="sessionFactory">Session factory.</param>
        public SessionEventProcessor(
            ILogger<SessionEventProcessor> logger,
            IOptionsMonitor<SessionConfiguration> sessionConfiguration, 
            ISessionEventQueue eventQueue,
            IProtocolHandlerFactory protocolHandlerFactory,
            ISessionFactory sessionFactory)
        {
            _logger = logger;
            _sessionConfiguration = sessionConfiguration;
            _eventQueue = eventQueue;
            _protocolHandlerFactory = protocolHandlerFactory;
            _sessionFactory = sessionFactory;
        }

        /// <summary>
        /// Initialize the session event handler.
        /// </summary>
        /// <param name="cancellationToken">The host's cancellation token.</param>
        /// <returns>The start task.</returns>
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("SessionEventProcessor: Event processor starting");
            _logger.LogInformation("SessionEventProcessor: Event processor started");

            return base.StartAsync(cancellationToken);
        }

        /// <summary>
        /// Called when the session event handler starts.
        /// This method executes the session event handler's work.
        /// </summary>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> used to stop the service.</param>
        /// <returns>A <see cref="Task"/>.</returns>
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            try
            {
                // process session events
                while (!cancellationToken.IsCancellationRequested)
                {
                    // Only process up to X events at a time.
                    var throttlingLimit = _sessionConfiguration.CurrentValue.ThrottlingEnabled ? _sessionConfiguration.CurrentValue.ThrottlingLimit : int.MaxValue;
                    for (var i = 0; i < throttlingLimit; i++)
                    {
                        if (!_eventQueue.TryPeek(out var sessionId, out var eventType, out var protocol))
                        {
                            // no more events to process
                            break;
                        }

                        // process the event
                        switch (eventType)
                        {
                            case SessionEventType.Start:
                                _logger.LogInformation("SessionEventProcessor: Session starting; Session: {Session}", sessionId);

                                _logger.LogInformation("SessionEventProcessor: Session started; Session: {Session}", sessionId);
                                break;
                            case SessionEventType.Protocol:
                                if (protocol.Array == null)
                                {
                                    _logger.LogError("SessionEventProcessor: Received empty protocol; Session: {Session}", sessionId);
                                    break;
                                }

                                if (!_protocolHandlerFactory.GetCommand(protocol, out var command))
                                {
                                    _logger.LogError("SessionEventProcessor: Failed to identify protocol; Session: {Session}", sessionId);
                                    break;
                                }

                                _logger.LogInformation("SessionEventProcessor: Received protocol; Session: {Session}, Command: {Command}", sessionId, (ProtocolCommand) command);

                                if (!await _protocolHandlerFactory.DeserializeAndHandleAsync(command, sessionId, new ArraySegment<byte>(protocol.Array, protocol.Offset + 2, protocol.Count - 2)))
                                {
                                    _logger.LogError("SessionEventProcessor: Failed to deserialize and handle protocol; Session: {Session}, Command: {Command}", sessionId, (ProtocolCommand) command);
                                }

                                break;
                            case SessionEventType.Destroy:
                                _logger.LogInformation("SessionEventProcessor: Destroying session; Session: {Session}", sessionId);

                                if (_sessionFactory.DestroySession(sessionId, out var unused))
                                {
                                    _logger.LogInformation("SessionEventProcessor: Session destroyed; Session: {Session}", sessionId);
                                }

                                break;
                            default:
                                _logger.LogWarning("SessionEventProcessor: Processed unknown event type; Event: {EventType}, Session: {Session}", eventType, sessionId);
                                break;
                        }

                        _eventQueue.TryDequeue();
                    }

                    await Task.Delay(10, cancellationToken);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "SessionEventProcessor: An exception occurred while processing events");
            }
        }

        /// <summary>
        /// Stop the session event processor.
        /// </summary>
        /// <param name="cancellationToken">The host's cancellation token.</param>
        /// <returns>The stop task.</returns>
        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("SessionEventProcessor: Stopping event processor");

            await base.StopAsync(cancellationToken); // wait for the execution thread to finish first

            _eventQueue.Clear();

            _logger.LogInformation("SessionEventProcessor: Stopped event processor");
        }
    }
}
