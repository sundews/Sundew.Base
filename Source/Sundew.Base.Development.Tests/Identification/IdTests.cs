// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IdTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Development.Tests.Identification;

using System;
using System.Globalization;
using System.Linq;
using AwesomeAssertions;
using AwesomeAssertions.Execution;
using Sundew.Base.Identification;
using static Sundew.Base.Development.Tests.Identification.IdTests;

public class IdTests
{
    [Test]
    [Obsolete("Obsolete")]
    public void T()
    {
        Uri.TryCreate("aid://user:pwd@Host:80/Name+Nested~Namespace$Assembly/Path?1", UriKind.Absolute, out var uri);
        Uri.TryCreate("aid://user:pwd@Host:80/Name+Nested~Namespace$Assembly/Find?Person=(Address=Home)&Description=(Eyes=Blue)", UriKind.Absolute, out var uri2);
        Uri.TryCreate("aid://user:pwd@Host:80/Name+Nested~Namespace$Assembly/Path?Name=Kim&LastName=Hugener", UriKind.Absolute, out var uri3);
        Uri.TryCreate("aid://user:pwd@Host:80/Name+Nested~Nam?espace$Assembly/Path?Name=Kim&LastName=Hugener", UriKind.Absolute, out var uri4);
        Uri.TryCreate("aid://user:pwd@Host:80/Name+Nested~Namespace$Assembly/Path?Name=Kim?LastName=Hugener", UriKind.Absolute, out var uri5);
        Uri.TryCreate("aid://user:pwd@Host:80/Name+Nested~Name.Space$Assembly/Find(person,description)?Person=(Address=Home,Number=15)&Description=(Eyes=Blue)", UriKind.Absolute, out var uri6);
        Uri.TryCreate("aid://user:pwd@Host:80/Name+Nested~Name.Space$Assembly/Find(person,description)?Person={Address=Home,Number=15}&Description={Eyes=Blue}", UriKind.Absolute, out var uri7);
        Uri.TryCreate("aid://user:pwd@Host:80/Name+Nested~Name.Space$Assembly/Find(Person,Description[])?person={Address=Home,Number=15}&description={Eyes=Blue}", UriKind.Absolute, out var uri8);
        Uri.TryCreate("aid://user:pwd@Host:80/Name+Nested~Name.Space$Assembly/Find(person,IList[Description])?person=(Address=Home,Number=15)&descriptions=[Blue,Green]", UriKind.Absolute, out var uri9);
        Uri.TryCreate("aid://user:pwd@Host:80/Name+Nested~Name.Space$Assembly/Find(person,descriptions)?person=(Address=Home,Number=15)&descriptions=[Blue,Green]", UriKind.Absolute, out var uri10);
        Uri.TryCreate("aid://user:pwd@Host:80/Name+Nested~Name.Space$Assembly/Find(query,descriptions)?Query!Name.Name.Space$Assembly=(Address=Home,Number=15)&descriptions=[Blue,Green]", UriKind.Absolute, out var uri11);
        Uri.TryCreate("aid://user:pwd@Host:80/Name+Nested~Name.Space$Assembly/Start(!Name~Name.Space$Assembly=15)/Find?!Name.Name.Space$Assembly=(Address=Home,Number=15)&string[]=[Blue,Green]", UriKind.Absolute, out var uri12);
        var t1 = Uri.EscapeUriString(uri6!.OriginalString);
        var t2 = Uri.EscapeUriString(uri8!.OriginalString);
        var t3 = Uri.EscapeUriString(uri9!.OriginalString);
        var t4 = Uri.EscapeUriString(uri10!.OriginalString);
        var t5 = Uri.EscapeUriString(uri11!.OriginalString);
        var t6 = Uri.EscapeUriString(uri12!.OriginalString);
    }

    [Test]
    [Arguments("Name+Nested~Name.Space$Assembly")]
    [Arguments("Name+Nested~Name.Space$Assembly/Path")]
    [Arguments("Name+Nested~Name.Space$Assembly/Path?1")]
    [Arguments("Name+Nested~Namespace$Assembly/Path?Name=John&LastName=Doe")]
    [Arguments("Name+Nested~Namespace$Assembly/Some/Path?Name=John&LastName=Doe")]
    [Arguments("Name+Nested~Name.Space$Assembly/Find?Person=(Address=Home&Number=15)&Description=(Eyes=Blue)")]
    [Arguments("Name+Nested~Name.Space$Assembly/Find?Person=(Address=Home&Number=15)&Colors=[Blue,Green]")]
    [Arguments("Name+Nested~Name.Space$Assembly/Find((Address=Home&Number=15)&Colors=[Blue,Green])")]
    [Arguments("Name+Nested~Name.Space$Assembly/Find(Query!Name~Name.Space$Assembly=(Address=Home&Number=15)&Colors=[Blue,Green])")]
    [Arguments("Name+Nested~Name.Space$Assembly/Find(!Name~Name.Space$Assembly=(Address=Home&Number=15)&Colors=[Blue,Green])")]
    [Arguments("Name+Nested~Name.Space$Assembly/Find(!Name~Name.Space$Assembly=(Address=Home&Number=15)&Colors=[!Colors~Assembly=Blue,Green])")]
    [Arguments("Name+Nested~Name.Space$Assembly/Find(!Name~Name.Space$Assembly=(Address=Home&Number=15)&Colors=[!Colors~Namespace$Assembly=Blue,Green])")]
    [Arguments("IdTests+INavigator~Sundew.Base.Development.Tests.Identification$Sundew.Base.Development.Tests/NavigateTo(position!IdTests+Position~Sundew.Base.Development.Tests.Identification$Sundew.Base.Development.Tests=null)")]
    public void Parse_Then_ResultShouldNotBeNull(string input)
    {
        var result = Id.Parse(input, CultureInfo.InvariantCulture);

        using (var scope = new AssertionScope())
        {
            scope.FormattingOptions.MaxDepth = 20;
            result.Should().NotBeNull();
            result.ToString().Should().Be(input);
        }
    }

