// <copyright file="ProtocolAttribute.cs" company="Jedi Contributors">
// Copyright (c) Jedi Contributors. All rights reserved.
//  
// This software is licensed under the MIT license. Read the LICENSE file in the 
// repository for more information.
// </copyright>

using Jedi.Common.Contracts.Protocols;

namespace Jedi.Common.Api
{
    /// <summary>
    /// Marks a method that defines how a protocol of a specific type
    /// should be handled when received from a session.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class ProtocolAttribute : Attribute
    {
        /// <summary>
        /// Create a new <see cref="ProtocolAttribute"/>.
        /// </summary>
        /// <param name="command">The <see cref="ProtocolCommand"/> that the method handles.</param>
        public ProtocolAttribute(ProtocolCommand command)
        {
            Command = command;
        }

        /// <summary>
        /// The command that the method handles.
        /// </summary>
        public ProtocolCommand Command { get; set; }
    }
}
