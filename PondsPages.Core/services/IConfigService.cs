using PondsPages.dataclasses;

namespace PondsPages.services;

public interface IConfigService
{
    public Config LoadConfig();
    public void SaveConfig();
}