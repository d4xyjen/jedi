// <copyright file="PROTO_MISC_HEARTBEAT_ACK.cs" company="Jedi Contributors">
// Copyright (c) Jedi Contributors. All rights reserved.
//  
// This software is licensed under the MIT license. Read the LICENSE file in the 
// repository for more information.
// </copyright>

using Jedi.Common.Contracts.Serialization;
using System.Runtime.Serialization;

namespace Jedi.Common.Contracts.Protocols.Miscellaneous
{
    /// <summary>
    /// Send a heartbeat.
    /// </summary>
    [DataContract]
    [Command(ProtocolCommand.MISC_HEARTBEAT_ACK)]
    public class PROTO_MISC_HEARTBEAT_ACK : Protocol
    {
    }
}
