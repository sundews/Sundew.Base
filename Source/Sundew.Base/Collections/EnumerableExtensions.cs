// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumerableExtensions.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Sundew.Base;
    using Sundew.Base.Collections.Internal;
    using Sundew.Base.Memory;

    /// <summary>
    /// Extends the generic IEnumerable interface with functions.
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Gets an <see cref="IEnumerable{TItem}"/> from an item.
        /// </summary>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <param name="item">The item.</param>
        /// <returns>An <see cref="IEnumerable{TItem}"/>.</returns>
        public static IEnumerable<TItem> ToEnumerable<TItem>(this TItem item)
        {
            if (item != null)
            {
                yield return item;
            }
        }

        /// <summary>
        /// Finds the index based on the given predicate.
        /// </summary>
        /// <param name="enumerable">The enumerable.</param>
        /// <param name="index">The index.</param>
        /// <returns>
        /// The index found by the matching predicate.
        /// </returns>
        public static object ElementAt(this IEnumerable enumerable, int index)
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
        /// <param name="enumerable">The enumerable.</param>
        /// <param name="indexPredicate">The index predicate.</param>
        /// <returns>The index found by the matching predicate.</returns>
        public static int IndexOf(this IEnumerable enumerable, Sundew.Base.Predicate<object> indexPredicate)
        {
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
        /// Finds the index based on the given predicate.
        /// </summary>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <param name="enumerable">The enumerable.</param>
        /// <param name="indexPredicate">The index predicate.</param>
        /// <returns>The index found by the matching predicate.</returns>
        public static int IndexOf<TItem>(this IEnumerable<TItem> enumerable, System.Predicate<TItem> indexPredicate)
        {
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
        /// Ensures the enumerable is an array or calls ToArray to create it.
        /// </summary>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <param name="enumerable">The enumerable.</param>
        /// <param name="createListFunction">The create list function.</param>
        /// <returns>
        /// The same enumerable casted to a list if possible, otherwise a new list containing the elements.
        /// </returns>
        public static TItem[] ToArrayIfNeeded<TItem>(this IEnumerable<TItem> enumerable, Func<IEnumerable<TItem>, TItem[]>? createListFunction = null)
        {
            if (enumerable is TItem[] array)
            {
                return array;
            }

            return createListFunction != null ? createListFunction(enumerable) : enumerable.ToArray();
        }

        /// <summary>
        /// Toes the list if needed.
        /// </summary>
        /// <param name="enumerable">The enumerable.</param>
        /// <param name="createListFunction">The create list function.</param>
        /// <returns>The same enumerable casted to a list if possible, otherwise a new list containing the elements.</returns>
        public static IList ToListIfNeeded(this IEnumerable enumerable, Func<IEnumerable, IList>? createListFunction = null)
        {
            if (enumerable is IList nonGenericListIfNeeded)
            {
                return nonGenericListIfNeeded;
            }

            if (createListFunction != null)
            {
                return createListFunction(enumerable);
            }

            var list = new List<object>();
            list.AddRange(enumerable.Cast<object>());
            return list;
        }

        /// <summary>
        /// Ensures the enumerable is a list or calls ToList on it.
        /// </summary>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <param name="enumerable">The enumerable.</param>
        /// <param name="createListFunction">The create list function.</param>
        /// <returns>The same enumerable casted to a list if possible, otherwise a new list containing the elements.</returns>
        public static IList<TItem> ToListIfNeeded<TItem>(this IEnumerable<TItem> enumerable, Func<IEnumerable<TItem>, IList<TItem>> createListFunction)
        {
            if (enumerable is IList<TItem> list)
            {
                return list;
            }

            return createListFunction != null ? createListFunction(enumerable) : enumerable.ToList();
        }

        /// <summary>
        /// Ensures the enumerable is a list or calls ToList on it.
        /// </summary>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <param name="enumerable">The enumerable.</param>
        /// <param name="createListFunction">The create list function.</param>
        /// <returns>The same enumerable casted to a list if possible, otherwise a new list containing the elements.</returns>
        public static List<TItem> ToListIfNeeded<TItem>(this IEnumerable<TItem> enumerable, Func<IEnumerable<TItem>, List<TItem>>? createListFunction = null)
        {
            if (enumerable is List<TItem> list)
            {
                return list;
            }

            return createListFunction != null ? createListFunction(enumerable) : enumerable.ToList();
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
            switch (enumerable)
            {
                case IReadOnlyCollection<object> readOnlyCollection:
                    return readOnlyCollection;
                case ICollection collection:
                    return new NonDeferredEnumerable<object>(enumerable.Cast<object>(), collection.Count);
                default:
                    return new ReadOnlyArray<object>(enumerable.Cast<object>().ToArray());
            }
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
            switch (enumerable)
            {
                case IReadOnlyCollection<TItem> readOnlyCollection:
                    return readOnlyCollection;
                case ICollection<TItem> collectionOfT:
                    return new ReadOnlyCollection<TItem>(collectionOfT);
                case ICollection collection:
                    return new NonDeferredEnumerable<TItem>(enumerable, collection.Count);
                default:
                    return new ReadOnlyArray<TItem>(enumerable.ToArray());
            }
        }

        /// <summary>
        /// Selects from the non generic enumerable.
        /// </summary>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <param name="enumerable">The enumerable.</param>
        /// <param name="selectFunction">The select function.</param>
        /// <returns>Yield returns an Enumerable based on the select function.</returns>
        public static IEnumerable<TItem> SelectFromNonGeneric<TItem>(this IEnumerable enumerable, Func<object, TItem> selectFunction)
        {
            foreach (var item in enumerable)
            {
                yield return selectFunction(item);
            }
        }

        /// <summary>
        /// Iterates through the enumerable and executes the specified action on all items.
        /// </summary>
        /// <param name="enumerable">The enumerable.</param>
        /// <param name="action">The action.</param>
        public static void ForEachNonGeneric(this IEnumerable enumerable, Action<object, int> action)
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
        /// <param name="enumerable">The enumerable.</param>
        /// <param name="action">The action.</param>
        public static void ForEachNonGeneric(this IEnumerable enumerable, Action<object> action)
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
        /// Iterates through the enumerable and executes the specified action on items that fulfill the predicate.
        /// The foreach will break when the until predicate is true.
        /// </summary>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <param name="enumerable">The enumerable.</param>
        /// <param name="predicateTrueAction">The predicate true action.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="predicateFalseAction">The predicate false action.</param>
        /// <param name="breakPredicate">The break predicate.</param>
        public static void ForEach<TItem>(
            this IEnumerable<TItem> enumerable,
            Action<TItem, int> predicateTrueAction,
            Predicate<TItem, int> predicate,
            Action<TItem, int>? predicateFalseAction = null,
            Predicate<TItem, int>? breakPredicate = null)
        {
            var i = 0;
            foreach (var item in enumerable)
            {
                ListExtensions.Iterate(item, item, i, predicateTrueAction, predicate, predicateFalseAction);
                if (breakPredicate != null && breakPredicate(item, i))
                {
                    break;
                }

                i++;
            }
        }

        /// <summary>
        /// Iterates through the enumerable and executes the specified action on items that fulfill the predicate.
        /// The foreach will break when the until predicate is true.
        /// </summary>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <param name="enumerable">The enumerable.</param>
        /// <param name="predicateTrueAction">The predicate true action.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="predicateFalseAction">The predicate false action.</param>
        /// <param name="breakPredicate">The break predicate.</param>
        public static void ForEach<TItem>(
            this IEnumerable<TItem> enumerable,
            Action<TItem> predicateTrueAction,
            System.Predicate<TItem> predicate,
            Action<TItem>? predicateFalseAction = null,
            System.Predicate<TItem>? breakPredicate = null)
        {
            foreach (var item in enumerable)
            {
                ListExtensions.Iterate(item, predicateTrueAction, predicate, predicateFalseAction);
                if (breakPredicate != null && breakPredicate(item))
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Create an array from the specified enumerable.
        /// Potentially more efficient than Linq Select + ToArray if the enumerable is not deferred.
        /// </summary>
        /// <typeparam name="TInItem">The type of the in item.</typeparam>
        /// <typeparam name="TOutItem">The type of the out item.</typeparam>
        /// <param name="enumerable">The enumerable.</param>
        /// <param name="selectFunc">The select function.</param>
        /// <returns>The new array.</returns>
        public static TOutItem[] ToArray<TInItem, TOutItem>(this IEnumerable<TInItem> enumerable, Func<TInItem, TOutItem> selectFunc)
        {
            if (enumerable is ICollection<TInItem> collectionOfT)
            {
                return ToArrayUnsafe(enumerable, selectFunc, collectionOfT.Count);
            }

            if (enumerable is IReadOnlyCollection<TInItem> readonlyCollectionOfT)
            {
                return ToArrayUnsafe(enumerable, selectFunc, readonlyCollectionOfT.Count);
            }

            if (enumerable is ICollection collection)
            {
                return ToArrayUnsafe(enumerable, selectFunc, collection.Count);
            }

            return enumerable.Select(selectFunc).ToArray();
        }

        /// <summary>Concats the specified additional.</summary>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <param name="enumerable">The enumerable.</param>
        /// <param name="second">The second enumerable.</param>
        /// <param name="third">The third enumerable.</param>
        /// <returns>The new Array.</returns>
        public static TItem[] Concat<TItem>(this IEnumerable<TItem> enumerable, IEnumerable<TItem> second, IEnumerable<TItem> third)
        {
            var estimatedCount = EstimateCount(enumerable);
            estimatedCount += EstimateCount(second);
            estimatedCount += EstimateCount(third);

            var buffer = new Buffer<TItem>(estimatedCount);
            buffer.WriteRange(enumerable);
            buffer.WriteRange(second);
            buffer.WriteRange(third);

            return buffer.ToArray();
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
            var estimatedCount = EstimateCount(enumerable);
            estimatedCount += EstimateCount(second);
            estimatedCount += EstimateCount(third);
            estimatedCount += EstimateCount(fourth);

            var buffer = new Buffer<TItem>(estimatedCount);
            buffer.WriteRange(enumerable);
            buffer.WriteRange(second);
            buffer.WriteRange(third);
            buffer.WriteRange(fourth);

            return buffer.ToArray();
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
            var estimatedCount = EstimateCount(enumerable);
            estimatedCount += EstimateCount(second);
            estimatedCount += EstimateCount(third);
            estimatedCount += EstimateCount(fourth);
            estimatedCount += additionalEnumerables.Sum(EstimateCount);

            var buffer = new Buffer<TItem>(estimatedCount);
            buffer.WriteRange(enumerable);
            buffer.WriteRange(second);
            buffer.WriteRange(third);
            buffer.WriteRange(fourth);
            foreach (var item in additionalEnumerables)
            {
                buffer.WriteRange(item);
            }

            return buffer.ToArray();
        }

        /// <summary>
        /// Aggregates to string builder.
        /// </summary>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <param name="enumerable">The enumerable.</param>
        /// <param name="aggregateFunction">The aggregate function.</param>
        /// <returns>A <see cref="StringBuilder"/>.</returns>
        public static StringBuilder AggregateToStringBuilder<TItem>(this IEnumerable<TItem> enumerable, Action<StringBuilder, TItem> aggregateFunction)
        {
            return enumerable.Aggregate(new StringBuilder(), (builder, item) =>
            {
                aggregateFunction.Invoke(builder, item);
                return builder;
            });
        }

        /// <summary>
        /// Aggregates to string builder.
        /// </summary>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <param name="enumerable">The enumerable.</param>
        /// <param name="stringBuilder">The string builder.</param>
        /// <param name="aggregateFunction">The aggregate function.</param>
        /// <returns>
        /// A <see cref="StringBuilder" />.
        /// </returns>
        public static StringBuilder AggregateToStringBuilder<TItem>(this IEnumerable<TItem> enumerable, StringBuilder stringBuilder, Action<StringBuilder, TItem> aggregateFunction)
        {
            return enumerable.Aggregate(
                stringBuilder,
                (builder, item) =>
            {
                aggregateFunction.Invoke(builder, item);
                return builder;
            });
        }

        /// <summary>
        /// Aggregates to string builder.
        /// </summary>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="enumerable">The enumerable.</param>
        /// <param name="aggregateFunction">The aggregate function.</param>
        /// <param name="resultFunc">The result function.</param>
        /// <returns>
        /// The result of the result function.
        /// </returns>
        public static TResult AggregateToStringBuilder<TItem, TResult>(this IEnumerable<TItem> enumerable, Action<StringBuilder, TItem> aggregateFunction, Func<StringBuilder, TResult> resultFunc)
        {
            return enumerable.Aggregate(
                new StringBuilder(),
                (builder, item) =>
                {
                    aggregateFunction.Invoke(builder, item);
                    return builder;
                },
                resultFunc);
        }

        /// <summary>
        /// Aggregates to string builder.
        /// </summary>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="enumerable">The enumerable.</param>
        /// <param name="stringBuilder">The string builder.</param>
        /// <param name="aggregateFunction">The aggregate function.</param>
        /// <param name="resultFunc">The result function.</param>
        /// <returns>
        /// The result of the result function.
        /// </returns>
        public static TResult AggregateToStringBuilder<TItem, TResult>(this IEnumerable<TItem> enumerable, StringBuilder stringBuilder, Action<StringBuilder, TItem> aggregateFunction, Func<StringBuilder, TResult> resultFunc)
        {
            return enumerable.Aggregate(
                stringBuilder,
                (builder, item) =>
                {
                    aggregateFunction.Invoke(builder, item);
                    return builder;
                },
                resultFunc);
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

        private static int EstimateCount<TItem>(IEnumerable<TItem> x)
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
}
