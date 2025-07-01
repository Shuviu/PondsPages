using CommunityToolkit.Mvvm.ComponentModel;
using PondsPages.dataclasses;

namespace PondsPages.ViewModel;

public partial class BookPreviewViewModel : ViewModelBase
{
    [ObservableProperty]
    private Book _book;
    
    
    // ---- Constructors ----

    /// <summary>
    /// Represents the view model for previewing book details.
    /// </summary>
    public BookPreviewViewModel()
    {
        _book = new Book();
    }

    /// <summary>
    /// Represents the view model for previewing book details.
    /// </summary>
    /// <param name="book">The book to be previewed.</param>
    public BookPreviewViewModel(Book book)
    {
        _book = book;
    }
}