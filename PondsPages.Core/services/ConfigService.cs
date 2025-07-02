using System;
using PondsPages.dataclasses;

namespace PondsPages.services;

public class ConfigService : IConfigService
{
    private readonly string _configPath;

    public ConfigService(string baseDir)
    {
        _configPath = baseDir + "/config.json";
    }
    
    public Config LoadConfig()
    {
        Console.WriteLine("Loading config from: " + _configPath + "");
        throw new NotImplementedException();
    }
    
    public void SaveConfig()
    {
        throw new NotImplementedException();
    }
}