// <copyright file="PROTO_USER_PASSWORD_CHECK_ACK.cs" company="Jedi Contributors">
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
    /// Password check response protocol.
    /// </summary>
    [DataContract]
    [Command(ProtocolCommand.USER_PASSWORD_CHECK_ACK)]
    public class PROTO_USER_PASSWORD_CHECK_ACK : Dto
    {
    }
}
