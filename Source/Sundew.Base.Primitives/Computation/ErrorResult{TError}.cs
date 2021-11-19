// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ErrorResult{TError}.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Primitives.Computation;

using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

/// <summary>
/// Result for indicating an error.
/// </summary>
/// <typeparam name="TError">The type of the error.</typeparam>
public readonly struct ErrorResult<TError>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ErrorResult{TError}"/> struct.
    /// </summary>
    /// <param name="error">The error.</param>
    internal ErrorResult(TError error)
    {
        this.Error = error;
    }

    /// <summary>Gets a value indicating whether this instance is success.</summary>
    /// <value>
    ///   <c>true</c> if this instance is success; otherwise, <c>false</c>.</value>
    public bool IsSuccess => false;

    /// <summary>
    /// Gets the error.
    /// </summary>
    /// <value>
    /// The error.
    /// </value>
    public TError Error { get; }

    /// <summary>
    /// Always returns false.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <returns>A value indicating whether the result was successful.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "By design.")]
    public static implicit operator bool(ErrorResult<TError> result)
    {
        return false;
    }

    /// <summary>
    /// Gets the result's value property.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <returns>A value indicating whether the result was successful.</returns>
    public static implicit operator TError(ErrorResult<TError> result)
    {
        return result.Error;
    }

    /// <summary>Performs an implicit conversion from <see cref="ErrorResult"/> to <see cref="Result.IfSuccess{TValue}"/>.</summary>
    /// <param name="result">The result.</param>
    /// <returns>The result of the conversion.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ValueTask<ErrorResult<TError>>(ErrorResult<TError> result)
    {
        return result.ToValueTask();
    }

    /// <summary>
    /// Converts this instance to a value task.
    /// </summary>
    /// <returns>The value task.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueTask<ErrorResult<TError>> ToValueTask()
    {
        return new ValueTask<ErrorResult<TError>>(this);
    }

    /// <summary>
    /// Creates a result based on the specified result .
    /// </summary>
    /// <typeparam name="TNewError">The type of the new error.</typeparam>
    /// <param name="errorFunc">The error function.</param>
    /// <returns>
    /// A new <see cref="Result{TNewValue, TNewError}" />.
    /// </returns>
    public ErrorResult<TNewError> Convert<TNewError>(Func<TError, TNewError> errorFunc)
    {
        return new ErrorResult<TNewError>(errorFunc(this.Error));
    }

    /// <summary>
    /// Creates a result based on the specified result .
    /// </summary>
    /// <typeparam name="TNewError">The type of the new error.</typeparam>
    /// <param name="errorFunc">The error function.</param>
    /// <returns>
    /// A new <see cref="Result{TNewValue, TNewError}" />.
    /// </returns>
    public ValueTask<ErrorResult<TNewError>> ConvertAsync<TNewError>(Func<TError, TNewError> errorFunc)
    {
        return new ErrorResult<TNewError>(errorFunc(this.Error)).ToValueTask();
    }

    /// <summary>
    /// Returns a <see cref="string" /> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="string" /> that represents this instance.
    /// </returns>
    public override string ToString()
    {
        return $"Error: {this.Error}";
    }

    /// <summary>
    /// Deconstructs the result and error.
    /// </summary>
    /// <param name="isSuccess">if set to <c>true</c> [is success].</param>
    /// <param name="error">The error.</param>
    public void Deconstruct(out bool isSuccess, out TError error)
    {
        isSuccess = false;
        error = this.Error;
    }
}