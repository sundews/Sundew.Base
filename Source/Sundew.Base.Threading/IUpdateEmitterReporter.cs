// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IUpdateEmitterReporter.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Threading;

using System;
using Sundew.Base.Reporting;

/// <summary>
/// Interface for implementing a reporter for an <see cref="IUpdateEmitter{TValue}"/>.
/// </summary>
/// <typeparam name="TValue">The value type.</typeparam>
public interface IUpdateEmitterReporter<in TValue> : IReporter
{
    /// <summary>
    /// Reports that the specified value was emitted.
    /// </summary>
    /// <param name="value">The value to emit. Can be <see langword="null"/> if <typeparamref name="TValue"/> is a nullable type.</param>
    [Report(Message = "Emitted: {Value}")]
    void Emitted(TValue? value);

    /// <summary>
    /// Reports an exception occuring during an update.
    /// </summary>
    /// <param name="exception">The exception that was thrown during the update. Cannot be null.</param>
    void ErrorDuringUpdate(Exception exception);
}