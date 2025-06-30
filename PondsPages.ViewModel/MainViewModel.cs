using CommunityToolkit.Mvvm.ComponentModel;
using PondsPages.dataclasses;

namespace PondsPages.ViewModel;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty] private ViewModelBase _currView;
    [ObservableProperty] private ViewModelBase _navBarView;

    public MainViewModel()
    {
        CurrView = new BookListViewModel();
        NavBarViewModel navbar = new NavBarViewModel();
        navbar.OnViewChangeRequested += HandleViewChangeRequest;
        NavBarView = navbar;
    }
    public void HandleBookSelection(object? sender, Book b)
    {
        CurrView = new BookDetailViewModel(b);
    }

    public void HandleViewChangeRequest(object? sender, ViewModelBase? e)
    {
        if (CurrView is not BookListViewModel)
        {
            CurrView = new BookListViewModel();
        }
    }

    partial void OnCurrViewChanged(ViewModelBase? oldValue, ViewModelBase? newValue)
    {
        if (oldValue is BookListViewModel oldBookListViewModel)
        {
            oldBookListViewModel.OnBookSelected -= HandleBookSelection;
        }

        else if (newValue is BookListViewModel newBookListViewModel)
        {
            newBookListViewModel.OnBookSelected += HandleBookSelection;
        }
    }
}