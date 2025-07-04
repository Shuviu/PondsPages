
using System;
using System.Collections.Generic;
using System.Linq;

namespace PondsPages.dataclasses;

public class Book
{
    /// <summary>
    /// Gets or sets the title of the book.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Gets or sets the authors names of the book.
    /// </summary>
    public string[] Authors { get; set; }
    private string _isbn = "";
    /// <summary>
    /// Gets or sets the ISBN of the book.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when the ISBN is invalid.</exception>
    public string Isbn
    {
        get => _isbn;
        private set => _isbn = IsbnCheck(value) ? value : throw new ArgumentException("Invalid ISBN");
    }

    /// <summary>
    /// Gets or sets the publishers names of the book.
    /// </summary>
    public string[] Publishers { get; set; }

    /// <summary>
    /// Gets or sets the publication date of the book.
    /// </summary>
    public DateOnly? Published { get; set; }
    
    /// <summary>
    /// Gets or sets the users description of the book.
    /// </summary>
    public string Description { get; set; }
    
    /// <summary>
    /// Gets or sets the cover of the book.
    /// Stores the book cover urls (small, medium, large)
    /// </summary>
    public Dictionary<string, string> Covers { get; set; }

    // ---- Constructors ---- //
    /// <summary>
    /// Represents a book with properties such as title, author, ISBN, publisher, publication date, description, and cover.
    /// </summary>
    /// <param name="title">The title of the book</param>
    /// <param name="authors">The authors names of the book</param>
    /// <param name="isbn">The ISBN of the book</param>
    /// <param name="publishers">The publishers names of the book</param>
    /// <param name="published">The publication date of the book</param>
    /// <param name="description">The users description of the book</param>
    /// <param name="covers">The cover url of the book</param>
    public Book(string title, string[] authors, string isbn, string[] publishers, DateOnly? published, string description,
        Dictionary<string, string> covers)
        => (Title, Authors, Isbn, Publishers, Published, Description, Covers) =
            (title, authors, isbn, publishers, published, description, covers);

    public Book(string title, string[] authors, string isbn, string[] publishers, DateOnly? published, string description)
        : this(title, authors, isbn, publishers, published, description, []) { }
    public Book() : this("default", [], "978-3-16-148410-0", [], null, "") { }
    public Book(Book book) : this(book.Title, book.Authors, book.Isbn, book.Publishers, book.Published, book.Description, book.Covers) { }
    
    // ---- Object Methods ---- //
    public override string ToString()
    {
        return $"Book [ISBN: {Isbn}; {Title} by {Authors}]";
    }
    public override bool Equals(object? obj)
    {
        if (obj is Book book)
            return Isbn == book.Isbn;
        return false;
    }
    public override int GetHashCode()
    {
        return Isbn.GetHashCode();
    }
    
    // ---- Class Methods ---- // 
    /// <summary>
    /// Checks if the parsed ISBN is valid.
    /// </summary>
    /// <param name="isbn">The ISBN to be checked</param>
    /// <returns>A bool describing if the ISBN is valid</returns>
    public static bool IsbnCheck(string isbn)
    {
        string isbnString = new(isbn.Where(x => x != '-').ToArray());
        if (isbnString.Length != 10 && isbnString.Length != 13)
            return false;

        if (isbnString.Any(x => !char.IsDigit(x)))
            return false;
        
        return isbnString.Length == 10 ? ShortIsbnCheck(isbnString) : LongIsbnCheck(isbnString);
    }
    /// <summary>
    /// Checks a 10 Digit ISBN.
    /// </summary>
    private static bool ShortIsbnCheck(string isbn)
    {
        int sum = 0;
        for (int i = 0; i < isbn.Length - 1; i++)
            sum += int.Parse(isbn[i].ToString()) * (10 - i);
        
        
        return int.Parse(isbn.Last().ToString()) == (11 - sum % 11);
    }
    /// <summary>
    /// Checks a 13 Digit ISBN.
    /// </summary>
    private static bool LongIsbnCheck(string isbn)
    {
        int sum = 0;
        for (int i = 0; i < isbn.Length - 1; i++)
        {
            int[] multipliers = [1, 3];
            sum += int.Parse(isbn[i].ToString()) * multipliers[i % 2];
        }
        
        int checkDigit = sum % 10 == 0 ? 0 : 10 - sum % 10;
        return int.Parse(isbn.Last().ToString()) == checkDigit;
    }
}