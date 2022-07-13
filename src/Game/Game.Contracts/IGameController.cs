// <copyright file="IGameController.cs" company="Jedi Contributors">
// Copyright (c) Jedi Contributors. All rights reserved.
//  
// This software is licensed under the MIT license. Read the LICENSE file in the 
// repository for more information.
// </copyright>

using Jedi.Common.Api.Messaging;

namespace Jedi.Game.Contracts
{
    /// <summary>
    /// Game service API.
    /// </summary>
    public interface IGameController
    {
        /// <summary>
        /// Create a new avatar.
        /// </summary>
        [ProtocolHandler(ProtocolType.AVATAR_CREATE_REQ)]
        public void AVATAR_CREATE_REQ();
    }
}
