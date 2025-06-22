using System.Net;
using System.Text;
using PondsPages.api_handlers;
using PondsPages.dataclasses;

namespace PondsPages.Core.Tests.api_handlers;

public class OpenLibraryHandlerTests
{
    [Fact]
    public async Task GetBookByIsbn_InvalidIsbn_ReturnsNull()
    {
        // ---- Arrange ---- //
        const string invalidIsbn = "invalid";
        HttpMessageHandler messageMock = new HttpMessageHandlerMock(new HttpResponseMessage()
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent("", Encoding.UTF8, "application/json")
        });
        
        // ---- Act ---- //
        OpenLibraryHandler apiHandler = new OpenLibraryHandler(new HttpClient(messageMock));
        Book? result = await apiHandler.GetBookByIsbn(invalidIsbn);
        
        // ---- Assert ---- //
        Assert.Null(result);
    }

    [Fact]
    public async Task GetBookByIsbn_ValidIsbn_ReturnsBook()
    {
        const string validIsbn = "978-0-316-38311-0";
        const string jsonBodyMock = @"
        {
          ""ISBN:978-0-316-38311-0"": {
            ""url"": ""https://openlibrary.org/books/OL26845560M/No_Game_No_Life_Vol._1_-_light_novel"",
            ""title"": ""No Game No Life, Vol. 1 - light novel"",
            ""authors"": [
              {
                ""url"": ""https://openlibrary.org/authors/OL7476943A/榎宮祐"",
                ""name"": ""榎宮祐""
              }
            ],
            ""publishers"": [
              {
                ""name"": ""Kindle Direct Publishing""
              }
            ],
            ""publish_date"": ""May 05, 2015"",
            ""cover"": {
              ""small"": ""https://covers.openlibrary.org/b/id/8537565-S.jpg"",
              ""medium"": ""https://covers.openlibrary.org/b/id/8537565-M.jpg"",
              ""large"": ""https://covers.openlibrary.org/b/id/8537565-L.jpg""
            }
          }
        }";
        
        HttpMessageHandler messageMock = new HttpMessageHandlerMock(new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonBodyMock, Encoding.UTF8, "application/json")
            });
        OpenLibraryHandler apiHandler = new OpenLibraryHandler(new HttpClient(messageMock));
        Book? result = await apiHandler.GetBookByIsbn(validIsbn);
        
        Assert.NotNull(result);
    }
    
    [Fact]
    public async Task GetBookByIsbn_BookNotFound_ReturnsNull()
    {
        const string validIsbn = "978-0-316-38311-0";

        HttpMessageHandler messageMock = new HttpMessageHandlerMock(new HttpResponseMessage()
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent("{}", Encoding.UTF8, "application/json")
        });
        
        OpenLibraryHandler apiHandler = new OpenLibraryHandler(new HttpClient(messageMock));
        Book? result = await apiHandler.GetBookByIsbn(validIsbn);
        
        Assert.Null(result);
    }
}