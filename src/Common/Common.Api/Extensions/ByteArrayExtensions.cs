// <copyright file="ByteArrayExtensions.cs" company="Jedi Contributors">
// Copyright (c) Jedi Contributors. All rights reserved.
//  
// This software is licensed under the MIT license. Read the LICENSE file in the 
// repository for more information.
// </copyright>
namespace Jedi.Common.Api.Extensions
{
    /// <summary>
    /// Extension methods for byte arrays.
    /// </summary>
    public static class ByteArrayExtensions
    {
        /// <summary>
        /// Write to the array without allocating.
        /// </summary>
        /// <param name="array">The array to write to.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="position">The position in the array to write to.</param>
        /// <param name="offset">The offset of the write position.</param>
        public static void WriteBigEndianNonAlloc(this byte[] array, int value, int position, int offset)
        {
            switch (value)
            {
                case <= byte.MaxValue:
                    WriteBigEndianNonAlloc(array, (byte) value, position, offset);
                    return;
                case <= ushort.MaxValue:
                    WriteBigEndianNonAlloc(array, (ushort) value, position, offset);
                    return;
            }
        }

        /// <summary>
        /// Write to the array without allocating.
        /// </summary>
        /// <param name="array">The array to write to.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="position">The position in the array to write to.</param>
        /// <param name="offset">The offset of the write position.</param>
        public static void WriteBigEndianNonAlloc(this byte[] array, byte value, int position, int offset)
        {
            array[position + offset] = value;
        }

        /// <summary>
        /// Write to the array without allocating.
        /// </summary>
        /// <param name="array">The array to write to.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="position">The position in the array to write to.</param>
        /// <param name="offset">The offset of the write position.</param>
        public static void WriteBigEndianNonAlloc(this byte[] array, ushort value, int position, int offset)
        {
            array[position + offset] = (byte) (value >> 8);
            array[position + offset + 1] = (byte) value;
        }
    }
}
