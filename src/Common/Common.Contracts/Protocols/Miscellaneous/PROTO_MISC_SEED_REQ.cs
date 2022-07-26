// <copyright file="PROTO_MISC_SEED_REQ.cs" company="Jedi Contributors">
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
    /// Requests seed information for session cryptography.
    /// </summary>
    [DataContract]
    [Command(ProtocolCommand.MISC_SEED_REQ)]
    public class PROTO_MISC_SEED_REQ : Protocol
    {
    }
}
