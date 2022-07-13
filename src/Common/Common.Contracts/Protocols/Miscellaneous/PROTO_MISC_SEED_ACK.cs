// <copyright file="PROTO_MISC_SEED_ACK.cs" company="Jedi Contributors">
// Copyright (c) Jedi Contributors. All rights reserved.
//  
// This software is licensed under the MIT license. Read the LICENSE file in the 
// repository for more information.
// </copyright>

namespace Jedi.Common.Contracts.Protocols.Miscellaneous
{
    /// <summary>
    /// Contains seed information for session cryptography.
    /// </summary>
    public class PROTO_MISC_SEED_ACK : Protocol
    {
        /// <summary>
        /// The seed.
        /// </summary>
        public ushort Seed { get; set; }

        /// <summary>
        /// Parameterless constructor for deserialization.
        /// </summary>
        public PROTO_MISC_SEED_ACK()
        {
        }

        /// <summary>
        /// Create a new <see cref="PROTO_MISC_SEED_ACK"/>.
        /// </summary>
        /// <param name="seed">The session seed.</param>
        public PROTO_MISC_SEED_ACK(ushort seed)
        {
            Seed = seed;
        }
    }
}
