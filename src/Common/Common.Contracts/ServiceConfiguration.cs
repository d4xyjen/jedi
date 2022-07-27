// <copyright file="ServiceConfiguration.cs" company="Jedi Contributors">
// Copyright (c) Jedi Contributors. All rights reserved.
//  
// This software is licensed under the MIT license. Read the LICENSE file in the 
// repository for more information.
// </copyright>
namespace Jedi.Common.Contracts
{
    /// <summary>
    /// Base configuration for services.
    /// </summary>
    public class ServiceConfiguration
    {
        /// <summary>
        /// Configuration for clients of the service.
        /// </summary>
        public ServiceClientConfiguration Client { get; set; } = new ServiceClientConfiguration();

        /// <summary>
        /// Configuration for sessions of the service.
        /// </summary>
        public SessionConfiguration Sessions { get; set; } = new SessionConfiguration();
    }
}
