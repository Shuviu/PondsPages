using System.Collections.Generic;
using PondsPages.dataclasses;

namespace PondsPages.services.database;

/// <summary>
/// Defines methods for interacting with a database.
/// </summary>
public interface IDatabaseService
{
    public void InitializeDatabase();
    
    /// <summary>
    /// Retrieves a list of all books from the database.
    /// </summary>
    /// <returns>A list of books available in the database.</returns>
    public List<Book> GetAllBooks();

    /// <summary>
    /// Retrieves a book from the database using the provided ISBN.
    /// </summary>
    /// <param name="isbn">The ISBN of the book to retrieve.</param>
    /// <returns>The book corresponding to the given ISBN, or null if not found.</returns>
    public Book? GetBookByIsbn(string isbn);

    /// <summary>
    /// Retrieves a list of books from the database using the provided list of ISBNs.
    /// </summary>
    /// <param name="isbns">A list of ISBNs to retrieve books for.</param>
    /// <returns>A list of books corresponding to the given ISBNs.</returns>
    public List<Book> GetBooksByIsbn(List<string> isbns);

    /// <summary>
    /// Retrieves the authors associated with a specified book's ISBN.
    /// </summary>
    /// <param name="isbn">The ISBN of the book for which authors are to be retrieved.</param>
    /// <returns>An array of author names associated with the given ISBN.</returns>
    public string[] GetAuthorsByIsbn(string isbn);

    /// <summary>
    /// Retrieves the publishers associated with a specific book ISBN from the database.
    /// </summary>
    /// <param name="isbn">The ISBN of the book for which to retrieve the publishers.</param>
    /// <returns>An array of strings representing the names of publishers associated with the specified ISBN.</returns>
    public string[] GetPublishersByIsbn(string isbn);

    /// <summary>
    /// Saves a collection of books to the database.
    /// </summary>
    /// <param name="books">A list of books to be saved in the database.</param>
    public void SaveBooks(List<Book> books);

    /// <summary>
    /// Saves a single book to the database.
    /// </summary>
    /// <param name="book">The book to be saved in the database.</param>
    public void SaveBook(Book book);

    /// <summary>
    /// Saves an author to the database and associates them with a specific book.
    /// </summary>
    /// <param name="isbn">The ISBN of the book to associate with the author.</param>
    /// <param name="author">The name of the author to be saved in the database.</param>
    public void SaveAuthor(string isbn, string author);

    /// <summary>
    /// Saves the specified publisher for a book with the given ISBN into the database.
    /// </summary>
    /// <param name="isbn">The ISBN of the book to associate with the publisher.</param>
    /// <param name="publisher">The name of the publisher to be saved.</param>
    public void SavePublisher(string isbn, string publisher);
}