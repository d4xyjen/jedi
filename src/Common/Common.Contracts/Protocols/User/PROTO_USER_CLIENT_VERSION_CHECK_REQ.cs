// <copyright file="PROTO_USER_CLIENT_VERSION_CHECK_REQ.cs" company="Jedi Contributors">
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
    /// Requests a version check.
    /// </summary>
    [DataContract]
    [Command(ProtocolCommand.USER_CLIENT_VERSION_CHECK_REQ)]
    public class PROTO_USER_CLIENT_VERSION_CHECK_REQ : Protocol
    {
        /// <summary>
        /// The client's version key.
        /// </summary>
        [DataMember]
        [Length(64)]
        public string Version { get; set; }
    }
}
