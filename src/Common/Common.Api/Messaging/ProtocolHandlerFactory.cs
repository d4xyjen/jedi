// <copyright file="ProtocolHandlerFactory.cs" company="Jedi Contributors">
// Copyright (c) Jedi Contributors. All rights reserved.
//  
// This software is licensed under the MIT license. Read the LICENSE file in the 
// repository for more information.
// </copyright>

using Jedi.Common.Api.Sessions;
using Jedi.Common.Contracts.Protocols;
using Jedi.Common.Contracts.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Reflection;
using Jedi.Common.Api.Constants;

namespace Jedi.Common.Api.Messaging
{
    /// <summary>
    /// Register and get proto handlers.
    /// </summary>
    public interface IProtocolHandlerFactory
    {
        /// <summary>
        /// Find and register all protocols.
        /// </summary>
        public void RegisterAllProtocols();

        /// <summary>
        /// Find and register all proto handlers in the current entry assembly.
        /// </summary>
        public void RegisterAllHandlers();

        /// <summary>
        /// Try to identify a message's protocol command.
        /// </summary>
        /// <param name="data">The message.</param>
        /// <param name="command">The message's protocol command.</param>
        public bool GetCommand(ArraySegment<byte> data, out ushort command);

        /// <summary>
        /// Handles a message from a session.
        /// </summary>
        /// <param name="command">The command of the message.</param>
        /// <param name="handlers">The handlers for the message.</param>
        public bool GetHandlers(ushort command, out List<ControllerMethod>? handlers);

        /// <summary>
        /// Deserialize the message and execute the handler.
        /// </summary>
        /// <param name="command">The type of protocol that was received.</param>
        /// <param name="sessionId">The ID of the session that sent the message.</param>
        /// <param name="data">The data to deserialize parameters from.</param>
        public Task<bool> DeserializeAndHandleAsync(ushort command, Guid sessionId, ArraySegment<byte> data);
    }

    /// <summary>
    /// Register and get controllers to handle messages from sessions.
    /// </summary>
    public class ProtocolHandlerFactory : IProtocolHandlerFactory
    {
        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly ISessionFactory _sessionFactory;
        private readonly ConcurrentDictionary<ushort, List<ControllerMethod>> _handlers;
        private ConcurrentDictionary<ProtocolCommand, Type> _protocolTypeMappingByCommand;

        /// <summary>
        /// Create a new <see cref="ProtocolHandlerFactory"/>.
        /// </summary>
        /// <param name="logger">Logger for the factory to use.</param>
        /// <param name="serviceProvider">Service provider for the factory to use.</param>
        /// <param name="sessionFactory">Sesion factory to consume.</param>
        public ProtocolHandlerFactory(ILogger<IProtocolHandlerFactory> logger, IServiceProvider serviceProvider, ISessionFactory sessionFactory)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _sessionFactory = sessionFactory;
            _handlers = new ConcurrentDictionary<ushort, List<ControllerMethod>>();
            _protocolTypeMappingByCommand = new ConcurrentDictionary<ProtocolCommand, Type>();
        }

        /// <summary>
        /// Find and register all proto handlers.
        /// </summary>
        public void RegisterAllHandlers()
        {
            // Get all methods within all controller services, which have the 
            // ProtoHandler attribute.
            //
            // Using the service provider also allows us to inject services into controllers
            // via dependency injection.

            var handlerMap = _serviceProvider
                .GetServices<Controller>()
                .Select(controller => new Tuple<Controller, IEnumerable<MethodInfo>>(controller, controller
                    .GetType()
                    .GetMethods()
                    .Where(method => method.GetCustomAttributes(typeof(ProtocolAttribute), false).Length > 0)))
                .ToArray();

            for (var i = 0; i < handlerMap.Length; i++)
            {
                var controllerHandlers = handlerMap[i];
                foreach (var handler in controllerHandlers.Item2)
                {
                    var attribute = handler.GetCustomAttribute<ProtocolAttribute>();
                    if (attribute == null)
                    {
                        continue;
                    }

                    var command = (ushort) attribute.Command;
                    if (!_handlers.TryGetValue(command, out var handlers))
                    {
                        _handlers[command] = new List<ControllerMethod>();
                        handlers = _handlers[command];
                    }

                    var bodyParameters = handler
                        .GetParameters()
                        .Where(param => param.GetCustomAttributes(typeof(FromBodyAttribute), false).Length > 0)
                        .ToList();

                    handlers.Add(new ControllerMethod(attribute.Command, controllerHandlers.Item1, handler, bodyParameters));
                }
            }
        }

