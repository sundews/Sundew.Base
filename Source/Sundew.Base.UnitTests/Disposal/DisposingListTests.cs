// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisposingListTests.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.UnitTests.Disposal
{
    using System.Collections.Generic;
    using FluentAssertions;
    using Sundew.Base.Disposal;
    using Xunit;

    public class DisposingListTests
    {
        [Fact]
        public void Dispose_Then_ItemsShouldBeDisposedInExpectedOrder()
        {
            var expectedOrder = new[] { 1, 2 };
            var testee = new DisposingList<DisposeAction>();
            var disposeOrder = new List<int>();
            testee.Add(new DisposeAction(() => disposeOrder.Add(1)));
            testee.Add(new DisposeAction(() => disposeOrder.Add(2)));

            testee.Dispose();

            disposeOrder.Should().Equal(expectedOrder);
        }
    }
}