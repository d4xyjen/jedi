// <copyright file="PROTO_USER_CLIENT_VERSION_CHECK_REQ.cs" company="Jedi Contributors">
// Copyright (c) Jedi Contributors. All rights reserved.
//  
// This software is licensed under the MIT license. Read the LICENSE file in the 
// repository for more information.
// </copyright>

using System.ComponentModel.DataAnnotations;

namespace Jedi.Common.Contracts.Protocols.User
{
    /// <summary>
    /// Requests a version check.
    /// </summary>
    public class PROTO_USER_CLIENT_VERSION_CHECK_REQ : Protocol
    {
        /// <summary>
        /// The client's version key.
        /// </summary>
        [MaxLength(64)]
        public string Version { get; set; }

        /// <summary>
        /// Parameterless constructor for deserialization.
        /// </summary>
        public PROTO_USER_CLIENT_VERSION_CHECK_REQ()
        {
        }

        /// <summary>
        /// Create a new <see cref="PROTO_USER_CLIENT_VERSION_CHECK_REQ"/>.
        /// </summary>
        /// <param name="version">The client version key.</param>
        public PROTO_USER_CLIENT_VERSION_CHECK_REQ(string version)
        {
            Version = version;
        }
    }
}
