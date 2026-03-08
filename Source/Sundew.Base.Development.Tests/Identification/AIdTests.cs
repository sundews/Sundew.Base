// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AIdTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Development.Tests.Identification;

using System;
using System.Globalization;
using AwesomeAssertions;
using AwesomeAssertions.Execution;
using Sundew.Base.Identification;
using static Sundew.Base.Development.Tests.Identification.AIdTests;

public class AIdTests
{
    [Test]
    [Obsolete("Obsolete")]
    public void T()
    {
        Uri.TryCreate("aid://user:pwd@Host:80/Name+Nested.Namespace$Assembly/Path?1", UriKind.Absolute, out var uri);
        Uri.TryCreate("aid://user:pwd@Host:80/Name+Nested.Namespace$Assembly/Find?Person=(Address=Home)&Description=(Eyes=Blue)", UriKind.Absolute, out var uri2);
        Uri.TryCreate("aid://user:pwd@Host:80/Name+Nested.Namespace$Assembly/Path?Name=Kim&LastName=Hugener", UriKind.Absolute, out var uri3);
        Uri.TryCreate("aid://user:pwd@Host:80/Name+Nested.Nam?espace$Assembly/Path?Name=Kim&LastName=Hugener", UriKind.Absolute, out var uri4);
        Uri.TryCreate("aid://user:pwd@Host:80/Name+Nested.Namespace$Assembly/Path?Name=Kim?LastName=Hugener", UriKind.Absolute, out var uri5);
        Uri.TryCreate("aid://user:pwd@Host:80/Name+Nested.Name.Space$Assembly/Find(person,description)?Person=(Address=Home,Number=15)&Description=(Eyes=Blue)", UriKind.Absolute, out var uri6);
        Uri.TryCreate("aid://user:pwd@Host:80/Name+Nested.Name.Space$Assembly/Find(person,description)?Person={Address=Home,Number=15}&Description={Eyes=Blue}", UriKind.Absolute, out var uri7);
        Uri.TryCreate("aid://user:pwd@Host:80/Name+Nested.Name.Space$Assembly/Find(Person,Description[])?person={Address=Home,Number=15}&description={Eyes=Blue}", UriKind.Absolute, out var uri8);
        Uri.TryCreate("aid://user:pwd@Host:80/Name+Nested.Name.Space$Assembly/Find(Person,IList[Description])?person=(Address=Home,Number=15)&descriptions=[Blue,Green]", UriKind.Absolute, out var uri9);
        var t1 = Uri.EscapeUriString(uri6!.OriginalString);
        var t2 = Uri.EscapeUriString(uri8!.OriginalString);
        var t3 = Uri.EscapeUriString(uri9!.OriginalString);
    }

