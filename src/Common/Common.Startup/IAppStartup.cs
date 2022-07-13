// <copyright file="IStartup.cs" company="Jedi Contributors">
// Copyright (c) Jedi Contributors. All rights reserved.
//  
// This software is licensed under the MIT license. Read the LICENSE file in the 
// repository for more information.
// </copyright>

using Microsoft.Extensions.DependencyInjection;

namespace Jedi.Common.Startup
{
    /// <summary>
    /// Interface used to start an application.
    /// </summary>
    public interface IAppStartup
    {
        /// <summary>
        /// Register general application components.
        /// </summary>
        /// <param name="services">Collection of services components passed in by the app host.</param>
        void ConfigureServices(IServiceCollection services);
    }
}
