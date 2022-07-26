// <copyright file="PROTO_USER_PASSWORD_CHECK_REQ.cs" company="Jedi Contributors">
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
    /// Requests a password check.
    /// </summary>
    [DataContract]
    [Command(ProtocolCommand.USER_PASSWORD_CHECK_REQ)]
    public class PROTO_USER_PASSWORD_CHECK_REQ : CorrelatedProtocol
    {
        /// <summary>
        /// The account username.
        /// </summary>
        [DataMember]
        [Length(256)]
        public string Username { get; set; }

        /// <summary>
        /// The password to check.
        /// </summary>
        [DataMember]
        [Length(32)]
        public string Password { get; set; }
    }
}
