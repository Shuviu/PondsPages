using System;
using System.IO;
using System.Text.Json;
using PondsPages.dataclasses;

namespace PondsPages.services;

public class ConfigService : IConfigService
{
    private readonly string _configPath;
    private readonly string _dbConfigPath;
    private readonly IFileService _fileService;

    public ConfigService(string baseDir, IFileService fileService)
    {
        _configPath = Path.Combine(baseDir, "config.json");
        _dbConfigPath = Path.Combine(baseDir, "db.json");
        _fileService = fileService;
    }
    
    public Config LoadConfig()
    {
        if (!_fileService.Exists(_configPath))
        {
            throw new Exception("Config file does not exist.");
        }
        if (!_fileService.Exists(_dbConfigPath))
        {
            throw new Exception("Database config file does not exist.");
        }
        
        Config config = ParseJsonToConfig(_fileService.ReadAllText(_configPath));
        FetchDbData(ref config);
        
        return config;
    }
    
    public void SaveConfig()
    {
        throw new NotImplementedException();
    }

    public static Config ParseJsonToConfig(string json)
    {
        Config config = JsonSerializer.Deserialize<Config>(json) ?? throw new Exception("Failed to parse config file.");
        return config;;
    }
    
    public void FetchDbData(ref Config config)
    {
        string connectionString;
        
        string fileText = _fileService.ReadAllText(_dbConfigPath);
        JsonDocument jsonDoc = JsonDocument.Parse(fileText);
        switch (config.Database)
        {
            case "local":
                if (!jsonDoc.RootElement.TryGetProperty("local", out JsonElement localDb))
                    throw new Exception("Local database configuration not found in db.json.");
                connectionString = FetchLocalDbData(localDb);
                break;
            case "remote":
                if (jsonDoc.RootElement.TryGetProperty("remote", out JsonElement remoteDb))
                    throw new Exception("Remote database configuration not found in db.json.");
                connectionString = FetchRemoteDbData(remoteDb);
                break;
            default:
                throw new Exception("Invalid database type.");
        }
        
        config.ConnectionString = connectionString;
    }

    private string FetchLocalDbData(JsonElement localDb)
    {
        if (!localDb.TryGetProperty("PathToDb", out JsonElement connectionString))
            throw new Exception("Path to database not found in local database configuration.");
        return connectionString.GetString() ?? "";
    }

    private string FetchRemoteDbData(JsonElement remoteDb)
    {
        throw new NotImplementedException();
    }
}