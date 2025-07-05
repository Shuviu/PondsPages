using System;

namespace PondsPages.dataclasses;

public class Config
{
    public string Database { get; set; }
    public string ConnectionString { get; set; }
    
    public Config(string database)
    {
        this.Database = database;
        this.ConnectionString = "";
    }

    public Config() : this(""){}

    
}
