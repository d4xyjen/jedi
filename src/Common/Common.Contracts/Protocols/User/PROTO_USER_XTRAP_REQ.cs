// <copyright file="PROTO_USER_XTRAP_REQ.cs" company="Jedi Contributors">
// Copyright (c) Jedi Contributors. All rights reserved.
//
// This software is licensed under the MIT license. Read the LICENSE file in the
// repository for more information.
// </copyright>
namespace Jedi.Common.Contracts.Protocols.User
{
    /// <summary>
    /// Verifies the client's XTrap key.
    /// </summary>
    public class PROTO_USER_XTRAP_REQ : Protocol
    {
        /// <summary>
        /// The client's XTrap key.
        /// </summary>
        [PrefixLength]
        public byte[] XTrapKey { get; set; }

        /// <summary>
        /// Parameterless constructor for deserialization.
        /// </summary>
        public PROTO_USER_XTRAP_REQ()
        {
        }

        /// <summary>
        /// Create a new <see cref="PROTO_USER_XTRAP_REQ"/>.
        /// </summary>
        /// <param name="xTrapKey">The client's XTrap key.</param>
        public PROTO_USER_XTRAP_REQ(byte[] xTrapKey)
        {
            XTrapKey = xTrapKey;
        }
    }
}
