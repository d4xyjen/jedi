// <copyright file="Session.cs" company="Jedi Contributors">
// Copyright (c) Jedi Contributors. All rights reserved.
//  
// This software is licensed under the MIT license. Read the LICENSE file in the 
// repository for more information.
// </copyright>

using Jedi.Common.Api.Extensions;
using Jedi.Common.Contracts;
using Jedi.Common.Contracts.Protocols;
using Jedi.Common.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.Net.Sockets;

namespace Jedi.Common.Api.Sessions
{
    /// <summary>
    /// Base session class.
    /// </summary>
    public class Session : Entity
    {
        private readonly ILogger<Session> _logger;
        private readonly IOptionsMonitor<SessionConfiguration> _sessionConfiguration;
        private readonly MessageQueue _messageQueue;
        private readonly ISessionEventQueue _eventQueue;
        private readonly ISessionCryptography _sessionCryptography;
        private readonly TcpClient _tcpClient;
        private readonly Thread _sendThread;
        private readonly Thread _receiveThread;
        private readonly ManualResetEvent _sendPending;
        private ushort? _seed;

        /// <summary>
        /// Create a new <see cref="Session"/>.
        /// </summary>
        /// <param name="tcpClient">TCP client for the session to use.</param>
        /// <param name="logger">The logger for the session.</param>
        /// <param name="sessionConfiguration">The global session configuration.</param>
        /// <param name="eventQueue">Event queue for the session to use.</param>
        /// <param name="sessionCryptography">Cryptography for the session.</param>
        public Session(
            TcpClient tcpClient, 
            ILogger<Session> logger, 
            IOptionsMonitor<SessionConfiguration> sessionConfiguration, 
            ISessionEventQueue eventQueue,
            ISessionCryptography sessionCryptography)
        {
            _tcpClient = tcpClient;
            _logger = logger;
            _messageQueue = new MessageQueue(sessionConfiguration.CurrentValue.MaxMessageSize);
            _sessionConfiguration = sessionConfiguration;
            _eventQueue = eventQueue;
            _sessionCryptography = sessionCryptography;
            _sendThread = new Thread(Send) { IsBackground = true };
            _receiveThread = new Thread(Receive) { IsBackground = true };
            _sendPending = new ManualResetEvent(false);
            PendingOperations = new ConcurrentDictionary<Guid, CorrelatedProtocol?>();
        }

        /// <summary>
        /// Whether or not the session is active.
        /// </summary>
        public bool Connected => _tcpClient.Client.Connected;

        /// <summary>
        /// Whether or not heartbeat is enabled for the session.
        /// </summary>
        public bool HeartbeatEnabled { get; set; } = false;

        /// <summary>
        /// The session's pending asynchronous operations.
        /// These are tasks that run until the session receives a correlated protocol.
        /// </summary>
        public ConcurrentDictionary<Guid, CorrelatedProtocol?> PendingOperations { get; }

        /// <summary>
        /// Start the session.
        /// This starts 2 new threads in the background per session, one for sending and one for receiving.
        /// </summary>
        public void Start()
        {
            _sendThread.Start();
            _receiveThread.Start();

            _eventQueue.Enqueue(Id, SessionEventType.Start);
        }

        /// <summary>
        /// Set a new seed for the session.
        /// </summary>
        public ushort? Seed()
        {
            return Seed(_sessionCryptography.GenerateSeed());
        }

        /// <summary>
        /// Set a new seed for the session.
        /// </summary>
        public ushort? Seed(ushort seed)
        {
            return _seed = seed;
        }

        /// <summary>
        /// Send an empty message to the session.
        /// </summary>
        /// <param name="command">The type of protocol to send.</param>
        /// <returns>True if the message was sent, false if it wasn't.</returns>

        public bool Send(ProtocolCommand command)
        {
            return Send(command, null);
        }

        /// <summary>
        /// Send a protocol to the session.
        /// </summary>
        /// <param name="command">The type of protocol to send.</param>
        /// <param name="protocol">The protocol to send.</param>
        /// <returns>True if the message was sent, false if it wasn't.</returns>
        public bool Send(ProtocolCommand command, Protocol? protocol)
        {
            try
            {
                _logger.LogInformation("Session: Serializing message; Session: {Session}, Command: {Command}, Protocol: {Protocol}", Id, command, protocol?.GetType().FullName);

                using var stream = new MemoryStream();
                using var writer = new BinaryWriter(stream);
                using var serializer = new BinarySerializer();

                writer.Write((ushort) command);

                if (protocol != null)
                {
                    writer.Write(serializer.Serialize(protocol));
                }

                var body = stream.ToArray();
                var headerSize = body.Length <= byte.MaxValue ? 1 : 3;
                var data = new byte[headerSize + body.Length];
                
                // Write the length.
                // This is either written as a byte (for messages with length < 255)
                // or a ushort (for all other messages).
                if (body.Length <= byte.MaxValue)
                {
                    data[0] = (byte) body.Length;
                }
                else
                {
                    Buffer.BlockCopy(BitConverter.GetBytes((ushort) body.Length), 0, data, 1, 2);
                }

                Buffer.BlockCopy(body, 0, data, headerSize, body.Length);
                return Send(new ArraySegment<byte>(data, 0, data.Length));
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Session: Failed to send message; Session: {Session}, Command: {Command}, Protocol: {Protocol}", Id, command, protocol?.GetType().FullName);
                return false;
            }
        }

