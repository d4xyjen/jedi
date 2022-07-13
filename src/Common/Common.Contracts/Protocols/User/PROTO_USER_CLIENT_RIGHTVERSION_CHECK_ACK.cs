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
        /// The length of the XTrap key.
        /// </summary>
        public byte XTrapKeyLength { get; set; }

        /// <summary>
        /// The XTrap key.
        /// </summary>
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
        /// <param name="xtrapKeyLength">The length of the XTrap key.</param>
        /// <param name="xtrapKey">The XTrap key.</param>
        public PROTO_USER_CLIENT_RIGHTVERSION_CHECK_ACK(byte xtrapKeyLength, byte[] xtrapKey)
        {
            XTrapKeyLength = xtrapKeyLength;
            XTrapKey = xtrapKey;
        }
    }
}
