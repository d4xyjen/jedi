// <copyright file="ILoggerExtensions.cs" company="Jedi Contributors">
// Copyright (c) Jedi Contributors. All rights reserved.
//  
// This software is licensed under the MIT license. Read the LICENSE file in the 
// repository for more information.
// </copyright>

using Jedi.Common.Logging.Events;
using System.Diagnostics;

namespace Jedi.Common.Logging.Extensions
{
    /// <summary>
    /// Wrapper methods for loggers.
    /// </summary>
    public static class LoggerExtensions
    {
        /// <summary>
        /// Log debugging information.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        /// <param name="messageTemplate">The template of the log message.</param>
        /// <param name="parameters">The event's parameters.</param>
        public static void LogDebug(this ILogger logger, string messageTemplate, params object?[] parameters)
        {
            LogDebug(logger, messageTemplate, null, null, null, null, parameters);
        }

        /// <summary>
        /// Log debugging information.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        /// <param name="messageTemplate">The template of the log message.</param>
        /// <param name="correlationId">The event's correlation identifier.</param>
        /// <param name="methodName">The method the event was fired in.</param>
        /// <param name="exception">Exception associated with the log event.</param>
        /// <param name="stackTrace">Stack trace associated with the log event.</param>
        /// <param name="parameters">The event's parameters.</param>
        public static void LogDebug(this ILogger logger, string messageTemplate, Guid? correlationId = null, string? methodName = null, Exception? exception = null, StackTrace? stackTrace = null, params object?[] parameters)
        {
            logger.LogEvent(LogEventSeverity.Debug, correlationId, methodName, exception, stackTrace, messageTemplate, parameters);
        }

        /// <summary>
        /// Log information about actions or events happening within the application.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        /// <param name="messageTemplate">The template of the log message.</param>
        /// <param name="parameters">The event's parameters.</param>
        public static void LogInformation(this ILogger logger, string messageTemplate, params object?[] parameters)
        {
            LogInformation(logger, messageTemplate, null, null, null, null, parameters);
        }

        /// <summary>
        /// Log information about actions or events happening within the application.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        /// <param name="messageTemplate">The template of the log message.</param>
        /// <param name="correlationId">The event's correlation identifier.</param>
        /// <param name="methodName">The method the event was fired in.</param>
        /// <param name="exception">Exception associated with the log event.</param>
        /// <param name="stackTrace">Stack trace associated with the log event.</param>
        /// <param name="parameters">The event's parameters.</param>
        public static void LogInformation(this ILogger logger, string messageTemplate, Guid? correlationId = null, string? methodName = null, Exception? exception = null, StackTrace? stackTrace = null, params object?[] parameters)
        {
            logger.LogEvent(LogEventSeverity.Information, correlationId, methodName, exception, stackTrace, messageTemplate, parameters);
        }

        /// <summary>
        /// Log a warning about a potential erorr.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        /// <param name="messageTemplate">The template of the log message.</param>
        /// <param name="parameters">The event's parameters.</param>
        public static void LogWarning(this ILogger logger, string messageTemplate, params object?[] parameters)
        {
            LogWarning(logger, messageTemplate, null, null, null, null, parameters);
        }

        /// <summary>
        /// Log a warning about a potential erorr.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        /// <param name="messageTemplate">The template of the log message.</param>
        /// <param name="correlationId">The event's correlation identifier.</param>
        /// <param name="methodName">The method the event was fired in.</param>
        /// <param name="exception">Exception associated with the log event.</param>
        /// <param name="stackTrace">Stack trace associated with the log event.</param>
        /// <param name="parameters">The event's parameters.</param>
        public static void LogWarning(this ILogger logger, string messageTemplate, Guid? correlationId = null, string? methodName = null, Exception? exception = null, StackTrace? stackTrace = null, params object?[] parameters)
        {
            logger.LogEvent(LogEventSeverity.Warning, correlationId, methodName, exception, stackTrace, messageTemplate, parameters);
        }

        /// <summary>
        /// Log information about an error.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        /// <param name="messageTemplate">The template of the log message.</param>
        /// <param name="parameters">The event's parameters.</param>
        public static void LogError(this ILogger logger, string messageTemplate, params object?[] parameters)
        {
            LogError(logger, messageTemplate, null, null, null, null, parameters);
        }

        /// <summary>
        /// Log information about an error.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        /// <param name="messageTemplate">The template of the log message.</param>
        /// <param name="correlationId">The event's correlation identifier.</param>
        /// <param name="methodName">The method the event was fired in.</param>
        /// <param name="exception">Exception associated with the log event.</param>
        /// <param name="stackTrace">Stack trace associated with the log event.</param>
        /// <param name="parameters">The event's parameters.</param>
        public static void LogError(this ILogger logger, string messageTemplate, Guid? correlationId = null, string? methodName = null, Exception? exception = null, StackTrace? stackTrace = null, params object?[] parameters)
        {
            logger.LogEvent(LogEventSeverity.Error, correlationId, methodName, exception, stackTrace, messageTemplate, parameters);
        }

        /// <summary>
        /// Log information about actions or events which caused the application to crash.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        /// <param name="messageTemplate">The template of the log message.</param>
        /// <param name="parameters">The event's parameters.</param>
        public static void LogFatal(this ILogger logger, string messageTemplate, params object?[] parameters)
        {
            LogFatal(logger, messageTemplate, null, null, null, null, parameters);
        }

        /// <summary>
        /// Log information about actions or events which caused the application to crash.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        /// <param name="messageTemplate">The template of the log message.</param>
        /// <param name="correlationId">The event's correlation identifier.</param>
        /// <param name="methodName">The method the event was fired in.</param>
        /// <param name="exception">Exception associated with the log event.</param>
        /// <param name="stackTrace">Stack trace associated with the log event.</param>
        /// <param name="parameters">The event's parameters.</param>
        public static void LogFatal(this ILogger logger, string messageTemplate, Guid? correlationId = null, string? methodName = null, Exception? exception = null, StackTrace? stackTrace = null, params object?[] parameters)
        {
            logger.LogEvent(LogEventSeverity.Fatal, correlationId, methodName, exception, stackTrace, messageTemplate, parameters);
        }
    }
}
