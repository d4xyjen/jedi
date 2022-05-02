// <copyright file="ILogSink.cs" company="Jedi Contributors">
// Copyright (c) Jedi Contributors. All rights reserved.
//  
// This software is licensed under the MIT license. Read the LICENSE file in the 
// repository for more information.
// </copyright>

using Jedi.Common.Logging.Events;

namespace Jedi.Common.Logging.Sinks
{
    /// <summary>
    /// Interface for log event sinks.
    /// </summary>
    public interface ILogSink
    {
        /// <summary>
        /// Log an event.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="logEvent">The log event that was fired.</param>
        public void CatchEvent(ILogger logger, LogEvent logEvent);
    }
}
