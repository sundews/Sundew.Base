// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExpressionEvaluator.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Identification;

using System;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Sundew.Base.Collections.Immutable;

/// <summary>
/// Provides static methods for evaluating mathematical and logical expressions represented as strings.
/// </summary>
internal static class ExpressionEvaluator
{
    /// <summary>
    /// Gets a <see cref="Path"/> for the specified expression.
    /// </summary>
    /// <param name="pathExpression">The path expression.</param>
    /// <param name="valueId">The value id.</param>
    /// <returns>A new <see cref="Path"/>.</returns>
    public static (Source Source, Path Path, Arguments? Arguments) From(LambdaExpression pathExpression, ValueId? valueId = null)
    {
        var isUsed = false;
        var segments = ImmutableArray.CreateBuilder<Segment>();
        var source = Source.FromType(pathExpression.Parameters.First().Type);
        var valueIds = ImmutableArray.CreateBuilder<Argument>();
        EvaluateToPath(pathExpression);

        return (source, new Path(segments.ToImmutable()), !valueId.HasValue || isUsed || !valueId.HasValue ? null : new Arguments(ValueArray<Argument>.Empty.Add(new Argument(null, valueId))));

        void EvaluateToPath(Expression expression)
        {
            switch (expression)
            {
                case LambdaExpression lambdaExpression:
                    EvaluateToPath(lambdaExpression.Body);
                    break;
                case MethodCallExpression methodCallExpression:
                    if (methodCallExpression.Object.HasValue)
                    {
                        EvaluateToPath(methodCallExpression.Object);
                    }

                    valueIds = ImmutableArray.CreateBuilder<Argument>();
                    var parameterInfos = methodCallExpression.Method.GetParameters();
                    foreach (var argument in methodCallExpression.Arguments.Zip(parameterInfos))
                    {
                        if (argument.First is MethodCallExpression argumentMethodCallExpression && argumentMethodCallExpression.Method.DeclaringType == typeof(Id) && argumentMethodCallExpression.Method.Name == nameof(Id.Argument) && valueId.HasValue)
                        {
                            valueIds.Add(new Argument(argument.Second.Name, valueId));
                            isUsed = true;
                        }
                        else
                        {
                            GetArgument(argument.First, argument.Second, valueIds);
                        }
                    }

                    segments.Add(new Segment(methodCallExpression.Method.Name, new Arguments(valueIds.ToValueArray())));

                    break;
                case MemberExpression memberExpression:
                    if (memberExpression.Expression.HasValue)
                    {
                        EvaluateToPath(memberExpression.Expression);
                    }

                    segments.Add(new Segment(memberExpression.Member.Name));
                    break;
                case UnaryExpression unaryExpression:
                    EvaluateToPath(unaryExpression.Operand);
                    break;
            }
        }
    }

    private static void GetArgument(Expression argument, ParameterInfo parameterInfo, ImmutableArray<Argument>.Builder builder)
    {
        switch (argument)
        {
            case ConstantExpression constantExpression:
                builder.Add(new Argument(parameterInfo.Name, new ValueId(GetMetadata(argument.Type), new ScalarValue(constantExpression.Value?.ToString() ?? (argument.Type.IsClass ? "null" : "default")))));
                break;
            case MemberExpression memberExpression:
                if (memberExpression.Expression is ConstantExpression constantExpression2)
                {
                    var container = constantExpression2.Value;
                    if (memberExpression.Member is FieldInfo fieldInfo)
                    {
                        var value = fieldInfo.GetValue(container);
                        builder.Add(new Argument(fieldInfo.Name, new ValueId(GetMetadata(fieldInfo.FieldType), new ScalarValue(value?.ToString() ?? string.Empty))));
                    }

                    if (memberExpression.Member is PropertyInfo propertyInfo)
                    {
                        var value = propertyInfo.GetValue(container);
                        builder.Add(new Argument(propertyInfo.Name, new ValueId(GetMetadata(propertyInfo.PropertyType), new ScalarValue(value?.ToString() ?? string.Empty))));
                    }
                }

                break;
            case NewExpression newExpression:
                ImmutableArray<Argument>.Builder newBuilder = ImmutableArray.CreateBuilder<Argument>();
                if (newExpression.Constructor.HasValue)
                {
                    foreach (var valueTuple in newExpression.Arguments.Zip(newExpression.Constructor.GetParameters()))
                    {
                        GetArgument(valueTuple.First, valueTuple.Second, newBuilder);
                    }

                    builder.Add(new Argument(parameterInfo.Name, new ValueId(GetMetadata(argument.Type), new ComplexValue(newBuilder.ToImmutable()))));
                }

                break;
        }
    }

    private static string? GetMetadata(Type argumentType)
    {
        return TargetEvaluator.IsKnownType(argumentType) ? null : Source.FromType(argumentType).ToString();
    }
}