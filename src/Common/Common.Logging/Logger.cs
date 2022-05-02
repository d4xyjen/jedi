// <copyright file="BaseLogger.cs" company="Jedi Contributors">
// Copyright (c) Jedi Contributors. All rights reserved.
//  
// This software is licensed under the MIT license. Read the LICENSE file in the 
// repository for more information.
// </copyright>

using Jedi.Common.Logging.Events;
using Jedi.Common.Threading;
using System.Diagnostics;

namespace Jedi.Common.Logging
{
    /// <summary>
    /// Core thread-safe implementation of the <see cref="ILogger"/> interface.
    /// </summary>
    internal class Logger : ILogger
    {
        /// <summary>
        /// The logger's configuration.
        /// </summary>
        public LoggerConfiguration Configuration { get; }

        /// <summary>
        /// The log's lock object.
        /// </summary>
        private readonly object _lockObject = new object();

        /// <summary>
        /// Create a new <see cref="Logger"/>.
        /// </summary>
        /// <param name="configuration">The logger's configuration.</param>
        public Logger(LoggerConfiguration configuration)
        {
            Configuration = configuration;
        }

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
        public void LogEvent(LogEventSeverity severity, Guid? correlationId, string? methodName, Exception? exception, StackTrace? stackTrace, string messageTemplate, params object?[] parameters)
        {
            if (severity < Configuration.MinimumSeverity && !(severity == LogEventSeverity.Debug && Configuration.IncludeDebug))
            {
                return;
            }

            using var scopedLock = new ScopedLock(_lockObject);

            // Log the event to each log sink.
            foreach (var sink in Configuration.Sinks)
            {
                sink.CatchEvent(this, new LogEvent(severity, DateTime.UtcNow, correlationId, methodName, exception, stackTrace, messageTemplate, parameters));
            }
        }
    }
}
