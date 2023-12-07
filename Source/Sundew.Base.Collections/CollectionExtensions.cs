// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CollectionExtensions.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Collections;

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Sundew.Base.Primitives;

/// <summary>
/// Defines extension methods for the generic ICollection interface.
/// </summary>
public static class CollectionExtensions
{
    /// <summary>
    /// Adds the range.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="list">The list to be added to.</param>
    /// <param name="enumerable">The enumerable.</param>
    public static void AddRange<TItem>(this ICollection<TItem> list, IEnumerable<TItem> enumerable)
    {
        switch (list)
        {
            case List<TItem> realList:
                realList.AddRange(enumerable);
                return;
            case ImmutableArray<TItem>.Builder arrayBuilder:
                arrayBuilder.AddRange(enumerable);
                return;
            case ImmutableList<TItem>.Builder listBuilder:
                listBuilder.AddRange(enumerable);
                return;
        }

        foreach (var item in enumerable)
        {
            list.Add(item);
        }
    }

    /// <summary>
    /// Adds the option value if it has one.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <param name="collection">The collection.</param>
    /// <param name="option">The option.</param>
    /// <returns><c>true</c>, if the value was added, otherwise <c>false</c>.</returns>
    public static bool TryAdd<TItem>(this ICollection<TItem> collection, TItem? option)
        where TItem : struct
    {
        if (option.HasValue)
        {
            collection.Add(option.Value);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Adds the option value if it has one.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <param name="collection">The collection.</param>
    /// <param name="option">The option.</param>
    /// <returns><c>true</c>, if the value was added, otherwise <c>false</c>.</returns>
    public static bool TryAdd<TItem>(this ICollection<TItem> collection, TItem? option)
        where TItem : class
    {
        if (option.HasValue())
        {
            collection.Add(option);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Adds the result error if the result failed.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <param name="collection">The collection.</param>
    /// <param name="result">The result.</param>
    /// <returns><c>true</c>, if the error was added, otherwise <c>false</c>.</returns>
    public static bool TryAdd<TItem>(this ICollection<TItem> collection, R<TItem> result)
    {
        if (result.IsSuccess)
        {
            return false;
        }

        collection.Add(result.Error);
        return true;
    }

    /// <summary>
    /// Tries to add the result item if it is successful.
    /// </summary>
    /// <typeparam name="TSuccess">The success type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="collection">The collection.</param>
    /// <param name="result">The result.</param>
    /// <returns> The resulting list.</returns>
    public static bool TryAddSuccess<TSuccess, TError>(this ICollection<TSuccess> collection, R<TSuccess, TError> result)
    {
        if (result.IsSuccess)
        {
            collection.Add(result.Value);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Tries to add the result error if there are any.
    /// </summary>
    /// <typeparam name="TSuccess">The success type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="collection">The collection.</param>
    /// <param name="result">The result.</param>
    /// <returns> The resulting list.</returns>
    public static bool TryAddError<TSuccess, TError>(this ICollection<TError> collection, R<TSuccess, TError> result)
    {
        if (result.HasError)
        {
            collection.Add(result.Error);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Adds the option value if it has one.
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <typeparam name="TOptionList">The result collection.</typeparam>
    /// <param name="collection">The collection.</param>
    /// <param name="option">The option.</param>
    /// <returns><c>true</c>, if the value was added, otherwise <c>false</c>.</returns>
    public static bool TryAdd<TValue, TOptionList>(this ICollection<TValue> collection, TOptionList? option)
        where TOptionList : struct, IEnumerable<TValue>
    {
        if (option.HasValue)
        {
            var count = collection.Count;
            collection.AddRange(option.Value);
            return count < collection.Count;
        }

        return false;
    }

    /// <summary>
    /// Adds the option value if it has one.
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <typeparam name="TOptionList">The result collection.</typeparam>
    /// <param name="collection">The collection.</param>
    /// <param name="option">The option.</param>
    /// <returns><c>true</c>, if the value was added, otherwise <c>false</c>.</returns>
    public static bool TryAdd<TValue, TOptionList>(this ICollection<TValue> collection, TOptionList? option)
        where TOptionList : class, IEnumerable<TValue>
    {
        if (option.HasValue())
        {
            var count = collection.Count;
            collection.AddRange(option);
            return count < collection.Count;
        }

        return false;
    }

    /// <summary>
    /// Adds the option value if it has one.
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="collection">The collection.</param>
    /// <param name="options">The options.</param>
    /// <returns><c>true</c>, if the value was added, otherwise <c>false</c>.</returns>
    public static bool TryAdd<TValue>(this ICollection<TValue> collection, IEnumerable<TValue?> options)
        where TValue : struct
    {
        var count = collection.Count;
        collection.AddRange(options.GetValues());
        return count < collection.Count;
    }

    /// <summary>
    /// Adds the option value if it has one.
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="collection">The collection.</param>
    /// <param name="options">The options.</param>
    /// <returns><c>true</c>, if the value was added, otherwise <c>false</c>.</returns>
    public static bool TryAdd<TValue>(this ICollection<TValue> collection, IEnumerable<TValue?> options)
        where TValue : class
    {
        var count = collection.Count;
        collection.AddRange(options.GetValues());
        return count < collection.Count;
    }

    /// <summary>
    /// Adds the option value if it has one.
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <typeparam name="TOptionList">The result collection.</typeparam>
    /// <param name="collection">The collection.</param>
    /// <param name="options">The options.</param>
    /// <returns><c>true</c>, if the value was added, otherwise <c>false</c>.</returns>
    public static bool TryAdd<TValue, TOptionList>(this ICollection<TValue> collection, IEnumerable<TOptionList?> options)
        where TOptionList : struct, IEnumerable<TValue>
    {
        var count = collection.Count;
        collection.AddRange(options.GetValues().SelectMany(x => x));
        return count < collection.Count;
    }

    /// <summary>
    /// Adds the option value if it has one.
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <typeparam name="TOptionList">The result collection.</typeparam>
    /// <param name="collection">The collection.</param>
    /// <param name="options">The options.</param>
    /// <returns><c>true</c>, if the value was added, otherwise <c>false</c>.</returns>
    public static bool TryAdd<TValue, TOptionList>(this ICollection<TValue> collection, IEnumerable<TOptionList?> options)
        where TOptionList : class, IEnumerable<TValue>
    {
        var count = collection.Count;
        collection.AddRange(options.GetValues().SelectMany(x => x));
        return count < collection.Count;
    }

    /// <summary>
    /// Adds the result error if the result failed.
    /// </summary>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <typeparam name="TResultList">The result collection.</typeparam>
    /// <param name="collection">The collection.</param>
    /// <param name="result">The result.</param>
    /// <returns><c>true</c>, if the error was added, otherwise <c>false</c>.</returns>
    public static bool TryAddErrors<TError, TResultList>(this ICollection<TError> collection, R<TResultList> result)
        where TResultList : IEnumerable<TError>
    {
        if (result.IsSuccess)
        {
            return false;
        }

        var count = collection.Count;
        collection.AddRange(result.Error);
        return count < collection.Count;
    }

    /// <summary>
    /// Tries to add the result item if it is successful.
    /// </summary>
    /// <typeparam name="TSuccess">The success type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <typeparam name="TSuccessList">The result collection.</typeparam>
    /// <param name="collection">The collection.</param>
    /// <param name="result">The result.</param>
    /// <returns><c>true</c>, if the error was added, otherwise <c>false</c>.</returns>
    public static bool TryAddSuccesses<TSuccess, TError, TSuccessList>(this ICollection<TSuccess> collection, R<TSuccessList, TError> result)
        where TSuccessList : IEnumerable<TSuccess>
    {
        if (result.IsSuccess)
        {
            var count = collection.Count;
            collection.AddRange(result.Value);
            return count < collection.Count;
        }

        return false;
    }

    /// <summary>
    /// Tries to add the result error if there are any.
    /// </summary>
    /// <typeparam name="TSuccess">The success type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <typeparam name="TErrorList">The result collection.</typeparam>
    /// <param name="collection">The collection.</param>
    /// <param name="result">The result.</param>
    /// <returns><c>true</c>, if the error was added, otherwise <c>false</c>.</returns>
    public static bool TryAddErrors<TSuccess, TError, TErrorList>(this ICollection<TError> collection, R<TSuccess, TErrorList> result)
        where TErrorList : IEnumerable<TError>
    {
        if (result.HasError)
        {
            var count = collection.Count;
            collection.AddRange(result.Error);
            return count < collection.Count;
        }

        return false;
    }

    /// <summary>
    /// Adds the result error if the result failed.
    /// </summary>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="collection">The collection.</param>
    /// <param name="result">The result.</param>
    /// <returns><c>true</c>, if the error was added, otherwise <c>false</c>.</returns>
    public static bool TryAddErrors<TError>(this ICollection<TError> collection, IEnumerable<R<TError>> result)
    {
        var count = collection.Count;
        collection.AddRange(result.GetErrors());
        return count < collection.Count;
    }

    /// <summary>
    /// Tries to add the result items if they are successful.
    /// </summary>
    /// <typeparam name="TSuccess">The success type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="collection">The collection.</param>
    /// <param name="result">The result.</param>
    /// <returns><c>true</c>, if the error was added, otherwise <c>false</c>.</returns>
    public static bool TryAddSuccesses<TSuccess, TError>(this ICollection<TSuccess> collection, IEnumerable<R<TSuccess, TError>> result)
    {
        var count = collection.Count;
        collection.AddRange(result.GetSuccesses());
        return count < collection.Count;
    }

    /// <summary>
    /// Adds the result errors if the result failed.
    /// </summary>
    /// <typeparam name="TSuccess">The success type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="collection">The collection.</param>
    /// <param name="results">The results.</param>
    /// <returns><c>true</c>, if the error was added, otherwise <c>false</c>.</returns>
    public static bool TryAddErrors<TSuccess, TError>(this ICollection<TError> collection, IEnumerable<R<TSuccess, TError>> results)
    {
        var count = collection.Count;
        collection.AddRange(results.GetErrors());
        return count < collection.Count;
    }

    /// <summary>
    /// Adds all the result errors if there are any.
    /// </summary>
    /// <typeparam name="TSuccess">The success type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="collection">The collection.</param>
    /// <param name="results">The results.</param>
    /// <returns><c>true</c>, if the error was added, otherwise <c>false</c>.</returns>
    public static bool TryAddAnyErrors<TSuccess, TError>(this ICollection<TError> collection, IEnumerable<R<TSuccess, TError>> results)
    {
        var count = collection.Count;
        collection.AddRange(results.GetAnyErrors());
        return count < collection.Count;
    }

    /// <summary>
    /// Adds the result error if the result failed.
    /// </summary>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <typeparam name="TResultList">The result collection.</typeparam>
    /// <param name="collection">The collection.</param>
    /// <param name="results">The results.</param>
    /// <returns><c>true</c>, if the error was added, otherwise <c>false</c>.</returns>
    public static bool TryAddErrors<TError, TResultList>(this ICollection<TError> collection, IEnumerable<R<TResultList>> results)
        where TResultList : IEnumerable<TError>
    {
        var count = collection.Count;
        collection.AddRange(results.GetErrors().SelectMany(x => x));
        return count < collection.Count;
    }

    /// <summary>
    /// Tries to add the result item if it is successful.
    /// </summary>
    /// <typeparam name="TSuccess">The success type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <typeparam name="TSuccessList">The result collection.</typeparam>
    /// <param name="collection">The collection.</param>
    /// <param name="results">The results.</param>
    /// <returns><c>true</c>, if the error was added, otherwise <c>false</c>.</returns>
    public static bool TryAddSuccesses<TSuccess, TError, TSuccessList>(this ICollection<TSuccess> collection, IEnumerable<R<TSuccessList, TError>> results)
        where TSuccessList : IEnumerable<TSuccess>
    {
        var count = collection.Count;
        collection.AddRange(results.GetSuccesses().SelectMany(x => x));
        return count < collection.Count;
    }

    /// <summary>
    /// Tries to add the result errors if the result failed.
    /// </summary>
    /// <typeparam name="TSuccess">The success type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <typeparam name="TErrorList">The result collection.</typeparam>
    /// <param name="collection">The collection.</param>
    /// <param name="results">The results.</param>
    /// <returns><c>true</c>, if the error was added, otherwise <c>false</c>.</returns>
    public static bool TryAddErrors<TSuccess, TError, TErrorList>(this ICollection<TError> collection, IEnumerable<R<TSuccess, TErrorList>> results)
        where TErrorList : IEnumerable<TError>
    {
        var count = collection.Count;
        collection.AddRange(results.GetErrors().SelectMany(x => x));
        return count < collection.Count;
    }

    /// <summary>
    /// Tries to add the result error if there are any.
    /// </summary>
    /// <typeparam name="TSuccess">The success type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <typeparam name="TErrorList">The result collection.</typeparam>
    /// <param name="collection">The collection.</param>
    /// <param name="results">The results.</param>
    /// <returns><c>true</c>, if the error was added, otherwise <c>false</c>.</returns>
    public static bool TryAddAnyErrors<TSuccess, TError, TErrorList>(this ICollection<TError> collection, IEnumerable<R<TSuccess, TErrorList>> results)
        where TErrorList : IEnumerable<TError>
    {
        var count = collection.Count;
        collection.AddRange(results.GetAnyErrors().SelectMany(x => x));
        return count < collection.Count;
    }
}