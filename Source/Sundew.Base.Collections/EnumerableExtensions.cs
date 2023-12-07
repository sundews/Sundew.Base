// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumerableExtensions.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Collections;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Sundew.Base.Collections.Immutable;
using Sundew.Base.Collections.Internal;
using Sundew.Base.Memory;
using Sundew.Base.Primitives;

/// <summary>
/// Extends the generic IEnumerable interface with functions.
/// </summary>
public static partial class EnumerableExtensions
{
    private static readonly Func<object?, bool> NotNullPredicate = x => x != default;
    private static readonly Func<object?, object> NotNullFunc = x => x!;

    /// <summary>
    /// Finds the index based on the given predicate.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <returns>The index found by the matching predicate.</returns>
    public static IEnumerable<TItem> WhereNotNull<TItem>(this IEnumerable<TItem?>? enumerable)
    {
        return enumerable == null ? Enumerable.Empty<TItem>() : enumerable.Where(x => x != null).Select(x => x!);
    }

    /// <summary>
    /// Finds the index based on the given predicate.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <returns>The index found by the matching predicate.</returns>
    public static IEnumerable<TItem> WhereNotDefault<TItem>(this IEnumerable<TItem?>? enumerable)
        where TItem : struct
    {
        return enumerable == null ? Enumerable.Empty<TItem>() : enumerable.Where(x => x.HasValue).Select(x => x!.Value);
    }

    /// <summary>
    /// Gets an <see cref="IEnumerable{TItem}"/> from an item.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="item">The item.</param>
    /// <returns>An <see cref="IEnumerable{TItem}"/>.</returns>
    public static IReadOnlyList<TItem> ToEnumerable<TItem>(this TItem? item)
    {
        if (item != null)
        {
            return new[] { item };
        }

        return Arrays.Empty<TItem>();
    }

    /// <summary>
    /// Finds the index based on the given predicate.
    /// </summary>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="index">The index.</param>
    /// <returns>
    /// The index found by the matching predicate.
    /// </returns>
    public static object? ElementAt(this IEnumerable enumerable, int index)
    {
        if (enumerable is IList list)
        {
            return list[index];
        }

        // ReSharper disable ExpressionIsAlwaysNull

        // ReSharper restore ExpressionIsAlwaysNull
        // ReSharper disable ConditionIsAlwaysTrueOrFalse
        if (enumerable is Array array)
        {
            return array.GetValue(index);
        }

        // ReSharper restore ConditionIsAlwaysTrueOrFalse
        var indexer = 0;
        foreach (var item in enumerable)
        {
            if (indexer++ == index)
            {
                return item;
            }
        }

        throw new IndexOutOfRangeException($"The index: {index} was out of range.");
    }

    /// <summary>
    /// Finds the index based on the given predicate.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="indexPredicate">The index predicate.</param>
    /// <returns>The index found by the matching predicate.</returns>
    public static int IndexOf<TItem>(this IEnumerable<TItem> enumerable, Func<TItem, bool> indexPredicate)
    {
        if (enumerable is List<TItem> list)
        {
            return list.FindIndex(x => indexPredicate(x));
        }

        var index = 0;
        foreach (var item in enumerable)
        {
            if (indexPredicate(item))
            {
                return index;
            }

            index++;
        }

        return -1;
    }

    /// <summary>
    /// Creates and returns an <see cref="IReadOnlyCollection{Object}" /> with the executed enumerable if needed.
    /// </summary>
    /// <param name="enumerable">The enumerable.</param>
    /// <returns>
    /// An <see cref="IReadOnlyCollection{Object}" /> containing the result of the executed ling query.
    /// </returns>
    public static IReadOnlyCollection<object> ToReadOnly(this IEnumerable enumerable)
    {
        return enumerable switch
        {
            IReadOnlyCollection<object> readOnlyCollection => readOnlyCollection,
            ICollection collection => new NonDeferredEnumerable<object>(enumerable.Cast<object>(), collection.Count),
            _ => new ReadOnlyArray<object>(enumerable.Cast<object>().ToArray()),
        };
    }