    [Test]
    public void From_When_TargetIsMethodWith0Parameters_Then_ResultShouldBeExpected()
    {
        const string expectedResult = "IdTests+INavigator~Sundew.Base.Development.Tests.Identification$Sundew.Base.Development.Tests/GoBack()";
        var result = Id.From<INavigator>(x => x.GoBack());

        using (var scope = new AssertionScope())
        {
            scope.FormattingOptions.MaxDepth = 20;
            var expected = Id.Parse(result.ToString(), CultureInfo.InvariantCulture);
            result.Should().Be(expected);
            result.ToString().Should().Be(expectedResult);
            result.TryGetInputTypes().Value.Should().Equal([]);
            result.TryGetResultType().Value.Should().Be(typeof(void));
            result.TryGetTargetContainingType().Value.Should().Be(typeof(INavigator));
        }
    }

    [Test]
    public void From_When_TargetIsMethodWith1ParameterAsNull_Then_ResultShouldBeExpected()
    {
        const string expectedResult = "IdTests+INavigator~Sundew.Base.Development.Tests.Identification$Sundew.Base.Development.Tests/NavigateTo(position!IdTests+Position~Sundew.Base.Development.Tests.Identification$Sundew.Base.Development.Tests=null)";
        var result = Id.From<INavigator>(x => x.NavigateTo(null!));

        using (var scope = new AssertionScope())
        {
            scope.FormattingOptions.MaxDepth = 20;
            result.Should().Be(Id.Parse(result.ToString(), CultureInfo.InvariantCulture));
            result.ToString().Should().Be(expectedResult);
            result.TryGetInputTypes().Value.Should().Equal([typeof(Position)]);
            result.TryGetResultType().Value.Should().Be(typeof(void));
            result.TryGetTargetContainingType().Value.Should().Be(typeof(INavigator));
        }
    }

    [Test]
    public void From_When_TargetIsMethodWith1Parameter_Then_ResultShouldBeExpected()
    {
        const string expectedResult = "IdTests+INavigator~Sundew.Base.Development.Tests.Identification$Sundew.Base.Development.Tests/NavigateTo(position!IdTests+Position~Sundew.Base.Development.Tests.Identification$Sundew.Base.Development.Tests=(X=6&Y=4))";
        var result = Id.From<INavigator>(x => x.NavigateTo(new Position(6, 4)));

        using (var scope = new AssertionScope())
        {
            scope.FormattingOptions.MaxDepth = 20;
            result.Should().Be(Id.Parse(result.ToString(), CultureInfo.InvariantCulture));
            result.ToString().Should().Be(expectedResult);
            result.TryGetInputTypes().Value.Should().Equal([typeof(Position)]);
            result.TryGetResultType().Value.Should().Be(typeof(void));
            result.TryGetTargetContainingType().Value.Should().Be(typeof(INavigator));
        }
    }

    [Test]
    public void From_When_TargetIsMethodWith2Parameters_Then_ResultShouldNotBeNull()
    {
        const string expectedResult = "IdTests+INavigator~Sundew.Base.Development.Tests.Identification$Sundew.Base.Development.Tests/NavigateTo(position!IdTests+Position~Sundew.Base.Development.Tests.Identification$Sundew.Base.Development.Tests=null&addToHistory=False)";
        var result = Id.From<INavigator>(x => x.NavigateTo(null!, default));

        using (var scope = new AssertionScope())
        {
            scope.FormattingOptions.MaxDepth = 20;
            result.Should().Be(Id.Parse(result.ToString(), CultureInfo.InvariantCulture));
            result.ToString().Should().Be(expectedResult);
            result.TryGetInputTypes().Value.Should().Equal([typeof(Position), typeof(bool)]);
            result.TryGetResultType().Value.Should().Be(typeof(void));
            result.TryGetTargetContainingType().Value.Should().Be(typeof(INavigator));
        }
    }

