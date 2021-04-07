// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAssociatedDisposer.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Disposal
{
    using System;

    /// <summary>
    /// Interface for implementing a disposer for object that do not implement <see cref="IDisposable"/> themselves, but are associated with disposable objects.
    /// </summary>
    /// <typeparam name="TObject">The type of the object.</typeparam>
    /// <seealso cref="System.IDisposable" />
    public interface IAssociatedDisposer<in TObject> : IDisposable
    {
        /// <summary>
        /// Disposes the resources associated with the specified object.
        /// </summary>
        /// <param name="value">The object.</param>
        void Dispose(TObject value);
    }
}