    /// <summary>
    /// Creates and returns an <see cref="IReadOnlyCollection{TItem}" /> with the executed enumerable if needed.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <returns>
    /// An <see cref="IReadOnlyCollection{TItem}" /> containing the result of the executed ling query.
    /// </returns>
    public static IReadOnlyCollection<TItem> ToReadOnly<TItem>(this IEnumerable<TItem> enumerable)
    {
        return enumerable switch
        {
            IReadOnlyCollection<TItem> readOnlyCollection => readOnlyCollection,
            ICollection<TItem> collectionOfT => new ReadOnlyCollection<TItem>(collectionOfT),
            ICollection collection => new NonDeferredEnumerable<TItem>(enumerable, collection.Count),
            _ => new ReadOnlyArray<TItem>(enumerable.ToArray()),
        };
    }

    /// <summary>
    /// Iterates through the enumerable and executes the specified action on all items.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="action">The action.</param>
    public static void ForEach<TItem>(this IEnumerable<TItem> enumerable, Action<TItem, int> action)
    {
        var i = 0;
        foreach (var item in enumerable)
        {
            action(item, i++);
        }
    }

    /// <summary>
    /// Iterates through the enumerable and executes the specified action on all items.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="action">The action.</param>
    public static void ForEach<TItem>(this IEnumerable<TItem> enumerable, Action<TItem> action)
    {
        foreach (var item in enumerable)
        {
            action(item);
        }
    }

    /// <summary>
    /// Iterates through the enumerable and executes the specified action on all items.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="action">The action.</param>
    /// <returns>The original enumerable.</returns>
    public static IEnumerable<TItem> ForEachItem<TItem>(this IEnumerable<TItem> enumerable, Action<TItem, int> action)
    {
        var i = 0;
        foreach (var item in enumerable)
        {
            action(item, i++);
            yield return item;
        }
    }

    /// <summary>
    /// Iterates through the enumerable and executes the specified action on all items.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="action">The action.</param>
    /// <returns>The original enumerable.</returns>
    public static IEnumerable<TItem> ForEachItem<TItem>(this IEnumerable<TItem> enumerable, Action<TItem> action)
    {
        foreach (var item in enumerable)
        {
            action(item);
            yield return item;
        }
    }

    /// <summary>
    /// Create an array from the specified enumerable.
    /// More efficient than Linq Select + ToArray if the enumerable is not deferred.
    /// </summary>
    /// <typeparam name="TInItem">The type of the in item.</typeparam>
    /// <typeparam name="TOutItem">The type of the out item.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="selectFunc">The select function.</param>
    /// <returns>The new array.</returns>
    public static TOutItem[] ToArray<TInItem, TOutItem>(this IEnumerable<TInItem> enumerable, Func<TInItem, TOutItem> selectFunc)
    {
        return enumerable switch
        {
            ImmutableArray<TInItem> immutableArray => ToArrayUnsafe(immutableArray, selectFunc),
            IImmutableList<TInItem> immutableList => ToArrayUnsafe(immutableList, selectFunc, immutableList.Count),
            IReadOnlyList<TInItem> readonlyListOfT => ToArrayUnsafe(readonlyListOfT, selectFunc),
            ICollection<TInItem> collectionOfT => ToArrayUnsafe(collectionOfT, selectFunc, collectionOfT.Count),
            IReadOnlyCollection<TInItem> readonlyCollectionOfT => ToArrayUnsafe(enumerable, selectFunc, readonlyCollectionOfT.Count),
            ICollection collection => ToArrayUnsafe(enumerable, selectFunc, collection.Count),
            _ => enumerable.Select(selectFunc).ToArray(),
        };
    }