    [Test]
    public void From_When_TargetIsProperty_Then_ResultShouldNotBeNull()
    {
        const string expectedResult = "IdTests+Position~Sundew.Base.Development.Tests.Identification$Sundew.Base.Development.Tests/X";
        var result = Id.From<Position>(x => x.X);

        using (var scope = new AssertionScope())
        {
            scope.FormattingOptions.MaxDepth = 20;
            result.Should().Be(Id.Parse(result.ToString(), CultureInfo.InvariantCulture));
            result.ToString().Should().Be(expectedResult);
            result.TryGetInputTypes().Value.Should().Equal([typeof(int)]);
            result.TryGetResultType().Value.Should().Be(typeof(int));
            result.TryGetTargetContainingType().Value.Should().Be(typeof(Position));
        }
    }

    [Test]
    public void ToValue_Then_ResultShouldBeExpectedResult()
    {
        const string expectedResult = "!IdTests+Position~Sundew.Base.Development.Tests.Identification$Sundew.Base.Development.Tests=(X=4&Y=5)";
        var position = new Position(4, 5);

        var valueId = position.Id;
        var result = valueId.ToValue(new Position(0, 0));

        using (var scope = new AssertionScope())
        {
            scope.FormattingOptions.MaxDepth = 20;
            valueId.ToString().Should().Be(expectedResult);
            result.Should().Be(position);
        }
    }

    [Test]
    public void ToValue_When_UsingNestedType_Then_ResultShouldBeExpectedResult()
    {
        const string expectedResult = "!IdTests+Position3D~Sundew.Base.Development.Tests.Identification$Sundew.Base.Development.Tests=(Position=(X=4&Y=5)&Z=6)";
        var position = new Position3D(new Position(4, 5), 6);

        var valueId = position.Id;
        var result = valueId.ToValue(new Position3D(new Position(0, 0), 0));

        using (var scope = new AssertionScope())
        {
            scope.FormattingOptions.MaxDepth = 20;
            valueId.ToString().Should().Be(expectedResult);
            result.Should().Be(position);
        }
    }

    [Test]
    public void From_When_TargetIsMethodWith1Parameters_Then_ResultShouldBeExpected()
    {
        const string expectedResult = "IdTests+INavigator~Sundew.Base.Development.Tests.Identification$Sundew.Base.Development.Tests/Navigate/Execute(parameter!IdTests+Position~Sundew.Base.Development.Tests.Identification$Sundew.Base.Development.Tests=(X=4&Y=6))";
        var result = Id.From<INavigator>(x => x.Navigate.Execute(Id.Argument<Position>()), new Position(4, 6));

        using (var scope = new AssertionScope())
        {
            scope.FormattingOptions.MaxDepth = 20;
            result.Should().Be(Id.Parse(result.ToString(), CultureInfo.InvariantCulture));
            result.ToString().Should().Be(expectedResult);
            result.TryGetInputTypes().Value.Should().Equal([typeof(Position)]);
            result.TryGetResultType().Value.Should().Be(typeof(void));
            result.TryGetTargetContainingType().Value.Should().Be(typeof(ICommand<Position>));
        }
    }

    [Test]
    public void From_When_TargetIsPropertyAndPassingArgument_Then_ResultShouldBeExpected()
    {
        const string expectedResult = "IdTests+INavigator~Sundew.Base.Development.Tests.Identification$Sundew.Base.Development.Tests/Navigate?!IdTests+Position~Sundew.Base.Development.Tests.Identification$Sundew.Base.Development.Tests=(X=4&Y=6)";
        var result = Id.From<INavigator>(x => x.Navigate, new Position(4, 6));

        using (var scope = new AssertionScope())
        {
            scope.FormattingOptions.MaxDepth = 20;
            var expected = Id.Parse(result.ToString(), CultureInfo.InvariantCulture);
            expected.Should().Be(result);
            result.ToString().Should().Be(expectedResult);
            result.TryGetInputTypes().Value.Should().Equal([typeof(Position)]);
            result.TryGetResultType().Value.Should().Be(typeof(ICommand<Position>));
            result.TryGetTargetContainingType().Value.Should().Be(typeof(INavigator));
        }
    }

#pragma warning disable SA1201
    public interface INavigator
#pragma warning restore SA1201
    {
        ICommand<Position> Navigate { get; }

        void GoBack();

        void NavigateTo(Position position);

        void NavigateTo(Position position, bool addToHistory);
    }

    public interface ICommand<TParameter>
    {
        void Execute(TParameter parameter);
    }

    public record Position(int X, int Y) : IValueIdentifiable<Position>
    {
        public ValueId Id => ValueId.From(this, (value, builder) => builder.Add(value.X).Add(value.Y));

        public static Position From(Position position, ValueId valueId, IFormatProvider? formatProvider)
        {
            return new Position(
                valueId.GetScalar(position.X, CultureInfo.InvariantCulture),
                valueId.GetScalar(position.Y, CultureInfo.InvariantCulture));
        }
    }

    public record Position3D(Position Position, int Z) : IValueIdentifiable<Position3D>
    {
        public ValueId Id => ValueId.From(this, (value, builder) => builder.Add(value.Position).Add(value.Z));

        public static Position3D From(Position3D value, ValueId valueId, IFormatProvider? formatProvider)
        {
            return new Position3D(
                valueId.GetValue(value.Position, CultureInfo.InvariantCulture),
                valueId.GetScalar(value.Z, CultureInfo.InvariantCulture));
        }
    }
}