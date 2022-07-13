// <copyright file="SessionEvent.cs" company="Jedi Contributors">
// Copyright (c) Jedi Contributors. All rights reserved.
//  
// This software is licensed under the MIT license. Read the LICENSE file in the 
// repository for more information.
// </copyright>

namespace Jedi.Common.Api.Sessions
{
    /// <summary>
    /// An event fired by a session.
    /// </summary>
    public struct SessionEvent
    {
        /// <summary>
        /// Create a new <see cref="SessionEvent"/>.
        /// </summary>
        /// <param name="sessionId">The session the event pertains to.</param>
        /// <param name="type">The type of the event.</param>
        /// <param name="message">A message from the event.</param>
        public SessionEvent(Guid sessionId, SessionEventType type, ArraySegment<byte> message)
        {
            Session = sessionId;
            Type = type;
            Message = message;
        }

        /// <summary>
        /// The GUID of the session this event pertains to.
        /// </summary>
        public Guid Session { get; set; }

        /// <summary>
        /// The type of the event.
        /// </summary>
        public SessionEventType Type { get; set; }

        /// <summary>
        /// The event's message.
        /// </summary>
        public ArraySegment<byte> Message { get; set; }
    }
}