        /// <summary>
        /// Find and register all protocols.
        /// </summary>
        public void RegisterAllProtocols()
        {
            _protocolTypeMappingByCommand = new ConcurrentDictionary<ProtocolCommand, Type>(AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(assembly => assembly
                    .GetTypes()
                    .Where(type => type.IsAssignableTo(typeof(Protocol)) && type.GetCustomAttribute<CommandAttribute>() != null)
                    .Select(type => new KeyValuePair<ProtocolCommand, Type>(type.GetCustomAttribute<CommandAttribute>()!.Command, type))
                ));
        }

        /// <summary>
        /// Read a message's protocol command.
        /// </summary>
        /// <param name="data">The message.</param>
        /// <param name="command">The message's protocol command.</param>
        public bool GetCommand(ArraySegment<byte> data, out ushort command)
        {
            command = ushort.MaxValue;

            if (data.Array == null)
            {
                _logger.LogError("Failed to deserialize message data; No data provided");
                return false;
            }

            using var stream = new MemoryStream(data.Array, data.Offset, data.Count);
            using var reader = new BinaryReader(stream);

            try
            {
                command = reader.ReadUInt16();
                return true;
            }
            catch (EndOfStreamException exception)
            {
                _logger.LogError(exception, "Reached end of the stream while reading message command");
                return false;
            }
        }

        /// <summary>
        /// Handles a message from a session.
        /// </summary>
        /// <param name="command">The command of the message.</param>
        /// <param name="handlers">The handlers for the message.</param>
        public bool GetHandlers(ushort command, out List<ControllerMethod>? handlers)
        {
            return _handlers.TryGetValue(command, out handlers);
        }

        /// <summary>
        /// Deserialize the message and execute the handler.
        /// </summary>
        /// <param name="command">The type of protocol that was received.</param>
        /// <param name="sessionId">The ID of the session that sent the message.</param>
        /// <param name="data">The data to deserialize parameters from.</param>
        public async Task<bool> DeserializeAndHandleAsync(ushort command, Guid sessionId, ArraySegment<byte> data)
        {
            if (!_protocolTypeMappingByCommand.TryGetValue((ProtocolCommand) command, out var protocolType))
            {
                _logger.LogError("ProtocolHandlerFactory: Protocol not found for command; Session: {Session}, Command: {Command}", sessionId, (ProtocolCommand) command);
                return false;
            }

            using var serializer = new BinarySerializer(data);
            var protocol = serializer.Deserialize(protocolType);

            if (_sessionFactory.GetSession(sessionId, out var session) && session != null)
            {
                if (protocol is CorrelatedProtocol correlatedProtocol && session.CompleteOperation(correlatedProtocol.OperationId, correlatedProtocol))
                {
                    // the protocol is part of an operation.
                    //
                    // no need to run the handlers, we'll continue in the context
                    // of the asynchronous operation
                    return true;
                }
            }

            if (GetHandlers(command, out var handlers) && handlers != null)
            {
                var parameters = new [] { sessionId, protocol };
                for (var i = 0; i < handlers.Count; i++)
                {
                    var handler = handlers[i];
                    var returnValue = handler.Method.Invoke(handler.Controller, parameters);

                    if (handler.Method.ReturnType != typeof(void))
                    {
                        var response = returnValue as Protocol;
                        if (response == null && returnValue is Task<Protocol> task)
                        {
                            if (await Task.WhenAny(task, Task.Delay(AsyncConstants.AsyncHandleTimeout)) == task)
                            {
                                // the handler completed before timeout
                                response = await task;
                            }
                            else
                            {
                                _logger.LogWarning("ProtocolHandlerFactory: Asynchronous protocol handler timed out; Session: {Session}, Handler: {Handler}, Controller: {Controller}, Request: {Request}, Response: {Response}", sessionId, handler.Method.Name, handler.Controller.GetType().FullName, (ProtocolCommand) command, response);
                            }
                        }

                        var responseCommand = response?.GetType().GetCustomAttribute<CommandAttribute>()?.Command;
                        if (responseCommand == null || !(session?.Send(responseCommand.Value, response) ?? false))
                        {
                            _logger.LogError("ProtocolHandlerFactory: Failed to send response; Session: {Session}, Handler: {Handler}, Controller: {Controller}, Request: {Request}, Response: {Response}", sessionId, handler.Method.Name, handler.Controller.GetType().FullName, (ProtocolCommand) command, response);
                        }
                    }
                }
            }

            return true;
        }
    }
}