    /// <summary>Concats the specified additional.</summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="second">The second enumerable.</param>
    /// <param name="third">The third enumerable.</param>
    /// <returns>The new Array.</returns>
    public static TItem[] Concat<TItem>(this IEnumerable<TItem> enumerable, IEnumerable<TItem> second, IEnumerable<TItem> third)
    {
        var estimatedCount = EstimateCount<TItem>(enumerable);
        estimatedCount += EstimateCount<TItem>(second);
        estimatedCount += EstimateCount<TItem>(third);

        var buffer = new Buffer<TItem>(estimatedCount);
        buffer.WriteRange(enumerable);
        buffer.WriteRange(second);
        buffer.WriteRange(third);

        return buffer.ToFinalArray();
    }

    /// <summary>Concats the specified additional.</summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="second">The second enumerable.</param>
    /// <param name="third">The third enumerable.</param>
    /// <param name="fourth">The fourth enumerable.</param>
    /// <returns>The new Array.</returns>
    public static TItem[] Concat<TItem>(this IEnumerable<TItem> enumerable, IEnumerable<TItem> second, IEnumerable<TItem> third, IEnumerable<TItem> fourth)
    {
        var estimatedCount = EstimateCount<TItem>(enumerable);
        estimatedCount += EstimateCount<TItem>(second);
        estimatedCount += EstimateCount<TItem>(third);
        estimatedCount += EstimateCount<TItem>(fourth);

        var buffer = new Buffer<TItem>(estimatedCount);
        buffer.WriteRange(enumerable);
        buffer.WriteRange(second);
        buffer.WriteRange(third);
        buffer.WriteRange(fourth);

        return buffer.ToFinalArray();
    }

    /// <summary>Concats the specified additional.</summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="second">The second enumerable.</param>
    /// <param name="third">The third enumerable.</param>
    /// <param name="fourth">The fourth enumerable.</param>
    /// <param name="additionalEnumerables">The additional enumerables.</param>
    /// <returns>The new Array.</returns>
    public static TItem[] Concat<TItem>(this IEnumerable<TItem> enumerable, IEnumerable<TItem> second, IEnumerable<TItem> third, IEnumerable<TItem> fourth, params IEnumerable<TItem>[] additionalEnumerables)
    {
        var estimatedCount = EstimateCount<TItem>(enumerable);
        estimatedCount += EstimateCount<TItem>(second);
        estimatedCount += EstimateCount<TItem>(third);
        estimatedCount += EstimateCount<TItem>(fourth);
        estimatedCount += additionalEnumerables.Sum(EstimateCount<TItem>);

        var buffer = new Buffer<TItem>(estimatedCount);
        buffer.WriteRange(enumerable);
        buffer.WriteRange(second);
        buffer.WriteRange(third);
        buffer.WriteRange(fourth);
        foreach (var item in additionalEnumerables)
        {
            buffer.WriteRange(item);
        }

        return buffer.ToFinalArray();
    }

    /// <summary>
    /// Gets the distinct items from the <see cref="IEnumerable{T}"/> while preserving the order.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <returns>A enumerable containing items only once.</returns>
    public static IEnumerable<TItem> DistinctInOrder<TItem>(this IEnumerable<TItem> enumerable)
    {
        return enumerable.DistinctInOrder(EqualityComparer<TItem>.Default);
    }

    /// <summary>
    /// Gets the distinct items from the <see cref="IEnumerable{T}"/> while preserving the order.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="equalityComparer">The equality comparer.</param>
    /// <returns>A enumerable containing items only once.</returns>
    public static IEnumerable<TItem> DistinctInOrder<TItem>(this IEnumerable<TItem> enumerable, IEqualityComparer<TItem> equalityComparer)
    {
        var hashSet = new HashSet<TItem>(equalityComparer);
        foreach (var item in enumerable)
        {
            if (hashSet.Add(item))
            {
                yield return item;
            }
        }
    }

