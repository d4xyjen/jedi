// <copyright file="EventMetadataConverter.cs" company="Jedi Contributors">
// Copyright (c) Jedi Contributors. All rights reserved.
//  
// This software is licensed under the MIT license. Read the LICENSE file in the 
// repository for more information.
// </copyright>

using System.Text.Json;
using System.Text.Json.Serialization;

namespace Jedi.Common.Logging.Events
{
    /// <summary>
    /// Convert event metadata to Json, and vice versa.
    /// </summary>
    public class EventMetadataJsonConverter : JsonConverter<EventMetadata>
    {
        public override EventMetadata? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Write metadata as Json.
        /// </summary>
        /// <param name="writer">The Json writer instance.</param>
        /// <param name="metadata">The metadata to write.</param>
        /// <param name="options">The serialization options.</param>
        public override void Write(Utf8JsonWriter writer, EventMetadata metadata, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString("Message", metadata.Message);
            writer.WriteString("MessageTemplate", metadata.MessageTemplate);

            foreach (var (name, value) in metadata.Parameters)
            {
                if (value == null)
                {
                    writer.WriteNull(name);
                    continue;
                }

                using var token = JsonDocument.Parse(JsonSerializer.SerializeToUtf8Bytes(value, value.GetType()));

                writer.WritePropertyName(name);
                token.RootElement.WriteTo(writer);
            }

            writer.WriteEndObject();
        }
    }
}
