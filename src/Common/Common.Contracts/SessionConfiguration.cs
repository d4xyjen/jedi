// <copyright file="SessionConfiguration.cs" company="Jedi Contributors">
// Copyright (c) Jedi Contributors. All rights reserved.
//  
// This software is licensed under the MIT license. Read the LICENSE file in the 
// repository for more information.
// </copyright>

using System.Net.Sockets;

namespace Jedi.Common.Contracts
{
    /// <summary>
    /// Session configuration options.
    /// </summary>
    public class SessionConfiguration
    {
        /// <summary>
        /// The service's base endpoint, e.g. '92.342.45.22' (static IP with dynamic port)
        /// or '127.0.0.1:3000' (static IP and port).
        /// If this isn't provided, any IP address and port will be used.
        /// </summary>
        public string? ServiceEndpoint { get; set; }

        /// <summary>
        /// Whether or not the external IP is set to Loopback (127.0.0.1) or All (0.0.0.0).
        /// Defaults to true.
        /// </summary>
        public bool Loopback { get; set; } = true;

        /// <summary>
        /// Whether or not the Nagle algorithm is used for the listening socket.
        /// Disabling this lowers CPU usage and latency, but increases bandwidth.
        /// </summary>
        public bool NoDelay { get; set; } = true;

        /// <summary>
        /// To prevent allocation attacks (attacker prefixing messages with a fake length
        /// header), limit the size of messages. Defaults to 16KB.
        /// </summary>
        public int MaxMessageSize { get; set; } = 16000;

        /// <summary>
        /// Limits the amount of events allowed per session in the receive pipe.
        /// Defaults to 10 thousand events.
        /// </summary>
        public int EventBacklogPerSession { get; set; } = 10000;

        /// <summary>
        /// Limits the amount of events allowed per session in the receive pipe.
        /// Defaults to 10 thousand messages.
        /// </summary>
        public int MessageBacklogPerSession { get; set; } = 10000;

        /// <summary>
        /// Timeout (in milliseconds) for sending data to prevent stalling if
        /// the network is cut off during a send.
        /// Defaults to 5 seconds.
        /// </summary>
        public int SendTimeout { get; set; } = 5000;

        /// <summary>
        /// Default TCP receive timeout can be huge (minutes).
        /// That's too much for games, so we make it configurable.
        /// Defaults to 0.
        /// </summary>
        public int ReceiveTimeout { get; set; } = 0;

        /// <summary>
        /// The backlog used for the session acceptor.
        /// Defaults to <see cref="SocketOptionName.MaxConnections"/>
        /// </summary>
        public int AcceptorBacklog { get; set; } = (int) SocketOptionName.MaxConnections;

        /// <summary>
        /// If enabled, session events in the event queue will be throttled.
        /// Defaults to enabled.
        /// </summary>
        public bool ThrottlingEnabled { get; set; } = true;

        /// <summary>
        /// The limit for session event queue throttling, if enabled.
        /// Defaults to 1 hundred events per tick.
        /// </summary>
        public int ThrottlingLimit { get; set; } = 100;
    }
}
