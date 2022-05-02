// <copyright file="FileSink.cs" company="Jedi Contributors">
// Copyright (c) Jedi Contributors. All rights reserved.
//  
// This software is licensed under the MIT license. Read the LICENSE file in the 
// repository for more information.
// </copyright>

using Jedi.Common.Logging.Events;

namespace Jedi.Common.Logging.Sinks
{
    /// <summary>
    /// Log messages to a file.
    /// </summary>
    internal class FileSink : ILogSink, IDisposable
    {
        /// <summary>
        /// The underlying <see cref="StreamWriter"/> that writes to the file.
        /// </summary>
        private readonly StreamWriter _writer;

        /// <summary>
        /// Create a new <see cref="FileSink"/>.
        /// </summary>
        /// <param name="path">The path to the log file.</param>
        public FileSink(string path)
        {
            _writer = new StreamWriter(path, true) { AutoFlush = true };
        }

        /// <summary>
        /// Log an event.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="logEvent">The log event that was fired.</param>
        public void CatchEvent(ILogger logger, LogEvent logEvent)
        {
            _writer.WriteLine(logEvent);
        }

        /// <summary>
        /// Dispose of the file sink.
        /// </summary>
        public void Dispose()
        {
            _writer.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
