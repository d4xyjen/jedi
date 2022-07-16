// <copyright file="BinarySerializer.cs" company="Jedi Contributors">
// Copyright (c) Jedi Contributors. All rights reserved.
//  
// This software is licensed under the MIT license. Read the LICENSE file in the 
// repository for more information.
// </copyright>

using Jedi.Common.Contracts.Protocols;
using Jedi.Common.Core.Exceptions;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace Jedi.Common.Api
{
    /// <summary>
    /// Serialize and deserialize objects into binary data.
    /// </summary>
    public class BinarySerializer : IDisposable
    {
        private readonly MemoryStream _stream;
        private readonly BinaryReader _reader;
        private readonly BinaryWriter _writer;

        /// <summary>
        /// Create a new <see cref="BinarySerializer"/>.
        /// </summary>
        public BinarySerializer()
        {
            _stream = new MemoryStream();
            _reader = new BinaryReader(_stream);
            _writer = new BinaryWriter(_stream);
        }

        /// <summary>
        /// Create a new <see cref="BinarySerializer"/>.
        /// </summary>
        public BinarySerializer(ArraySegment<byte> data)
        {
            _stream = data.Array == null ? new MemoryStream() : new MemoryStream(data.Array, data.Offset, data.Count);
            _reader = new BinaryReader(_stream);
            _writer = new BinaryWriter(_stream);
        }

        /// <summary>
        /// Serialize an object into binary data.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <returns>The serialized data.</returns>
        public byte[] Serialize(object obj)
        {
            var properties = GetDataMembers(obj);

            for (var i = 0; i < properties.Length; i++)
            {
                var property = properties[i];
                var propertyValue = property?.GetValue(obj);

                if (property == null || propertyValue == null)
                {
                    continue;
                }

                Serialize(propertyValue, property);
            }

            return _stream.ToArray();
        }

        /// <summary>
        /// Deserialize an object from data.
        /// </summary>
        /// <param name="type">The type of object to deserialize.</param>
        /// <returns>The deserialized object.</returns>
        public Protocol? Deserialize(Type type)
        {
            var obj = Activator.CreateInstance(type);
            if (obj == null)
            {
                throw new SystemError($"BinarySerializer.Deserialize: Failed to create instance of target type {type.FullName}");
            }

            var properties = GetDataMembers(obj);

            for (var i = 0; i < properties.Length; i++)
            {
                var property = properties[i];
                if (property == null)
                {
                    continue;
                }

                var propertyValue = GetDefaultValue(property);
                if (propertyValue == null)
                {
                    continue;
                }

                var deserializedProperty = Deserialize(propertyValue, property);
                if (deserializedProperty == null)
                {
                    // Failed to deserialize a property.
                    // To prevent further errors, just return null.
                    return null;
                }

                property.SetValue(obj, deserializedProperty);
            }

            return obj as Protocol;
        }

        private object? GetDefaultValue(PropertyInfo property)
        {
            if (property.PropertyType == typeof(string))
            {
                return string.Empty;
            }
            else if (property.PropertyType == typeof(byte[]))
            {
                return Array.Empty<byte>();
            }
            else
            {
                return Activator.CreateInstance(property.PropertyType);
            }
        }


        private void Serialize(object obj, PropertyInfo propertyInfo)
        {
            switch (obj)
            {
                case byte:
                    _writer.Write((byte) obj);
                    break;
                case char:
                    _writer.Write((char) obj);
                    break;
                case bool:
                    _writer.Write((bool) obj);
                    break;
                case short:
                    _writer.Write((short) obj);
                    break;
                case ushort:
                    _writer.Write((ushort) obj);
                    break;
                case int:
                    _writer.Write((int) obj);
                    break;
                case uint:
                    _writer.Write((uint) obj);
                    break;
                case long:
                    _writer.Write((long) obj);
                    break;
                case ulong:
                    _writer.Write((ulong) obj);
                    break;
                case double:
                    _writer.Write((double) obj);
                    break;
                case float:
                    _writer.Write((float) obj);
                    break;
                case decimal:
                    _writer.Write((decimal) obj);
                    break;
                case string str:
                    if (propertyInfo.GetCustomAttribute<PrefixLengthAttribute>() != null)
                    {
                        _writer.Write((byte) str.Length);
                    }

                    var stringBuffer = Encoding.ASCII.GetBytes(str);
                    _writer.Write(stringBuffer);

                    for (var i = 0; i < str.Length - (propertyInfo.GetCustomAttribute<MaxLengthAttribute>()?.Length ?? stringBuffer.Length); i++)
                    {
                        _writer.Write((byte) 0);
                    }

                    break;
                case byte[] byteArray:
                    if (propertyInfo.GetCustomAttribute<PrefixLengthAttribute>() != null)
                    {
                        _writer.Write((byte) byteArray.Length);
                    }

                    _writer.Write(byteArray);

                    for (var i = 0; i < byteArray.Length - (propertyInfo.GetCustomAttribute<MaxLengthAttribute>()?.Length ?? byteArray.Length); i++)
                    {
                        _writer.Write((byte) 0);
                    }

                    break;
                default:
                    _writer.Write(Serialize(obj));
                    break;
            }
        }

        private object? Deserialize(object obj, PropertyInfo propertyInfo)
        {
            try
            {
                switch (obj)
                {
                    case byte:
                        return _reader.ReadByte();
                    case char:
                        return _reader.ReadChar();
                    case bool:
                        return _reader.ReadBoolean();
                    case short:
                        return _reader.ReadInt16();
                    case ushort:
                        return _reader.ReadUInt16();
                    case int:
                        return _reader.ReadInt32();
                    case uint:
                        return _reader.ReadUInt32();
                    case long:
                        return _reader.ReadInt64();
                    case ulong:
                        return _reader.ReadUInt64();
                    case double:
                        return _reader.ReadDouble();
                    case float:
                        return _reader.ReadSingle();
                    case decimal:
                        return _reader.ReadDecimal();
                    case string:
                        var stringLength = propertyInfo.GetCustomAttribute<PrefixLengthAttribute>() != null
                            ? _reader.ReadByte()
                            : propertyInfo.GetCustomAttribute<MaxLengthAttribute>()?.Length ?? 0;

                        var stringBuffer = new byte[stringLength];
                        var charCount = 0;

                        _reader.Read(stringBuffer, 0, stringBuffer.Length);

                        if (stringBuffer[stringLength - 1] != 0)
                        {
                            charCount = stringLength;
                        }
                        else
                        {
                            while (stringBuffer[charCount] != 0 && charCount < stringLength)
                            {
                                charCount++;
                            }
                        }

                        return charCount > 0 ? Encoding.ASCII.GetString(stringBuffer, 0, charCount) : string.Empty;
                    case byte[]:
                        // If no max length is specified, just read until we reach the end
                        // of the stream.
                        var byteArrayLength = propertyInfo.GetCustomAttribute<PrefixLengthAttribute>() != null
                            ? _reader.ReadByte()
                            : propertyInfo.GetCustomAttribute<MaxLengthAttribute>()?.Length ??
                              (int) (_stream.Length - _stream.Position);

                        return _reader.ReadBytes(byteArrayLength);
                    default:
                        return Deserialize(typeof(object));
                }
            }
            catch (EndOfStreamException)
            {
                return null;
            }
        }

        /// <summary>
        /// Dispose of the serializer.
        /// </summary>
        public void Dispose()
        {
            _reader.Dispose();
            _writer.Dispose();
        }

        private static PropertyInfo?[] GetDataMembers(object obj)
        {
            return obj
                .GetType()
                .GetProperties()
                .Select(property => property.GetCustomAttribute<IgnoreDataMemberAttribute>() != null ? null : property)
                .ToArray();
        }
    }
}
