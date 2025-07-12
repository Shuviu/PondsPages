using Xunit;
using Moq;
using System;
using System.IO;
using PondsPages.dataclasses; // Assuming your Config class is here
using PondsPages.services;
using JsonException = System.Text.Json.JsonException;
namespace PondsPages.Core.Tests.services;

public class ConfigServiceTests
{
    // A dummy base directory for tests. The actual path doesn't matter
    // because we are mocking file system operations.
    private readonly string _baseDir = "/app/testdata";

    // --- Tests for LoadConfig() method ---

    [Fact]
    public void LoadConfig_ConfigAndDbFilesExist_LocalDb_ReturnsConfigWithCorrectConnectionString()
    {
        // Arrange
        var mockFileService = new Mock<IFileService>();
        var configJsonContent = @"{""Database"": ""local""}"; // Config.Database will be "local"
        var dbJsonContent = @"{""local"": {""PathToDb"": ""testPath""}, ""remote"": {}}";

        // Set up mock to simulate files existing and returning specific content
        mockFileService.Setup(f => f.Exists(Path.Combine(_baseDir, "config.json"))).Returns(true);
        mockFileService.Setup(f => f.Exists(Path.Combine(_baseDir, "db.json"))).Returns(true);
        mockFileService.Setup(f => f.ReadAllText(Path.Combine(_baseDir, "config.json"))).Returns(configJsonContent);
        mockFileService.Setup(f => f.ReadAllText(Path.Combine(_baseDir, "db.json"))).Returns(dbJsonContent);

        var service = new ConfigService(_baseDir, mockFileService.Object);

        // Act
        Config result = service.LoadConfig();

        // Assert
        Assert.NotNull(result);
        Assert.Equal("local", result.Database);
        Assert.Equal("Data Source=testPath", result.ConnectionString);
    }

    [Fact]
    public void LoadConfig_ConfigFileNotFound_ThrowsException()
    {
        // Arrange
        var mockFileService = new Mock<IFileService>();
        var dbJsonContent = @"{""local"": {""PathToDb"": ""testPath;""}, ""remote"": {}}";

        // Simulate config.json not existing
        mockFileService.Setup(f => f.Exists(Path.Combine(_baseDir, "config.json"))).Returns(false);
        mockFileService.Setup(f => f.Exists(Path.Combine(_baseDir, "db.json"))).Returns(true);
        mockFileService.Setup(f => f.ReadAllText(Path.Combine(_baseDir, "db.json"))).Returns(dbJsonContent);


        var service = new ConfigService(_baseDir, mockFileService.Object);

        // Act & Assert
        var exception = Assert.Throws<Exception>(() => service.LoadConfig());
        Assert.Equal("Config file does not exist.", exception.Message);
    }

    [Fact]
    public void LoadConfig_DbFileNotFound_ThrowsException()
    {
        // Arrange
        var mockFileService = new Mock<IFileService>();
        var configJsonContent = @"{""Database"": ""local""}";

        // Simulate config.json existing, but db.json not existing
        mockFileService.Setup(f => f.Exists(Path.Combine(_baseDir, "config.json"))).Returns(true);
        mockFileService.Setup(f => f.Exists(Path.Combine(_baseDir, "db.json"))).Returns(false);
        mockFileService.Setup(f => f.ReadAllText(Path.Combine(_baseDir, "config.json"))).Returns(configJsonContent);

        var service = new ConfigService(_baseDir, mockFileService.Object);

        // Act & Assert
        var exception = Assert.Throws<Exception>(() => service.LoadConfig());
        Assert.Equal("Database config file does not exist.", exception.Message);
    }

    [Fact]
    public void LoadConfig_ConfigJsonInvalid_ThrowsJsonException()
    {
        // Arrange
        var mockFileService = new Mock<IFileService>();
        var invalidConfigJsonContent = @"{""Database"": ""local"","; // Malformed JSON
        var dbJsonContent = @"{""local"": {""PathToDb"": ""Data Source=test.db;""}}";

        mockFileService.Setup(f => f.Exists(Path.Combine(_baseDir, "config.json"))).Returns(true);
        mockFileService.Setup(f => f.Exists(Path.Combine(_baseDir, "db.json"))).Returns(true);
        mockFileService.Setup(f => f.ReadAllText(Path.Combine(_baseDir, "config.json"))).Returns(invalidConfigJsonContent);
        mockFileService.Setup(f => f.ReadAllText(Path.Combine(_baseDir, "db.json"))).Returns(dbJsonContent);

        var service = new ConfigService(_baseDir, mockFileService.Object);

        // Act & Assert
        Assert.Throws<JsonException>(() => service.LoadConfig());
    }
    
