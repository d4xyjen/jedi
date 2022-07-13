// <copyright file="ServiceClientConfiguration.cs" company="Jedi Contributors">
// Copyright (c) Jedi Contributors. All rights reserved.
//  
// This software is licensed under the MIT license. Read the LICENSE file in the 
// repository for more information.
// </copyright>
namespace Jedi.Common.Contracts
{
    /// <summary>
    /// Configuration for clients of a service.
    /// </summary>
    public class ServiceClientConfiguration
    {
        /// <summary>
        /// The endpoint of the service.
        /// </summary>
        public string ServiceEndpoint { get; set; } = default!;
    }
}
