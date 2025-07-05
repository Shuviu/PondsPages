using System;
using System.IO;

namespace PondsPages.services;

public class LocalFileService : IFileService
{
    public bool Exists(string path)
    {
        return File.Exists(path);
    }

    public string ReadAllText(string path)
    {
        if (!Exists(path))
            throw new ArgumentException("Invalid Path provided");
        return File.ReadAllText(path);
    }
}