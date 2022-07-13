// <copyright file="NetworkStreamExtensions.cs" company="Jedi Contributors">
// Copyright (c) Jedi Contributors. All rights reserved.
//  
// This software is licensed under the MIT license. Read the LICENSE file in the 
// repository for more information.
// </copyright>

using System.Net.Sockets;

namespace Jedi.Common.Api.Extensions
{
    /// <summary>
    /// Extensions for the <see cref="NetworkStream"/> class.
    /// </summary>
    public static class NetworkStreamExtensions
    {
        /// <summary>
        /// Returns 0 safely while reading data from a network stream.
        /// </summary>
        /// <param name="stream">The stream to read data from.</param>
        /// <param name="buffer">The buffer to read the data into.</param>
        /// <param name="offset">The offset in the buffer to read the data into,</param>
        /// <param name="size">The size of the data to read.</param>
        /// <returns>The number of bytes that were read.</returns>
        public static int ReadSafely(this NetworkStream stream, byte[] buffer, int offset, int size)
        {
            try
            {
                return stream.Read(buffer, offset, size);
            }
            // if we voluntarily close our own session
            catch (IOException)
            {
                return 0;
            }
            // can be thrown if session.Destroy() has been called
            catch (ObjectDisposedException)
            {
                return 0;
            }
        }

        /// <summary>
        /// Read exactly n bytes from the stream.
        /// This method blocks until n bytes are read.
        /// </summary>
        /// <param name="stream">The stream to read the data from.</param>
        /// <param name="buffer">The buffer to read the data into.</param>
        /// <param name="amount">The number of bytes to read.</param>
        /// <returns>True if the data was read, false if it was not.</returns>
        public static bool ReadExactly(this NetworkStream stream, byte[] buffer, int amount)
        {
            var bytesRead = 0;
            while (bytesRead < amount)
            {
                var remaining = amount - bytesRead;
                var result = stream.ReadSafely(buffer, bytesRead, remaining);

                if (result == 0)
                {
                    return false;
                }

                bytesRead += result;
            }

            return true;
        }
    }
}
