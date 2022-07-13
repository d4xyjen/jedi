// <copyright file="Pool.cs" company="Jedi Contributors">
// Copyright (c) Jedi Contributors. All rights reserved.
//  
// This software is licensed under the MIT license. Read the LICENSE file in the 
// repository for more information.
// </copyright>
namespace Jedi.Common.Core
{
    /// <summary>
    /// A pool of objects that avoids allocations.
    /// </summary>
    /// <typeparam name="T">The type of the objects in the pool.</typeparam>
    public class Pool<T>
    {
        private readonly Stack<T> _objects = new Stack<T>();
        private readonly Func<T> _objectGenerator;

        /// <summary>
        /// Create a new <see cref="Pool{T}"/>.
        /// </summary>
        /// <param name="objectGenerator">The functioned used to generate new objects.</param>
        public Pool(Func<T> objectGenerator)
        {
            _objectGenerator = objectGenerator;
        }

        /// <summary>
        /// The number of objects in the pool.
        /// </summary>
        public int Count => _objects.Count;

        /// <summary>
        /// Take an element from the pool, or create a new one if the pool is empty.
        /// </summary>
        /// <returns>A new element.</returns>
        public T Take()
        {
            return _objects.Count > 0 ? _objects.Pop() : _objectGenerator();
        }

        /// <summary>
        /// Return an item to the pool.
        /// </summary>
        /// <param name="item">The item to return.</param>
        public void Return(T item)
        {
            _objects.Push(item);
        }

        /// <summary>
        /// Clear the pool of all objects.
        /// </summary>
        public void Clear()
        {
            _objects.Clear();
        }
    }
}
