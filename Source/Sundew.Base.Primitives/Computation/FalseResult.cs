// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FalseResult.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Primitives.Computation;

using System.Runtime.CompilerServices;
using System.Threading.Tasks;

/// <summary>
/// Indicates an erroneous result.
/// </summary>
public readonly struct FalseResult
{
    private const string FalseText = "False";

    /// <summary>Gets a value indicating whether this instance is success.</summary>
    /// <value>
    ///   <c>true</c> if this instance is success; otherwise, <c>false</c>.</value>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Alternative to implicit boolean operator.")]
    public bool IsSuccess => false;

    internal static FalseResult Result => default;

    /// <summary>
    /// Always returns false.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <returns>A value indicating whether the result was successful.</returns>
    public static implicit operator bool(FalseResult result)
    {
        return false;
    }

    /// <summary>Performs an implicit conversion from <see cref="FalseResult"/> to <see cref="ValueTask{TResult}"/>.</summary>
    /// <param name="result">The result.</param>
    /// <returns>The result of the conversion.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ValueTask<FalseResult>(FalseResult result)
    {
        return result.ToValueTask();
    }

    /// <summary>
    /// Converts this instance to a value task.
    /// </summary>
    /// <returns>The value task.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueTask<FalseResult> ToValueTask()
    {
        return new ValueTask<FalseResult>(this);
    }

    /// <summary>
    /// Creates a result based on the specified result .
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="value">The value.</param>
    /// <returns>
    /// A new <see cref="Result{TNewValue, TNewError}" />.
    /// </returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Part of the Api design.")]
    public Result<TValue> WithValue<TValue>(TValue value)
    {
        return new Result<TValue>(false, value);
    }

    /// <summary>
    /// Creates a result based on the specified result .
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="value">The value.</param>
    /// <returns>
    /// A new <see cref="Result{TNewValue, TNewError}" />.
    /// </returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Part of the Api design.")]
    public ValueTask<Result<TValue>> WithErrorAsync<TValue>(TValue value)
    {
        return new Result<TValue>(false, value).ToValueTask();
    }

    /// <summary>
    /// Returns a <see cref="string" /> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="string" /> that represents this instance.
    /// </returns>
    public override string ToString()
    {
        return FalseText;
    }
}