// <copyright file="EntryPoint.cs" company="Jedi Contributors">
// Copyright (c) Jedi Contributors. All rights reserved.
//  
// This software is licensed under the MIT license. Read the LICENSE file in the 
// repository for more information.
// </copyright>

using Jedi.Common.Api;
using Jedi.Common.Api.Messaging;
using Jedi.Common.Api.Sessions;
using Jedi.Common.Mathematics;
using Jedi.Common.Startup.Constants;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using Serilog;
using Serilog.Events;
using System.Diagnostics;
using System.Reflection;
using Jedi.Common.Contracts;

namespace Jedi.Common.Startup
{
    /// <summary>
    /// Entry point for applications.
    /// </summary>
    public abstract class EntryPoint<TStartup> where TStartup : class, IAppStartup
    {
        /// <summary>
        /// Run the application.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        public void Run(string[] args)
        {
            // Set environment variables
            if (Debugger.IsAttached)
            {
                Environment.SetEnvironmentVariable(EnvironmentVariables.Environment, Environments.Development);
            }

            var host = GetHostBuilder().Build();
            var protocolHandlerFactory = host.Services.GetRequiredService<IProtocolHandlerFactory>();

            // Register protocols and their handlers.
            protocolHandlerFactory.RegisterAllProtocols();
            protocolHandlerFactory.RegisterAllHandlers();

            host.Run();
        }

        /// <summary>
        /// Get a <see cref="IHostBuilder"/> for building the application host.
        /// </summary>
        /// <returns>The application host builder.</returns>
        /// <exception cref="FileNotFoundException">Thrown if the executing assembly file was not found.</exception>
        private IHostBuilder GetHostBuilder()
        {
            var hostBuilder = new HostBuilder();
            var environment = new HostingEnvironment
            {
                ApplicationName = Assembly.GetEntryAssembly()?.GetName().Name,
                EnvironmentName = Environment.GetEnvironmentVariable(EnvironmentVariables.Environment) ?? Environments.Production,
                ContentRootPath = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location) ?? throw new FileNotFoundException("The entry assembly was not found")
            };

            // load config from the app's method
            hostBuilder.ConfigureAppConfiguration(configurationBuilder => BuildConfiguration(configurationBuilder, environment.EnvironmentName, environment.ContentRootPath));

            hostBuilder.UseSerilog((_, _, loggerConfiguration) =>
            {
                loggerConfiguration.MinimumLevel.Is(Debugger.IsAttached ? LogEventLevel.Debug : LogEventLevel.Information);
                loggerConfiguration.WriteTo.Console();
                loggerConfiguration.Enrich.WithThreadId();
            });

            hostBuilder.ConfigureServices((context, services) =>
            {
                services.AddSingleton<TStartup>();
                services.AddSingleton(context.Configuration);
                services.AddSingleton<IHostEnvironment>(environment);

                services.AddSingleton<IRandomGenerator, RandomGenerator>();

                services.AddSingleton<IProtocolHandlerFactory, ProtocolHandlerFactory>();

                services.AddSingleton<ISessionCryptography, SessionCryptography>();
                services.AddSingleton<ISessionEventQueue, SessionEventQueue>();
                services.AddSingleton<ISessionFactory, SessionFactory>();

                services.AddHostedService<SessionEventProcessor>();
                services.AddHostedService<SessionAcceptor>();

                services.Configure<SessionConfiguration>(context.Configuration.GetSection(ConfigurationConstants.SessionConfigurationSectionName));

                // Application services
                services.BuildServiceProvider().GetRequiredService<TStartup>().ConfigureServices(services);

                // Get all types that derive from the Controller class.
                var controllers = Assembly
                    .GetEntryAssembly()?
                    .GetTypes()
                    .Where(type => type.IsClass && type.IsSubclassOf(typeof(Controller))).ToList() ?? new List<Type>();

                foreach (var controllerType in controllers)
                {
                    services.AddSingleton(typeof(Controller), controllerType);
                }
            });

            return hostBuilder;
        }

        /// <summary>
        /// Configure the application.
        /// </summary>
        /// <param name="configurationBuilder">A <see cref="IConfigurationBuilder"/> used to build the application's configuration.</param>
        /// <param name="environmentName">The name of the environment.</param>
        /// <param name="appRootPath">The root directory for the application.</param>
        public abstract IConfiguration BuildConfiguration(IConfigurationBuilder configurationBuilder, string environmentName, string appRootPath);
    }
}
