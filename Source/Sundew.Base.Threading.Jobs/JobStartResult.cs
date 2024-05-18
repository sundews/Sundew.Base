// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JobStartResult.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Threading.Jobs;

using System.Threading;

/// <summary>
/// A result providing access to the jobs cancellation token and a value indicating whether the job was already started.
/// </summary>
/// <param name="CancellationToken">The cancellation token.</param>
/// <param name="WasAlreadyRunning">A value indicating whether the job was already running.</param>
public readonly record struct JobStartResult(CancellationToken CancellationToken, bool WasAlreadyRunning);