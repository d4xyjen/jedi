// <copyright file="ProtocolHandlerAttribute.cs" company="Jedi Contributors">
// Copyright (c) Jedi Contributors. All rights reserved.
//  
// This software is licensed under the MIT license. Read the LICENSE file in the 
// repository for more information.
// </copyright>
namespace Jedi.Common.Api.Messaging
{
    /// <summary>
    /// Marks a method that defines how a protocol of a specific type
    /// should be handled when received from a session.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class ProtocolHandlerAttribute : Attribute
    {
        /// <summary>
        /// Create a new <see cref="ProtocolHandlerAttribute"/>.
        /// </summary>
        /// <param name="command">The <see cref="ProtocolType"/> that the method handles.</param>
        public ProtocolHandlerAttribute(ProtocolType command)
        {
            Command = command;
        }

        /// <summary>
        /// The command that the method handles.
        /// </summary>
        public ProtocolType Command { get; set; }
    }
}
