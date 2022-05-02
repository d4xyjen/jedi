// <copyright file="ILogger.cs" company="Jedi Contributors">
// Copyright (c) Jedi Contributors. All rights reserved.
//  
// This software is licensed under the MIT license. Read the LICENSE file in the 
// repository for more information.
// </copyright>

using Jedi.Common.Logging.Events;
using System.Diagnostics;

namespace Jedi.Common.Logging
{
    /// <summary>
    /// Core logger implementation.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Log an event.
        /// </summary>
        /// <param name="severity">The severity of the event.</param>
        /// <param name="correlationId">The event's correlation identifier.</param>
        /// <param name="methodName">The method the event was fired in.</param>
        /// <param name="exception">Exception associated with the log event.</param>
        /// <param name="stackTrace">Stack trace associated with the log event.</param>
        /// <param name="messageTemplate">The template, or format, of the event message.</param>
        /// <param name="parameters">The event's arguments.</param>
        public void LogEvent(LogEventSeverity severity, Guid? correlationId, string? methodName, Exception? exception, StackTrace? stackTrace, string messageTemplate, params object?[] parameters);
    }
}
