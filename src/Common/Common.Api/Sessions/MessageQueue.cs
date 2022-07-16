// <copyright file="MessageQueue.cs" company="Jedi Contributors">
// Copyright (c) Jedi Contributors. All rights reserved.
//  
// This software is licensed under the MIT license. Read the LICENSE file in the 
// repository for more information.
// </copyright>

using Jedi.Common.Core;

namespace Jedi.Common.Api.Sessions
{
    /// <summary>
    /// Thread-safe mechanism used to send messages.
    /// Message queues are created on a per-session basis.
    /// </summary>
    public class MessageQueue
    {
        private readonly Queue<ArraySegment<byte>> _messageQueue = new Queue<ArraySegment<byte>>();
        private readonly Pool<byte[]> _arrayPool;

        /// <summary>
        /// Create a new <see cref="MessageQueue"/> to send messages through.
        /// </summary>
        /// <param name="payloadSize">The payload size.</param>
        public MessageQueue(int payloadSize)
        {
            _arrayPool = new Pool<byte[]>(() => new byte[payloadSize]);
        }

        /// <summary>
        /// The number of messages currently in the queue.
        /// </summary>
        public int Count
        {
            get
            {
                lock (this)
                {
                    return _messageQueue.Count;
                }
            }
        }

        /// <summary>
        /// Enqueue a message.
        /// The segment's array is only used until Enqueue() returns.
        /// </summary>
        /// <param name="message">The message to queue.</param>
        public void Enqueue(ArraySegment<byte> message)
        {
            if (message.Array == null)
            {
                return; // the original array might be null, don't enqueue
            }

            lock (this)
            {
                // get a byte array from the pool, indicate which part of the array
                // is actually the message, copy the message into it, and enqueue it
                var bytes = _arrayPool.Take();
                var data = new ArraySegment<byte>(bytes, 0, message.Count);

                Buffer.BlockCopy(message.Array, message.Offset, bytes, 0, message.Count);
                _messageQueue.Enqueue(data);
            }
        }

        /// <summary>
        /// Dequeue and serialize all messages from the queue into a single packet.
        /// This is much faster than getting a list with all messages and writing each one into the socket.
        /// </summary>
        /// <param name="payload">An existing payload to write the messages to.</param>
        /// <param name="packetSize">The size of the packet that was serialized.</param>
        /// <returns>True if any messages were serialized, false if they were not.</returns>
        public bool DequeueAndSerializeAll(ref byte[]? payload, out int packetSize)
        {
            lock (this)
            {
                packetSize = 0;

                // don't do anything if the queue is empty
                if (_messageQueue.Count == 0)
                {
                    return false;
                }

                // merge all pending messages into one packet to avoid TCP
                // overhead and improve performance
                foreach (var message in _messageQueue)
                {
                    packetSize += message.Count;
                }

                // create a new payload buffer if one hasn't been created or 
                // the previous one is too small
                if (payload == null || payload.Length < packetSize)
                {
                    payload = new byte[packetSize];
                }

                var position = 0;

                // dequeue all messages and serialize them into the packet
                while (_messageQueue.Count > 0)
                {
                    var message = _messageQueue.Dequeue();
                    if (message.Array == null)
                    {
                        continue;
                    }

                    Buffer.BlockCopy(message.Array, message.Offset, payload, position, message.Count);
                    position += message.Count;

                    // return the buffer to the pool so it can be reused (avoids allocation)
                    _arrayPool.Return(message.Array);
                }

                return true;
            }
        }

        /// <summary>
        /// Clear the send pipe, returning all message buffers back to the pool.
        /// </summary>
        public void Clear()
        {
            lock (this)
            {
                while (_messageQueue.Count > 0)
                {
                    var message = _messageQueue.Dequeue();
                    if (message.Array != null)
                    {
                        // clear the queue via dequeue to return each message's array to the pool
                        _arrayPool.Return(message.Array);
                    }
                }
            }
        }
    }
}
