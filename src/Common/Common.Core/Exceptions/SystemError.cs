// <copyright file="SystemError.cs" company="Jedi Contributors">
// Copyright (c) Jedi Contributors. All rights reserved.
//  
// This software is licensed under the MIT license. Read the LICENSE file in the 
// repository for more information.
// </copyright>
namespace Jedi.Common.Core.Exceptions
{
    /// <summary>
    /// An error that occurs due to a system failure.
    /// </summary>
    public class SystemError : Exception
    {
        /// <summary>
        /// Create a new <see cref="SystemError"/>.
        /// </summary>
        /// <param name="message">A message explaining the error that occurred.</param>
        public SystemError(string? message) : this(message, null)
        {
        }

        /// <summary>
        /// Create a new <see cref="SystemError"/>.
        /// </summary>
        /// <param name="message">A message explaining the error that occurred.</param>
        /// <param name="exception">An exception that was thrown.</param>
        public SystemError(string? message, Exception? exception) : base(message, exception)
        {
        }
    }
}
