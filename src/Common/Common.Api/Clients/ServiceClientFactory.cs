// <copyright file="ServiceClientFactory.cs" company="Jedi Contributors">
// Copyright (c) Jedi Contributors. All rights reserved.
//  
// This software is licensed under the MIT license. Read the LICENSE file in the 
// repository for more information.
// </copyright>

using Jedi.Common.Api.Sessions;
using Jedi.Common.Core.Exceptions;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Reflection;
using Jedi.Common.Contracts.Protocols;

namespace Jedi.Common.Api.Clients
{
    /// <summary>
    /// Generate classes based on interfaces that allow you to invoke service methods remotely.
    /// </summary>
    public interface IServiceClientFactory<out TInterface>
    {
        /// <summary>
        /// Create a new service client.
        /// </summary>
        /// <param name="serviceEndpoint">The endpoint of the service to connect to.</param>
        /// <returns>An interface used to communicate with the service.</returns>
        public TInterface CreateClient(string serviceEndpoint);
    }

    /// <summary>
    /// Generate classes based on interfaces that allow you to invoke service methods remotely.
    /// </summary>
    public class ServiceClientFactory<TInterface> : IServiceClientFactory<TInterface>
    {
        private TInterface? _serviceClient;
        private readonly ILogger<ServiceClientFactory<TInterface>> _logger;
        private readonly ISessionFactory _sessionFactory;

        /// <summary>
        /// Create a new <see cref="ServiceClientFactory{TInterface}"/>
        /// </summary>
        /// <param name="logger">Logger for the factory to use.</param>
        /// <param name="sessionFactory">Session factory used to create sessions for clients.</param>
        public ServiceClientFactory(ILogger<ServiceClientFactory<TInterface>> logger, ISessionFactory sessionFactory)
        {
            _logger = logger;
            _sessionFactory = sessionFactory;
        }

        /// <summary>
        /// Create a new service client.
        /// </summary>
        /// <param name="serviceEndpoint">The endpoint of the service to connect to.</param>
        /// <returns>An interface used to communicate with the service.</returns>
        public TInterface CreateClient(string serviceEndpoint)
        {
            if (!IPEndPoint.TryParse(serviceEndpoint, out var serviceIPEndpoint))
            {
                throw new SystemError($"ServiceClientFactory: Failed to parse service endpoint; Endpoint: {serviceEndpoint}");
            }

            // Try to get an available session for the client to use first.
            if (!_sessionFactory.CreateSession(serviceIPEndpoint, out var session) || session == null)
            {
                throw new SystemError($"ServiceClientFactory: Failed to get or create session; Endpoint: {serviceEndpoint}");
            }

            // Cache the client, so we don't have to recompile everytime we want 
            // to use this interface.
            if (_serviceClient == null)
            {
                _logger.LogInformation("ServiceClientFactory: Compiling source; Endpoint: {ServiceEndpoint}, API: {Interface}", serviceEndpoint, typeof(TInterface).FullName);

                var interfaceType = typeof(TInterface);
                var serviceClientBuilder = new ServiceClientBuilder($"{interfaceType.Name}{serviceIPEndpoint.GetHashCode()}", interfaceType);

                Type clientType;

                try
                {
                    var serviceClientSource = CSharpScript.Create(serviceClientBuilder.ToString(), ScriptOptions.Default.WithReferences(
                        Assembly.GetEntryAssembly(),
                        typeof(Task).Assembly,
                        typeof(Task<>).Assembly,
                        typeof(Protocol).Assembly,
                        typeof(CorrelatedProtocol).Assembly
                    ).WithImports(
                        typeof(Task).Namespace,
                        typeof(Task<>).Namespace,
                        typeof(Protocol).Namespace,
                        typeof(CorrelatedProtocol).Namespace));
                    
                    serviceClientSource.Compile();

                    clientType = (Type) serviceClientSource.RunAsync().Result.ReturnValue;
                }
                catch (Exception exception)
                {
                    throw new SystemError($"ServiceClientFactory: Exception thrown while compiling source; Endpoint: {serviceEndpoint}, Interface: {typeof(TInterface).FullName}", exception);
                }

                _serviceClient = (TInterface?) Activator.CreateInstance(clientType, session);
                if (_serviceClient == null)
                {
                    throw new SystemError($"ServiceClientFactory: Failed to create client; Activator.CreateInstance returned null; Endpoint: {serviceEndpoint}, API: {typeof(TInterface).FullName}");
                }

                _logger.LogInformation("ServiceClientFactory: Compiled source; Endpoint: {ServiceEndpoint}, API: {Interface}", serviceEndpoint, typeof(TInterface).FullName);
            }

            // Update the session if we have to.
            // This may happen if we've disconnected from the session and had to create a new one.
            if (_serviceClient is ServiceClient serviceClient && session != serviceClient.Session)
            {
                _logger.LogInformation("ServiceClientFactory: Assigning new session to client; Endpoint: {ServiceEndpoint}, API: {Interface}", serviceEndpoint, typeof(TInterface).FullName);
                serviceClient.Session = session;
            }

            return _serviceClient;
        }
    }
}
