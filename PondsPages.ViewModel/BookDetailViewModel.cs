using CommunityToolkit.Mvvm.ComponentModel;
using PondsPages.dataclasses;

namespace PondsPages.ViewModel;

public partial class BookDetailViewModel : ViewModelBase
{
    /// <summary>
    /// Backing field for the book information being handled within the view model.
    /// </summary>
    [ObservableProperty] private Book _book;

    
    // ---- Constructors ---- //
    
    /// <summary>
    /// Provides a view model for managing the details of a book.
    /// </summary>
    public BookDetailViewModel() : this(new Book()){}

    /// <summary>
    /// Provides a view model for managing the details of a book.
    /// </summary>
    public BookDetailViewModel(Book book)
    {
        _book = book;
    }

}