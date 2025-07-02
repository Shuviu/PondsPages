using System.IO;
using Android.App;
using Android.Content.PM;
using Avalonia;
using Avalonia.Android;
using Avalonia.Controls.ApplicationLifetimes;
using Microsoft.Extensions.DependencyInjection;
using PondsPages.Android.Views;
using PondsPages.services;
using PondsPages.ViewModel;

namespace PondsPages.Android;

[Activity(
    Label = "PondsPages.Core.Android",
    Theme = "@style/MyTheme.NoActionBar",
    Icon = "@drawable/icon",
    MainLauncher = true,
    ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.UiMode)]
public class MainActivity : AvaloniaMainActivity<App>
{
    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
    {
        return base.CustomizeAppBuilder(builder)
            .WithInterFont()
            .AfterSetup(build =>
            {
                
                // Create the main window with the right service contexts
                var mainView = new MainView()
                {
                    DataContext = new MainViewModel(FetchConfigDirectory())
                };
                
                if (Avalonia.Application.Current?.ApplicationLifetime is ISingleViewApplicationLifetime singleViewApplication)
                {
                    singleViewApplication.MainView = mainView;
                }
            });;
    }

    /// <summary>
    /// Configures and sets up the required services for the application.
    /// </summary>
    /// <returns>
    /// A <see cref="ServiceProvider"/> instance that contains the registered services
    /// and dependencies required for the application.
    /// </returns>
    static string FetchConfigDirectory()
    {
        
        // Fetches the configuration directory for the application.
        string configDirectory = Path.Combine(global::Android.App.Application.Context.FilesDir?.AbsolutePath ?? "" , "PondsPages");
        Directory.CreateDirectory(configDirectory);

        return configDirectory;
    }
}