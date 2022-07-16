// <copyright file="PROTO_USER_CLIENT_RIGHTVERSION_CHECK_ACK.cs" company="Jedi Contributors">
// Copyright (c) Jedi Contributors. All rights reserved.
//  
// This software is licensed under the MIT license. Read the LICENSE file in the 
// repository for more information.
// </copyright>
namespace Jedi.Common.Contracts.Protocols.User
{
    /// <summary>
    /// Notifies the client of a valid client version.
    /// </summary>
    public class PROTO_USER_CLIENT_RIGHTVERSION_CHECK_ACK : Protocol
    {
        /// <summary>
        /// The XTrap key.
        /// </summary>
        [PrefixLength]
        public byte[] XTrapKey { get; set; }

        /// <summary>
        /// Parameterless constructor for deserialization.
        /// </summary>
        public PROTO_USER_CLIENT_RIGHTVERSION_CHECK_ACK()
        {
        }

        /// <summary>
        /// Create a new <see cref="PROTO_USER_CLIENT_RIGHTVERSION_CHECK_ACK"/>.
        /// </summary>
        /// <param name="xtrapKey">The authorization service's XTrap key.</param>
        public PROTO_USER_CLIENT_RIGHTVERSION_CHECK_ACK(byte[] xtrapKey)
        {
            XTrapKey = xtrapKey;
        }
    }
}
