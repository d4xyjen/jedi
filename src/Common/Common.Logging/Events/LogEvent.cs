// <copyright file="LogEvent.cs" company="Jedi Contributors">
// Copyright (c) Jedi Contributors. All rights reserved.
//  
// This software is licensed under the MIT license. Read the LICENSE file in the 
// repository for more information.
// </copyright>

using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Jedi.Common.Logging.Events
{
    /// <summary>
    /// Core log event that is fired by a logger.
    /// </summary>
    public class LogEvent
    {
        /// <summary>
        /// The event's correlation identifier.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Guid? CorrelationId { get; }

        /// <summary>
        /// The method the event was fired in.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? MethodName { get; }

        /// <summary>
        /// An associated exception.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Exception? Exception { get; }

        /// <summary>
        /// An associated stack trace.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public StackTrace? StackTrace { get; }

        /// <summary>
        /// The severity of the log event.
        /// </summary>
        public LogEventSeverity Severity { get; }

        /// <summary>
        /// The time the event was fired.
        /// </summary>
        public DateTime Timestamp { get; }

        /// <summary>
        /// Metadata about the event.
        /// </summary>
        [JsonConverter(typeof(EventMetadataJsonConverter))]
        public EventMetadata Metadata { get; }

        /// <summary>
        /// Create a new <see cref="LogEvent"/>.
        /// </summary>
        /// <param name="severity">The severity of the event.</param>
        /// <param name="timestamp">The time the event was fired.</param>
        /// <param name="correlationId">The correlation identifier of the event. Groups this event with related events.</param>
        /// <param name="methodName">The method the event was fired in.</param>
        /// <param name="exception">The exception associated with the event.</param>
        /// <param name="stackTrace">The stack trace associated with the event.</param>
        /// <param name="messageTemplate">The template used to generate the event's message.</param>
        /// <param name="parameters">Optional metadata that the message can reference.</param>
        public LogEvent(
            LogEventSeverity severity,
            DateTime timestamp,
            Guid? correlationId,
            string? methodName,
            Exception? exception,
            StackTrace? stackTrace,
            string messageTemplate,
            params object?[] parameters)
        {
            Severity = severity;
            Timestamp = timestamp;
            CorrelationId = correlationId;
            MethodName = methodName;
            Exception = exception;
            StackTrace = stackTrace;
            Metadata = EventMetadata.FromTemplate(messageTemplate, parameters);
        }

        /// <summary>
        /// Convert the event to Json.
        /// </summary>
        /// <returns>Serialized Json representing the log event.</returns>
        public string ToJson()
        {
            return JsonSerializer.Serialize(this);
        }

        /// <summary>
        /// Convert the event to a string.
        /// </summary>
        /// <returns>A string representing the log event.</returns>
        public override string ToString()
        {
            return Metadata.Message;
        }
    }
}
