using CommunityToolkit.Mvvm.ComponentModel;
using PondsPages.dataclasses;

namespace PondsPages.ViewModel;

public partial class BookDetailViewModel : ViewModelBase
{

    [ObservableProperty] private Book _book;
    
    public BookDetailViewModel(Book book)
    {
        _book = book;
    }

    public BookDetailViewModel()
    {
        // _book = new Book("test book", ["tester1", "tester2"], "978-3-16-148410-0", ["tester1", "tester2"], new DateOnly(), "test description", new Dictionary<string, string>(){{"large", "PondsPages.Desktop/Assets/0008537565-L.jpg"}});
        _book = new Book();
    }
}