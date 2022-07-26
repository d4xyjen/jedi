// <copyright file="BinarySerializer.cs" company="Jedi Contributors">
// Copyright (c) Jedi Contributors. All rights reserved.
//  
// This software is licensed under the MIT license. Read the LICENSE file in the 
// repository for more information.
// </copyright>

using Jedi.Common.Api.Constants;
using Jedi.Common.Contracts.Serialization;
using Jedi.Common.Core.Exceptions;
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
            var properties = GetContractDataMembers(obj.GetType());
            for (var i = 0; i < properties.Count; i++)
            {
                var property = properties[i];
                var dataMember = property.GetCustomAttribute<DataMemberAttribute>();

                if (dataMember != null)
                {
                    var propertyValue = property.GetValue(obj);
                    var value = propertyValue == null && dataMember.EmitDefaultValue ? GetDefaultValue(property.PropertyType, property) : propertyValue;

                    if (value == null)
                    {
                        if (dataMember.IsRequired)
                        {
                            throw new SystemError("BinarySerializer.Serialize: Failed to serialize", new ArgumentNullException(dataMember.Name ?? property.Name, "Required data member was not present"));
                        }

                        continue;
                    }

                    Serialize(value, property);
                }
            }

            return _stream.ToArray();
        }

        /// <summary>
        /// Deserialize an object from data.
        /// </summary>
        /// <param name="type">The type of object to deserialize.</param>
        /// <returns>The deserialized object.</returns>
        public object Deserialize(Type type)
        {
            var obj = GetDefaultValue(type);
            if (obj == null)
            {
                throw new SystemError($"BinarySerializer.Deserialize: Failed to create instance of target type {type.FullName}");
            }

            var properties = GetContractDataMembers(type);
            for (var i = 0; i < properties.Count; i++)
            {
                var property = properties[i];
                var dataMember = property.GetCustomAttribute<DataMemberAttribute>();

                if (dataMember != null)
                {
                    var defaultValue = GetDefaultValue(property.PropertyType, property);
                    var value = Deserialize(defaultValue, property);

                    if (value == null && dataMember.IsRequired)
                    {
                        throw new SystemError("BinarySerializer.Deserialize: Failed to deserialize", new ArgumentNullException(dataMember.Name ?? property.Name, "Required data member was not initialized."));
                    }

                    property.SetValue(obj, value);
                }
            }

            return obj;
        }

        private object? GetDefaultValue(Type? type, PropertyInfo? propertyInfo = null)
        {
            if (type == null)
            {
                return default;
            }

            if (type == typeof(string))
            {
                return string.Empty;
            }

            if (type.IsArray)
            {
                var elementType = type.GetElementType();
                var arrayLength = propertyInfo?.GetCustomAttribute<LengthAttribute>()?.Length ?? _reader.ReadByte();

                return elementType == null ? null : Array.CreateInstance(elementType, arrayLength);
            }

            return Activator.CreateInstance(type);
        }

        private void Serialize(object obj, PropertyInfo propertyInfo)
        {
            if (obj is Array array)
            {
                if (!Attribute.IsDefined(propertyInfo, typeof(LengthAttribute)))
                {
                    _writer.Write((byte) array.Length);
                }
                else if (propertyInfo.GetCustomAttribute<LengthAttribute>()?.Length != array.Length)
                {
                    // if the length attribute is specified, the array length must match that length
                    throw new InvalidDataContractException("Length of the member was not equal its attribute's specification.");
                }

                for (var i = 0; i < array.Length; i++)
                {
                    var value = array.GetValue(i);
                    if (value == null)
                    {
                        throw new NullReferenceException($"Array element at index {i} was null.");
                    }

                    // serialize each object in the array to the message
                    Serialize(value, propertyInfo);
                }

                return;
            }

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
                case Guid guid:
                    WriteString(guid.ToString(), GuidConstants.GuidLength);
                    break;
                case string str:
                    if (!Attribute.IsDefined(propertyInfo, typeof(LengthAttribute)))
                    {
                        _writer.Write((byte) str.Length);
                    }
                    
                    WriteString(str, propertyInfo.GetCustomAttribute<LengthAttribute>()?.Length);
                    break;
                default:
                    _writer.Write(Serialize(obj));
                    break;
            }
        }

        private void WriteString(string value, int? length)
        {
            var stringBuffer = Encoding.ASCII.GetBytes(value);
            _writer.Write(stringBuffer);

            for (var i = 0; i < length - stringBuffer.Length; i++)
            {
                _writer.Write((byte) 0);
            }
        }

        private object? Deserialize(object? obj, PropertyInfo propertyInfo)
        {
            if (obj == null)
            {
                return null;
            }

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
                    case Guid:
                        return Guid.Parse(ReadString(GuidConstants.GuidLength));
                    case string:
                        return ReadString(propertyInfo.GetCustomAttribute<LengthAttribute>()?.Length ?? _reader.ReadByte());

                    // array is initialized with the correct length before this method is
                    // called so there's no need to get it again here
                    case Array array:
                        var elementType = propertyInfo.PropertyType.GetElementType();
                        if (elementType == null)
                        {
                            return null;
                        }

                        for (var i = 0; i < array.Length; i++)
                        {
                            array.SetValue(Deserialize(elementType), i);
                        }

                        return array;
                    default:
                        return Deserialize(typeof(object));
                }
            }
            catch (FormatException)
            {
                if (obj is Guid)
                {
                    return Guid.Empty;
                }

                throw;
            }
            catch (EndOfStreamException)
            {
                return null;
            }
        }

        private string ReadString(int length)
        {
            var stringBuffer = new byte[length];
            var charCount = 0;

            _reader.Read(stringBuffer, 0, stringBuffer.Length);

            if (stringBuffer[length - 1] != 0)
            {
                charCount = length;
            }
            else
            {
                while (stringBuffer[charCount] != 0 && charCount < length)
                {
                    charCount++;
                }
            }

            return charCount > 0 ? Encoding.ASCII.GetString(stringBuffer, 0, charCount) : string.Empty;
        }

        /// <summary>
        /// Dispose of the serializer.
        /// </summary>
        public void Dispose()
        {
            _reader.Dispose();
            _writer.Dispose();
        }

        private static List<PropertyInfo> GetContractDataMembers(Type type)
        {
            var properties = new List<PropertyInfo>();
            GetContractDataMembers(type, properties);
            return properties;
        }

        private static void GetContractDataMembers(Type type, List<PropertyInfo> properties)
        {
            // get the properties with the DataMember attribute in order and including
            // inherited properties that also specify the DataMember attribute
            //
            // the order of properties is as follows:
            //  - data members of base types
            //  - data members of current type without the Order property of the DataMemberAttribute set, in alphabetical order
            //  - any data members with the Order property of the DataMemberAttribute set, ordered by the Order property first, then 
            //  alphabetically if there is more than one member of a certain Order value

            if (type.BaseType?.GetCustomAttribute<DataContractAttribute>() != null)
            {
                GetContractDataMembers(type.BaseType, properties);
            }

            properties.AddRange(type
                .GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)
                .Where(property => Attribute.IsDefined(property, typeof(DataMemberAttribute)) && !Attribute.IsDefined(property, typeof(IgnoreDataMemberAttribute)))
            );

            properties.Sort((property1, property2) =>
            {
                var dataMember1 = property1.GetCustomAttribute<DataMemberAttribute>()!;
                var dataMember2 = property2.GetCustomAttribute<DataMemberAttribute>()!;

                return dataMember2.Order - dataMember1.Order;
            });
        }
    }
}
