// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TargetEvaluator.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Identification;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using Sundew.Base.Collections.Linq;
using Sundew.Base.Identification.Parsing;
using Sundew.Base.Text;

internal static class TargetEvaluator
{
    private static readonly Dictionary<Type, string> PrimitiveAliases = new()
    {
        [typeof(bool)] = "bool",
        [typeof(byte)] = "byte",
        [typeof(sbyte)] = "sbyte",
        [typeof(char)] = "char",
        [typeof(decimal)] = "decimal",
        [typeof(double)] = "double",
        [typeof(float)] = "float",
        [typeof(int)] = "int",
        [typeof(uint)] = "uint",
        [typeof(long)] = "long",
        [typeof(ulong)] = "ulong",
        [typeof(short)] = "short",
        [typeof(ushort)] = "ushort",
        [typeof(string)] = "string",
        [typeof(object)] = "object",
        [typeof(void)] = "void",
    };

    public static R<Type> GetResultType(Source source, Path? path)
    {
        var sourceType = source.TryGetType();
        if (sourceType.IsError)
        {
            return R.Error();
        }

        if (!path.HasValue)
        {
            return sourceType;
        }

        var memberInfo = GetTargetMemberInfo(sourceType.Value, path);
        if (memberInfo.HasValue)
        {
            switch (memberInfo)
            {
                case MethodInfo methodInfo:
                    return R.Success(methodInfo.ReturnType);
                case PropertyInfo propertyInfo:
                    return R.Success(propertyInfo.PropertyType);
            }
        }

        return R.Error();
    }

    public static R<IReadOnlyList<Type>> GetInputTypes(Source source, Path? path, IArguments? arguments)
    {
        var sourceType = source.TryGetType();
        if (sourceType.IsError)
        {
            return R.Error();
        }

        if (arguments.HasValue)
        {
            return arguments.ToValueIds().Items.Select(x => x.TryGetType()).AllOrFailed(x => x.ToItem()).Map(x => (IReadOnlyList<Type>)x.Items);
        }

        if (!path.HasValue)
        {
            return R.Success<IReadOnlyList<Type>>([sourceType.Value]);
        }

        var memberInfo = GetTargetMemberInfo(sourceType.Value, path);
        if (memberInfo.HasValue)
        {
            switch (memberInfo)
            {
                case MethodInfo methodInfo:
                    return R.Success<IReadOnlyList<Type>>(methodInfo.GetParameters().Select(x => x.ParameterType).ToArray());
                case PropertyInfo propertyInfo:
                    return R.Success<IReadOnlyList<Type>>([propertyInfo.PropertyType]);
            }
        }

        return R.Error();
    }

    public static R<Type> GetDeclaringType(Source source, Path? path)
    {
        var sourceType = source.TryGetType();
        if (sourceType.IsError)
        {
            return R.Error();
        }

        if (!path.HasValue)
        {
            return sourceType;
        }

        var memberInfo = GetTargetMemberInfo(sourceType.Value, path);
        if (memberInfo.HasValue)
        {
            return R.From(memberInfo.DeclaringType);
        }

        return R.Error();
    }

    public static bool IsKnownType(Type type)
    {
        return PrimitiveAliases.ContainsKey(type);
    }

    internal static R<Type> TryGetKnownType(string? inputSource)
    {
        return R.From(PrimitiveAliases.FirstOrDefault(x => x.Value == inputSource).Key);
    }

