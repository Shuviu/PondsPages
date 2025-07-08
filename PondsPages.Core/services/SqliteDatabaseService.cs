using System;
using System.Collections.Generic;
using PondsPages.dataclasses;
using Microsoft.Data.Sqlite;

namespace PondsPages.services;

public class SqliteDatabaseService : IDatabaseService
{
    /// <summary>
    /// The connection string used to connect to the database.
    /// </summary>
    private readonly string _connectionString;
    
    // ---- Constructors ---- //
    /// <summary>
    /// Provides methods for interacting with an SQLite database and managing data operations related to books, authors, and publishers.
    /// </summary>
    /// <param name="connectionString">The connection string used to connect to the database.</param>
    public SqliteDatabaseService(string connectionString)
    {
        _connectionString = connectionString;
    }

    // ---- Methods ---- //

    /// <summary>
    /// Retrieves all books stored in the database.
    /// </summary>
    /// <returns>A list of <see cref="Book"/> objects representing all the books in the database.</returns>
    public List<Book> GetAllBooks()
    {
        List<Book> books = new();
        using SqliteConnection conn = new(_connectionString);
        // Create command
        SqliteCommand fetchBooks = conn.CreateCommand();
        fetchBooks.CommandText = "SELECT * FROM book";
            
        conn.Open();
        SqliteDataReader reader = fetchBooks.ExecuteReader();
        // Read all SQL results
        while (reader.Read())
        {
            // Parse trivial data
            string isbn = reader.GetString(0);
            string title = reader.GetString(1);
            DateOnly? publishdate = DateOnly.FromDateTime(reader.GetDateTime(2));
            string description = reader.GetString(3);
            string coverSmall = reader.GetString(4);
            string coverMedium = reader.GetString(5);
            string coverLarge = reader.GetString(6);
                
            // Fetch authors and publishers from other tables
            string[] authors = GetAuthorsByIsbn(isbn);
            string[] publishers = GetPublishersByIsbn(isbn);
                
            Dictionary<string, string> covers = new Dictionary<string, string>{ {"small", coverSmall}, {"medium", coverMedium}, {"large", coverLarge} };
                
            books.Add(new Book(title, authors, isbn, publishers, publishdate, description, covers));
        }
        // Cleanup   
        reader.Close();
        conn.Close();
        return books;
    }

    /// <summary>
    /// Retrieves a book from the database based on the provided ISBN.
    /// </summary>
    /// <param name="isbn">The ISBN of the book to retrieve.</param>
    /// <returns>A <see cref="Book"/> object representing the book with the specified ISBN, or <c>null</c> if no such book is found.</returns>
    public Book? GetBookByIsbn(string isbn)
    {
        return GetAllBooks().Find(book => book.Isbn == isbn);
    }

    /// <summary>
    /// Retrieves a list of books matching the provided list of ISBNs.
    /// </summary>
    /// <param name="isbns">A list of ISBNs to search for in the database.</param>
    /// <returns>A list of <see cref="Book"/> objects that match the provided ISBNs.</returns>
    public List<Book> GetBooksByIsbn(List<string> isbns)
    {
        return GetAllBooks().FindAll(book => isbns.Contains(book.Isbn));
    }

    /// <summary>
    /// Retrieves the authors associated with a given book's ISBN.
    /// </summary>
    /// <param name="isbn">The ISBN of the book for which authors should be retrieved.</param>
    /// <returns>An array of strings representing the names of the authors associated with the specified ISBN.</returns>
    public string[] GetAuthorsByIsbn(string isbn)
    {
        List<string> authors = new();
        using SqliteConnection conn = new(_connectionString);
        
        // Create command
        SqliteCommand fetchAuthors = conn.CreateCommand();
        fetchAuthors.CommandText = "SELECT * from author where author.id in (SELECT author_id from book_author where book_author.book_isbn = @isbn)";
        fetchAuthors.Parameters.AddWithValue("@isbn", isbn);
        
        conn.Open();
        // Execute command and read results
        SqliteDataReader reader = fetchAuthors.ExecuteReader();
        while (reader.Read())
            authors.Add(reader.GetString(1));
        
        // cleanup
        reader.Close();
        conn.Close();
        
        return authors.ToArray();
    }

    /// <summary>
    /// Retrieves the publishers associated with a specific book ISBN from the database.
    /// </summary>
    /// <param name="isbn">The ISBN of the book for which to retrieve the publishers.</param>
    /// <returns>An array of strings representing the names of publishers associated with the specified ISBN.</returns>
    public string[] GetPublishersByIsbn(string isbn)
    {
        List<string> publishers = new();
        using SqliteConnection conn = new(_connectionString);
        
        // Create command
        SqliteCommand cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT * from publisher where publisher.id in (SELECT publisher_id from book_publisher where book_publisher.book_isbn = @isbn)";
        cmd.Parameters.AddWithValue("@isbn", isbn);
        
        conn.Open();
        // Execute command and read results
        using SqliteDataReader reader = cmd.ExecuteReader();
        while (reader.Read())
            publishers.Add(reader.GetString(1));
        
        // cleanup
        reader.Close();
        conn.Close();
        
        return publishers.ToArray();
    }

