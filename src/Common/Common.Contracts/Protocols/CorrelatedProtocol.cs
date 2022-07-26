// <copyright file="CorrelatedProtocol.cs" company="Jedi Contributors">
// Copyright (c) Jedi Contributors. All rights reserved.
//  
// This software is licensed under the MIT license. Read the LICENSE file in the 
// repository for more information.
// </copyright>

using System.Runtime.Serialization;

namespace Jedi.Common.Contracts.Protocols
{
    /// <summary>
    /// A protocol that's related to other protocols.
    /// </summary>
    [DataContract]
    public class CorrelatedProtocol : Protocol
    {
        /// <summary>
        /// The identifier that specifies the operation this protocol is related to.
        /// </summary>
        [DataMember]
        public Guid OperationId { get; set; }
    }
}
