// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExceptionFilter.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Computation;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

/// <summary>
/// A filter to determine whether various types of exceptions are considered handled.
/// </summary>
public sealed class ExceptionFilter
{
    private readonly IReadOnlyCollection<Func<Exception, bool>> handlers;

    private ExceptionFilter(params Func<Exception, bool>[] handlers)
    {
        this.handlers = handlers;
    }

    /// <summary>
    /// Creates an <see cref="ExceptionFilter"/> that handles the exceptions specified.
    /// </summary>
    /// <param name="exceptions">The exceptions to handle.</param>
    /// <returns>The exception filter.</returns>
    public static ExceptionFilter HandleOnly(params Type[] exceptions)
    {
        return new ExceptionFilter(HandleType(ExceptionFilterBehavior.Include, exceptions));
    }

    /// <summary>
    /// Creates an <see cref="ExceptionFilter"/> that handles all exceptions except the ones specified.
    /// </summary>
    /// <param name="exceptions">The exceptions not to handle.</param>
    /// <returns>The exception filter.</returns>
    public static ExceptionFilter HandleAllExcept(params Type[] exceptions)
    {
        return new ExceptionFilter(HandleType(ExceptionFilterBehavior.Exclude, exceptions));
    }

    /// <summary>
    /// Creates an <see cref="ExceptionFilter"/> that handles all exceptions.
    /// </summary>
    /// <returns>The exception filter.</returns>
    public static ExceptionFilter HandleAll()
    {
        return new ExceptionFilter(_ => true);
    }

    /// <summary>
    /// Gets a value indicating whether the specified exception was handled.
    /// </summary>
    /// <param name="exception">The exception.</param>
    /// <returns><c>true</c>, if the exception should be considered handled,otherwise <c>false</c>.</returns>
    public bool IsInFilter(Exception exception)
    {
        return this.handlers.Any(x => x(exception));
    }

    private static Func<Exception, bool> HandleType(ExceptionFilterBehavior exceptionFilterBehavior, IReadOnlyList<Type> exceptionTypes)
    {
        return exception =>
        {
            var isInList =
                exceptionTypes.Any(x => x.GetTypeInfo().IsAssignableFrom(exception.GetType().GetTypeInfo()));
            return exceptionFilterBehavior == ExceptionFilterBehavior.Include ? isInList : !isInList;
        };
    }
}