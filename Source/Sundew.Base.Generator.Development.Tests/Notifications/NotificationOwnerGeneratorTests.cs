// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NotificationOwnerGeneratorTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

extern alias NotificationsGenerator;

namespace Sundew.Base.Generator.Development.Tests.Notifications;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AwesomeAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using NotificationsGenerator::Sundew.Base.Notifications.Generator;

public class NotificationOwnerGeneratorTests
{
    private const string Usings = """
        using System.Threading;
        using System.Threading.Tasks;
        using Sundew.Base.Notifications;

        """;

    private const string TargetSubscriptionsImplementation = "global::Sundew.Base.Notifications.INotificationOwner.TargetSubscriptions";
    private const string SubscribeToDeclaration = " global::Sundew.Base.Notifications.Subscription SubscribeTo<";
    private const string ProtectedSubscribeToDeclaration = "protected" + SubscribeToDeclaration;
    private const string PrivateSubscribeToDeclaration = "private" + SubscribeToDeclaration;

    [Test]
    [Arguments("namespace Tests; public partial class Target : INotificationOwner { }", "Tests.Target.INotificationOwner.g.cs")]
    [Arguments("public partial class Target : INotificationOwner { }", "Target.INotificationOwner.g.cs")]
    [Arguments("namespace Tests; public partial record Target : INotificationOwner { }", "Tests.Target.INotificationOwner.g.cs")]
    [Arguments("namespace Tests; public partial class Target<TKey, TValue> : INotificationOwner where TKey : notnull { }", "Tests.Target`2.INotificationOwner.g.cs")]
    [Arguments("namespace Tests; public partial class Outer<T> { internal sealed partial class Inner : INotificationOwner { } }", "Tests.Outer`1.Inner.INotificationOwner.g.cs")]
    [Arguments("namespace Tests; public interface IExtended : INotificationOwner { } public partial class Target : IExtended { }", "Tests.Target.INotificationOwner.g.cs")]
    [Arguments("namespace Tests; public partial class Target : INotificationOwner { } public partial class Target { }", "Tests.Target.INotificationOwner.g.cs")]
    [Arguments("namespace Tests; public partial class Base : INotificationOwner { } public partial class Derived : Base { }", "Tests.Base.INotificationOwner.g.cs")]
    public void Generate_WhenPartial_Then_TargetSubscriptionsAndSubscribeToShouldBeGenerated(string source, string expectedHintName)
    {
        var (diagnostics, compileErrors, generated) = Generate(Usings + source);

        diagnostics.Should().BeEmpty();
        compileErrors.Should().BeEmpty();
        generated.Should().ContainSingle().Which.FilePath.Should().EndWith(expectedHintName);
        generated[0].Text.Should().Contain(TargetSubscriptionsImplementation).And.Contain(SubscribeToDeclaration);
    }

    [Test]
    [Arguments("namespace Tests; public partial class Target : INotificationOwner { }", ProtectedSubscribeToDeclaration)]
    [Arguments("namespace Tests; public sealed partial class Target : INotificationOwner { }", PrivateSubscribeToDeclaration)]
    [Arguments("namespace Tests; public sealed partial record Target : INotificationOwner { }", PrivateSubscribeToDeclaration)]
    public void Generate_WhenPartial_Then_SubscribeToShouldBeProtectedUnlessSealed(string source, string expectedDeclaration)
    {
        var (diagnostics, compileErrors, generated) = Generate(Usings + source);

        diagnostics.Should().BeEmpty();
        compileErrors.Should().BeEmpty();
        generated.Should().ContainSingle().Which.Text.Should().Contain(expectedDeclaration);
    }

