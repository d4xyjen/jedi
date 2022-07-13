// <copyright file="ServiceWorker.cs" company="Jedi Contributors">
// Copyright (c) Jedi Contributors. All rights reserved.
//  
// This software is licensed under the MIT license. Read the LICENSE file in the 
// repository for more information.
// </copyright>

namespace Jedi.World.EntryPoint
{
    /// <summary>
    /// Performs background processing for the service.
    /// </summary>
    public class ServiceWorker : BackgroundService
    {
        /// <summary>
        /// Create a new <see cref="ServiceWorker"/>.
        /// </summary>
        public ServiceWorker()
        {
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(10, stoppingToken);
            }
        }
    }
}