    [Test]
    [Arguments("Name+Nested.Name.Space$Assembly")]
    [Arguments("Name+Nested.Name.Space$Assembly/Path")]
    [Arguments("Name+Nested.Name.Space$Assembly/Path?1")]
    [Arguments("Name+Nested.Namespace$Assembly/Path?Name=John&LastName=Doe")]
    [Arguments("Name+Nested.Namespace$Assembly/Some.Path?Name=John&LastName=Doe")]
    [Arguments("Name+Nested.Name.Space$Assembly/Find?Person=(Address=Home,Number=15)&Description=(Eyes=Blue)")]
    [Arguments("Name+Nested.Name.Space$Assembly/Find(Person,Description)?person=(Address=Home,Number=15)&description=(Eyes=Blue)")]
    [Arguments("Name+Nested.Name.Space$Assembly/Find(Person,IList[Description])?person=(Address=Home,Number=15)&descriptions=[Blue,Green]")]
    public void Parse_Then_ResultShouldNotBeNull(string input)
    {
        var result = AId.Parse(input, CultureInfo.InvariantCulture);

        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result.ToString().Should().Be(input);
        }
    }

    [Test]
    public void From_When_TargetIsMethodWith0Parameters_Then_ResultShouldNotBeNull()
    {
        const string expectedResult = "AIdTests+INavigator.Sundew.Base.Development.Tests.Identification$Sundew.Base.Development.Tests/GoBack()";
        var result = AId.From<INavigator>(x => x.GoBack());

        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result.ToString().Should().Be(expectedResult);
            result.TryGetInputTypes().Value.Should().Equal([]);
            result.TryGetResultType().Value.Should().Be(typeof(void));
            result.TryGetTargetContainingType().Value.Should().Be(typeof(INavigator));
        }
    }

    [Test]
    public void From_When_TargetIsMethodWith1Parameter_Then_ResultShouldNotBeNull()
    {
        const string expectedResult = "AIdTests+INavigator.Sundew.Base.Development.Tests.Identification$Sundew.Base.Development.Tests/Navigate(AIdTests+Position)";
        var result = AId.From<INavigator>(x => x.Navigate(null!));

        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result.ToString().Should().Be(expectedResult);
            result.TryGetInputTypes().Value.Should().Equal([typeof(Position)]);
            result.TryGetResultType().Value.Should().Be(typeof(void));
            result.TryGetTargetContainingType().Value.Should().Be(typeof(INavigator));
        }
    }

    [Test]
    public void From_When_TargetIsMethodWith2Parameters_Then_ResultShouldNotBeNull()
    {
        const string expectedResult = "AIdTests+INavigator.Sundew.Base.Development.Tests.Identification$Sundew.Base.Development.Tests/Navigate(AIdTests+Position,bool)";
        var result = AId.From<INavigator>(x => x.Navigate(null!, default));

        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result.ToString().Should().Be(expectedResult);
            result.TryGetInputTypes().Value.Should().Equal([typeof(Position), typeof(bool)]);
            result.TryGetResultType().Value.Should().Be(typeof(void));
            result.TryGetTargetContainingType().Value.Should().Be(typeof(INavigator));
        }
    }

    [Test]
    public void From_When_TargetIsProperty_Then_ResultShouldNotBeNull()
    {
        const string expectedResult = "AIdTests+Position.Sundew.Base.Development.Tests.Identification$Sundew.Base.Development.Tests/X";
        var result = AId.From<Position>(x => x.X);

        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result.ToString().Should().Be(expectedResult);
            result.TryGetInputTypes().Value.Should().Equal([typeof(int)]);
            result.TryGetResultType().Value.Should().Be(typeof(int));
            result.TryGetTargetContainingType().Value.Should().Be(typeof(Position));
        }
    }

    [Test]
    public void AsArguments_Then_ResultShouldBeExpectedResult()
    {
        const string expectedResult = "X=4&Y=5";
        var position = new Position(4, 5);

        var arguments = position.AsArguments();
        var result = Position.From(new Position(0, 0), arguments);

        using (new AssertionScope())
        {
            arguments.ToString().Should().Be(expectedResult);
            result.Should().Be(position);
        }
    }

    [Test]
    public void AsArguments_Then_ResultShouldBeExpectedResult2()
    {
        const string expectedResult = "Position=(X=4&Y=5)&Z=6";
        var position = new Position3D(new Position(4, 5), 6);

        var arguments = position.AsArguments();
        var result = Position3D.From(new Position3D(new Position(0, 0), 0), arguments);

        using (new AssertionScope())
        {
            arguments.ToString().Should().Be(expectedResult);
            result.Should().Be(position);
        }
    }

#pragma warning disable SA1201
    public interface INavigator
#pragma warning restore SA1201
    {
        void GoBack();

        void Navigate(Position position);

        void Navigate(Position position, bool addToHistory);
    }

    public record Position(int X, int Y) : IValueIdentifiable<Position>
    {
        public Arguments AsArguments() => Arguments.From(builder => builder.Add(this.X).Add(this.Y));

        public static Position From(Position position, Arguments arguments)
        {
            return new Position(
                arguments.Get(position.X, CultureInfo.InvariantCulture),
                arguments.Get(position.Y, CultureInfo.InvariantCulture));
        }
    }

    public record Position3D(Position Position, int Z) : IValueIdentifiable<Position3D>
    {
        public Arguments AsArguments()
        {
            return Arguments.From(builder => builder.Add(this.Position).Add(this.Z));
        }

        public static Position3D From(Position3D value, Arguments arguments)
        {
            return new Position3D(
                arguments.Get2(value.Position, CultureInfo.InvariantCulture),
                arguments.Get(value.Z, CultureInfo.InvariantCulture));
        }
    }
}