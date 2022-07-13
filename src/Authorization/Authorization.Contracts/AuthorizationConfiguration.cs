// <copyright file="AuthorizationConfiguration.cs" company="Jedi Contributors">
// Copyright (c) Jedi Contributors. All rights reserved.
//  
// This software is licensed under the MIT license. Read the LICENSE file in the 
// repository for more information.
// </copyright>

using Jedi.Common.Contracts;

namespace Jedi.Authorization.Contracts
{
    /// <summary>
    /// Configuration for the authorization service.
    /// </summary>
    public class AuthorizationConfiguration : ServiceConfiguration
    {
        /// <summary>
        /// Supported game client versions.
        /// </summary>
        public string[]? SupportedGameClientVersions { get; set; }
    }
}
