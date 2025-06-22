using PondsPages.dataclasses;

namespace PondsPages.Core.Tests.dataclasses;

public class BookTests
{
    [Fact]
    public void IsbnCheck_TestValid13Digit_ReturnsTrue()
    {
        // ---- Arrange ---- //
        const string isbn = "978-0-316-38311-0";
        
        // ---- Act ---- //
        bool isValid = Book.IsbnCheck(isbn);
        
        // ---- Assert ---- //
        Assert.True(isValid);
    }
    [Fact]
    public void IsbnCheck_TestInvalid13Digit_ReturnsFalse()
    {
        // ---- Arrange ---- //
        const string isbn = "978-3-7416-4175-8";
        
        // ---- Act ---- //
        bool isValid = Book.IsbnCheck(isbn);
        
        // ---- Assert ---- //
        Assert.False(isValid);
    }
    [Fact]
    public void IsbnCheck_TestValid10Digit_ReturnsTrue()
    {
        // ---- Arrange ---- //
        const string isbn = "0-19-853453-1";
        
        // ---- Act ---- //
        bool isValid = Book.IsbnCheck(isbn);
        
        // ---- Assert ---- //
        Assert.True(isValid);
    }

    [Fact]
    public void IsbnCheck_TestInvalid10Digit_ReturnsFalse()
    {
        // ---- Arrange ---- //
        const string isbn = "0-19-853453-2";
        
        // ---- Act ---- //
        bool isValid = Book.IsbnCheck(isbn);
        
        // ---- Assert ---- //
        Assert.False(isValid);
    }
    
    [Fact]
    public void IsbnCheck_TestInvalidChar_ReturnsFalse()
    {
        // ---- Arrange ---- //
        const string isbn = "invalid";
        
        // ---- Act ---- //
        bool isValid = Book.IsbnCheck(isbn);
        
        // ---- Assert ---- //
        Assert.False(isValid);
    }
    [Fact]
    public void IsbnCheck_TestInvalidLength_ReturnsFalse()
    {
        // ---- Arrange ---- //
        const string isbn = "978-3-7416-41-8";
        
        // ---- Act ---- //
        bool isValid = Book.IsbnCheck(isbn);
        
        // ---- Assert ---- //
        Assert.False(isValid);
    }
}