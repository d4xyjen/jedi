// <copyright file="ProtocolHandlerFactory.cs" company="Jedi Contributors">
// Copyright (c) Jedi Contributors. All rights reserved.
//  
// This software is licensed under the MIT license. Read the LICENSE file in the 
// repository for more information.
// </copyright>

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.Serialization;

namespace Jedi.Common.Api.Messaging
{
    /// <summary>
    /// Register and get proto handlers.
    /// </summary>
    public interface IProtocolHandlerFactory
    {
        /// <summary>
        /// Find and register all proto handlers in the current entry assembly.
        /// </summary>
        public void RegisterAll();

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
        /// <param name="method">The handler method to execute.</param>
        /// <param name="sessionId">The ID of the session that sent the message.</param>
        /// <param name="data">The message to deserialize.</param>
        public bool DeserializeAndExecute(ControllerMethod method, Guid sessionId, ArraySegment<byte> data);
    }

    /// <summary>
    /// Register and get controllers to handle messages from sessions.
    /// </summary>
    public class ProtocolHandlerFactory : IProtocolHandlerFactory
    {
        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly ConcurrentDictionary<ushort, List<ControllerMethod>> _handlers;

        /// <summary>
        /// Create a new <see cref="ProtocolHandlerFactory"/>.
        /// </summary>
        /// <param name="logger">Logger for the factory to use.</param>
        /// <param name="serviceProvider">Service provider for the factory to use.</param>
        public ProtocolHandlerFactory(ILogger<IProtocolHandlerFactory> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _handlers = new ConcurrentDictionary<ushort, List<ControllerMethod>>();
        }

        /// <summary>
        /// Find and register all proto handlers.
        /// </summary>
        public void RegisterAll()
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
                    .Where(method => method.GetCustomAttributes(typeof(ProtocolHandlerAttribute), false).Length > 0)))
                .ToArray();

            for (var i = 0; i < handlerMap.Length; i++)
            {
                var controllerHandlers = handlerMap[i];
                foreach (var handler in controllerHandlers.Item2)
                {
                    var attribute = handler.GetCustomAttribute<ProtocolHandlerAttribute>();
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
        /// <param name="method">The handler method to execute.</param>
        /// <param name="sessionId">The ID of the session that sent the message.</param>
        /// <param name="data">The data to deserialize parameters from.</param>
        public bool DeserializeAndExecute(ControllerMethod method, Guid sessionId, ArraySegment<byte> data)
        {
            var parameters = new List<object> { sessionId };
            using var serializer = new BinarySerializer(data);

            for (var i = 0; i < method.BodyParameters.Count; i++)
            {
                var bodyParameter = method.BodyParameters[i];

                try
                {
                    var deserializedParameter = serializer.Deserialize(bodyParameter.ParameterType);
                    if (deserializedParameter == null)
                    {
                        _logger.LogError("Failed to deserialize parameter from message body; Name: {ParameterName}, Type: {ParameterType}, Method: {Method}, Controller: {Controller}", bodyParameter.Name, bodyParameter.ParameterType, method.Method.Name, method.Controller.GetType().FullName);
                        continue;
                    }

                    parameters.Add(deserializedParameter);
                }
                catch (SerializationException exception)
                {
                    _logger.LogError(exception, "Failed to deserialize parameter {Parameter} from message body; Session: {Session}, Controller: {Controller}, Handler: {Handler}", bodyParameter.Name, sessionId, method.Controller.GetType().FullName, method.Method.Name);
                    return false;
                }
            }

            method.Method.Invoke(method.Controller, parameters.ToArray());
            return true;
        }
    }
}