    [Test]
    [Arguments("namespace Tests; public partial class Target : INotificationOwner { public Subscriptions TargetSubscriptions { get; } = new(); }")]
    [Arguments("namespace Tests; public partial class Target : INotificationOwner { Subscriptions INotificationOwner.TargetSubscriptions { get; } = new(); }")]
    [Arguments("namespace Tests; public class Base : INotificationOwner { public Subscriptions TargetSubscriptions { get; } = new(); } public partial class Derived : Base, INotificationOwner { }")]
    [Arguments("namespace Tests; public class Base { private void SubscribeTo() { } } public partial class Target : Base, INotificationOwner { public Subscriptions TargetSubscriptions { get; } = new(); }")]
    public void Generate_WhenTargetSubscriptionsIsImplemented_Then_OnlySubscribeToShouldBeGenerated(string source)
    {
        var (diagnostics, compileErrors, generated) = Generate(Usings + source);

        diagnostics.Should().BeEmpty();
        compileErrors.Should().BeEmpty();
        generated.Should().ContainSingle().Which.Text.Should().NotContain(TargetSubscriptionsImplementation).And.Contain(SubscribeToDeclaration);
    }

    [Test]
    [Arguments("namespace Tests; public partial class Target : INotificationOwner { private void SubscribeTo() { } }")]
    [Arguments("namespace Tests; public class Base { protected void SubscribeTo() { } } public partial class Target : Base, INotificationOwner { }")]
    public void Generate_WhenSubscribeToIsDeclared_Then_OnlyTargetSubscriptionsShouldBeGenerated(string source)
    {
        var (diagnostics, compileErrors, generated) = Generate(Usings + source);

        diagnostics.Should().BeEmpty();
        compileErrors.Should().BeEmpty();
        generated.Should().ContainSingle().Which.Text.Should().Contain(TargetSubscriptionsImplementation).And.NotContain(SubscribeToDeclaration);
    }

    [Test]
    [Arguments("namespace Tests; public class Target : INotificationOwner { public Subscriptions TargetSubscriptions { get; } = new(); }")]
    [Arguments("namespace Tests; public partial class Target : INotificationOwner { public Subscriptions TargetSubscriptions { get; } = new(); private void SubscribeTo() { } }")]
    public void Generate_WhenNothingIsMissing_Then_NothingShouldBeGenerated(string source)
    {
        var (diagnostics, compileErrors, generated) = Generate(Usings + source);

        diagnostics.Should().BeEmpty();
        compileErrors.Should().BeEmpty();
        generated.Should().BeEmpty();
    }

    [Test]
    [Arguments("namespace Tests; public partial class Base : INotificationOwner { } public partial class Derived : Base, INotificationOwner { }")]
    [Arguments("namespace Tests; public partial class Base : INotificationOwner { protected void SubscribeTo() { } } public partial class Derived : Base, INotificationOwner { }")]
    public void Generate_WhenBaseIsGenerated_Then_DerivedShouldGetNothing(string source)
    {
        var (diagnostics, compileErrors, generated) = Generate(Usings + source);

        diagnostics.Should().BeEmpty();
        compileErrors.Should().BeEmpty();
        generated.Should().ContainSingle().Which.FilePath.Should().EndWith("Tests.Base.INotificationOwner.g.cs");
    }

    [Test]
    public void Generate_WhenDerivedSubscribes_Then_BaseSubscribeToShouldBeUsed()
    {
        const string source = """
            namespace Tests;

            public partial class Base : INotificationOwner
            {
            }

            public sealed class Derived : Base
            {
                public Derived(INotify<int> notify, INotifyAny<object> notifyAny)
                {
                    this.SubscribeTo(notify, (int @event) => default);
                    this.SubscribeTo(notifyAny, (string @event, Subscription subscription) => default);
                }
            }
            """;

        var (diagnostics, compileErrors, generated) = Generate(Usings + source);

        diagnostics.Should().BeEmpty();
        compileErrors.Should().BeEmpty();
        generated.Should().ContainSingle();
    }

    [Test]
    public void Generate_WhenSubscribeToIsUsed_Then_EveryHandlerShapeShouldCompile()
    {
        const string source = """
            namespace Tests;

            public partial class Target : INotificationOwner
            {
                public Target(INotify<int> notify, INotifyAny<object> notifyAny)
                {
                    this.SubscribeTo(notify, (int @event, Subscription subscription, CancellationToken cancellationToken) => default);
                    this.SubscribeTo(notify, (int @event, CancellationToken cancellationToken) => default);
                    this.SubscribeTo(notify, (int @event, Subscription subscription) => default);
                    this.SubscribeTo(notify, (int @event) => default);
                    this.SubscribeTo(notifyAny, (string @event, Subscription subscription, CancellationToken cancellationToken) => default);
                    this.SubscribeTo(notifyAny, (string @event, CancellationToken cancellationToken) => default);
                    this.SubscribeTo(notifyAny, (string @event, Subscription subscription) => default);
                    this.SubscribeTo(notifyAny, (string @event) => default);
                }
            }
            """;

        var (diagnostics, compileErrors, generated) = Generate(Usings + source);

        diagnostics.Should().BeEmpty();
        compileErrors.Should().BeEmpty();
        generated.Should().ContainSingle();
    }

