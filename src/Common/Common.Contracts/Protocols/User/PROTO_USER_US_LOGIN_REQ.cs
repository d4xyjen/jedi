// <copyright file="PROTO_USER_US_LOGIN_REQ.cs" company="Jedi Contributors">
// Copyright (c) Jedi Contributors. All rights reserved.
//  
// This software is licensed under the MIT license. Read the LICENSE file in the 
// repository for more information.
// </copyright>

using System.ComponentModel.DataAnnotations;

namespace Jedi.Common.Contracts.Protocols.User
{
    /// <summary>
    /// Contains credentials that a client is attempting to login with.
    /// </summary>
    public class PROTO_USER_US_LOGIN_REQ : Protocol
    {
        /// <summary>
        /// The username credential.
        /// </summary>
        [MaxLength(260)]
        public string Username { get; set; }

        /// <summary>
        /// The password credential.
        /// </summary>
        [MaxLength(36)]
        public string Password { get; set; }

        /// <summary>
        /// Spawn apps.
        /// </summary>
        [MaxLength(20)]
        public string Spawnapps { get; set; }

        /// <summary>
        /// Parameterless constructor for deserialization.
        /// </summary>
        public PROTO_USER_US_LOGIN_REQ()
        {
        }

        /// <summary>
        /// Create a new <see cref="PROTO_USER_US_LOGIN_REQ"/>.
        /// </summary>
        /// <param name="username">The username credential.</param>
        /// <param name="password">The password credential.</param>
        /// <param name="spawnapps">The spawnapps credential.</param>
        public PROTO_USER_US_LOGIN_REQ(string username, string password, string spawnapps)
        {
            Username = username;
            Password = password;
            Spawnapps = spawnapps;
        }
    }
}
