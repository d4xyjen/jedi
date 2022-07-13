// <copyright file="GameController.cs" company="Jedi Contributors">
// Copyright (c) Jedi Contributors. All rights reserved.
//  
// This software is licensed under the MIT license. Read the LICENSE file in the 
// repository for more information.
// </copyright>

using Jedi.Common.Api;

namespace Jedi.Game.EntryPoint.Controllers
{
    /// <summary>
    /// Handles game business logic.
    /// </summary>
    public class GameController : Controller
    {
        private readonly ILogger<GameController> _logger;

        /// <summary>
        /// Create a new <see cref="GameController"/>.
        /// </summary>
        /// <param name="logger">Logger for the controller to use.</param>
        public GameController(ILogger<GameController> logger)
        {
            _logger = logger;
        }
    }
}
