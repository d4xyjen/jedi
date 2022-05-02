// <copyright file="LogEventSeverity.cs" company="Jedi Contributors">
// Copyright (c) Jedi Contributors. All rights reserved.
//  
// This software is licensed under the MIT license. Read the LICENSE file in the 
// repository for more information.
// </copyright>
namespace Jedi.Common.Logging.Events
{
    /// <summary>
    /// The severity of a log event.
    /// </summary>
    public enum LogEventSeverity
    {
        /// <summary>
        /// An event that provides contextual information useful for debugging, usually ignored in release builds.
        /// </summary>
        Debug,

        /// <summary>
        /// An event that provides contextual information about actions or events happening within the application.
        /// </summary>
        Information,

        /// <summary>
        /// An event that warns of unwanted states within the application.
        /// </summary>
        Warning,

        /// <summary>
        /// An event that provides contextual information about errors occurring within the application.
        /// </summary>
        Error,

        /// <summary>
        /// An event that provides contextual information about actions or events which caused the application to crash.
        /// </summary>
        Fatal
    }
}
