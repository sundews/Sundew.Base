// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Paths.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.IO;

using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;

/// <summary>
/// Record containing an array of file system paths.
/// Contains various methods for working with Paths.
/// </summary>
/// <param name="FileSystemPaths">The file system paths.</param>
public readonly record struct Paths(params string[] FileSystemPaths)
{
    /// <summary>
    /// Searches upwards from the current directory, to find an absolute path based on the specified relative path.
    /// </summary>
    /// <param name="path">The relative path to find.</param>
    /// <param name="foundPath">The found path.</param>
    /// <returns><c>true</c>, if the path was found, otherwise <c>false</c>.</returns>
    public static bool TryFindPathUpwards(string path, [NotNullWhen(true)] out string? foundPath)
    {
        return TryFindPathUpwardsFrom(Directory.GetCurrentDirectory(), path, out foundPath);
    }

    /// <summary>
    /// Searches upwards from the entry assembly path, to find an absolute path based on the specified relative path.
    /// </summary>
    /// <param name="path">The relative path to find.</param>
    /// <param name="foundPath">The found path.</param>
    /// <returns><c>true</c>, if the path was found, otherwise <c>false</c>.</returns>
    public static bool TryFindPathUpwardsFromEntryAssembly(string path, [NotNullWhen(true)] out string? foundPath)
    {
        var assembly = Assembly.GetEntryAssembly();
        if (assembly == null)
        {
            foundPath = null;
            return false;
        }

        var basePath = Directory.GetParent(assembly.Location);
        if (basePath == null)
        {
            foundPath = null;
            return false;
        }

        return TryFindPathUpwardsFrom(basePath.FullName, path, out foundPath);
    }

    /// <summary>
    /// Searches upwards from the specified base path, to find an absolute path based on the specified relative path.
    /// </summary>
    /// <param name="basePath">The base path to start searching from.</param>
    /// <param name="path">The relative path to find.</param>
    /// <param name="foundPath">The found path.</param>
    /// <returns><c>true</c>, if the path was found, otherwise <c>false</c>.</returns>
    public static bool TryFindPathUpwardsFrom(string basePath, string path, [NotNullWhen(true)] out string? foundPath)
    {
        if (Path.IsPathRooted(path))
        {
            foundPath = null;
            return false;
        }

        var currentPath = basePath;
        foundPath = Path.Combine(currentPath, path);
        while (foundPath != null && !Exists(foundPath))
        {
            if (currentPath == null)
            {
                return false;
            }

            currentPath = Directory.GetParent(currentPath)?.FullName;
            foundPath = currentPath != null ? Path.Combine(currentPath, path) : null;
        }

        return !string.IsNullOrEmpty(foundPath);
    }

    /// <summary>
    /// Searches upwards from the specified base path, to find an absolute path based on the specified relative path.
    /// </summary>
    /// <param name="basePath">The base path to start searching from.</param>
    /// <param name="path">The relative path to find.</param>
    /// <returns>The path if found otherwise none/null.</returns>
    public static string? FindPathUpwardsFrom(string basePath, string path)
    {
        if (TryFindPathUpwardsFrom(basePath, path, out var foundPath))
        {
            return foundPath;
        }

        return null;
    }

    /// <summary>
    /// Searches upwards from the entry assembly, to find an absolute path based on the specified relative path.
    /// </summary>
    /// <param name="path">The relative path to find.</param>
    /// <returns>The path if found otherwise none/null.</returns>
    public static string? FindPathUpwardsFromEntryAssembly(string path)
    {
        if (TryFindPathUpwardsFromEntryAssembly(path, out var foundPath))
        {
            return foundPath;
        }

        return null;
    }

    /// <summary>
    /// Searches upwards from the current directory, to find an absolute path based on the specified relative path.
    /// </summary>
    /// <param name="path">The relative path to find.</param>
    /// <returns>The path if found otherwise none/null.</returns>
    public static string? FindPathUpwards(string path)
    {
        if (TryFindPathUpwards(path, out var foundPath))
        {
            return foundPath;
        }

        return null;
    }

    /// <summary>
    /// A value indicating whether the path exists.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <returns><c>true</c>, if the path exists, otherwise <c>false</c>.</returns>
    public static bool Exists(string path)
    {
        return Directory.Exists(path) || File.Exists(path);
    }
}
