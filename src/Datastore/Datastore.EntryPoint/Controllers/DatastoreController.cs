// <copyright file="DatastoreController.cs" company="Jedi Contributors">
// Copyright (c) Jedi Contributors. All rights reserved.
//  
// This software is licensed under the MIT license. Read the LICENSE file in the 
// repository for more information.
// </copyright>

using Jedi.Common.Api;

namespace Jedi.Datastore.EntryPoint.Controllers
{
    /// <summary>
    /// Handles datastore business logic.
    /// </summary>
    public class DatastoreController : Controller
    {
        private readonly ILogger<DatastoreController> _logger;

        /// <summary>
        /// Create a new <see cref="DatastoreController"/>.
        /// </summary>
        /// <param name="logger">Logger for the controller to use.</param>
        public DatastoreController(ILogger<DatastoreController> logger)
        {
            _logger = logger;
        }
    }
}
