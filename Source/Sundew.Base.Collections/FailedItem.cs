﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FailedItem.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Collections;

/// <summary>
/// Represents a failed items and its index in the list.
/// </summary>
/// <typeparam name="TItem">The item type.</typeparam>
/// <param name="Index">The index.</param>
/// <param name="Item">The item.</param>
public readonly record struct FailedItem<TItem>(int Index, TItem? Item);