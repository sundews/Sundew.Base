// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConcurrentList.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Collections.Concurrent;

using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;

/// <summary>
/// An ordered concurrent list.
/// </summary>
/// <typeparam name="TItem">The item type.</typeparam>
public class ConcurrentList<TItem> : IList<TItem>
{
    private ImmutableList<TItem> inner = ImmutableList<TItem>.Empty;

    /// <inheritdoc/>
    public int Count => this.inner.Count;

    /// <inheritdoc/>
    public bool IsReadOnly => false;

    /// <inheritdoc/>
    public TItem this[int index]
    {
        get => this.inner[index];
        set
        {
            ImmutableList<TItem> oldList, newList;
            do
            {
                oldList = this.inner;
                newList = oldList.Replace(this.inner[index], value);
            }
            while (Interlocked.CompareExchange(ref this.inner, newList, oldList) != oldList);
        }
    }

    /// <inheritdoc/>
    public void Add(TItem item)
    {
        ImmutableList<TItem> oldList, newList;
        do
        {
            oldList = this.inner;
            newList = oldList.Add(item);
        }
        while (Interlocked.CompareExchange(ref this.inner, newList, oldList) != oldList);
    }

    /// <inheritdoc/>
    public void Clear()
    {
        ImmutableList<TItem> oldList, newList;
        do
        {
            oldList = this.inner;
            newList = oldList.Clear();
        }
        while (Interlocked.CompareExchange(ref this.inner, newList, oldList) != oldList);
    }

    /// <inheritdoc/>
    public bool Contains(TItem item)
    {
        return this.inner.Contains(item);
    }

    /// <inheritdoc/>
    public void CopyTo(TItem[] array, int arrayIndex)
    {
        this.inner.CopyTo(array, arrayIndex);
    }

    /// <inheritdoc/>
    public IEnumerator<TItem> GetEnumerator()
    {
        return this.inner.GetEnumerator();
    }

    /// <inheritdoc/>
    public int IndexOf(TItem item)
    {
        return this.inner.IndexOf(item);
    }

    /// <inheritdoc/>
    public void Insert(int index, TItem item)
    {
        ImmutableList<TItem> oldList, newList;
        do
        {
            oldList = this.inner;
            newList = oldList.Insert(index, item);
        }
        while (Interlocked.CompareExchange(ref this.inner, newList, oldList) != oldList);
    }

    /// <inheritdoc/>
    public bool Remove(TItem item)
    {
        ImmutableList<TItem> oldList, newList;
        do
        {
            oldList = this.inner;
            newList = oldList.Remove(item);
        }
        while (Interlocked.CompareExchange(ref this.inner, newList, oldList) != oldList);
        return oldList != newList;
    }

    /// <inheritdoc/>
    public void RemoveAt(int index)
    {
        ImmutableList<TItem> oldList, newList;
        do
        {
            oldList = this.inner;
            newList = oldList.RemoveAt(index);
        }
        while (Interlocked.CompareExchange(ref this.inner, newList, oldList) != oldList);
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }
}