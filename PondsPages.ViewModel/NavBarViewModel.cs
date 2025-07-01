using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace PondsPages.ViewModel;

public partial class NavBarViewModel : ViewModelBase
{
    /// <summary>
    /// Raised to request a change of the current view in the navigation system.
    /// </summary>
    public event EventHandler<ViewModelBase>? OnViewChangeRequested;

    /// <summary>
    /// Represents a collection of navigation entry <see cref="NavEntry"/> on the left side
    /// of the navigation bar.
    /// </summary>
    public ObservableCollection<NavEntry> LeftNavEntries { get; set; }

    /// <summary>
    /// Represents a collection of navigation entries <see cref="NavEntry"/> displayed on the right side of the navigation bar.
    /// </summary>
    public ObservableCollection<NavEntry> RightNavEntries { get; set; }

    // ---- Constructors ---- //

    /// <summary>
    /// Represents a view model for a navigation bar, containing collections for left and right navigation entries.
    /// Manages view-related commands and notifies subscribers when a view change request is triggered.
    /// </summary>
    public NavBarViewModel()
    {
        LeftNavEntries = new ObservableCollection<NavEntry>()
        {
            new("Main", new RelayCommand(() => OnViewChangeRequested?.Invoke(this, new BookListViewModel())))
        };
        RightNavEntries = new ObservableCollection<NavEntry>()
        {   
            new("Search", new RelayCommand((() => { }))),
            new("Sort", new RelayCommand((() => { }))),
            new("Add", new RelayCommand((() => { }))),
        };
    }
}

/// <summary>
/// Represents a single navigation item in a navigation bar.
/// </summary>
public class NavEntry
{
    public string Name { get; set; }
    public RelayCommand Command { get; set; }
    
    public NavEntry(string name, RelayCommand command) => (Name, Command) = (name, command);
}