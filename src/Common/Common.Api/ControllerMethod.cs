// <copyright file="ControllerMethod.cs" company="Jedi Contributors">
// Copyright (c) Jedi Contributors. All rights reserved.
//  
// This software is licensed under the MIT license. Read the LICENSE file in the 
// repository for more information.
// </copyright>

using Jedi.Common.Contracts.Protocols;
using System.Reflection;

namespace Jedi.Common.Api
{
    /// <summary>
    /// A method inside of a controller class.
    /// </summary>
    public class ControllerMethod
    {
        /// <summary>
        /// Create a new <see cref="ControllerMethod"/>.
        /// </summary>
        /// <param name="command">The command that the method handles.</param>
        /// <param name="controller">The controller that the method is in.</param>
        /// <param name="method">The method that gets called to handle the protocol.</param>
        /// <param name="bodyParameters">The parameters that should be deserialized from the message body.</param>
        public ControllerMethod(ProtocolCommand command, Controller controller, MethodInfo method, List<ParameterInfo> bodyParameters)
        {
            Command = command;
            Controller = controller;
            Method = method;
            BodyParameters = bodyParameters;
        }

        /// <summary>
        /// The command that the method handles.
        /// </summary>
        public ProtocolCommand Command { get; }

        /// <summary>
        /// The type of controller that the handler is in.
        /// </summary>
        public Controller Controller { get; }

        /// <summary>
        /// The method that gets called to handle the protocol.
        /// </summary>
        public MethodInfo Method { get; }

        /// <summary>
        /// The parameters with the FromData attribute that should be deserialized from the message body.
        /// </summary>
        public List<ParameterInfo> BodyParameters { get; }
    }
}
