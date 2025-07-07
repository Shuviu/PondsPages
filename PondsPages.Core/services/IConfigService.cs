using PondsPages.dataclasses;

namespace PondsPages.services;

/// <summary>
/// Defines methods for handling configuration loading and saving within the application.
/// </summary>
public interface IConfigService
{
    /// <summary>
    /// Loads the configuration for the application.
    /// </summary>
    /// <returns>A <see cref="Config"/> instance containing the application's configuration data.</returns>
    public Config LoadConfig();

    /// <summary>
    /// Saves the current configuration of the application to disk.
    /// </summary>
    public void SaveConfig();
}