    [Fact]
    public void LoadConfig_DatabaseTypeLocal_MissingLocalPropertyInDbJson_ThrowsException()
    {
        // Arrange
        var mockFileService = new Mock<IFileService>();
        var configJsonContent = @"{""Database"": ""local""}";
        var dbJsonContent = @"{""remote"": {}}"; // 'local' property is missing

        mockFileService.Setup(f => f.Exists(Path.Combine(_baseDir, "config.json"))).Returns(true);
        mockFileService.Setup(f => f.Exists(Path.Combine(_baseDir, "db.json"))).Returns(true);
        mockFileService.Setup(f => f.ReadAllText(Path.Combine(_baseDir, "config.json"))).Returns(configJsonContent);
        mockFileService.Setup(f => f.ReadAllText(Path.Combine(_baseDir, "db.json"))).Returns(dbJsonContent);

        var service = new ConfigService(_baseDir, mockFileService.Object);

        // Act & Assert
        var exception = Assert.Throws<Exception>(() => service.LoadConfig());
        Assert.Equal("Local database configuration not found in db.json.", exception.Message);
    }

    [Fact]
    public void LoadConfig_DatabaseTypeLocal_LocalPropertyExists_MissingPathToDb_ReturnsConfigWithEmptyConnectionString()
    {
        // Arrange
        var mockFileService = new Mock<IFileService>();
        var configJsonContent = @"{""Database"": ""local""}";
        var dbJsonContent = @"{""local"": {""someOtherKey"": ""value""}}"; // PathToDb is missing

        mockFileService.Setup(f => f.Exists(Path.Combine(_baseDir, "config.json"))).Returns(true);
        mockFileService.Setup(f => f.Exists(Path.Combine(_baseDir, "db.json"))).Returns(true);
        mockFileService.Setup(f => f.ReadAllText(Path.Combine(_baseDir, "config.json"))).Returns(configJsonContent);
        mockFileService.Setup(f => f.ReadAllText(Path.Combine(_baseDir, "db.json"))).Returns(dbJsonContent);

        var service = new ConfigService(_baseDir, mockFileService.Object);

        // Act
        var exception = Assert.Throws<Exception>(() => service.LoadConfig());
        Assert.Equal("Path to database not found in local database configuration.", exception.Message);
    }

    [Fact]
    public void LoadConfig_InvalidDatabaseTypeInConfig_ThrowsException()
    {
        // Arrange
        var mockFileService = new Mock<IFileService>();
        var configJsonContent = @"{""Database"": ""invalid_type""}"; // Invalid database type
        var dbJsonContent = @"{""local"": {""PathToDb"": ""Data Source=test.db;""}}";

        mockFileService.Setup(f => f.Exists(Path.Combine(_baseDir, "config.json"))).Returns(true);
        mockFileService.Setup(f => f.Exists(Path.Combine(_baseDir, "db.json"))).Returns(true);
        mockFileService.Setup(f => f.ReadAllText(Path.Combine(_baseDir, "config.json"))).Returns(configJsonContent);
        mockFileService.Setup(f => f.ReadAllText(Path.Combine(_baseDir, "db.json"))).Returns(dbJsonContent);

        var service = new ConfigService(_baseDir, mockFileService.Object);

        // Act & Assert
        var exception = Assert.Throws<Exception>(() => service.LoadConfig());
        Assert.Equal("Invalid database type.", exception.Message); // Message from your switch default
    }

    // --- Tests for ParseJsonToConfig(string json) static method ---

    [Fact]
    public void ParseJsonToConfig_ValidJson_ReturnsCorrectConfigObject()
    {
        // Arrange
        var json = @"{""Database"": ""test"", ""ConnectionString"": ""test_conn""}"; 

        // Act
        Config result = ConfigService.ParseJsonToConfig(json);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("test", result.Database);
        Assert.Equal("test_conn", result.ConnectionString);
    }

    [Fact]
    public void ParseJsonToConfig_InvalidJson_ThrowsJsonException()
    {
        // Arrange
        var invalidJson = @"{""Database"": ""test"""; // Malformed JSON

        // Act & Assert
        Assert.Throws<JsonException>(() => ConfigService.ParseJsonToConfig(invalidJson));
    }

    [Fact]
    public void ParseJsonToConfig_EmptyJson_ThrowsJsonException()
    {
        // Arrange
        var emptyJson = "";

        // Act & Assert
        Assert.Throws<JsonException>(() => ConfigService.ParseJsonToConfig(emptyJson));
    }

    [Fact]
    public void ParseJsonToConfig_JsonWithNullRoot_ThrowsException()
    {
        // Arrange
        var jsonWithNullRoot = "null";

        // Act & Assert
        var exception = Assert.Throws<Exception>(() => ConfigService.ParseJsonToConfig(jsonWithNullRoot));
        Assert.Equal("Failed to parse config file.", exception.Message);
    }

    // --- Test for SaveConfig() method (NotImplementedException) ---

    [Fact]
    public void SaveConfig_ThrowsNotImplementedException()
    {
        // Arrange
        var mockFileService = new Mock<IFileService>();
        var service = new ConfigService(_baseDir, mockFileService.Object);

        // Act & Assert
        Assert.Throws<NotImplementedException>(() => service.SaveConfig());
    }
}
