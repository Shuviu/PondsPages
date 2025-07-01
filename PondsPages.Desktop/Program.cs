using System;
using System.ComponentModel.Design;
using System.IO;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Microsoft.Extensions.DependencyInjection;
using PondsPages.dataclasses;
using PondsPages.Desktop.Views;
using PondsPages.services;
using PondsPages.ViewModel;

namespace PondsPages.Desktop;

sealed class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace()
            .AfterSetup(build =>
            {
                ServiceProvider serviceProvider = SetupServices();
                
                // Create the main window with the right service contexts
                var mainWindow = new MainWindow()
                {
                    DataContext = serviceProvider.GetRequiredService<MainViewModel>()
                };
                
                if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                {
                    desktop.MainWindow = mainWindow;
                }
            });

    /// <summary>
    /// Configures and sets up the required services for the application.
    /// </summary>
    /// <returns>
    /// A <see cref="ServiceProvider"/> instance that contains the registered services
    /// and dependencies required for the application.
    /// </returns>
    static ServiceProvider SetupServices()
    {
        ServiceCollection serviceCollection = new();
        
        // Fetches the configuration directory for the application.
        string configDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PondsPages");
        Directory.CreateDirectory(configDirectory);

        // Add the required services to the service collection.
        serviceCollection.AddSingleton<ConfigService>(provider => new ConfigService(configDirectory));

        // Add the required viewmodels to the service collection.
        serviceCollection.AddSingleton<MainViewModel>();
                
        return serviceCollection.BuildServiceProvider();
    }

}