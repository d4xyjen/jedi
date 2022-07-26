// <copyright file="SessionEventQueue.cs" company="Jedi Contributors">
// Copyright (c) Jedi Contributors. All rights reserved.
//  
// This software is licensed under the MIT license. Read the LICENSE file in the 
// repository for more information.
// </copyright>

using Jedi.Common.Contracts;
using Jedi.Common.Core;
using Microsoft.Extensions.Options;

namespace Jedi.Common.Api.Sessions
{
    /// <summary>
    /// Thread-safe mechanism used to process events from sessions.
    /// </summary>
    public interface ISessionEventQueue
    {
        /// <summary>
        /// Get the number of events in the receive pipe from a session.
        /// </summary>
        /// <param name="sessionId">The session.</param>
        /// <returns>The number of events in the receive pipe from the session.</returns>
        public int GetCounter(Guid sessionId);

        /// <summary>
        /// Enqueue an event in the network's receive pipe.
        /// </summary>
        /// <param name="sessionId">The session that is enqueueing the event.</param>
        /// <param name="eventType">The type of the event.</param>
        /// <param name="message">The event's message.</param>
        public void Enqueue(Guid sessionId, SessionEventType eventType, ArraySegment<byte> message = default);

        /// <summary>
        /// Peek at the next event.
        /// </summary>
        /// <param name="sessionId">The session for the event.</param>
        /// <param name="eventType">The type of the event.</param>
        /// <param name="message">The event's message.</param>
        /// <returns>True if we were able to peek.</returns>
        public bool TryPeek(out Guid sessionId, out SessionEventType eventType, out ArraySegment<byte> message);

        /// <summary>
        /// Dequeue the next event.
        /// </summary>
        /// <returns>True if we were able to dequeue an event.</returns>
        public bool TryDequeue();

        /// <summary>
        /// Clear the pipe.
        /// </summary>
        public void Clear();
    }

    /// <summary>
    /// Thread-safe mechanism used to process events from sessions.
    /// </summary>
    public class SessionEventQueue : ISessionEventQueue
    {
        private readonly Queue<SessionEvent> _eventQueue = new Queue<SessionEvent>();
        private readonly Dictionary<Guid, int> _sessionEventCounter = new Dictionary<Guid, int>();
        private readonly Pool<byte[]> _arrayPool;

        /// <summary>
        /// Create a new <see cref="SessionEventQueue"/> to receive messages through.
        /// </summary>
        /// <param name="configuration">The session configuration.</param>
        public SessionEventQueue(IOptionsMonitor<SessionConfiguration> configuration)
        {
            _arrayPool = new Pool<byte[]>(() => new byte[configuration.CurrentValue.MaxMessageSize]);
        }

        /// <summary>
        /// Get the number of events in the receive pipe from a session.
        /// </summary>
        /// <param name="sessionId">The session.</param>
        /// <returns>The number of events in the receive pipe from the session.</returns>
        public int GetCounter(Guid sessionId)
        {
            lock (this)
            {
                return _sessionEventCounter.TryGetValue(sessionId, out var count) ? count : 0;
            }
        }

        /// <summary>
        /// Enqueue an event in the network's receive pipe.
        /// </summary>
        /// <param name="sessionId">The session that is enqueueing the event.</param>
        /// <param name="eventType">The type of the event.</param>
        /// <param name="message">The event's message.</param>
        public void Enqueue(Guid sessionId, SessionEventType eventType, ArraySegment<byte> message = default)
        {
            lock (this)
            {
                // does the message have data in its array?
                if (message != default && message.Array != null)
                {
                    var bytes = _arrayPool.Take();
                    Buffer.BlockCopy(message.Array, message.Offset, bytes, 0, message.Count);
                    message = new ArraySegment<byte>(bytes, 0, message.Count);
                }

                _eventQueue.Enqueue(new SessionEvent(sessionId, eventType, message));
                _sessionEventCounter[sessionId] = GetCounter(sessionId) + 1;
            }
        }

        /// <summary>
        /// Peek at the next event.
        /// </summary>
        /// <param name="sessionId">The session for the event.</param>
        /// <param name="eventType">The type of the event.</param>
        /// <param name="message">The event's message.</param>
        /// <returns>True if we were able to peek.</returns>
        public bool TryPeek(out Guid sessionId, out SessionEventType eventType, out ArraySegment<byte> message)
        {
            sessionId = Guid.Empty;
            eventType = SessionEventType.Destroy;
            message = default;

            lock (this)
            {
                if (_eventQueue.Count <= 0)
                {
                    return false;
                }

                // TryPeek and Dequeue need to be called from the same thread, so we use Peek
                var sessionEvent = _eventQueue.Peek();

                sessionId = sessionEvent.Session;
                eventType = sessionEvent.Type;
                message = sessionEvent.Message;

                return true;
            }
        }

        /// <summary>
        /// Dequeue the next event.
        /// </summary>
        /// <returns>True if we were able to dequeue an event.</returns>
        public bool TryDequeue()
        {
            lock (this)
            {
                if (_eventQueue.Count <= 0)
                {
                    return false;
                }

                var sessionEvent = _eventQueue.Dequeue();
                if (sessionEvent.Message != default && sessionEvent.Message.Array != null)
                {
                    _arrayPool.Return(sessionEvent.Message.Array);
                }

                _sessionEventCounter[sessionEvent.Session]--;

                if (_sessionEventCounter[sessionEvent.Session] <= 0)
                {
                    _sessionEventCounter.Remove(sessionEvent.Session);
                }

                return true;
            }
        }

        /// <summary>
        /// Clear the event queue.
        /// </summary>
        public void Clear()
        {
            lock (this)
            {
                while (_eventQueue.Count > 0)
                {
                    var sessionEvent = _eventQueue.Dequeue();
                    if (sessionEvent.Message != default && sessionEvent.Message.Array != null)
                    {
                        // return the byte array back to the pool.
                        _arrayPool.Return(sessionEvent.Message.Array);
                    }
                }

                _sessionEventCounter.Clear();
            }
        }
    }
}
