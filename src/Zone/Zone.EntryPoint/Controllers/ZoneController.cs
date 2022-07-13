// <copyright file="ZoneController.cs" company="Jedi Contributors">
// Copyright (c) Jedi Contributors. All rights reserved.
//  
// This software is licensed under the MIT license. Read the LICENSE file in the 
// repository for more information.
// </copyright>

using Jedi.Common.Api;

namespace Jedi.Zone.EntryPoint.Controllers
{
    /// <summary>
    /// Handles zone business logic.
    /// </summary>
    public class ZoneController : Controller
    {
        private readonly ILogger<ZoneController> _logger;

        /// <summary>
        /// Create a new <see cref="ZoneController"/>.
        /// </summary>
        /// <param name="logger">Logger for the controller to use.</param>
        public ZoneController(ILogger<ZoneController> logger)
        {
            _logger = logger;
        }
    }
}
