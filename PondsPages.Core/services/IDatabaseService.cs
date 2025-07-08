using System.Collections.Generic;
using PondsPages.dataclasses;

namespace PondsPages.services;

/// <summary>
/// Defines methods for interacting with a database.
/// </summary>
public interface IDatabaseService
{
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
    /// Saves a collection of books to the database.
    /// </summary>
    /// <param name="books">A list of books to be saved in the database.</param>
    public void SaveBooks(List<Book> books);

    /// <summary>
    /// Saves a single book to the database.
    /// </summary>
    /// <param name="book">The book to be saved in the database.</param>
    public void SaveBook(Book book);
}