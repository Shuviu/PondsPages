using CommunityToolkit.Mvvm.ComponentModel;
using PondsPages.dataclasses;

namespace PondsPages.ViewModel;

public partial class BookPreviewViewModel : ViewModelBase
{
    [ObservableProperty]
    private Book _book;
    
    public BookPreviewViewModel()
    {
        _book = new Book();
    }
    
    public BookPreviewViewModel(Book book)
    {
        _book = book;
    }
}