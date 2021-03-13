// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDisposableReporter.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Disposal
{
    using Sundew.Base.Reporting;

    /// <summary>Interface for implementing a disposing subject that reports, what it disposes.</summary>
    public interface IDisposableReporter : IReporter
    {
        /// <summary>Called when object is disposed.</summary>
        /// <param name="disposable">The disposable.</param>
        void Disposed(object disposable);
    }
}