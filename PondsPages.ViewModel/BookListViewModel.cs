using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using PondsPages.dataclasses;

namespace PondsPages.ViewModel;

public partial class BookListViewModel : ViewModelBase
{
    [ObservableProperty]
    private ObservableCollection<BookPreviewViewModel> _bookPreviews;
    
    public BookListViewModel()
    {
        _bookPreviews = new ObservableCollection<BookPreviewViewModel>(LoadTestBooks());
    }

    public BookListViewModel(List<Book> books)
    {
        
    }
    private static List<BookPreviewViewModel> LoadTestBooks()
    {
        List<BookPreviewViewModel> testBooks =
        [
            new(),
            new(),
            new()
        ];
        
        return testBooks;
    }
}