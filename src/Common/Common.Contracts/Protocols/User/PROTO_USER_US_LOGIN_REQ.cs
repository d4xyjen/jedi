// <copyright file="PROTO_USER_US_LOGIN_REQ.cs" company="Jedi Contributors">
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
    /// Contains credentials that a client is attempting to login with.
    /// </summary>
    [DataContract]
    [Command(ProtocolCommand.USER_US_LOGIN_REQ)]
    public class PROTO_USER_US_LOGIN_REQ : Protocol
    {
        /// <summary>
        /// The username credential.
        /// </summary>
        [DataMember]
        [Length(260)]
        public string Username { get; set; }

        /// <summary>
        /// The password credential.
        /// </summary>
        [DataMember]
        [Length(36)]
        public string Password { get; set; }

        /// <summary>
        /// Spawn apps.
        /// </summary>
        [DataMember]
        [Length(20)]
        public string Spawnapps { get; set; }
    }
}
