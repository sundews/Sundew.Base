// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IReportingDisposable.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Disposal.Internal
{
    internal interface IReportingDisposable
    {
        /// <summary>Sets the reporter.</summary>
        /// <param name="disposableReporter">The disposable reporter.</param>
        void SetReporter(IDisposableReporter? disposableReporter);
    }
}