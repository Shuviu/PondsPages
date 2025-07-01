using System;

namespace PondsPages.services;

public class ConfigService
{
    private string _configPath { get; set; }

    public ConfigService(string baseDir)
    {
        _configPath = baseDir + "/config.json";
    }
    
    public void LoadConfig()
    {
        Console.WriteLine("Loading config from: " + _configPath + "");
    }
    
    public void SaveConfig()
    {
        
    }
}