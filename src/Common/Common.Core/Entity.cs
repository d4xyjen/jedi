// <copyright file="Entity.cs" company="Jedi Contributors">
// Copyright (c) Jedi Contributors. All rights reserved.
//  
// This software is licensed under the MIT license. Read the LICENSE file in the 
// repository for more information.
// </copyright>

namespace Jedi.Common.Core
{
    /// <summary>
    /// Base class for all entities in Jedi.
    /// </summary>
    public class Entity : IDisposable
    {
        private int _disposed;

        private bool Disposed => Interlocked.CompareExchange(ref _disposed, 1, 0) != 0;

        /// <summary>
        /// Create a new <see cref="Entity"/>.
        /// </summary>
        public Entity()
        {
            Id = Guid.NewGuid();
        }

        /// <summary>
        /// The entity's GUID.
        /// </summary>
        public Guid Id { get; protected set; }

        /// <summary>
        /// Dispose of the entity.
        /// </summary>
        public void Dispose() => Destroy();

        /// <summary>
        /// Dispose of the entity.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            // base entity dispose code goes here
        }

        /// <summary>
        /// Entity destructor.
        /// </summary>
        ~Entity()
        {
            Dispose(false);
        }

        /// <summary>
        /// Destroy the entity.
        /// </summary>
        public void Destroy()
        {
            if (Disposed)
            {
                return;
            }

            Dispose(true);
            Interlocked.Exchange(ref _disposed, 1);
        }
    }
}
