// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TargetEvaluator.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Identification;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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

    public static R<IReadOnlyList<Type>> GetInputTypes(Source source, Path? path)
    {
        var sourceType = source.TryGetType();
        if (sourceType.IsError)
        {
            return R.Error();
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

    internal static string? GetTypeName(Type type)
    {
        if (type.IsArray)
        {
            var elementType = type.GetElementType()!;
            var rank = type.GetArrayRank();
            var commas = rank > 1 ? new string(',', rank - 1) : string.Empty;
            return $"{GetTypeName(elementType)}[{commas}]";
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

            var genericParameters = type.GetGenericArguments().Select(GetTypeName);

            return $"{baseName}[{string.Join(',', genericParameters)}]";
        }

        if (PrimitiveAliases.TryGetValue(type, out var alias))
        {
            return alias;
        }

        // Nested types: strip the declaring type prefix, keep the + separator
        // e.g. "MyType+MyNestedType" from FullName "MyNameSpace.MyType+MyNestedType"
        if (type.IsNested)
        {
            // Walk up to build "OuterType+InnerType" without namespace
            return BuildNestedName(type);
        }

        // Regular type: just the simple name
        return type.Name;
    }

    private static MemberInfo? GetTargetMemberInfo(Type sourceType, Path path)
    {
        var currentType = sourceType;
        MemberInfo? memberInfo = null;
        foreach (var segment in path.Segments)
        {
            var segmentType = GetSegment(segment);
            var memberInfos = currentType.GetMember(segmentType.Name, MemberTypes.Method | MemberTypes.Property, BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
            if (segmentType.IsProperty)
            {
                var propertyInfo = memberInfos.OfType<PropertyInfo>().FirstOrDefault();
                memberInfo = propertyInfo;
                if (propertyInfo.HasValue)
                {
                    currentType = propertyInfo.PropertyType;
                }
            }
            else
            {
                var methodInfo = memberInfos.OfType<MethodInfo>().FirstOrDefault(x => x.GetParameters().Select(x => GetTypeName(x.ParameterType)).SequenceEqual(segmentType.ParameterNames));
                memberInfo = methodInfo;
                if (methodInfo.HasValue)
                {
                    currentType = methodInfo.ReturnType;
                }
            }
        }

        return memberInfo;
    }

    private static (string Name, bool IsProperty, IReadOnlyList<string> ParameterNames) GetSegment(string segment)
    {
        if (segment.EndsWith(')'))
        {
            var parametersStartIndex = segment.IndexOf('(');
            return (segment.Substring(0, parametersStartIndex), false, segment.Substring(parametersStartIndex + 1, segment.Length - parametersStartIndex - 2).Split(',', StringSplitOptions.RemoveEmptyEntries));
        }

        return (segment, true, Array.Empty<string>());
    }

    private static string BuildNestedName(Type type)
    {
        if (!type.DeclaringType.HasValue)
        {
            return type.Name;
        }

        return $"{BuildNestedName(type.DeclaringType)}+{type.Name}";
    }
}
