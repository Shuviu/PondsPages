using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace PondsPages.ViewModel;

public partial class NavBarViewModel : ViewModelBase
{
    public event Action<ViewModelBase>? OnViewChangeRequested;
    public ObservableCollection<NavEntry> LeftNavEntries { get; set; }
    public ObservableCollection<NavEntry> RightNavEntries { get; set; }

    public NavBarViewModel()
    {
        LeftNavEntries = new ObservableCollection<NavEntry>()
        {
            new("Main", new RelayCommand(() => OnViewChangeRequested?.Invoke(new MainViewModel()))),
        };
        RightNavEntries = new ObservableCollection<NavEntry>()
        {   
            new("Search", new RelayCommand((() => { }))),
            new("Sort", new RelayCommand((() => { }))),
            new("Add", new RelayCommand((() => { }))),
        };
    }
}

public class NavEntry
{
    public string Name { get; set; }
    public RelayCommand Command { get; set; }
    
    public NavEntry(string name, RelayCommand command) => (Name, Command) = (name, command);
}