    public void SaveBooks(List<Book> books)
    {
        foreach (Book book in books)
            SaveBook(book);
    }

    /// <summary>
    /// Saves a book into the database.
    /// </summary>
    /// <param name="book">The <see cref="Book"/> object to be saved into the database.</param>
    /// <exception cref="Exception">Thrown when the book cannot be inserted into the database.</exception>
    public void SaveBook(Book book)
    {
        using SqliteConnection conn = new(_connectionString);
        // create command
        SqliteCommand insertBook = conn.CreateCommand();
        insertBook.CommandText = "INSERT INTO book (isbn, title, publishdate, description, cover_small, cover_medium, cover_large) VALUES (@isbn, @title, @publishdate, @description, @cover_small, @cover_medium, @cover_large)";
        insertBook.Parameters.AddWithValue("@isbn", book.Isbn);
        insertBook.Parameters.AddWithValue("@title", book.Title);
        insertBook.Parameters.AddWithValue("@publishdate", book.Published.ToString());
        insertBook.Parameters.AddWithValue("@description", book.Description);
        insertBook.Parameters.AddWithValue("@cover_small", book.Covers["small"]);
        insertBook.Parameters.AddWithValue("@cover_medium", book.Covers["medium"]);
        insertBook.Parameters.AddWithValue("@cover_large", book.Covers["large"]);
        
        // Execute command
        conn.Open();
        if (insertBook.ExecuteNonQuery() <= 0)
            throw new Exception("Could not insert the Book into the database.");
        conn.Close();
        
        // Save authors and publishers linked to the book
        foreach (string author in book.Authors)
            SaveAuthor(book.Isbn, author);

        foreach (string publisher in book.Publishers)
        {
            SavePublisher(book.Isbn, publisher);;
        }
    }
    // ---- Author and Publisher Methods ---- //
    // -- NOTE: These methods could be combined, but are not to be ready for future expansion. -- // 
    
    /// <summary>
    /// Saves an author to the database and associates them with a specific book.
    /// </summary>
    /// <param name="isbn">The ISBN of the book to associate with the author.</param>
    /// <param name="author">The name of the author to be saved in the database.</param>
    /// <exception cref="Exception">Thrown when the author could not be inserted into the database or the author ID was not found.</exception>
    public void SaveAuthor(string isbn, string author)
    {
        using SqliteConnection conn = new(_connectionString);
        
        // Create insert command for the author
        SqliteCommand authorInsert = conn.CreateCommand();
        authorInsert.CommandText = "INSERT INTO author (name) VALUES (@name); SELECT last_insert_rowid()";
        authorInsert.Parameters.AddWithValue("@name", author);
        
        // Create insert command for linking author to book
        SqliteCommand authorBookLinkInsert = conn.CreateCommand();
        authorBookLinkInsert.CommandText = "INSERT INTO book_author (book_isbn, author_id) VALUES (@isbn, @author_id)";
        authorBookLinkInsert.Parameters.AddWithValue("@isbn", isbn);
        conn.Open();
            
        if (authorInsert.ExecuteNonQuery() > 0)
        {
            // fetch author ID from last insert
            int authorId = (int)(authorInsert.ExecuteScalar() ?? throw new Exception("Author ID not found"));
            // link author to book using author ID
            authorBookLinkInsert.Parameters.AddWithValue("@author_id", authorId);
            authorBookLinkInsert.ExecuteNonQuery();
        }
        else
            throw new Exception("Could not insert the Author into the database.");;
            
        conn.Close();
    }

    /// <summary>
    /// Saves the specified publisher for a book with the given ISBN into the database.
    /// </summary>
    /// <param name="isbn">The ISBN of the book to associate with the publisher.</param>
    /// <param name="publisher">The name of the publisher to be saved.</param>
    /// <exception cref="Exception">Thrown when the publisher cannot be inserted into the database or when a publisher ID cannot be retrieved.</exception>
    public void SavePublisher(string isbn, string publisher)
    {
        using SqliteConnection conn = new(_connectionString);
        
        // Create insert command for the publisher
        SqliteCommand publisherInsert = conn.CreateCommand();
        publisherInsert.CommandText = "INSERT INTO publisher (name) VALUES (@name); SELECT last_insert_rowid()";
        publisherInsert.Parameters.AddWithValue("@name", publisher);
        
        // Create insert command for linking publisher to book
        SqliteCommand publisherBookLinkInsert = conn.CreateCommand();
        publisherBookLinkInsert.CommandText = "INSERT INTO book_publisher (book_isbn, publisher_id) VALUES (@isbn, @publisher_id)";
        publisherBookLinkInsert.Parameters.AddWithValue("@isbn", isbn);
        
        conn.Open();

        if (publisherInsert.ExecuteNonQuery() > 0)
        {
            // fetch publisher ID from last insert
            int publisherId = (int)(publisherInsert.ExecuteScalar() ?? throw new Exception("Publisher ID not found"));
            // link publisher to book using publisher ID
            publisherBookLinkInsert.Parameters.AddWithValue("@publisher_id", publisherId);
            publisherBookLinkInsert.ExecuteNonQuery();
        }
        else
            throw new Exception("Could not insert the Publisher into the database.");;
        
        conn.Close();
    }
}