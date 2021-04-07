// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisposableStateTests.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.UnitTests.Disposal
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Sundew.Base.Disposal;
    using Xunit;

    public class DisposableStateTests
    {
        [Fact]
        public async Task DisposeAsync_WhenDisposableDerivedIsUsed_Then_DisposableDerivedShouldBeCalledFirst()
        {
            var expectedCalls = new[] { nameof(DisposableDerived), nameof(DisposableBase) };
            var testee = new DisposableDerived();

            await testee.DisposeAsync().ConfigureAwait(false);

            testee.Calls.Should().Equal(expectedCalls);
        }

        [Fact]
        public async Task DisposeAsync_WhenDisposableDerivedIsUsedAndDisposeAsyncIsCalledTwice_Then_DisposeShouldOnlyBeCalledOnce()
        {
            var expectedCalls = new[] { nameof(DisposableDerived), nameof(DisposableBase) };
            var testee = new DisposableDerived();
            await testee.DisposeAsync().ConfigureAwait(false);

            await testee.DisposeAsync().ConfigureAwait(false);

            testee.Calls.Should().Equal(expectedCalls);
        }

        internal class DisposableBase : IDisposable, IAsyncDisposable
        {
            private readonly DisposableState disposer;

            public DisposableBase()
            {
                this.disposer = new DisposableState(this);
            }

            ~DisposableBase()
            {
                this.disposer.Dispose(false, this.Dispose);
            }

            public List<string> Calls { get; } = new List<string>();

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1816:TryDispose methods should call SuppressFinalize", Justification = "It is done by disposer.")]
            public void Dispose()
            {
                this.disposer.Dispose(true, this.Dispose);
            }

            public ValueTask DisposeAsync()
            {
                return this.disposer.DisposeAsync(true, this.DisposeAsync);
            }

            protected virtual void Dispose(bool isDisposing)
            {
                if (isDisposing)
                {
                    this.Calls.Add(nameof(DisposableBase));
                }
            }

            protected virtual ValueTask DisposeAsync(bool isDisposing)
            {
                if (isDisposing)
                {
                    this.Calls.Add(nameof(DisposableBase));
                }

                return default;
            }
        }

        internal class DisposableDerived : DisposableBase
        {
            protected override void Dispose(bool isDisposing)
            {
                if (isDisposing)
                {
                    this.Calls.Add(nameof(DisposableDerived));
                }

                base.Dispose(isDisposing);
            }

            protected override async ValueTask DisposeAsync(bool isDisposing)
            {
                if (isDisposing)
                {
                    this.Calls.Add(nameof(DisposableDerived));
                }

                await base.DisposeAsync(isDisposing).ConfigureAwait(false);
            }
        }
    }
}