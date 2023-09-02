// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IVisitor.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Primitives.Visiting;

/// <summary>
/// Interface for implementing the visitor pattern with a parameter and a variable.
/// The convention is that parameters are readonly and provided by the caller.
/// Variables are read write and may also be provided by the called, but visitors are allowed to change values while visiting.
/// </summary>
/// <typeparam name="TVisitable">The type of the visitable.</typeparam>
/// <typeparam name="TParameter">The type of the parameter.</typeparam>
/// <typeparam name="TVariable">The type of the variable.</typeparam>
/// <typeparam name="TResult">The type of the result.</typeparam>
public interface IVisitor<in TVisitable, in TParameter, in TVariable, out TResult>
{
    /// <summary>
    /// Visits the specified visitable.
    /// </summary>
    /// <param name="visitable">The visitable.</param>
    /// <param name="parameter">The parameter.</param>
    /// <param name="variable">The variable.</param>
    /// <returns>
    /// The TResult.
    /// </returns>
    TResult Visit(TVisitable visitable, TParameter parameter, TVariable variable);

    /// <summary>
    /// Visits the unknown.
    /// </summary>
    /// <param name="visitable">The visitable.</param>
    /// <param name="parameter">The parameter.</param>
    /// <param name="variable">The variable.</param>
    void VisitUnknown(TVisitable visitable, TParameter parameter, TVariable variable);
}

/// <summary>
/// Interface for implementing the visitor pattern with a parameter and a variable.
/// The convention is that parameters are readonly and provided by the caller.
/// Variables are read write and may also be provided by the called, but visitors are allowed to change values while visiting.
/// </summary>
/// <typeparam name="TEntry">The type of the entry.</typeparam>
/// <typeparam name="TVisitable">The type of the visitable.</typeparam>
/// <typeparam name="TParameter">The type of the parameter.</typeparam>
/// <typeparam name="TVariable">The type of the variable.</typeparam>
/// <typeparam name="TResult">The type of the result.</typeparam>
public interface IVisitor<in TEntry, in TVisitable, in TParameter, in TVariable, out TResult>
{
    /// <summary>
    /// Visits the specified visitable.
    /// </summary>
    /// <param name="entry">The entry.</param>
    /// <param name="parameter">The parameter.</param>
    /// <param name="variable">The variable.</param>
    /// <returns>
    /// The TResult.
    /// </returns>
    TResult Visit(TEntry entry, TParameter parameter, TVariable variable);

    /// <summary>
    /// Visits the unknown.
    /// </summary>
    /// <param name="visitable">The visitable.</param>
    /// <param name="parameter">The parameter.</param>
    /// <param name="variable">The variable.</param>
    void VisitUnknown(TVisitable visitable, TParameter parameter, TVariable variable);
}