// <copyright file="LoggerConfiguration.cs" company="Jedi Contributors">
// Copyright (c) Jedi Contributors. All rights reserved.
//  
// This software is licensed under the MIT license. Read the LICENSE file in the 
// repository for more information.
// </copyright>

using Jedi.Common.Logging.Events;
using Jedi.Common.Logging.Sinks;
using System.Diagnostics;

namespace Jedi.Common.Logging
{
    /// <summary>
    /// Configures a <see cref="ILogger"/>.
    /// </summary>
    public class LoggerConfiguration
    {
        /// <summary>
        /// Globally shared default logging configuration.
        /// </summary>
        public static readonly LoggerConfiguration Default = new LoggerConfiguration();

        /// <summary>
        /// The name of the logger.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// The configured log sinks.
        /// </summary>
        public List<ILogSink> Sinks { get; } = new List<ILogSink>();

        /// <summary>
        /// If true, debug events will be logged.
        /// By default, this is set to true if a debugger is attached to the process.
        /// </summary>
        public bool IncludeDebug { get; set; } = Debugger.IsAttached;

        /// <summary>
        /// The minimum event severity to be logged in release builds.
        /// </summary>
        public LogEventSeverity MinimumSeverity { get; set; } = LogEventSeverity.Information;

        /// <summary>
        /// Loggers using this configuration.
        /// </summary>
        private ILogger? _logger;

        /// <summary>
        /// Add a custom log sink to the log's configuration.
        /// </summary>
        public LoggerConfiguration AddSink(ILogSink sink)
        {
            Sinks.Add(sink);
            return this;
        }

        /// <summary>
        /// Add a console sink to the log's configuration.
        /// Logs will be written to the console.
        /// </summary>
        public LoggerConfiguration AddConsole()
        {
            if (Sinks.Contains(ConsoleSink.Default))
            {
                return this;
            }

            Sinks.Add(ConsoleSink.Default);
            return this;
        }

        /// <summary>
        /// Add a file sink to the log's configuration.
        /// Logs will be written to this file.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        public LoggerConfiguration AddFile(string path)
        {
            Sinks.Add(new FileSink(path));
            return this;
        }

        /// <summary>
        /// Create a thread-safe <see cref="ILogger"/> using the specified configuration.
        /// </summary>
        /// <returns>A logger.</returns>
        public ILogger CreateLogger()
        {
            return _logger ??= new Logger(this);
        }

        /// <summary>
        /// Delete the logger.
        /// Logs will no longer be written.
        /// </summary>
        public void DeleteLogger()
        {
            _logger = null;
        }

        /// <summary>
        /// Clear the log sinks.
        /// </summary>
        public void ClearSinks()
        {
            Sinks.Clear();
        }
    }
}
