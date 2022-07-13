// <copyright file="Program.cs" company="Jedi Contributors">
// Copyright (c) Jedi Contributors. All rights reserved.
//  
// This software is licensed under the MIT license. Read the LICENSE file in the 
// repository for more information.
// </copyright>

using Jedi.Common.Startup;

namespace Jedi.Datastore.EntryPoint
{
    /// <summary>
    /// Entry point for the service.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The main entry point method.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        public static void Main(string[] args)
        {
            new DatastoreEntryPoint<Startup>().Run(args);
        }
    }

    /// <summary>
    /// Datastore service entry point.
    /// </summary>
    /// <typeparam name="TStartup">The type of the startup object.</typeparam>
    internal class DatastoreEntryPoint<TStartup> : EntryPoint<TStartup> where TStartup : class, IAppStartup
    {
        /// <summary>
        /// Build the application configuration.
        /// </summary>
        /// <param name="configurationBuilder">The configuration builder.</param>
        /// <param name="environmentName">The environment name.</param>
        /// <param name="appRootPath">The app's root directory.</param>
        /// <returns>The application configuration.</returns>
        public override IConfiguration BuildConfiguration(IConfigurationBuilder configurationBuilder, string environmentName, string appRootPath)
        {
            return configurationBuilder
                .SetBasePath(appRootPath)
                .AddJsonFile("appsettings.json", false)
                .AddJsonFile($"appsettings.{environmentName.ToLower()}.json", true, true)
                .AddJsonFile("appsettings.override.json", true, true)
                .AddEnvironmentVariables()
                .Build();
        }
    }
}