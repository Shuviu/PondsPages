using System;
using System.IO;

namespace PondsPages.services;

/// <summary>
/// Provides implementation for file operations such as checking for file existence
/// and reading the contents of a file from the local file system.
/// </summary>
public class LocalFileService : IFileService
{
    /// <summary>
    /// Checks whether a file exists at the specified path in the local file system.
    /// </summary>
    /// <param name="path">The path of the file to check for existence.</param>
    /// <returns>True if the file exists; otherwise, false.</returns>
    public bool Exists(string path)
    {
        return File.Exists(path);
    }

    /// <summary>
    /// Reads all text from a file at the specified path in the local file system.
    /// </summary>
    /// <param name="path">The path of the file to read.</param>
    /// <returns>The content of the file as a string.</returns>
    /// <exception cref="ArgumentException">Thrown when the specified path does not exist.</exception>
    public string ReadAllText(string path)
    {
        if (!Exists(path))
            throw new ArgumentException("Invalid Path provided");
        return File.ReadAllText(path);
    }

    /// <summary>
    /// Writes the specified content to a file at the given path in the local file system, overwriting the file if it already exists.
    /// </summary>
    /// <param name="path">The path of the file where the content will be written.</param>
    /// <param name="content">The content to write to the file.</param>
    public void WriteAllText(string path, string content)
    {
        File.WriteAllText(path, content);
    }
}