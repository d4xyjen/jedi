// <copyright file="WorldController.cs" company="Jedi Contributors">
// Copyright (c) Jedi Contributors. All rights reserved.
//  
// This software is licensed under the MIT license. Read the LICENSE file in the 
// repository for more information.
// </copyright>

using Jedi.Common.Api;

namespace Jedi.World.EntryPoint.Controllers
{
    /// <summary>
    /// Handles world business logic.
    /// </summary>
    public class WorldController : Controller
    {
        private readonly ILogger<WorldController> _logger;

        /// <summary>
        /// Create a new <see cref="WorldController"/>.
        /// </summary>
        /// <param name="logger">Logger for the controller to use.</param>
        public WorldController(ILogger<WorldController> logger)
        {
            _logger = logger;
        }
    }
}
