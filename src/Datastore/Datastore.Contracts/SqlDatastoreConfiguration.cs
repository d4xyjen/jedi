// <copyright file="SqlDatastoreConfiguration.cs" company="Jedi Contributors">
// Copyright (c) Jedi Contributors. All rights reserved.
//  
// This software is licensed under the MIT license. Read the LICENSE file in the 
// repository for more information.
// </copyright>
namespace Jedi.Datastore.Contracts
{
    /// <summary>
    /// Configuration for an Sql datastore.
    /// </summary>
    public class SqlDatastoreConfiguration
    {
        /// <summary>
        /// The Sql server.
        /// </summary>
        public string Server { get; set; }

        /// <summary>
        /// The database user.
        /// </summary>
        public string User { get; set; }

        /// <summary>
        /// The database password.
        /// </summary>
        public string Password { get; set; }
    }
}
