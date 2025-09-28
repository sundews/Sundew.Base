// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BufferTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.UnitTests.Memory;

using System;
using System.Collections.Generic;
using System.Linq;
using AwesomeAssertions;
using Sundew.Base.Collections;
using Sundew.Base.Memory;
using Telerik.JustMock;
using Xunit;

public class BufferTests
{
    private static readonly byte[] ExpectedResult = [1, 2, 3, 4, 5];

    [Fact]
    public void Write_Then_ToArrayShouldEqualExpectedResult()
    {
        var testee = new Buffer<byte>();

        testee.Write(ExpectedResult);

        testee.AsSpan().Should().Equal(ExpectedResult);
    }

    [Fact]
    public void Write_When_BufferHasBeenSliced_Then_SliceAndBufferShouldEqualExpectedResults()
    {
        var testee = new Buffer<byte>();
        var slice = testee.Slice(7, 5);

        slice.Write(ExpectedResult);

        testee.Length.Should().Be(ExpectedResult.Length);
        slice.Length.Should().Be(ExpectedResult.Length);
        slice.AsSpan().Should().Equal(ExpectedResult);
        testee.AsSpan().Should().Equal(ExpectedResult);
        testee.Capacity.Should().BeGreaterThanOrEqualTo(slice.StartIndex + slice.Length);
    }

    [Fact]
    public void Write_When_BufferHasBeenSlicedMultipleTimes_Then_SliceAndBufferShouldEqualExpectedResults()
    {
        var testee = new Buffer<byte>();
        var totalExpectedArray = new byte[] { 5, 4, 3, 2, 1, 0, 1, 2, 3, 4, 5 };
        var slice = testee.Slice(7, 5);
        slice.Write(ExpectedResult);
        var slice2 = testee.Slice(1, 6);
        var slice2LengthBeforeWrite = slice2.Length;

        slice2.Write(totalExpectedArray.AsSpan(0, 6));

        slice2LengthBeforeWrite.Should().Be(0);
        slice.Length.Should().Be(ExpectedResult.Length);
        slice.AsSpan().Should().Equal(ExpectedResult);
        testee.Length.Should().Be(totalExpectedArray.Length);
        testee.AsSpan().Should().Equal(totalExpectedArray);
    }

    [Fact]
    public void Slice_When_ExceedsCapacity_Then_BufferShouldBeResized()
    {
        const int expectedLength = 5;
        var bufferResizer = Mock.Create<IBufferResizer<byte>>();
        var testee = new Buffer<byte>(bufferResizer);
        var resizeArrange = Mock.Arrange(() => bufferResizer.Resize(Arg.IsAny<byte[]>(), Arg.AnyInt))
            .Returns((byte[] _, int requiredCapacity) => new byte[requiredCapacity]);

        var slice = testee.Slice(12, expectedLength);

        slice.Capacity.Should().Be(expectedLength);
        resizeArrange.OccursOnce();
    }

    [Fact]
    public void Write_When_SliceHasToGrow_Then_TesteeAsSpanShouldEqualExpectedResult()
    {
        var buffer = new Buffer<byte>();
        var testee = buffer.Slice(12, 5).Slice(2, 4);

        testee.Write(ExpectedResult);

        testee.AsSpan().Should().Equal(ExpectedResult);
    }

    [Fact]
    public void Write_When_SliceHasToGrowAndIsNotLastSlice_Then_NotSupportedExceptionShouldBeThrown()
    {
        var buffer = new Buffer<byte>();
        var testee = buffer.Slice(12, 5).Slice(1, 1);

        Action action = () => testee.Write(new byte[] { 1, 2 });

        action.Should().Throw<NotSupportedException>();
    }

    [Fact]
    public void Indexer_When_SetThroughIndexerAndFetched_Then_ResultShouldBeExpectedResult()
    {
        const int expectedResult = 0xFF;
        var testee = new Buffer<byte>();
        testee[20] = expectedResult;

        var result = testee[20];

        result.Should().Be(expectedResult);
    }

    [Fact]
    public void WriteRange_When_RangeIsAList_Then_ResultShouldBeExpectedResult()
    {
        var testee = new Buffer<byte>();
        var expectedResult = new List<byte> { 1, 2, 3, 4, 5 };

        testee.WriteRange(expectedResult);

        testee.AsSpan().Should().Equal(expectedResult.ToArray());
    }

    [Fact]
    public void WriteRange_When_RangeIsAnArray_Then_ResultShouldBeExpectedResult()
    {
        var testee = new Buffer<byte>();

        testee.WriteRange(ExpectedResult);

        testee.AsSpan().Should().Equal(ExpectedResult);
    }

    [Fact]
    public void WriteRange_When_RangeIsAnEnumerable_Then_ResultShouldBeExpectedResult()
    {
        var testee = new Buffer<int>();
        var expectedResult = Enumerable.Range(1, 5);

        testee.WriteRange(expectedResult);

        testee.AsSpan().Should().Equal(expectedResult.ToArray());
    }

    [Fact]
    public void WriteRange_When_RangeIsAList_ThenInReadOnlyItems_ResultShouldBeExpectedResult()
    {
        var testee = new Buffer<byte>();
        var expectedResult = new List<byte> { 1, 2, 3, 4, 5 }.ToReadOnlyCollection();

        testee.WriteRange(expectedResult);

        testee.AsSpan().Should().Equal(expectedResult.ToArray());
    }