    [Test]
    public void Generate_WhenTypeParametersClash_Then_GeneratedTypeParametersShouldBeRenamed()
    {
        var (diagnostics, compileErrors, generated) = Generate(Usings + "namespace Tests; public partial class Target<TEvent, TSubscribedEvent> : INotificationOwner { }");

        diagnostics.Should().BeEmpty();
        compileErrors.Should().BeEmpty();
        generated.Should().ContainSingle().Which.Text.Should().Contain("SubscribeTo<TEvent1>(").And.Contain("SubscribeTo<TEvent1, TSubscribedEvent1>(");
    }

    [Test]
    [Arguments("namespace Tests; public class Target : INotificationOwner { }")]
    [Arguments("namespace Tests; public class Outer { public partial class Inner : INotificationOwner { } }")]
    public void Generate_WhenNotPartial_Then_NotPartialInfoShouldBeReportedAndNothingGenerated(string source)
    {
        var (diagnostics, compileErrors, generated) = Generate(Usings + source);

        diagnostics.Select(x => x.Id).Should().Equal("SBN0001");
        diagnostics.Should().OnlyContain(x => x.Severity == DiagnosticSeverity.Info);
        compileErrors.Should().NotBeEmpty();
        generated.Should().BeEmpty();
    }

    /// <summary>
    /// Runs the generator and returns its diagnostics, the errors of the resulting compilation together with any warnings in generated
    /// code, and the generated files.
    /// </summary>
    private static (List<Diagnostic> Diagnostics, List<string> CompileErrors, List<(string FilePath, string Text)> Generated) Generate(string source)
    {
        var parseOptions = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Preview);
        var compilation = CSharpCompilation.Create(
            "GeneratorTest",
            [CSharpSyntaxTree.ParseText(source, parseOptions, path: "Target.cs")],
            GetReferences(),
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, nullableContextOptions: NullableContextOptions.Enable));
        var driver = CSharpGeneratorDriver.Create([new NotificationOwnerGenerator().AsSourceGenerator()], parseOptions: parseOptions);

        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var generatorDiagnostics);

        var diagnostics = generatorDiagnostics.ToList();
        var compileErrors = outputCompilation.GetDiagnostics()
            .Where(x => x.Severity == DiagnosticSeverity.Error || (x.Severity == DiagnosticSeverity.Warning && IsGenerated(x.Location.SourceTree)))
            .Select(x => x.ToString())
            .ToList();
        var generated = outputCompilation.SyntaxTrees.Where(IsGenerated).Select(x => (x.FilePath, x.ToString())).ToList();
        return (diagnostics, compileErrors, generated);
    }

    private static bool IsGenerated(SyntaxTree? syntaxTree)
    {
        return syntaxTree?.FilePath.EndsWith(".g.cs") == true;
    }

    private static IEnumerable<MetadataReference> GetReferences()
    {
        // The list uses the platform's path separator: a semicolon on Windows, a colon elsewhere.
        var trustedAssemblies = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!).Split(Path.PathSeparator);
        var sundewAssemblies = new[]
        {
            typeof(Sundew.Base.Notifications.Subscription).Assembly.Location,
            typeof(Sundew.Base.Collections.Parallelism).Assembly.Location,
            typeof(Sundew.Base.Disposal.DisposeAction).Assembly.Location,
        };

        // The generator assembly carries its own copy of the contract, so it must not be a reference of the compilation it runs on.
        var generatorAssembly = typeof(NotificationOwnerGenerator).Assembly.Location;
        return trustedAssemblies.Concat(sundewAssemblies).Distinct().Where(x => x != generatorAssembly).Select(x => MetadataReference.CreateFromFile(x));
    }
}
