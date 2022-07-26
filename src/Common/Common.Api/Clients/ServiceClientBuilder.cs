// <copyright file="ServiceClientBuilder.cs" company="Jedi Contributors">
// Copyright (c) Jedi Contributors. All rights reserved.
//  
// This software is licensed under the MIT license. Read the LICENSE file in the 
// repository for more information.
// </copyright>

using Jedi.Common.Api.Extensions;
using Jedi.Common.Api.Sessions;
using System.Reflection;
using System.Text;

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
                .Where(method => method.GetCustomAttributes(typeof(ProtocolAttribute), false).Length > 0)
                .ToArray();

            for (var i = 0; i < handlers.Length; i++)
            {
                var handler = handlers[i];

                stringBuilder.AppendLine(handler.GetSignature());
                stringBuilder.AppendLine("{");

                GetBodyForInterfaceMethod(handler, stringBuilder);
                
                stringBuilder.AppendLine("}");
            }

            stringBuilder.AppendLine("}");
            stringBuilder.AppendLine($"return typeof({safeName});");

            return stringBuilder.ToString();
        }

        private static void GetBodyForInterfaceMethod(MethodInfo method, StringBuilder stringBuilder)
        {
            try
            {
                var protocolHandlerAttribute = method.GetCustomAttribute<ProtocolAttribute>();
                if (protocolHandlerAttribute == null)
                {
                    throw new ArgumentNullException(nameof(protocolHandlerAttribute), "No protocol handler specified for controller interface method.");
                }

                var parameterNames = method.GetParameters().Select(parameter => parameter.Name).ToArray();
                var parameters = new string[parameterNames.Length + 1];

                parameters[0] = $"{protocolHandlerAttribute.Command.GetType().FullName}.{protocolHandlerAttribute.Command}";
                Array.Copy(parameterNames, 0, parameters, 1, parameterNames.Length);

                if (method.ReturnType == typeof(Task))
                {
                    stringBuilder.AppendLine($"return await SendAsync({string.Join(',', parameters)});");
                    return;
                }

                if (method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))
                {
                    var genericArguments = method.ReturnType.GenericTypeArguments;
                    stringBuilder.AppendLine($"return await SendAsync<{string.Join(',', genericArguments.Select(argument => argument.FullName).ToArray())}>({string.Join(',', parameters)});");
                    return;
                }

                if (method.ReturnType != typeof(void))
                {
                    throw new ArgumentException("Interface method return type should either be Task or void", nameof(method));
                }

                stringBuilder.AppendLine($"Send({string.Join(',', parameters)});");
            }
            catch
            {
                if (method.ReturnType != typeof(void))
                {
                    stringBuilder.AppendLine("return default;");
                }
            }
        }
    }
}
