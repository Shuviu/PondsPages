using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Markup.Xaml;
using PondsPages.Android.Views;
using PondsPages.ViewModel;

namespace PondsPages.Android;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }
}