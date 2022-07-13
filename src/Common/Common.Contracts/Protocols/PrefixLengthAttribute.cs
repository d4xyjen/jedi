// <copyright file="WithLengthAttribute.cs" company="Jedi Contributors">
// Copyright (c) Jedi Contributors. All rights reserved.
//  
// This software is licensed under the MIT license. Read the LICENSE file in the 
// repository for more information.
// </copyright>
namespace Jedi.Common.Contracts.Protocols
{
    /// <summary>
    /// If present, the length of the field will be serialized and deserialized.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class PrefixLengthAttribute : Attribute
    {
    }
}
