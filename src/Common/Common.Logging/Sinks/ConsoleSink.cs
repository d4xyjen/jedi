// <copyright file="ConsoleSink.cs" company="Jedi Contributors">
// Copyright (c) Jedi Contributors. All rights reserved.
//  
// This software is licensed under the MIT license. Read the LICENSE file in the 
// repository for more information.
// </copyright>

using Jedi.Common.Logging.Events;

namespace Jedi.Common.Logging.Sinks
{
    /// <summary>
    /// Log messages to the console.
    /// </summary>
    internal class ConsoleSink : ILogSink
    {
        /// <summary>
        /// The globally shared default console sink.
        /// </summary>
        public static ConsoleSink Default = new ConsoleSink();

        /// <summary>
        /// Only allow one console sink.
        /// </summary>
        private ConsoleSink()
        {
        }

        /// <summary>
        /// Log an event.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="logEvent">The log event that was fired.</param>
        public void CatchEvent(ILogger logger, LogEvent logEvent)
        {
            Console.BackgroundColor = GetColorForLog(logEvent);
            Console.Write($"{GetTagForLog(logEvent)}");
            
            Console.ResetColor();
            Console.WriteLine($": {logEvent.Timestamp} {logEvent}");
        }

        /// <summary>
        /// Gets the appropriate tag for the log.
        /// </summary>
        /// <param name="logEvent">The log event that was fired.</param>
        /// <returns>The tag for the log.</returns>
        private static string GetTagForLog(LogEvent logEvent)
        {
            switch (logEvent.Severity)
            {
                case LogEventSeverity.Debug:
                    return "debug";
                case LogEventSeverity.Information:
                    return "info";
                case LogEventSeverity.Warning:
                    return "warning";
                case LogEventSeverity.Error:
                    return "error";
                case LogEventSeverity.Fatal:
                    return "fatal";
                default:
                    return "";
            }
        }

        /// <summary>
        /// Gets the color of a log's tag.
        /// </summary>
        /// <param name="logEvent">The log event that was fired.</param>
        /// <returns>The color for the tag.</returns>
        private static ConsoleColor GetColorForLog(LogEvent logEvent)
        {
            switch (logEvent.Severity)
            {
                case LogEventSeverity.Information:
                    return ConsoleColor.DarkGreen;
                case LogEventSeverity.Warning:
                    return ConsoleColor.DarkMagenta;
                case LogEventSeverity.Error:
                    return ConsoleColor.DarkRed;
                case LogEventSeverity.Fatal:
                    return ConsoleColor.Red;
                default:
                    return ConsoleColor.Black;
            }
        }
    }
}
