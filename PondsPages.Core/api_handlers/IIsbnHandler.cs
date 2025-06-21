using System.Threading.Tasks;
using PondsPages.dataclasses;

namespace PondsPages.api_handlers;
public interface IIsbnHandler
{
    public Task<Book> GetBookByIsbn(string isbn);
}