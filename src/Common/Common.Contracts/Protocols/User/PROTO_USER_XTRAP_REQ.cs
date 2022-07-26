// <copyright file="PROTO_USER_XTRAP_REQ.cs" company="Jedi Contributors">
// Copyright (c) Jedi Contributors. All rights reserved.
//
// This software is licensed under the MIT license. Read the LICENSE file in the
// repository for more information.
// </copyright>

using Jedi.Common.Contracts.Serialization;
using System.Runtime.Serialization;

namespace Jedi.Common.Contracts.Protocols.User
{
    /// <summary>
    /// Verifies the client's XTrap key.
    /// </summary>
    [DataContract]
    [Command(ProtocolCommand.USER_XTRAP_REQ)]
    public class PROTO_USER_XTRAP_REQ : Protocol
    {
        /// <summary>
        /// The client's XTrap key.
        /// </summary>
        [DataMember]
        public byte[] XTrapKey { get; set; }
    }
}
