﻿using CommunityToolkit.Mvvm.ComponentModel;
using PondsPages.dataclasses;
using PondsPages.services;
using PondsPages.services.database;

namespace PondsPages.ViewModel;

public partial class MainViewModel : ViewModelBase
{
    /// <summary>
    /// Represents the current view's ViewModel in the application.
    /// </summary>
    [ObservableProperty] private ViewModelBase _currView;

    /// <summary>
    /// Represents the ViewModel for the navigation bar in the application.
    /// </summary>
    [ObservableProperty] private ViewModelBase _navBarView;

    /// <summary>
    /// Represents the currently implemented ConfigService
    /// </summary>
    private readonly Config _currConfig;
    private readonly IDatabaseService _databaseService;
    
    // ---- Constructors ---- //
    
    /// <summary>
    /// Represents the main view model for the application. This serves as the central point
    /// for coordinating navigation and managing the current view in the application.
    /// </summary>
    public MainViewModel(string configBaseDir)
    {
        // Load the config service and its config data.
        IConfigService configService = new ConfigService(configBaseDir, new LocalFileService());
        _currConfig = configService.LoadConfig();
        
        // Load the database service based on the selected database type.
        switch (_currConfig.Database)
        {
            case "remote":
                throw new NotImplementedException();
            default:
                _databaseService = new SqliteDatabaseService(_currConfig.ConnectionString);
                break;
        }
        
        _databaseService.InitializeDatabase();
        NavBarViewModel navbar = new NavBarViewModel(_databaseService);
        navbar.OnViewChangeRequested += HandleViewChangeRequest;
        NavBarView = navbar;
        CurrView = new BookListViewModel(_databaseService.GetAllBooks());
    }
    
    /// <summary>
    /// Represents the main view model for the application. This serves as the central point
    /// for coordinating navigation and managing the current view in the application.
    /// Default constructor. Used for testing purposes.
    /// </summary>
    public MainViewModel() : this(""){}
    
    // ---- Event Handlers ---- //

    /// <summary>
    /// Handles the selection of a book by updating the current view to display book details.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="b">The selected book object.</param>
    public void HandleBookSelection(object? sender, Book b)
    {
        CurrView = new BookDetailViewModel(b);
    }

    /// <summary>
    /// Handles the view change request initiated from the navigation bar.
    /// This method updates the current view model based on the requested change.
    /// </summary>
    /// <param name="sender">The source object that triggered the view change event.</param>
    /// <param name="e">The new view model to switch to, or null if not specified.</param>
    public void HandleViewChangeRequest(object? sender, ViewModelBase? e)
    {
        if (e is null) return;

        CurrView = e;
    }

    /// <summary>
    /// Handles logic when the current view model is changed. This method manages the subscription
    /// or unsubscription of events based on the old and new view models of type <see cref="ViewModelBase"/>.
    /// </summary>
    /// <param name="oldValue">The previously active view model.</param>
    /// <param name="newValue">The newly active view model.</param>
    partial void OnCurrViewChanged(ViewModelBase? oldValue, ViewModelBase newValue)
    {
        if (oldValue is BookListViewModel oldBookListViewModel)
        {
            oldBookListViewModel.OnBookSelected -= HandleBookSelection;
        }
        if (newValue is BookListViewModel newBookListViewModel)
        {
            newBookListViewModel.OnBookSelected += HandleBookSelection;
        }
    }
}