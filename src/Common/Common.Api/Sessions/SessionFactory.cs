// <copyright file="SessionFactory.cs" company="Jedi Contributors">
// Copyright (c) Jedi Contributors. All rights reserved.
//  
// This software is licensed under the MIT license. Read the LICENSE file in the 
// repository for more information.
// </copyright>

using Jedi.Common.Contracts;
using Jedi.Common.Core.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;

namespace Jedi.Common.Api.Sessions
{
    /// <summary>
    /// Factory used to create sessions.
    /// </summary>
    public interface ISessionFactory
    {
        /// <summary>
        /// Get all active sessions.
        /// </summary>
        /// <returns></returns>
        public ConcurrentDictionary<Guid, Session> GetSessions();

        /// <summary>
        /// Get a session.
        /// </summary>
        /// <param name="sessionId">The session's ID.</param>
        /// <param name="session">The session being queried.</param>
        /// <returns>True if the session was found.</returns>
        public bool GetSession(Guid sessionId, out Session? session);

        /// <summary>
        /// Remove a session.
        /// </summary>
        /// <param name="sessionId">The session's ID.</param>
        /// <param name="session">The session that was removed.</param>
        public bool DestroySession(Guid sessionId, out Session? session);

        /// <summary>
        /// Create a new session.
        /// </summary>
        /// <param name="tcpClient">TCP client for the session to use.</param>
        /// <returns>The new session.</returns>
        public Session CreateSession(TcpClient tcpClient);

        /// <summary>
        /// Create a new session which is connected to the endpoint.
        /// </summary>
        /// <param name="serviceEndpoint">The endpoint of the service to connect to.</param>
        /// <param name="session">The session that was created.</param>
        /// <returns>Whether or not the session was created.</returns>
        public bool CreateSession(string serviceEndpoint, out Session? session);

        /// <summary>
        /// Create a new session which is connected to the endpoint.
        /// </summary>
        /// <param name="serviceEndpoint">The endpoint of the service to connect to.</param>
        /// <param name="session">The session that was created.</param>
        /// <returns>Whether or not the session was created.</returns>
        public bool CreateSession(IPEndPoint serviceEndpoint, out Session? session);
    }

    /// <summary>
    /// Factory used to create sessions.
    /// </summary>
    public class SessionFactory : ISessionFactory
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly IOptionsMonitor<SessionConfiguration> _sessionConfiguration;
        private readonly ISessionEventQueue _eventQueue;
        private readonly ISessionCryptography _sessionCryptography;
        private readonly ConcurrentDictionary<int, Session> _sessionsByEndpoint;
        private readonly ConcurrentDictionary<Guid, Session> _sessionsById;

        /// <summary>
        /// Create a new <see cref="SessionFactory"/> to create sessions.
        /// </summary>
        /// <param name="loggerFactory">Factory to create loggers for sessions.</param>
        /// <param name="sessionConfiguration">The global session configuration.</param>
        /// <param name="eventQueue">Receive pipe for sessions to use.</param>
        /// <param name="sessionCryptography">Global session cryptography.</param>
        public SessionFactory(
            ILoggerFactory loggerFactory, 
            IOptionsMonitor<SessionConfiguration> sessionConfiguration, 
            ISessionEventQueue eventQueue, 
            ISessionCryptography sessionCryptography)
        {
            _loggerFactory = loggerFactory;
            _sessionConfiguration = sessionConfiguration;
            _eventQueue = eventQueue;
            _sessionCryptography = sessionCryptography;
            _sessionsByEndpoint = new ConcurrentDictionary<int, Session>();
            _sessionsById = new ConcurrentDictionary<Guid, Session>();
        }

        /// <summary>
        /// Get all active sessions.
        /// </summary>
        /// <returns>A dictionary of all active sessions, mapped by ID.</returns>
        public ConcurrentDictionary<Guid, Session> GetSessions()
        {
            return _sessionsById;
        }

        /// <summary>
        /// Get a session.
        /// </summary>
        /// <param name="sessionId">The session's ID.</param>
        /// <param name="session">The session being queried.</param>
        /// <returns>True if the session was found.</returns>
        public bool GetSession(Guid sessionId, out Session? session)
        {
            return _sessionsById.TryGetValue(sessionId, out  session);
        }

        /// <summary>
        /// Remove a session.
        /// </summary>
        /// <param name="sessionId">The session's ID.</param>
        /// <param name="session">The session that was removed.</param>
        public bool DestroySession(Guid sessionId, out Session? session)
        {
            if (_sessionsById.TryRemove(sessionId, out session))
            {
                session.Destroy();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Create a new session.
        /// </summary>
        /// <param name="tcpClient">TCP client for the session to use.</param>
        /// <returns>The new session.</returns>
        public Session CreateSession(TcpClient tcpClient)
        {
            tcpClient.NoDelay = _sessionConfiguration.CurrentValue.NoDelay;
            tcpClient.SendTimeout = _sessionConfiguration.CurrentValue.SendTimeout;
            tcpClient.ReceiveTimeout = _sessionConfiguration.CurrentValue.ReceiveTimeout;

            var session = new Session(tcpClient, _loggerFactory.CreateLogger<Session>(), _sessionConfiguration, _eventQueue, _sessionCryptography);
            if (!_sessionsById.TryAdd(session.Id, session))
            {
                throw new SystemError($"SessionFactory: Failed to add session to lookup; Session: {session.Id}");
            }

            session.Start();
            return session;
        }

        /// <summary>
        /// Create a new session which is connected to the endpoint.
        /// </summary>
        /// <param name="serviceEndpoint">The endpoint of the service to connect to.</param>
        /// <param name="session">The session that was created.</param>
        /// <returns>Whether or not the session was created.</returns>
        public bool CreateSession(string serviceEndpoint, out Session? session)
        {
            return CreateSession(IPEndPoint.Parse(serviceEndpoint), out session);
        }

        /// <summary>
        /// Create a new session which is connected to the endpoint.
        /// </summary>
        /// <param name="serviceEndpoint">The endpoint of the service to connect to.</param>
        /// <param name="session">The session that was created.</param>
        /// <returns>Whether or not the session was created.</returns>
        public bool CreateSession(IPEndPoint serviceEndpoint, out Session? session)
        {
            var serviceEndpointHash = serviceEndpoint.GetHashCode();
            if (_sessionsByEndpoint.TryGetValue(serviceEndpointHash, out session) && session.Connected)
            {
                return true;
            }

            session?.Destroy();

            try
            {
                var client = new TcpClient();
                client.Connect(serviceEndpoint);

                if (client.Connected)
                {
                    session = _sessionsByEndpoint[serviceEndpointHash] = CreateSession(client);
                    return true;
                }
            }
            catch (SocketException)
            {
            }

            return false;
        }
    }
}
