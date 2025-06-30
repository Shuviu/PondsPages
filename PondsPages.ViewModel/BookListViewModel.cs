using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.ComponentModel;
using PondsPages.dataclasses;
using Avalonia.Reactive;

namespace PondsPages.ViewModel;

public partial class BookListViewModel : ViewModelBase
{
    [ObservableProperty]
    private ObservableCollection<BookPreviewViewModel> _bookPreviews;
    [ObservableProperty]
    private BookPreviewViewModel? _selectedBook;
    public event EventHandler<Book>? OnBookSelected;
    
    public BookListViewModel() : this(LoadTestBooks()){}

    public BookListViewModel(List<Book> books)
    {
        _bookPreviews = new ObservableCollection<BookPreviewViewModel>();
        foreach (Book book in books)
        {
            _bookPreviews.Add(new BookPreviewViewModel(book));
        }
    }
    private static List<Book> LoadTestBooks()
    {
        List<Book> testBooks =
        [
            new(),
            new(),
            new()
        ];
        
        return testBooks;
    }

    partial void OnSelectedBookChanged(BookPreviewViewModel? value)
    {
        OnBookSelected?.Invoke(this, value!.Book);
    }
}