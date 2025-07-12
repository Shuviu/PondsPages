using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using PondsPages.dataclasses;

namespace PondsPages.api_handlers;


public class OpenLibraryHandler : IIsbnHandler
{
    private readonly HttpClient _httpClient;

    public OpenLibraryHandler(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _httpClient.BaseAddress = new Uri("https://openlibrary.org/");
    }
    
    public async Task<Book?> GetBookByIsbn(string isbn)
    {
        try
        {
            // Make the request.   
            string requestUri = $"api/books?bibkeys=ISBN:{isbn}&format=json&jscmd=data";
            HttpResponseMessage response = await _httpClient.GetAsync(requestUri);

            // Check for a successful request.    
            response.EnsureSuccessStatusCode();
            
            string jsonResponse = await response.Content.ReadAsStringAsync();
            
            // Parse the response to get the book.   
            using (JsonDocument jsonDoc = JsonDocument.Parse(jsonResponse))
            {
                // Check for empty response.
                if (!jsonDoc.RootElement.TryGetProperty("ISBN:" + isbn, out JsonElement jsonRoot))
                    return null;

                return ParseJson(jsonRoot, isbn);
            }
        }
        catch (Exception)
        {
            return null;
        }
    }

    private static Book ParseJson(JsonElement jsonRoot, string isbn)
    {
        string title = jsonRoot.GetProperty("title").GetString() ?? "";
        
        // Parse the publishDate.
        DateOnly? publishDate = null;
        string dateString = jsonRoot.GetProperty("publish_date").GetString() ?? "";
        if (dateString != "")
            publishDate = DateOnly.Parse(dateString);

        // Parse all authors.
        string[] authors = jsonRoot.GetProperty("authors").EnumerateArray().Select(x => x.GetProperty("name").ToString() ?? "")
            .ToArray();

        // Fetch all cover urls.
        Dictionary<string, string> covers = new Dictionary<string, string>{ {"small", ""}, {"medium", ""}, {"large", ""} };
        JsonElement coversJson = jsonRoot.GetProperty("cover");
        foreach (string key in covers.Keys)
        {
            covers[key] = coversJson.GetProperty(key).GetString() ?? "";
        }
        
        // Fetch the first publisher.
        string[] publishers = jsonRoot.GetProperty("publishers").EnumerateArray().Select(x => x.GetProperty("name").ToString() ?? "").ToArray();
        
        return new Book(title, authors, isbn, publishers, publishDate, "", covers);
    }
}