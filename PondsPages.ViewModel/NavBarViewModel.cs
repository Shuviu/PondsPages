using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace PondsPages.ViewModel;

public partial class NavBarViewModel : ViewModelBase
{
    public event Action<ViewModelBase>? OnViewChangeRequested;
    public ObservableCollection<NavEntry> NavEntries { get; set; }
    [ObservableProperty]
    private string _greeting = "Welcome to Avalonia!";

    public NavBarViewModel()
    {
        NavEntries = new ObservableCollection<NavEntry>()
        {
            new("Main", new RelayCommand(() => OnViewChangeRequested?.Invoke(new MainViewModel()))),
            new("Library", new RelayCommand((() => {})))
        };
    }
}

public class NavEntry
{
    public string Name { get; set; }
    public RelayCommand Command { get; set; }
    
    public NavEntry(string name, RelayCommand command) => (Name, Command) = (name, command);
}