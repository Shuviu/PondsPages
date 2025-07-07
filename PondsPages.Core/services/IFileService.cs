namespace PondsPages.services;

/// <summary>
/// Defines methods for interacting with the file system.
/// </summary>
public interface IFileService
{
    public bool Exists(string path);
    public string ReadAllText(string path);
}