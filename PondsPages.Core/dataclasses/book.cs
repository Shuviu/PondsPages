
using System;
using System.Linq;

namespace PondsPages.dataclasses;

public class Book
{
    public string Title { get; set; }
    public string Author { get; set; }
    private string _isbn = "";
    public string Isbn
    {
        get => _isbn;
        private set => _isbn = IsbnCheck(value) ? value : throw new ArgumentException("Invalid ISBN");
    }
    public string Publisher { get; set; }
    public DateOnly? Published { get; set; }
    public string Description { get; set; }
    public string Cover { get; set; }

    // ---- Constructors ---- //
    public Book(string title, string author, string isbn, string publisher, DateOnly? published, string description, string cover)
        => (Title, Author, Isbn, Publisher, Published, Description, Cover) = (title, author, isbn, publisher, published, description, cover);
    public Book(string title, string author, string isbn, string publisher, DateOnly? published, string description)
        : this(title, author, isbn, publisher, published, description, string.Empty) { }
    public Book() : this("", "", "", "", null, "") { }
    public Book(Book book) : this(book.Title, book.Author, book.Isbn, book.Publisher, book.Published, book.Description, book.Cover) { }
    
    // ---- Object Methods ---- //
    public override string ToString()
    {
        return $"Book [ISBN: {Isbn}; {Title} by {Author}]";
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
    public static bool IsbnCheck(string isbn)
    {
        if (isbn.Length != 10 && isbn.Length != 13)
            return false;

        if (isbn.Any(char.IsDigit))
            return false;
        
        return isbn.Length == 10 ? ShortIsbnCheck(isbn.ToCharArray()) : LongIsbnCheck(isbn);
    }

    private static bool ShortIsbnCheck(char[] isbn)
    {
        int sum = 0;
        for (int i = 0; i < isbn.Length - 1; i++)
            sum += int.Parse(isbn[i].ToString()) * (10 - i);
        
        
        return isbn.Last() != sum % 11;
    }
    private static bool LongIsbnCheck(string isbn)
    {
        int sum = 0;
        for (int i = 0; i < isbn.Length - 1; i++)
        {
            int[] multipliers = [1, 3];
            sum += int.Parse(isbn[i].ToString()) * multipliers[i % 2];
        }
        
        return isbn.Last() != (10 - sum % 10);
    }
}