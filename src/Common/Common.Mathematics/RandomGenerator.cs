// <copyright file="RandomGenerator.cs" company="Jedi Contributors">
// Copyright (c) Jedi Contributors. All rights reserved.
//  
// This software is licensed under the MIT license. Read the LICENSE file in the 
// repository for more information.
// </copyright>

namespace Jedi.Common.Mathematics
{
    /// <summary>
    /// Class used to generate random numbers.
    /// </summary>
    public interface IRandomGenerator
    {
        /// <summary>
        /// Get a random number between <paramref name="minValue"/> and <paramref name="maxValue"/>.
        /// </summary>
        /// <param name="minValue">The minimum value.</param>
        /// <param name="maxValue">The maximum value.</param>
        /// <returns>A random number between <paramref name="minValue"/> and <paramref name="maxValue"/>.</returns>
        public int GetRandomNumber(int minValue, int maxValue);

        /// <summary>
        /// Get a random number between 0 and 2147483647.
        /// </summary>
        /// <returns>A random number between 0 and 2147483647.</returns>
        public int GetRandomNumber();

        /// <summary>
        /// Get a random number between 0 and <paramref name="maxValue"/>.
        /// </summary>
        /// <param name="maxValue">The maximum value.</param>
        /// <returns>A random number between 0 and <paramref name="maxValue"/>.</returns>
        public int GetRandomNumber(int maxValue);

        /// <summary>
        /// Get a random double.
        /// </summary>
        /// <returns>A random double between 0.0 and 1.0.</returns>
        public double GetRandomDouble();
    }

    /// <summary>
    /// Class used to generate random numbers.
    /// </summary>
    public class RandomGenerator : IRandomGenerator
    {
        private readonly System.Security.Cryptography.RandomNumberGenerator _randomNumberGenerator = System.Security.Cryptography.RandomNumberGenerator.Create();

        /// <summary>
        /// Get a random number between <paramref name="minValue"/> and <paramref name="maxValue"/>.
        /// </summary>
        /// <param name="minValue">The minimum value.</param>
        /// <param name="maxValue">The maximum value.</param>
        /// <returns>A random number between <paramref name="minValue"/> and <paramref name="maxValue"/>.</returns>
        public int GetRandomNumber(int minValue, int maxValue)
        {
            return (int) Math.Round(GetRandomDouble() * (maxValue - minValue)) + minValue;
        }

        /// <summary>
        /// Get a random number between 0 and 2147483647.
        /// </summary>
        /// <returns>A random number between 0 and 2147483647.</returns>
        public int GetRandomNumber()
        {
            return GetRandomNumber(0, int.MaxValue);
        }

        /// <summary>
        /// Get a random number between 0 and <paramref name="maxValue"/>.
        /// </summary>
        /// <param name="maxValue">The maximum value.</param>
        /// <returns>A random number between 0 and <paramref name="maxValue"/>.</returns>
        public int GetRandomNumber(int maxValue)
        {
            return GetRandomNumber(0, maxValue);
        }

        /// <summary>
        /// Get a random double.
        /// </summary>
        /// <returns>A random double between 0.0 and 1.0.</returns>
        public double GetRandomDouble()
        {
            var buffer = new byte[4];
            _randomNumberGenerator.GetBytes(buffer);

            return (double) BitConverter.ToUInt32(buffer, 0) / uint.MaxValue;
        }
    }
}
