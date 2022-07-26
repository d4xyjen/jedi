// <copyright file="Command.cs" company="Jedi Contributors">
// Copyright (c) Jedi Contributors. All rights reserved.
//  
// This software is licensed under the MIT license. Read the LICENSE file in the 
// repository for more information.
// </copyright>

using Jedi.Common.Contracts.Protocols;

namespace Jedi.Common.Contracts.Serialization
{
    /// <summary>
    /// Specifies a protocol's command.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class CommandAttribute : Attribute
    {
        /// <summary>
        /// The protocol's command.
        /// </summary>
        public ProtocolCommand Command { get; set; }

        /// <summary>
        /// Create a new <see cref="CommandAttribute"/>.
        /// </summary>
        /// <param name="command">The protocol's command.</param>
        public CommandAttribute(ProtocolCommand command)
        {
            Command = command;
        }
    }
}
