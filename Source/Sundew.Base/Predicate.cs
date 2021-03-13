// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Predicate.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base
{
    /// <summary>
    /// Predicate with no parameter.
    /// </summary>
    /// <returns>The result of the predicate.</returns>
    public delegate bool Predicate();

    /// <summary>
    /// Predicate with one parameter.
    /// </summary>
    /// <typeparam name="TParameter">The type of the parameter.</typeparam>
    /// <param name="parameter">The parameter.</param>
    /// <returns>The result of the predicate.</returns>
    public delegate bool Predicate<in TParameter>(TParameter parameter);

    /// <summary>
    /// Predicate with two parameters.
    /// </summary>
    /// <typeparam name="TParameter1">The type of the parameter1.</typeparam>
    /// <typeparam name="TParameter2">The type of the parameter2.</typeparam>
    /// <param name="parameter1">The parameter1.</param>
    /// <param name="parameter2">The parameter2.</param>
    /// <returns>The result of the predicate.</returns>
    public delegate bool Predicate<in TParameter1, in TParameter2>(TParameter1 parameter1, TParameter2 parameter2);

    /// <summary>
    /// Predicate with three parameters.
    /// </summary>
    /// <typeparam name="TParameter1">The type of the parameter1.</typeparam>
    /// <typeparam name="TParameter2">The type of the parameter2.</typeparam>
    /// <typeparam name="TParameter3">The type of the parameter3.</typeparam>
    /// <param name="parameter1">The parameter1.</param>
    /// <param name="parameter2">The parameter2.</param>
    /// <param name="parameter3">The parameter3.</param>
    /// <returns>The result of the predicate.</returns>
    public delegate bool Predicate<in TParameter1, in TParameter2, in TParameter3>(TParameter1 parameter1, TParameter2 parameter2, TParameter3 parameter3);

    /// <summary>
    /// Predicate with four parameters.
    /// </summary>
    /// <typeparam name="TParameter1">The type of the parameter1.</typeparam>
    /// <typeparam name="TParameter2">The type of the parameter2.</typeparam>
    /// <typeparam name="TParameter3">The type of the parameter3.</typeparam>
    /// <typeparam name="TParameter4">The type of the parameter4.</typeparam>
    /// <param name="parameter1">The parameter1.</param>
    /// <param name="parameter2">The parameter2.</param>
    /// <param name="parameter3">The parameter3.</param>
    /// <param name="parameter4">The parameter4.</param>
    /// <returns>The result of the predicate.</returns>
    public delegate bool Predicate<in TParameter1, in TParameter2, in TParameter3, in TParameter4>(TParameter1 parameter1, TParameter2 parameter2, TParameter3 parameter3, TParameter4 parameter4);
}