using System;

namespace PondsPages.dataclasses;

/// <summary>
/// Represents the data structure for the configuration of the app.
/// </summary>
public class Config
{
    /// <summary>
    /// Determines which database configuration to load.
    /// </summary>
    public string Database { get; set; }

    /// <summary>
    /// Represents the connection string used to connect to the database.
    /// This value is determined based on the database configuration specified.
    /// </summary>
    public string ConnectionString { get; set; }
    
    // ---- Constructors ---- //
    public Config(string database)
    {
        this.Database = database;
        this.ConnectionString = "";
    }

    public Config() : this(""){}

    
}