        /// <summary>
        /// Send a message to the session asynchronously.
        /// </summary>
        /// <param name="command">The type of protocol to send.</param>
        /// <param name="protocol">The protocol to send.</param>
        public Task SendAsync(ProtocolCommand command, Protocol protocol)
        {
            return Task.Run(() =>
            {
                Send(command, protocol);
            });
        }

        /// <summary>
        /// Send a protocol to the session asynchronously and get a protocol back.
        /// </summary>
        /// <param name="command">The type of protocol to send.</param>
        /// <param name="protocol">The protocol to send.</param>
        public Task<TResponse?> SendAsync<TResponse>(ProtocolCommand command, CorrelatedProtocol protocol) where TResponse : CorrelatedProtocol
        {
            return SendAsync<TResponse>(command, CancellationToken.None, protocol);
        }

        /// <summary>
        /// Send a protocol to the session asynchronously and get a protocol back with support for cancellation.
        /// </summary>
        /// <param name="command">The type of protocol to send.</param>
        /// <param name="cancellationToken">Cancellation token used to cancel the operation.</param>
        /// <param name="protocol">The protocol to send.</param>
        public Task<TResponse?> SendAsync<TResponse>(ProtocolCommand command, CancellationToken cancellationToken, CorrelatedProtocol protocol) where TResponse : CorrelatedProtocol
        {
            return Task.Run(async () =>
            {
                if (!PendingOperations.TryAdd(protocol.OperationId, null))
                {
                    return null;
                }

                Send(command, protocol);

                CorrelatedProtocol? response = null;
                while (!cancellationToken.IsCancellationRequested)
                {
                    if (PendingOperations.TryGetValue(protocol.OperationId, out response) && response != null)
                    {
                        break;
                    }

                    await Task.Delay(10, cancellationToken);
                }

                PendingOperations.TryRemove(protocol.OperationId, out _);
                return (TResponse?) response;

            }, cancellationToken);
        }

        /// <summary>
        /// Send a message to the session.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <returns>True if the message was sent, false if it wasn't.</returns>
        public bool Send(ArraySegment<byte> message)
        {
            if (message.Count > _sessionConfiguration.CurrentValue.MaxMessageSize)
            {
                _logger.LogError("Session: Failed to send message, size too big; Session: {Session}, Size: {Size}", Id, message.Count);
            }

            if (_messageQueue.Count >= _sessionConfiguration.CurrentValue.MessageBacklogPerSession)
            {
                _logger.LogError("Session: Failed to send message, backlog exceeded. Will destroy session for load balancing; Session: {Session}, Backlog: {Backlog}", Id, _sessionConfiguration.CurrentValue.MessageBacklogPerSession);

                Destroy();
                return false;
            }

            _messageQueue.Enqueue(message);
            _sendPending.Set();

            _logger.LogInformation("Session: Sent message; Session: {Session}, Size: {Size}", Id, message.Count);
            return true;
        }

        /// <summary>
        /// Continuously send messages to the session.
        /// This method will run until the thread is interrupted.
        /// </summary>
        protected void Send()
        {
            NetworkStream? stream = null;

            // wrap with try/catch, otherwise thread exceptions will 
            // not be thrown.

            try
            {
                stream = _tcpClient.GetStream(); // tcp client might have been disposed
                byte[]? payload = null;

                while (_tcpClient.Connected)
                {
                    _sendPending.Reset();

                    // dequeue and serialize all messages
                    if (_messageQueue.DequeueAndSerializeAll(ref payload, out var packetSize))
                    {
                        // payload shouldn't be null here, but just check to make sure
                        if (payload == null || !SendMessages(stream, payload, packetSize))
                        {
                            break;
                        }
                    }

                    _sendPending.WaitOne();
                }
            }
            catch (ThreadAbortException)
            {
                // this happens on stop, no need to log anything
            }
            catch (ThreadInterruptedException)
            {
                // happens when the session is destroyed
            }
            catch (Exception exception)
            {
                // this is probably important
                _logger.LogError(exception, "Session: Failed to run send thread; Session: {Session}", Id);
            }
            finally
            {
                stream?.Close(); // always cleanup the stream
                Destroy();
            }
        }

