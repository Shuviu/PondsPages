using Microsoft.Data.Sqlite;
using PondsPages.dataclasses;
using PondsPages.services;
namespace PondsPages.Core.Tests.services;

public class SqliteDatabseServiceTests : IDisposable
{
    private const string _connectionString = "Data Source=test.db;Mode=Memory;Cache=Shared";
    private readonly SqliteDatabaseService _service;
    private readonly SqliteConnection _initconn;
    
    public SqliteDatabseServiceTests()
    {
        _initconn = new SqliteConnection(_connectionString);
        SqliteCommand initSetup = _initconn.CreateCommand();
        initSetup.CommandText = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "TestAssets", "InitialDBTestState.sql"));
        
        _initconn.Open();
        initSetup.ExecuteNonQuery();
        
        _service = new SqliteDatabaseService(_connectionString);
        CheckDbState();
    }

    public void Dispose()
    {
        _initconn.Close();
    }

    private void CheckDbState()
    {
        using var conn = new SqliteConnection(_connectionString);
        var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT name FROM sqlite_master WHERE type='table'";
        conn.Open();
        var reader = cmd.ExecuteReader();
        Console.WriteLine("---Tables---");
        while (reader.Read())
        {
            Console.WriteLine(reader.GetString(0));
        }
        
    }
    
    
    [Fact]
    public void TestingIfTestsWork()
    {
        CheckDbState();
        List<Book> books = _service.GetAllBooks();
    }
    
    [Fact]
    public void GetAllBooks_ReturnsAllBooks()
    {  
         List<Book> books = _service.GetAllBooks();
         Assert.NotNull(books);
         Assert.Equal(4, books.Count);
         Assert.NotNull(books.Find(x => x.Title == "It"));
         Assert.NotNull(books.Find(x => x.Title == "1984"));
    }
    
    [Fact]
    public void GetBookByIsbn_ReturnsBook()
    {
        string validIsbn = "9780451169518";
        Book? book = _service.GetBookByIsbn(validIsbn);
        Assert.NotNull(book);
        Assert.Equal("9780451169518", book.Isbn);
        Assert.Equal("It", book.Title);
    }
    
    [Fact]
    public void GetBookByInvalidIsbn_ReturnsNull()
    {
        string invalidIsbn = "invalid";
        Book? book = _service.GetBookByIsbn(invalidIsbn);
        Assert.Null(book);
    }
    
    [Fact]
    public void GetBooksByIsbn_ReturnsBooks()
    {
        List<string> validIsbns = ["9780747532743", "9780451169518"];
        
        List<Book> books = _service.GetBooksByIsbn(validIsbns);
        Assert.NotNull(books);
        Assert.Equal(2, books.Count);
        Assert.NotNull(books.Find(x => x.Title == "It"));
        Assert.NotNull(books.Find(x => x.Title == "Harry Potter and the Philosopher's Stone"));

    }
    
    [Fact]
    public void GetBooksByInvalidIsbn_ReturnsEmptyList()
    {
        List<string> invalidIsbns = ["invalid", "invalid2"];
        List<Book> books = _service.GetBooksByIsbn(invalidIsbns);
        Assert.NotNull(books);
        Assert.Empty(books);
    }
    
    [Fact]
    public void GetAuthorsByIsbn_ReturnsAuthors()
    {
        string validIsbn = "9780451169518";
        string[] authors = _service.GetAuthorsByIsbn(validIsbn);
        Assert.NotNull(authors);
        Assert.Single(authors);
        Assert.Equal("Stephen King", authors[0]);
    }
    
    [Fact]
    public void GetAuthorsByInvalidIsbn_ReturnsEmptyArray()
    {
        string invalidIsbn = "invalid";
        string[] authors = _service.GetAuthorsByIsbn(invalidIsbn);
        Assert.NotNull(authors);
        Assert.Empty(authors);
    }
    
    [Fact]
    public void GetPublishersByIsbn_ReturnsPublishers()
    {
        string validIsbn = "9780451169518";
        string[] publishers = _service.GetPublishersByIsbn(validIsbn);
        Assert.NotNull(publishers);
        Assert.Single(publishers);
        Assert.Equal("Scribner", publishers[0]);
        
    }
    
    [Fact]
    public void GetPublishersByInvalidIsbn_ReturnsEmptyArray()
    {
        string invalidIsbn = "invalid";
        string[] publishers = _service.GetPublishersByIsbn(invalidIsbn);
        Assert.NotNull(publishers);
        Assert.Empty(publishers);
    }
    
     // --- Tests for SaveBook ---
    [Fact]
    public void SaveBook_InsertsNewBookAndAssociationsCorrectly()
    {
        // Before saving, verify initial state
        var initialBooks = _service.GetAllBooks();
        Assert.Equal(4, initialBooks.Count);
        Assert.DoesNotContain(initialBooks, b => b.Isbn == "978-3-16-148410-0");

        // Create a new book with a new author and new publisher
        var newBookIsbn = "978-3-16-148410-0";
        var newBookTitle = "The Grand Adventure";
        var newAuthor = "Alice Wonderland";
        var newPublisher = "Dreamland Publishing";

        var newBook = new Book(
            newBookTitle,
            new string[] { newAuthor },
            newBookIsbn,
            new string[] { newPublisher },
            new DateOnly(2025, 7, 10),
            "A story of a grand adventure.",
            new Dictionary<string, string> { { "small", "grand_small.jpg" }, { "medium", "grand_medium.jpg" }, { "large", "grand_large.jpg" } }
        );

        // Act
        _service.SaveBook(newBook);

        // Assert - Verify book was inserted using GetBookByIsbn
        var retrievedBook = _service.GetBookByIsbn(newBookIsbn);
        Assert.NotNull(retrievedBook);
        Assert.Equal(newBookTitle, retrievedBook.Title);
        Assert.Equal(newBookIsbn, retrievedBook.Isbn);
        Assert.Equal("A story of a grand adventure.", retrievedBook.Description);
        Assert.Equal(new DateOnly(2025, 7, 10), retrievedBook.Published);
        Assert.Equal("grand_small.jpg", retrievedBook.Covers["small"]);

        // Assert - Verify author was inserted and linked using GetAuthorsByIsbn
        var authorsForNewBook = _service.GetAuthorsByIsbn(newBookIsbn);
        Assert.Single(authorsForNewBook);
        Assert.Contains(newAuthor, authorsForNewBook);

        // Assert - Verify publisher was inserted and linked using GetPublishersByIsbn
        var publishersForNewBook = _service.GetPublishersByIsbn(newBookIsbn);
        Assert.Single(publishersForNewBook);
        Assert.Contains(newPublisher, publishersForNewBook);

        // Assert overall book count using GetAllBooks
        var allBooksAfterInsert = _service.GetAllBooks();
        Assert.Equal(5, allBooksAfterInsert.Count); // 4 initial + 1 new
    }

    [Fact]
    public void SaveBook_LinksToExistingAuthorAndPublisher()
    {
        // Use existing author and publisher names from the initial data
        var existingAuthor = "J.K. Rowling";
        var existingPublisher = "Bloomsbury Publishing";

        var newBook = new Book(
            "Another Harry Potter",
            new string[] { existingAuthor },
            "978-3-16-148410-0", // New ISBN
            new string[] { existingPublisher },
            new DateOnly(2000, 1, 1),
            "Another book by JK.",
            new Dictionary<string, string> { { "small", "grand_small.jpg" }, { "medium", "grand_medium.jpg" }, { "large", "grand_large.jpg" } }
        );

        // Act
        _service.SaveBook(newBook);

        // Assert
        var retrievedBook = _service.GetBookByIsbn("978-3-16-148410-0");
        Assert.NotNull(retrievedBook);
        Assert.Contains(existingAuthor, retrievedBook.Authors);
        Assert.Contains(existingPublisher, retrievedBook.Publishers);

        // Verify that the existing author and publisher are correctly linked
        // We rely on the UNIQUE constraint in the DB and the fact that SaveAuthor/SavePublisher
        // will attempt to insert (which fails if name exists) and then fetch the ID.
        // The service's GetAuthorsByIsbn and GetPublishersByIsbn are the way to verify.
        var allAuthors = _service.GetAllBooks().SelectMany(b => b.Authors).Distinct().ToList();
        Assert.Contains(existingAuthor, allAuthors);

        var allPublishers = _service.GetAllBooks().SelectMany(b => b.Publishers).Distinct().ToList();
        Assert.Contains(existingPublisher, allPublishers);
    }

    [Fact]
    public void SaveBook_HandlesMultipleAuthorsAndPublishers()
    {
        var newBookIsbn = "978-3-16-148410-0";
        var authors = new string[] { "New Multi-Author 1", "New Multi-Author 2" };
        var publishers = new string[] { "New Multi-Publisher 1", "New Multi-Publisher 2" };

        var newBook = new Book(
            "Multi-Author Book",
            authors,
            newBookIsbn,
            publishers,
            new DateOnly(2023, 5, 15),
            "A collaborative work.",
            new Dictionary<string, string>(){ { "small", "smoll" }, {"medium", "mid"}, {"large", "large"}}
        );

        // Act
        _service.SaveBook(newBook);

        // Assert
        var retrievedBook = _service.GetBookByIsbn(newBookIsbn);
        Assert.NotNull(retrievedBook);
        Assert.Contains(authors[0], retrievedBook.Authors);
        Assert.Contains(authors[1], retrievedBook.Authors);
        Assert.Contains(publishers[0], retrievedBook.Publishers);
        Assert.Contains(publishers[1], retrievedBook.Publishers);
        Assert.Equal(2, retrievedBook.Authors.Length);
        Assert.Equal(2, retrievedBook.Publishers.Length);
    }

    [Fact]
    public void SaveBook_ThrowsSqliteExceptionOnDuplicateIsbn()
    {
        // Create a book with an ISBN that already exists in the initial data
        var existingIsbn = "9780747532743"; // Harry Potter's ISBN
        var bookWithExistingIsbn = new Book(
            "Duplicate Title",
            new string[] { "Some Author" },
            existingIsbn,
            new string[] { "Some Publisher" },
            null,
            "Duplicate description",
            new Dictionary<string, string>(){ { "small", "smoll" }, {"medium", "mid"}, {"large", "large"}}
        );

        // Act & Assert
        var ex = Assert.Throws<SqliteException>(() => _service.SaveBook(bookWithExistingIsbn));
        Assert.Contains("UNIQUE constraint failed: book.isbn", ex.Message);

        // Verify that the number of books hasn't changed (no duplicate was inserted)
        Assert.Equal(4, _service.GetAllBooks().Count);
    }

    // --- Tests for SaveBooks (List<Book>) ---
    [Fact]
    public void SaveBooks_InsertsMultipleNewBooksCorrectly()
    {
        // Before saving, verify initial state
        var initialBooks = _service.GetAllBooks();
        Assert.Equal(4, initialBooks.Count);

        var book1 = new Book(
            "New Book A", new string[] { "New Author A" }, "978-3-16-148410-0", new string[] { "New Publisher X" }, new DateOnly(2024, 1, 1), "Desc A", new Dictionary<string, string>(){ { "small", "smoll" }, {"medium", "mid"}, {"large", "large"}}
        );
        var book2 = new Book(
            "New Book B", new string[] { "New Author B" }, "978-3-7657-1111-4", new string[] { "New Publisher Y" }, new DateOnly(2024, 2, 1), "Desc B", new Dictionary<string, string>(){ { "small", "smoll" }, {"medium", "mid"}, {"large", "large"}}
        );

        var booksToSave = new List<Book> { book1, book2 };

        // Act
        _service.SaveBooks(booksToSave);

        // Assert
        var allBooks = _service.GetAllBooks();
        Assert.Equal(initialBooks.Count + 2, allBooks.Count);
        Assert.NotNull(_service.GetBookByIsbn("978-3-16-148410-0"));
        Assert.NotNull(_service.GetBookByIsbn("978-3-7657-1111-4"));

        // Verify authors and publishers are linked
        Assert.Contains("New Author A", _service.GetAuthorsByIsbn("978-3-16-148410-0"));
        Assert.Contains("New Author B", _service.GetAuthorsByIsbn("978-3-7657-1111-4"));
        Assert.Contains("New Publisher X", _service.GetPublishersByIsbn("978-3-16-148410-0"));
        Assert.Contains("New Publisher Y", _service.GetPublishersByIsbn("978-3-7657-1111-4"));
    }

    [Fact]
    public void SaveBooks_ThrowsExceptionIfAnyBookHasDuplicateIsbn()
    {
        // Book 1 is good, Book 2 has a duplicate ISBN (will cause error)
        var book1 = new Book(
            "Valid Book", new string[] { "Valid Author" }, "978-3-16-148410-0", new string[] { "Valid Publisher" }, new DateOnly(2024, 3, 1), "Valid Desc", new Dictionary<string, string>(){ { "small", "smoll" }, {"medium", "mid"}, {"large", "large"}}
        );
        var book2 = new Book(
            "Duplicate Book", new string[] { "Dup Author" }, "9780747532743", // Existing ISBN
            new string[] { "Dup Publisher" }, new DateOnly(2024, 4, 1), "Dup Desc", new Dictionary<string, string>(){ { "small", "smoll" }, {"medium", "mid"}, {"large", "large"}}
        );

        var booksToSave = new List<Book> { book1, book2 };

        // Act & Assert
        // Since SaveBooks iterates and calls SaveBook, the exception from SaveBook will propagate.
        var ex = Assert.Throws<SqliteException>(() => _service.SaveBooks(booksToSave));
        Assert.Contains("UNIQUE constraint failed: book.isbn", ex.Message);

        // Verify that Book1 was likely saved before the error occurred (due to foreach order)
        // If the order of booksToSave matters, this confirms the first book was processed.
        Assert.NotNull(_service.GetBookByIsbn("978-3-16-148410-0"));
        // Verify that the duplicate book was NOT inserted
        Assert.Equal(5, _service.GetAllBooks().Count); // 4 initial + 1 valid new book
    }


    // --- Tests for SaveAuthor ---
    [Fact]
    public void SaveAuthor_InsertsNewAuthorAndLinksToBook()
    {
        string existingBookIsbn = "9780747532743"; // Harry Potter book
        string newAuthorName = "New Test Author";

        // Initial check: J.K. Rowling should be the only author for this book
        var initialAuthorsForBook = _service.GetAuthorsByIsbn(existingBookIsbn);
        Assert.Single(initialAuthorsForBook);
        Assert.Contains("J.K. Rowling", initialAuthorsForBook);

        // Act
        _service.SaveAuthor(existingBookIsbn, newAuthorName);

        // Assert
        var authorsForBookAfterSave = _service.GetAuthorsByIsbn(existingBookIsbn);
        Assert.Equal(2, authorsForBookAfterSave.Length); // Should now have two authors
        Assert.Contains("J.K. Rowling", authorsForBookAfterSave);
        Assert.Contains(newAuthorName, authorsForBookAfterSave);

        // Verify that the new author appears in any book's authors list if queried directly
        // This is an indirect way to check if the author was added to the 'author' table
        var allAuthorsFromBooks = _service.GetAllBooks().SelectMany(b => b.Authors).Distinct().ToList();
        Assert.Contains(newAuthorName, allAuthorsFromBooks);
    }

    [Fact]
    public void SaveAuthor_LinksExistingAuthorToBook()
    {
        string existingBookIsbn = "9780451524935"; // 1984 by George Orwell
        string existingAuthorName = "Stephen King"; // Already in DB but not linked to 1984

        // Initial check: only George Orwell for 1984
        var initialAuthorsForBook = _service.GetAuthorsByIsbn(existingBookIsbn);
        Assert.Single(initialAuthorsForBook);
        Assert.Contains("George Orwell", initialAuthorsForBook);

        // Act
        _service.SaveAuthor(existingBookIsbn, existingAuthorName);

        // Assert
        var authorsForBookAfterSave = _service.GetAuthorsByIsbn(existingBookIsbn);
        Assert.Equal(2, authorsForBookAfterSave.Length); // Should now have two authors
        Assert.Contains("George Orwell", authorsForBookAfterSave);
        Assert.Contains(existingAuthorName, authorsForBookAfterSave);

        // Verify that the existing author is still present (not duplicated) in general data
        var allAuthorsFromBooks = _service.GetAllBooks().SelectMany(b => b.Authors).Distinct().ToList();
        Assert.Contains(existingAuthorName, allAuthorsFromBooks);
        Assert.Equal(4, allAuthorsFromBooks.Count); // Should still be 4 distinct authors, not 5
    }

    [Fact]
    public void SaveAuthor_ThrowsSqliteExceptionIfBookDoesNotExist()
    {
        string nonExistentIsbn = "NONEXISTENT_ISBN";
        string authorName = "Author For NonExistent Book";

        // Act & Assert
        var ex = Assert.Throws<SqliteException>(() => _service.SaveAuthor(nonExistentIsbn, authorName));
        Assert.Contains("FOREIGN KEY constraint failed", ex.Message);

        // Verify no new authors were added (cannot be linked to non-existent book)
        var allAuthors = _service.GetAllBooks().SelectMany(b => b.Authors).Distinct().ToList();
        Assert.DoesNotContain(authorName, allAuthors);
    }
    
    // --- Tests for SavePublisher ---
    [Fact]
    public void SavePublisher_InsertsNewPublisherAndLinksToBook()
    {
        string existingBookIsbn = "9780451169518"; // "It" by Stephen King
        string newPublisherName = "New Test Publisher";

        // Initial check: Scribner should be the only publisher for this book
        var initialPublishersForBook = _service.GetPublishersByIsbn(existingBookIsbn);
        Assert.Single(initialPublishersForBook);
        Assert.Contains("Scribner", initialPublishersForBook);

        // Act
        _service.SavePublisher(existingBookIsbn, newPublisherName);

        // Assert
        var publishersForBookAfterSave = _service.GetPublishersByIsbn(existingBookIsbn);
        Assert.Equal(2, publishersForBookAfterSave.Length); // Should now have two publishers
        Assert.Contains("Scribner", publishersForBookAfterSave);
        Assert.Contains(newPublisherName, publishersForBookAfterSave);

        // Verify that the new publisher appears in any book's publishers list
        var allPublishersFromBooks = _service.GetAllBooks().SelectMany(b => b.Publishers).Distinct().ToList();
        Assert.Contains(newPublisherName, allPublishersFromBooks);
    }

    [Fact]
    public void SavePublisher_LinksExistingPublisherToBook()
    {
        string existingBookIsbn = "9780062073488"; // And Then There Were None
        string existingPublisherName = "Bloomsbury Publishing"; // Already in DB but not linked to this book

        // Initial check: only HarperCollins for "And Then There Were None"
        var initialPublishersForBook = _service.GetPublishersByIsbn(existingBookIsbn);
        Assert.Single(initialPublishersForBook);
        Assert.Contains("HarperCollins", initialPublishersForBook);

        // Act
        _service.SavePublisher(existingBookIsbn, existingPublisherName);

        // Assert
        var publishersForBookAfterSave = _service.GetPublishersByIsbn(existingBookIsbn);
        Assert.Equal(2, publishersForBookAfterSave.Length); // Should now have two publishers
        Assert.Contains("HarperCollins", publishersForBookAfterSave);
        Assert.Contains(existingPublisherName, publishersForBookAfterSave);

        // Verify that the existing publisher is still present (not duplicated) in general data
        var allPublishersFromBooks = _service.GetAllBooks().SelectMany(b => b.Publishers).Distinct().ToList();
        Assert.Contains(existingPublisherName, allPublishersFromBooks);
        Assert.Equal(4, allPublishersFromBooks.Count); // Should still be 4 distinct publishers, not 5
    }

    [Fact]
    public void SavePublisher_ThrowsSqliteExceptionIfBookDoesNotExist()
    {
        string nonExistentIsbn = "NONEXISTENT_ISBN_PUB";
        string publisherName = "Publisher For NonExistent Book";

        // Act & Assert
        var ex = Assert.Throws<SqliteException>(() => _service.SavePublisher(nonExistentIsbn, publisherName));
        Assert.Contains("FOREIGN KEY constraint failed", ex.Message);

        // Verify no new publishers were added (cannot be linked to non-existent book)
        var allPublishers = _service.GetAllBooks().SelectMany(b => b.Publishers).Distinct().ToList();
        Assert.DoesNotContain(publisherName, allPublishers);
    }
}