    /// <summary>
    /// Gets a value indicating whether only one item exists in the enumerable and gets it.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="item">The item.</param>
    /// <returns><c>true</c> if exactly one item exists otherwise <c>false</c>.</returns>
    public static bool TryGetOnlyOne<TItem>(this IEnumerable<TItem?>? enumerable, [NotNullWhen(true)] out TItem? item)
    {
        item = OnlyOneOrDefault(enumerable);
        return !EqualityComparer<TItem?>.Default.Equals(item, default);
    }

    /// <summary>
    /// Gets a value indicating whether only one item exists in the enumerable and gets it.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="item">The item.</param>
    /// <param name="predicate">The predicate.</param>
    /// <returns><c>true</c> if exactly one item exists otherwise <c>false</c>.</returns>
    public static bool TryGetOnlyOne<TItem>(this IEnumerable<TItem?>? enumerable, [NotNullWhen(true)] out TItem? item, Func<TItem?, bool> predicate)
    {
        item = OnlyOneOrDefault(enumerable, predicate);
        return !EqualityComparer<TItem?>.Default.Equals(item, default);
    }

    /// <summary>
    /// Gets the item if the enumerable contains exactly one item and otherwise returns the default value.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <returns>The item or the default value.</returns>
    public static TItem? OnlyOneOrDefault<TItem>(this IEnumerable<TItem?>? enumerable)
    {
        if (enumerable == null)
        {
            return default;
        }

        using var enumerator = enumerable.GetEnumerator();
        var item = enumerator.MoveNext() ? enumerator.Current : default;
        item = enumerator.MoveNext() ? default : item;
        return item;
    }

    /// <summary>
    /// Gets the item if the enumerable contains exactly one item and otherwise returns the default value.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="predicate">The predicate.</param>
    /// <returns>The item or the default value.</returns>
    public static TItem? OnlyOneOrDefault<TItem>(this IEnumerable<TItem?>? enumerable, Func<TItem?, bool> predicate)
    {
        if (enumerable == null)
        {
            return default;
        }

        TItem? result = default;
        var count = 0;
        foreach (var item in enumerable)
        {
            if (!predicate(item))
            {
                continue;
            }

            if (count > 0)
            {
                result = default;
                break;
            }

            result = item;
            count++;
        }

        return result;
    }

    /// <summary>
    /// Converts the specified <see cref="ImmutableArray{T}"/> to a <see cref="ValueArray{TItem}"/>.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <returns>The value array.</returns>
    public static ValueArray<TItem> ToValueArray<TItem>(this IEnumerable<TItem> enumerable)
    {
        return new ValueArray<TItem>(enumerable.ToImmutableArray());
    }

    /// <summary>
    /// Converts the specified <see cref="IImmutableList{T}"/> to a <see cref="ValueList{TItem}"/>.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <returns>The value list.</returns>
    public static ValueList<TItem> ToValueList<TItem>(this IEnumerable<TItem> enumerable)
    {
        return new ValueList<TItem>(enumerable.ToImmutableList());
    }

    private static TOutItem[] ToArrayUnsafe<TInItem, TOutItem>(IReadOnlyList<TInItem> list, Func<TInItem, TOutItem> selectFunc)
    {
        var count = list.Count;
        var result = new TOutItem[count];
        for (int i = 0; i < count; i++)
        {
            result[i] = selectFunc(list[i]);
        }

        return result;
    }

    private static TOutItem[] ToArrayUnsafe<TInItem, TOutItem>(IEnumerable<TInItem> enumerable, Func<TInItem, TOutItem> selectFunc, int count)
    {
        var result = new TOutItem[count];
        var index = 0;
        foreach (var inItem in enumerable)
        {
            result[index++] = selectFunc(inItem);
        }

        return result;
    }

    private static int EstimateCount<TItem>(object x)
    {
        if (x is ICollection<TItem> collectionOfT)
        {
            return collectionOfT.Count;
        }

        if (x is IReadOnlyList<TItem> readonlyCollectionOfT)
        {
            return readonlyCollectionOfT.Count;
        }

        if (x is ICollection collection)
        {
            return collection.Count;
        }

        return 0;
    }
}