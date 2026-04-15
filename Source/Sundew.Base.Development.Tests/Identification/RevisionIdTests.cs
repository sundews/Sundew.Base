// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RevisionIdTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Development.Tests.Identification;

using AwesomeAssertions;
using Sundew.Base.Identification;

public class RevisionIdTests
{
    [Test]
    public void Next_WhenCalledASecondTime_Then_TheTwoShouldNotBeEqual()
    {
        var id1 = RevisionId.Next();
        var id2 = RevisionId.Next();

        id1.Should().NotBe(id2);
    }

    [Test]
    public void Next_WhenCalledASecondTime_Then_TheFirstShouldNotBeNewer()
    {
        var id1 = RevisionId.Next();
        var id2 = RevisionId.Next();

        var result = id1.IsNewer(id2);

        result.Should().BeFalse();
    }

    [Test]
    public void Next_WhenCalledASecondTime_Then_TheSecondShouldBeNewer()
    {
        var id1 = RevisionId.Next();
        var id2 = RevisionId.Next();

        var result = id2.IsNewer(id1);

        result.Should().BeTrue();
    }

    [Test]
    public void Next_WhenCalledASecondTimeAndOverflows_Then_TheSecondShouldBeNewer()
    {
        var currentId = SequenceId<RevisionId>.CurrentId;
        SequenceId<RevisionId>.CurrentId = uint.MaxValue - 1;
        var id1 = RevisionId.Next();
        var id2 = RevisionId.Next();
        SequenceId<RevisionId>.CurrentId = currentId;

        var result = id2.IsNewer(id1);

        result.Should().BeTrue();
    }
}