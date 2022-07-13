// <copyright file="ServiceClientBuilder.cs" company="Jedi Contributors">
// Copyright (c) Jedi Contributors. All rights reserved.
//  
// This software is licensed under the MIT license. Read the LICENSE file in the 
// repository for more information.
// </copyright>

using System.Reflection;
using System.Text;
using Jedi.Common.Api.Extensions;
using Jedi.Common.Api.Messaging;
using Jedi.Common.Api.Sessions;

namespace Jedi.Common.Api.Clients
{
    /// <summary>
    /// Builds source code for service invokers from controller interfaces.
    /// </summary>
    public class ServiceClientBuilder
    {
        /// <summary>
        /// Create new <see cref="ServiceClientBuilder"/>.
        /// </summary>
        /// <param name="name">The name of the new controller.</param>
        /// <param name="implements">The interface the controller will implement.</param>
        public ServiceClientBuilder(string name, Type implements)
        {
            Name = name;
            Implements = implements;
        }

        /// <summary>
        /// The name of the dynamic type.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The type that the class implements.
        /// </summary>
        public Type Implements { get; }

        /// <summary>
        /// Get the source code for the service client.
        /// </summary>
        /// <returns>The service client's source code.</returns>
        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            var safeName = Name.Replace("-", "");

            stringBuilder.AppendLine($"public class {safeName} : {typeof(ServiceClient).FullName}, {Implements.FullName}");
            stringBuilder.AppendLine("{");

            stringBuilder.AppendLine($"public {safeName}({typeof(Session).FullName} session) : base(session)");
            stringBuilder.AppendLine("{");
            stringBuilder.AppendLine("}");

            var handlers = Implements
                .GetMethods()
                .Where(method => method.GetCustomAttributes(typeof(ProtocolHandlerAttribute), false).Length > 0)
                .ToArray();

            for (var i = 0; i < handlers.Length; i++)
            {
                var handler = handlers[i];
                var attribute = handler.GetCustomAttribute<ProtocolHandlerAttribute>();

                if (attribute == null)
                {
                    continue;
                }

                var parameters = handler.GetParameters().Select(parameter => parameter.Name).ToArray();
                var parameterArray = new string[parameters.Length + 1];

                parameterArray[0] = $"{attribute.Command.GetType().FullName}.{attribute.Command}";
                Array.Copy(parameters, 0, parameterArray, 1, parameters.Length);

                stringBuilder.AppendLine(handler.GetSignature());
                stringBuilder.AppendLine("{");
                stringBuilder.AppendLine($"base.BuildAndSend({string.Join(',', parameterArray)});");
                stringBuilder.AppendLine("}");
            }

            stringBuilder.AppendLine("}");
            stringBuilder.AppendLine($"return typeof({safeName});");

            return stringBuilder.ToString();
        }
    }
}
