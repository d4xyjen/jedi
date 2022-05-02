// <copyright file="EventMessage.cs" company="Jedi Contributors">
// Copyright (c) Jedi Contributors. All rights reserved.
//  
// This software is licensed under the MIT license. Read the LICENSE file in the 
// repository for more information.
// </copyright>

using Jedi.Common.Logging.Constants;
using System.Text.RegularExpressions;

namespace Jedi.Common.Logging.Events
{
    /// <summary>
    /// Rich metadata about a log event.
    /// </summary>
    public class EventMetadata
    {
        /// <summary>
        /// Regex used to match parameters in event messages.
        /// </summary>
        private static readonly Regex ParameterRegex = new Regex(MessageConstants.ParameterRegexPattern, RegexOptions.Compiled);

        /// <summary>
        /// Create new metadata for an event.
        /// </summary>
        /// <param name="message">The formatted event message.</param>
        /// <param name="messageTemplate">The template, or format, for the event message.</param>
        /// <param name="parameters">The event's parameters.</param>
        private EventMetadata(string message, string messageTemplate, Dictionary<string, object?> parameters)
        {
            Message = message;
            MessageTemplate = messageTemplate;
            Parameters = parameters;
        }
        
        /// <summary>
        /// Format an event's message.
        /// </summary>
        /// <param name="template">The template, or format, for the message.</param>
        /// <param name="parameters">Optional parameters that the message can reference.</param>
        /// <returns>The formatted message.</returns>
        public static EventMetadata FromTemplate(string template, params object?[] parameters)
        {
            var capturedParameters = new Dictionary<string, object?>();
            var parameterIndex = 0;
            var message = ParameterRegex.Replace(template, match =>
            {
                var parameterName = match.Groups[1].Value;
                if (!capturedParameters.TryGetValue(parameterName, out var parameterValue))
                {
                    if (parameterIndex + 1 > parameters.Length)
                    {
                        throw new ArgumentOutOfRangeException(nameof(parameterIndex), "Too many parameters provided");
                    }

                    parameterValue = parameters[parameterIndex++];
                    capturedParameters.Add(parameterName, parameterValue);
                }

                return parameterValue?.ToString() ?? MessageConstants.NullParameterValue;
            });

            return new EventMetadata(message, template, capturedParameters);
        }

        /// <summary>
        /// The formatted message, built from its template and parameters.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// The template, or format, of the message, which can specify references to properties in the event's metadata.
        /// </summary>
        public string MessageTemplate { get; }

        /// <summary>
        /// The parameters of the message.
        /// </summary>
        public Dictionary<string, object?> Parameters { get; }
    }
}
