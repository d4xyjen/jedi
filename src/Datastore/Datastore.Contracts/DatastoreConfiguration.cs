// <copyright file="DatastoreConfiguration.cs" company="Jedi Contributors">
// Copyright (c) Jedi Contributors. All rights reserved.
//  
// This software is licensed under the MIT license. Read the LICENSE file in the 
// repository for more information.
// </copyright>

using Jedi.Common.Contracts;

namespace Jedi.Datastore.Contracts
{
    /// <summary>
    /// Configuration for the datastore service.
    /// </summary>
    public class DatastoreConfiguration : ServiceConfiguration
    {
        /// <summary>
        /// Configuration for an Sql datastore.
        /// </summary>
        public SqlDatastoreConfiguration Sql { get; set; } = new SqlDatastoreConfiguration();
    }
}
