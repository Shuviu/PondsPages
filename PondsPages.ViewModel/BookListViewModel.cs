using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.ComponentModel;
using PondsPages.dataclasses;
using Avalonia.Reactive;

namespace PondsPages.ViewModel;

public partial class BookListViewModel : ViewModelBase
{
    /// <summary>
    /// Backing field for the collection of book preview viewmodels displayed in the book list.
    /// </summary>
    [ObservableProperty] private ObservableCollection<BookPreviewViewModel> _bookPreviews;

    /// <summary>
    /// Backing field for the currently selected book in the book list.
    /// </summary>
    [ObservableProperty] private BookPreviewViewModel? _selectedBook;

    /// <summary>
    /// Event triggered when a book is selected from the book list.
    /// </summary>
    public event EventHandler<Book>? OnBookSelected;
    
    // ---- Constructors ---- //
    
    /// <summary>
    /// Default constructor. Loads a list of test books.
    /// </summary>
    public BookListViewModel() : this(LoadTestBooks()){}

    /// <summary>
    /// Represents a view model containing a list of books to be displayed in the UI.
    /// Provides functionality to initialize with specified book information.
    /// </summary>
    /// <param name="books">A list of books to be displayed in the UI.</param>
    public BookListViewModel(List<Book> books)
    {
        _bookPreviews = new ObservableCollection<BookPreviewViewModel>();
        foreach (Book book in books)
        {
            _bookPreviews.Add(new BookPreviewViewModel(book));
        }
    }
    
    // ---- Event Handlers ---- //
    
    /// <summary>
    /// Invoked whenever the selected book changes.
    /// Triggers the OnBookSelected event with the updated book information.
    /// </summary>
    /// <param name="value">The ViewModel of the newly selected book. May be null if no book is selected.</param>
    partial void OnSelectedBookChanged(BookPreviewViewModel? value)
    {
        OnBookSelected?.Invoke(this, value!.Book);
    }
    
    // ---- Private Methods ---- //
    
    /// <summary>
    /// Creates and returns a list of test book objects.
    /// </summary>
    /// <returns>A list of test book objects.</returns>
    private static List<Book> LoadTestBooks()
    {
        List<Book> testBooks =
        [
            new(),
            new(),
            new(),
            new Book("testing book", [], "978-3-16-148410-0", [], new DateOnly(), "")
        ];
        
        return testBooks;
    }

   
}