        /// <summary>
        /// Send messages as one packet to the session.
        /// </summary>
        /// <param name="stream">The stream to send the messages to.</param>
        /// <param name="payload">The payload to send the packet from.</param>
        /// <param name="packetSize">The size of the packet to send.</param>
        /// <returns>True if the packet was sent, false if it was not.</returns>
        protected bool SendMessages(NetworkStream stream, byte[] payload, int packetSize)
        {
            try
            {
                stream.Write(payload, 0, packetSize);
                return true;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Session: Failed to send packet; Session: {Session}", Id);
                return false;
            }
        }

        /// <summary>
        /// Continuously receive messages from the session.
        /// This method will run until the thread is interrupted.
        /// </summary>
        protected void Receive()
        {
            // wrap with try/catch, otherwise thread exceptions will 
            // not be thrown.

            try
            {
                var stream = _tcpClient.GetStream();

                var headerBuffer = new byte[2];
                var payloadBuffer = new byte[_sessionConfiguration.CurrentValue.MaxMessageSize];

                // keep enqueueing messages until we don't have any 
                // more data left or the queue is full

                while (true)
                {
                    if (!ReadMessage(stream, headerBuffer, payloadBuffer, out var payloadSize))
                    {
                        break;
                    }

                    var payload = new ArraySegment<byte>(payloadBuffer, 0, payloadSize);

                    if (_seed.HasValue)
                    {
                        _seed = _sessionCryptography.Xor(payload, _seed.Value);
                    }

                    _eventQueue.Enqueue(Id, SessionEventType.Protocol, payload);

                    if (_eventQueue.GetCounter(Id) >= _sessionConfiguration.CurrentValue.EventBacklogPerSession)
                    {
                        _logger.LogError("Session: Failed to receive message, event backlog exceeded. Will destroy session for load balancing; Session: {Session}, Backlog: {Backlog}", Id, _sessionConfiguration.CurrentValue.MessageBacklogPerSession);
                        break;
                    }
                }
            }
            catch (ThreadAbortException)
            {
                // this happens on stop, no need to log anything
            }
            catch (Exception exception)
            {
                // this is probably important
                _logger.LogError(exception, "Session: Failed to run receive thread; Session: {Session}", Id);
            }
            finally
            {
                Destroy();
            }
        }

        /// <summary>
        /// Reads a messages from a stream into two arrays and returns the number of bytes read.
        /// </summary>
        /// <param name="stream">The stream to read the message from.</param>
        /// <param name="headerBuffer">The header buffer to read the header into.</param>
        /// <param name="payloadBuffer">The buffer to read the payload into.</param>
        /// <param name="size">The size of the message that was read.</param>
        /// <returns>True if a message was read successfully, false if it was not.</returns>
        protected bool ReadMessage(NetworkStream stream, byte[] headerBuffer, byte[] payloadBuffer, out int size)
        {
            size = 0;

            if (payloadBuffer.Length != _sessionConfiguration.CurrentValue.MaxMessageSize)
            {
                _logger.LogError("Session: Failed to read message, payload size not equal to max message size; Session: {Session}, PayloadSize: {PayloadSize}, MaxMessageSize: {MaxMessageSize}", Id, payloadBuffer.Length, _sessionConfiguration.CurrentValue.MaxMessageSize);
                return false;
            }

            // if the message content size is less than or equal to 255 (i.e. can be represented by a single byte),
            // only use 1 byte for the header, else use 3, to support messages up to 65535 bytes in size (more than enough)
            // ->  packet size = size of header + size of content

            var headerSize = 1;
            if (!stream.ReadExactly(headerBuffer, headerSize))
            {
                return false;
            }

            if (headerBuffer[0] == 0)
            {
                headerSize = 2;

                // if we try to read the header from 2 bytes and still can't
                if (!stream.ReadExactly(headerBuffer, headerSize) || headerBuffer[0] == 0)
                {
                    return false;
                }
            }

            size = headerSize == 1 ? headerBuffer[0] : BitConverter.ToUInt16(headerBuffer);
            if (size > 0 && size <= _sessionConfiguration.CurrentValue.MaxMessageSize)
            {
                return stream.ReadExactly(payloadBuffer, size);
            }

            _logger.LogWarning("Session: Possible header attack; Session: {Session}, MessageSize: {MessageSize}", Id, size);
            return false;
        }

        /// <summary>
        /// Complete an asynchronous operation.
        /// </summary>
        /// <param name="operationId">The operation's identifier.</param>
        /// <param name="response">The response for the operation.</param>
        public bool CompleteOperation(Guid operationId, CorrelatedProtocol response)
        {
            return PendingOperations.TryUpdate(operationId, response, null);
        }

        /// <summary>
        /// Dispose of the session.
        /// This is an internal method and should not be called. Entities should use Destroy() instead.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    _sendThread.Interrupt();
                    _receiveThread.Interrupt();

                    _messageQueue.Clear();

                    _tcpClient.Close();
                }
            }
            catch (ObjectDisposedException)
            {
                // things may have already been disposed
                // in this case, an object disposed exception will be thrown, which can be ignored
            }
            finally
            {
                _eventQueue.Enqueue(Id, SessionEventType.Destroy);
            }
        }
    }
}
