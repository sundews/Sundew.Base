// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TemporarilySet{TValue}.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.UnitTests.Infrastructure;

using System;

public sealed class TemporarilySet<TValue> : IDisposable
{
    private readonly TValue originalValue;
    private readonly Func<TValue> getValueFunc;
    private readonly Action<TValue> setValueFunc;

    internal TemporarilySet(TValue temporaryValue, Action<TValue> setValueFunc, Func<TValue> getValueFunc)
    {
        this.getValueFunc = getValueFunc;
        this.setValueFunc = setValueFunc;
        this.originalValue = this.getValueFunc();
        this.setValueFunc(temporaryValue);
    }

    public void Dispose()
    {
        this.setValueFunc(this.originalValue);
    }
}