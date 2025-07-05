namespace PondsPages.services;

public interface IFileService
{
    public bool Exists(string path);
    public string ReadAllText(string path);
}