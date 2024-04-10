// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EqualityTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.UnitTests.Equality;

using System;
using FluentAssertions;
using Sundew.Base.Equality;
using Xunit;

public class EqualityTests
{
    [Theory]
    [InlineData(1, 2, 1, 2, true)]
    [InlineData(1, 3, 1, 2, false)]
    public void StructEquals_Then_ResultShouldBeExpectedResult(double x1, double y1, double x2, double y2, bool expectedResult)
    {
        var point1 = new Point(x1, y1);
        var point2 = new Point(x2, y2);

        var result = point1.Equals(point2);

        result.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData(1, 2, 1, 2, true)]
    [InlineData(1, 3, 1, 2, false)]
    [InlineData(2, 1, 1, 2, false)]
    public void StructGetHashCode_Then_ResultShouldBeExpectedResult(double x1, double y1, double x2, double y2, bool expectedResult)
    {
        var point1 = new Point(x1, y1);
        var point2 = new Point(x2, y2);

        var result = point1.GetHashCode().Equals(point2.GetHashCode());

        result.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData(1, 2, 1, 2, true)]
    [InlineData(1, 3, 1, 2, false)]
    public void ClassEquals_Then_ResultShouldBeExpectedResult(double x1, double y1, double x2, double y2, bool expectedResult)
    {
        var vector1 = new Vector(x1, y1);
        var vector2 = new Vector(x2, y2);

        var result = vector1.Equals(vector2);

        result.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData(1, 2, 1, 2, true)]
    [InlineData(1, 3, 1, 2, false)]
    [InlineData(2, 1, 1, 2, false)]
    public void ClassGetHashCode_Then_ResultShouldBeExpectedResult(double x1, double y1, double x2, double y2, bool expectedResult)
    {
        var vector1 = new Vector(x1, y1);
        var vector2 = new Vector(x2, y2);

        var result = vector1.GetHashCode().Equals(vector2.GetHashCode());

        result.Should().Be(expectedResult);
    }

    private readonly struct Point : IEquatable<Point>
    {
        public Point(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }

        public double X { get; }

        public double Y { get; }

        public bool Equals(Point other)
        {
            return this.X == other.X && this.Y == other.Y;
        }

        public override bool Equals(object? obj)
        {
            return Equality.Equals(this, obj);
        }

        public override int GetHashCode()
        {
            return Equality.GetHashCode(this.X.GetHashCode(), this.Y.GetHashCode());
        }
    }

    private class Vector : IEquatable<Vector>
    {
        public Vector(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }

        public double X { get; }

        public double Y { get; }

        public bool Equals(Vector? other)
        {
            return Equality.Equals(this, other, rhs => this.X == rhs.X && this.Y == rhs.Y);
        }

        public override bool Equals(object? obj)
        {
            return Equality.Equals(this, obj);
        }

        public override int GetHashCode()
        {
            return Equality.GetHashCode(this.X.GetHashCode(), this.Y.GetHashCode());
        }
    }
}