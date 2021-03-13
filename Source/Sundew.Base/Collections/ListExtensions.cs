// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ListExtensions.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Collections
{
    using System;
    using System.Collections.Generic;

    using Sundew.Base;

    /// <summary>
    /// Defines extension methods for the generic IList interface.
    /// </summary>
    public static class ListExtensions
    {
        /// <summary>
        /// Adds the range.
        /// </summary>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <param name="list">The list to be added to.</param>
        /// <param name="enumerable">The enumerable.</param>
        public static void AddRange<TItem>(this IList<TItem> list, IEnumerable<TItem> enumerable)
        {
            if (list is List<TItem> realList)
            {
                realList.AddRange(enumerable);
                return;
            }

            foreach (var item in enumerable)
            {
                list.Add(item);
            }
        }

        /// <summary>
        /// For loops the IList and executes action on all items.
        /// </summary>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <param name="list">The list to be for looped.</param>
        /// <param name="action">The action.</param>
        public static void For<TItem>(this IList<TItem> list, Action<TItem> action)
        {
            foreach (var item in list)
            {
                action(item);
            }
        }

        /// <summary>
        /// For loops the IList and executes action on all items.
        /// </summary>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <param name="list">The list to be for looped.</param>
        /// <param name="action">The action.</param>
        public static void For<TItem>(this IReadOnlyList<TItem> list, Action<TItem, int> action)
        {
            for (var i = 0; i < list.Count; i++)
            {
                var item = list[i];
                action(item, i);
            }
        }

        /// <summary>
        /// For loops the IList and executes action on all items.
        /// </summary>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <typeparam name="TOutItem">The type of the out item.</typeparam>
        /// <param name="list">The list to be for looped.</param>
        /// <param name="getItemFunc">The get item function.</param>
        /// <param name="action">The action.</param>
        public static void For<TItem, TOutItem>(this IReadOnlyList<TItem> list, Func<TItem, TOutItem> getItemFunc, Action<TItem, TOutItem, int> action)
        {
            for (var i = 0; i < list.Count; i++)
            {
                var item = list[i];
                action(item, getItemFunc(item), i);
            }
        }

        /// <summary>
        /// For loops the IList and executes action on all items living up to the specified predicate.
        /// </summary>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <param name="list">The list to be for looped.</param>
        /// <param name="predicateTrueAction">The predicate true action.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="predicateFalseAction">The predicate false action.</param>
        /// <param name="breakPredicate">The break predicate.</param>
        public static void For<TItem>(
            this IReadOnlyList<TItem> list,
            Action<TItem, int> predicateTrueAction,
            Predicate<TItem, int> predicate,
            Action<TItem, int>? predicateFalseAction = null,
            Predicate<TItem, int>? breakPredicate = null)
        {
            for (var i = 0; i < list.Count; i++)
            {
                var item = list[i];
                Iterate(item, item, i, predicateTrueAction, predicate, predicateFalseAction);
                if (breakPredicate != null && breakPredicate(item, i))
                {
                    break;
                }
            }
        }

        /// <summary>
        /// For loops the IList and executes action on all items living up to the specified predicate.
        /// </summary>
        /// <typeparam name="TInItem">The type of the in item.</typeparam>
        /// <typeparam name="TOutItem">The type of the out item.</typeparam>
        /// <param name="list">The target list.</param>
        /// <param name="getItemFunc">The get item function.</param>
        /// <param name="predicateTrueAction">The predicate true action.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="predicateFalseAction">The predicate false action.</param>
        /// <param name="breakPredicate">The break predicate.</param>
        public static void For<TInItem, TOutItem>(
            this IReadOnlyList<TInItem> list,
            Func<TInItem, TOutItem> getItemFunc,
            Action<TInItem, TOutItem, int> predicateTrueAction,
            Predicate<TInItem, TOutItem, int>? predicate = null,
            Action<TInItem, TOutItem, int>? predicateFalseAction = null,
            Predicate<TInItem, int>? breakPredicate = null)
        {
            for (var i = 0; i < list.Count; i++)
            {
                var inItem = list[i];
                Iterate(inItem, getItemFunc(inItem), i, predicateTrueAction, predicate, predicateFalseAction);
                if (breakPredicate != null && breakPredicate(inItem, i))
                {
                    break;
                }
            }
        }

        /// <summary>
        /// For loops the IList in reverse order and executes action on all item living up to the specified predicate.
        /// </summary>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <param name="list">The target list.</param>
        /// <param name="predicateTrueAction">The predicate true action.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="predicateFalseAction">The predicate false action.</param>
        /// <param name="breakPredicate">The break predicate.</param>
        public static void ForReverse<TItem>(
            this IReadOnlyList<TItem> list,
            Action<TItem> predicateTrueAction,
            System.Predicate<TItem>? predicate = null,
            Action<TItem>? predicateFalseAction = null,
            System.Predicate<TItem>? breakPredicate = null)
        {
            for (var i = list.Count - 1; i >= 0; i--)
            {
                var item = list[i];
                Iterate(item, predicateTrueAction, predicate, predicateFalseAction);
                if (breakPredicate != null && breakPredicate(item))
                {
                    break;
                }
            }
        }

        /// <summary>
        /// For loops the IList in reverse order and executes action on all item living up to the specified predicate.
        /// </summary>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <param name="list">The target list.</param>
        /// <param name="predicateTrueAction">The predicate true action.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="predicateFalseAction">The predicate false action.</param>
        /// <param name="breakPredicate">The break predicate.</param>
        public static void ForReverse<TItem>(
            this IReadOnlyList<TItem> list,
            Action<TItem, int> predicateTrueAction,
            Predicate<TItem, int>? predicate = null,
            Action<TItem, int>? predicateFalseAction = null,
            Predicate<TItem, int>? breakPredicate = null)
        {
            for (var i = list.Count - 1; i >= 0; i--)
            {
                var item = list[i];
                Iterate(item, item, i, predicateTrueAction, predicate, predicateFalseAction);
                if (breakPredicate != null && breakPredicate(item, i))
                {
                    break;
                }
            }
        }

        /// <summary>
        /// For loops the IList in reverse order and executes action on all item living up to the specified predicate.
        /// </summary>
        /// <typeparam name="TInItem">The type of the in item.</typeparam>
        /// <typeparam name="TOutItem">The type of the out item.</typeparam>
        /// <param name="list">The target list.</param>
        /// <param name="getItemFunc">The get item function.</param>
        /// <param name="predicateTrueAction">The predicate true action.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="predicateFalseAction">The predicate false action.</param>
        /// <param name="breakPredicate">The break predicate.</param>
        public static void ForReverse<TInItem, TOutItem>(
            this IReadOnlyList<TInItem> list,
            Func<TInItem, TOutItem> getItemFunc,
            Action<TInItem, TOutItem, int> predicateTrueAction,
            Predicate<TInItem, TOutItem, int>? predicate = null,
            Action<TInItem, TOutItem, int>? predicateFalseAction = null,
            Predicate<TInItem, int>? breakPredicate = null)
        {
            for (var i = list.Count - 1; i >= 0; i--)
            {
                var inItem = list[i];
                Iterate(inItem, getItemFunc(inItem), i, predicateTrueAction, predicate, predicateFalseAction);
                if (breakPredicate != null && breakPredicate(inItem, i))
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Whiles the specified list the continueWhilePredicate return false.
        /// </summary>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <param name="list">The target list.</param>
        /// <param name="continueWhilePredicate">The continue loop predicate.</param>
        /// <param name="predicateTrueAction">The predicate true action.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="predicateFalseAction">The predicate false action.</param>
        /// <param name="breakPredicate">The break predicate.</param>
        public static void While<TItem>(
            this IReadOnlyList<TItem> list,
            Predicate<TItem, int> continueWhilePredicate,
            Action<TItem, int> predicateTrueAction,
            Predicate<TItem, int>? predicate = null,
            Action<TItem, int>? predicateFalseAction = null,
            Predicate<TItem, int>? breakPredicate = null)
        {
            if (list.IsEmpty())
            {
                return;
            }

            var obj = list[0];
            var i = 0;
            while (continueWhilePredicate(obj, i))
            {
                Iterate(obj, obj, i, predicateTrueAction, predicate, predicateFalseAction);
                if (breakPredicate != null && breakPredicate(obj, i))
                {
                    break;
                }

                obj = list[++i];
            }
        }

        /// <summary>
        /// Whiles the specified list the continueWhilePredicate return false.
        /// </summary>
        /// <typeparam name="TInItem">The type of the in item.</typeparam>
        /// <typeparam name="TOutItem">The type of the out item.</typeparam>
        /// <param name="list">The target list.</param>
        /// <param name="continueWhilePredicate">The continue loop predicate.</param>
        /// <param name="getItemFunc">The get item function.</param>
        /// <param name="predicateTrueAction">The predicate true action.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="predicateFalseAction">The predicate false action.</param>
        /// <param name="breakPredicate">The break predicate.</param>
        public static void While<TInItem, TOutItem>(
            this IReadOnlyList<TInItem> list,
            Predicate<TInItem, int> continueWhilePredicate,
            Func<TInItem, TOutItem> getItemFunc,
            Action<TInItem, TOutItem, int> predicateTrueAction,
            Predicate<TInItem, TOutItem, int>? predicate = null,
            Action<TInItem, TOutItem, int>? predicateFalseAction = null,
            Predicate<TInItem, int>? breakPredicate = null)
        {
            if (list.IsEmpty())
            {
                return;
            }

            var obj = list[0];
            var i = 0;
            while (continueWhilePredicate(obj, i))
            {
                Iterate(obj, getItemFunc(obj), i, predicateTrueAction, predicate, predicateFalseAction);
                if (breakPredicate != null && breakPredicate(obj, i))
                {
                    break;
                }

                obj = list[++i];
            }
        }

        internal static void Iterate<TItem>(TItem item, Action<TItem> predicateTrueAction, System.Predicate<TItem>? predicate, Action<TItem>? predicateFalseAction)
        {
            if (predicate == null || predicate(item))
            {
                predicateTrueAction(item);
            }
            else
            {
                predicateFalseAction?.Invoke(item);
            }
        }

        internal static void Iterate<TInItem, TOutItem>(TInItem inItem, TOutItem outItem, int i, Action<TInItem, TOutItem, int> predicateTrueAction, Predicate<TInItem, TOutItem, int>? predicate, Action<TInItem, TOutItem, int>? predicateFalseAction)
        {
            if (predicate == null || predicate(inItem, outItem, i))
            {
                predicateTrueAction(inItem, outItem, i);
            }
            else
            {
                predicateFalseAction?.Invoke(inItem, outItem, i);
            }
        }

        internal static void Iterate<TInItem, TOutItem>(TInItem inItem, TOutItem outItem, int i, Action<TOutItem, int> predicateTrueAction, Predicate<TInItem, int>? predicate, Action<TOutItem, int>? predicateFalseAction)
        {
            if (predicate == null || predicate(inItem, i))
            {
                predicateTrueAction(outItem, i);
            }
            else
            {
                predicateFalseAction?.Invoke(outItem, i);
            }
        }
    }
}
