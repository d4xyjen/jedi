// <copyright file="ArrayLengthAttribute.cs" company="Jedi Contributors">
// Copyright (c) Jedi Contributors. All rights reserved.
//  
// This software is licensed under the MIT license. Read the LICENSE file in the 
// repository for more information.
// </copyright>
namespace Jedi.Common.Contracts.Serialization
{
    /// <summary>
    /// Specifies the length of array or string data in a property.
    /// </summary>
    public class LengthAttribute : Attribute
    {
        /// <summary>
        /// Create a new <see cref="LengthAttribute"/>.
        /// </summary>
        /// <param name="length">The length of the data.</param>
        public LengthAttribute(int length)
        {
            Length = length;
        }

        /// <summary>
        /// The length of the data.
        /// </summary>
        public int Length { get; }
    }
}
