// <copyright file="PROTO_USER_CLIENT_RIGHTVERSION_CHECK_ACK.cs" company="Jedi Contributors">
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
    /// Notifies the client of a valid client version.
    /// </summary>
    [DataContract]
    [Command(ProtocolCommand.USER_CLIENT_RIGHTVERSION_CHECK_ACK)]
    public class PROTO_USER_CLIENT_RIGHTVERSION_CHECK_ACK : Protocol
    {
        /// <summary>
        /// The XTrap key.
        /// </summary>
        [DataMember]
        public byte[] XTrapKey { get; set; }
    }
}