    [Fact]
    public void WriteRange_When_RangeIsAnArrayInReadOnlyItems_Then_ResultShouldBeExpectedResult()
    {
        var testee = new Buffer<byte>();
        var expectedResult = BufferTests.ExpectedResult.ToReadOnlyCollection();

        testee.WriteRange(expectedResult);

        testee.AsSpan().Should().Equal(expectedResult.ToArray());
    }

    [Fact]
    public void WriteRange_When_RangeIsAnEnumerableInReadOnlyItems_Then_ResultShouldBeExpectedResult()
    {
        var testee = new Buffer<int>();
        var expectedResult = Enumerable.Range(1, 5).ToReadOnlyCollection();

        testee.WriteRange(expectedResult);

        testee.AsSpan().Should().Equal(expectedResult.ToArray());
    }

    [Fact]
    public void Write_When_PreviousDataHasBeenWritten_Then_ResultsShouldBeExpectedResult()
    {
        var testee = new Buffer<byte>();
        testee.Write(ExpectedResult.AsSpan(1, 2));

        testee.Write(ExpectedResult.AsSpan(3, 2));

        testee.AsSpan().Should().Equal(ExpectedResult.AsSpan(1, 4));
        testee.Position.Should().Be(4);
        testee.Length.Should().Be(4);
    }

    [Theory]
    [InlineData(null, 10, 10, 20)]
    [InlineData(null, 1000, 128, 1128)]
    [InlineData(1500, 1000, 128, 1500)]
    [InlineData(100, 20, 30, 100)]
    public void SliceDefault_Then_SliceShouldBeInMinimumAt4AndMaxAt128(int? startCapacity, int expectedCapacity, int expectedStartIndex, int expectedTotalCapacity)
    {
        var testee = startCapacity.HasValue ? new Buffer<byte>(startCapacity.Value) : new Buffer<byte>();

        var slice = testee.SliceDefault(expectedCapacity);

        slice.StartIndex.Should().Be(expectedStartIndex);
        slice.Capacity.Should().Be(expectedCapacity);
        slice.Length.Should().Be(0);
        testee.Capacity.Should().Be(expectedTotalCapacity);
    }

    [Theory]
    [InlineData(null, 2, 4, 16)]
    [InlineData(null, 100, 1000, 2300)]
    [InlineData(1500, 100, 1000, 1500)]
    public void SliceDefault_When_CalledMultipleTimes_Then_SliceShouldBeAfterFirstSlice(int? startCapacity, int firstSliceCapacity, int expectedCapacity, int expectedTotalCapacity)
    {
        var testee = startCapacity.HasValue ? new Buffer<byte>(startCapacity.Value) : new Buffer<byte>();
        var slice = testee.SliceDefault(firstSliceCapacity);

        var result = testee.SliceDefault(expectedCapacity);

        result.StartIndex.Should().Be(slice.StartIndex + slice.Capacity);
        result.Capacity.Should().Be(expectedCapacity);
        result.Length.Should().Be(0);
        testee.Capacity.Should().Be(expectedTotalCapacity);
    }

    [Fact]
    public void SliceDefault_When_BufferHasContent_Then_SliceShouldBeAfterFirstSlice()
    {
        var testee = new Buffer<byte>();
        ExpectedResult.AsSpan().CopyTo(testee.GetSpan(5, 5));

        var result = testee.SliceDefault(5);

        result.Capacity.Should().Be(5);
        testee.Length.Should().Be(5);
    }

    [Fact]
    public void Slice_Then_ResultShouldBeExpectedResult()
    {
        var testee = new Buffer<byte>();
        var slice = testee.Slice(6, 10);
        slice.Write(ExpectedResult);

        var result = testee.Slice();

        result.StartIndex.Should().Be(6);
        result.Length.Should().Be(ExpectedResult.Length);
        result.AsSpan().Should().Equal(ExpectedResult);
        slice.AsSpan().Should().Equal(ExpectedResult);
    }

    [Fact]
    public void GetSpan_Then_LengthShouldBeIncrementedToTheLengthOrTheRequestedSpan()
    {
        var testee = new Buffer<byte>();
        var slice = testee.SliceDefault(ExpectedResult.Length);

        ExpectedResult.AsSpan().CopyTo(slice.GetSpan());

        testee.AsSpan().Should().Equal(ExpectedResult);
        testee.Length.Should().Be(slice.Capacity);
        testee.StartIndex.Should().Be(slice.StartIndex);
    }

    [Fact]
    public void GetSpan_When_WritingToASecondSlice_Then_LengthShouldBeIncrementedToTheLengthOrTheRequestedSpan()
    {
        var testee = new Buffer<byte>();
        var slice1 = testee.SliceDefault(ExpectedResult.Length);
        ExpectedResult.AsSpan().CopyTo(slice1.GetSpan());
        var slice = testee.SliceDefault(ExpectedResult.Length);

        ExpectedResult.AsSpan().CopyTo(slice.GetSpan());

        testee.AsSpan().Should().Equal(ExpectedResult.Concat(ExpectedResult).ToArray());
        testee.Length.Should().Be(ExpectedResult.Length * 2);
        testee.StartIndex.Should().Be(slice1.StartIndex);
    }
}