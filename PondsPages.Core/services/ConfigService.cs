using System;
using System.IO;
using System.Text.Json;
using PondsPages.dataclasses;

namespace PondsPages.services;

/// <summary>
/// Provides configuration services for handling application settings and database paths.
/// </summary>
public class ConfigService : IConfigService
{
    /// <summary>
    /// The file path to the main configuration file (config.json) that contains
    /// application-specific settings. This path is dynamically generated based on the provided base directory.
    /// </summary>
    private readonly string _configPath;

    /// <summary>
    /// The file path to the database configuration file (db.json).
    /// This path is constructed based on the provided base directory.
    /// </summary>
    private readonly string _dbConfigPath;
    
    /// <summary>
    /// The file service used to access the file system.
    /// </summary>
    private readonly IFileService _fileService;

    /// <summary>
    /// Provides methods to handle configuration operations for the application, including
    /// reading and processing configuration and database files.
    /// </summary>
    /// <param name="baseDir">The base directory for the config files.</param>
    /// <param name="fileService">The file service used to access the file system.</param>
    public ConfigService(string baseDir, IFileService fileService)
    {
        _configPath = Path.Combine(baseDir, "config.json");
        _dbConfigPath = Path.Combine(baseDir, "db.json");
        _fileService = fileService;
    }

    /// <summary>
    /// Loads the application configuration by reading and parsing the necessary configuration
    /// files and combining their data into a Config object.
    /// </summary>
    /// <returns>A Config object containing the application configuration settings.</returns>
    /// <exception cref="Exception">Thrown when the configuration file or database configuration file does not exist.</exception>
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
        // Adds the database config to the config object.
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
        return config;
    }

    /// <summary>
    /// Fetches database configuration details and updates the provided config object
    /// with the appropriate connection string based on the selected database type.
    /// </summary>
    /// <param name="config">A reference to the configuration object that will be updated </param>
    /// <exception cref="Exception">
    /// Thrown when the database configuration is missing or invalid for the specified database type.
    /// </exception>
    public void FetchDbData(ref Config config)
    {
        string connectionString;

        // Read the db config and parse
        string fileText = _fileService.ReadAllText(_dbConfigPath);
        JsonDocument jsonDoc = JsonDocument.Parse(fileText);
        
        // decide which database to use based on the config
        switch (config.Database)
        {
            case "local":
                if (!jsonDoc.RootElement.TryGetProperty("local", out JsonElement localDb))
                    throw new Exception("Local database configuration not found in db.json.");
                connectionString = CreateLocalConnString(localDb);
                break;
            case "remote":
                if (jsonDoc.RootElement.TryGetProperty("remote", out JsonElement remoteDb))
                    throw new Exception("Remote database configuration not found in db.json.");
                connectionString = CreateRemoteConnString(remoteDb);
                break;
            default:
                throw new Exception("Invalid database type.");
        }
        
        config.ConnectionString = connectionString;
    }

    private string CreateLocalConnString(JsonElement localDb)
    {
        if (!localDb.TryGetProperty("PathToDb", out JsonElement connectionString))
            throw new Exception("Path to database not found in local database configuration.");
        return connectionString.GetString() ?? "";
    }

    private string CreateRemoteConnString(JsonElement remoteDb)
    {
        throw new NotImplementedException();
    }
}