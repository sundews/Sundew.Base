// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SplitFunc.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Text
{
    using System.Text;

    /// <summary>
    /// Delegate for splitting strings in a LINQ manner.
    /// </summary>
    /// <param name="character">The character.</param>
    /// <param name="index">The index.</param>
    /// <param name="stringBuilder">The string builder.</param>
    /// <returns>
    /// A value indicating whether the character should be added.
    /// </returns>
    public delegate SplitAction SplitFunc(char character, int index, StringBuilder stringBuilder);

    /// <summary>
    /// Delegate for splitting strings in a LINQ manner.
    /// </summary>
    /// <param name="character">The character.</param>
    /// <param name="index">The index.</param>
    /// <returns>
    /// A value indicating whether the character should be added.
    /// </returns>
    public delegate SplitAction SplitMemoryFunc(char character, int index);
}