    internal static bool GetTypeName(Type type, StringBuilder stringBuilder)
    {
        if (PrimitiveAliases.TryGetValue(type, out var alias))
        {
            stringBuilder.Append(alias);
            return true;
        }

        if (type.IsArray)
        {
            var elementType = type.GetElementType()!;
            var rank = type.GetArrayRank();
            var commas = rank > 1 ? new string(',', rank - 1) : string.Empty;
            GetTypeName(elementType, stringBuilder);
            stringBuilder.Append($"[{commas}]");
            return false;
        }

        if (type.IsGenericType)
        {
            var genericDef = type.GetGenericTypeDefinition();
            var baseName = genericDef.Name;
            var backtickIndex = baseName.IndexOf('`');
            if (backtickIndex > 0)
            {
                baseName = baseName[..backtickIndex];
            }

            stringBuilder
                .Append(baseName)
                .Append(Grammar.ArrayStart)
                .AppendItems(type.GetGenericArguments(), (builder, x) => GetTypeName(x, builder), Grammar.ArgumentSeparator)
                .Append(Grammar.ArrayEnd);

            return false;
        }

        // Nested types: strip the declaring type prefix, keep the + separator
        // e.g. "MyType+MyNestedType" from FullName "MyNameSpace.MyType+MyNestedType"
        if (type.IsNested)
        {
            // Walk up to build "OuterType+InnerType" without namespace
            BuildNestedName(type, stringBuilder);
            return false;
        }

        // Regular type: just the simple name
        stringBuilder.Append(type.Name);
        return false;
    }

    private static void BuildNestedName(Type type, StringBuilder stringBuilder)
    {
        if (!type.DeclaringType.HasValue)
        {
            stringBuilder.Append(type.Name);
            return;
        }

        BuildNestedName(type.DeclaringType, stringBuilder);
        stringBuilder.Append('+');
        stringBuilder.Append(type.Name);
    }

    private static MemberInfo? GetTargetMemberInfo(Type sourceType, Path path)
    {
        var currentType = sourceType;
        MemberInfo? memberInfo = null;
        foreach (var segment in path.Segments)
        {
            var memberInfos = currentType.GetMember(segment.Name, MemberTypes.Method | MemberTypes.Property, BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
            var cardinality = memberInfos.ByCardinality();
            switch (cardinality)
            {
                case Empty<MemberInfo> empty:
                    break;
                case Multiple<MemberInfo> multiple:
                    var valueIds = segment.Arguments?.ToValueIds() ?? new ComplexValue([]);
                    var methodInfo = multiple.Items.OfType<MethodInfo>()
                        .Select(methodInfo => (methodInfo, parameters: methodInfo.GetParameters()))
                        .Where(x => x.parameters.Length == valueIds.Items.Count)
                        .FirstOrDefault(x => IsMatch(x.parameters, valueIds)).methodInfo;
                    memberInfo = methodInfo;
                    if (methodInfo.HasValue)
                    {
                        currentType = methodInfo.ReturnType;
                    }

                    break;
                case Single<MemberInfo> single:
                    if (single.Item is MethodInfo singleMethodInfo)
                    {
                        memberInfo = singleMethodInfo;
                        currentType = singleMethodInfo.ReturnType;
                    }
                    else if (single.Item is PropertyInfo propertyInfo)
                    {
                        memberInfo = propertyInfo;
                        currentType = propertyInfo.PropertyType;
                    }

                    break;
            }
        }

        return memberInfo;
    }

    private static bool IsMatch(ParameterInfo[] parameterInfos, ComplexValue complexValue)
    {
        if (!complexValue.HasValue)
        {
            return parameterInfos.Length == 0;
        }

        return parameterInfos.Zip(complexValue.Items).All(x =>
        {
            var argumentType = x.Second.Metadata.HasValue
                ? Source.Parse(x.Second.Metadata, CultureInfo.InvariantCulture).TryGetType().Value
                : GetTypeFromArgument(x.First.ParameterType, x.Second.Value);
            return x.First.ParameterType.IsAssignableFrom(argumentType);
        });
    }

    private static Type? GetTypeFromArgument(Type firstParameterType, IValue value)
    {
        const string parseName = "Parse";
        var parseMethod = firstParameterType.GetMethod(parseName, BindingFlags.Public | BindingFlags.Static, [typeof(string), typeof(IFormatProvider)]);
        if (parseMethod.HasValue)
        {
            return parseMethod.Invoke(null, [value.ToString(), CultureInfo.InvariantCulture])?.GetType();
        }

        parseMethod = firstParameterType.GetMethod(parseName, BindingFlags.Public | BindingFlags.Static, [typeof(string)]);
        return parseMethod?.Invoke(null, [value.ToString()])?.GetType();
    }
}
