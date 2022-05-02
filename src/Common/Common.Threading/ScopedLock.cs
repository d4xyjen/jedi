// <copyright file="ScopedLock.cs" company="Jedi Contributors">
// Copyright (c) Jedi Contributors. All rights reserved.
//  
// This software is licensed under the MIT license. Read the LICENSE file in the 
// repository for more information.
// </copyright>
namespace Jedi.Common.Threading
{
    /// <summary>
    /// Provides a scoped lock object without having to define a new scope.
    /// </summary>
    public class ScopedLock : IDisposable
    {
        /// <summary>
        /// The lock object to use.
        /// </summary>
        private readonly object _lockObject;

        /// <summary>
        /// Create a new scoped lock.
        /// </summary>
        /// <param name="lockObject">The lock object to use.</param>
        public ScopedLock(object lockObject)
        {
            _lockObject = lockObject;
            Monitor.Enter(_lockObject);
        }

        /// <summary>
        /// Dispose of the scoped lock.
        /// This is used to release the lock on the object when out of scope.
        /// </summary>
        public void Dispose()
        {
            Monitor.Exit(_lockObject);
        }
    }
}
