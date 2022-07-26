// <copyright file="IDatastoreController.cs" company="Jedi Contributors">
// Copyright (c) Jedi Contributors. All rights reserved.
//  
// This software is licensed under the MIT license. Read the LICENSE file in the 
// repository for more information.
// </copyright>

using Jedi.Common.Api;
using Jedi.Common.Contracts.Protocols;
using Jedi.Common.Contracts.Protocols.User;

namespace Jedi.Datastore.Contracts
{
    /// <summary>
    /// Datastore service API.
    /// </summary>
    public interface IDatastoreController
    {
        /// <summary>
        /// Check a password asynchronously.
        /// </summary>
        /// <param name="protocol">The protocol to send.</param>
        /// <returns>The password check response.</returns>
        [Protocol(ProtocolCommand.USER_PASSWORD_CHECK_REQ)]
        public Task<PROTO_USER_PASSWORD_CHECK_ACK> CheckPasswordAsync(PROTO_USER_PASSWORD_CHECK_REQ protocol);
    }
}
