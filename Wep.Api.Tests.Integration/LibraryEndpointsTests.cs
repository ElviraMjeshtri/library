using System.Net;
using System.Text.RegularExpressions;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Web.API;

namespace Wep.Api.Tests.Integration;

public class LibraryEndpointsTests : IClassFixture<WebApplicationFactory<IApiMarker>>, IAsyncLifetime
{
    
    private readonly WebApplicationFactory<IApiMarker> _factory;
    private readonly List<string> _createdIsbns = new ();
    private static readonly Random random = new Random();

    public LibraryEndpointsTests(WebApplicationFactory<IApiMarker> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CreateBook_CreatesBook_WhenDataIsCorrect()
    {
        //arrange 
        var httpClient = _factory.CreateClient();
        var book = GenerateBook();
        //act 
        var result = await httpClient.PostAsJsonAsync("/books", book);
        _createdIsbns.Add(book.Isbn);
        var createdBook = await result.Content.ReadFromJsonAsync<Book>();
        //assert
        result.StatusCode.Should().Be(HttpStatusCode.Created);
        createdBook.Should().BeEquivalentTo(book);
        result.Headers.Location.Should().Be($"http://localhost/books/{book.Isbn}");
    }

    [Fact]
    public async Task CreateBook_Fails_WhenIsbnIsInvalid()
    {
        //arrange
        var httpClient = _factory.CreateClient();
        var book = GenerateBook();
        book.Isbn = "invalid-isbn";
        //act
        var result = await httpClient.PostAsJsonAsync("/books", book);
        var errors = await result.Content.ReadFromJsonAsync<IEnumerable<ValidationError>>();
        var error = errors!.Single();
        //assert
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        error.PropertyName.Should().Be("Isbn");
        error.ErrorMessage.Should().Be("ISBN should contain 13 digits");
    }
    
    [Fact]
    public async Task CreateBook_Fails_WhenBookExists()
    {
        //arrange
        var httpClient = _factory.CreateClient();
        var book = GenerateBook();
        //act
        await httpClient.PostAsJsonAsync("/books", book);
        _createdIsbns.Add(book.Isbn);
        var result = await httpClient.PostAsJsonAsync("/books", book);
        var errors = await result.Content.ReadFromJsonAsync<IEnumerable<ValidationError>>();
        var error = errors!.Single();
        //assert
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        error.PropertyName.Should().Be("Isbn");
        error.ErrorMessage.Should().Be("A book with this ISBN-13 already exists.");
    }

    [Fact]
    public async Task GetBook_ReturnsBook_WhenBookExists()
    {
        //arrange
        var httpClient = _factory.CreateClient();
        var book = GenerateBook();
        await httpClient.PostAsJsonAsync("/books", book);
        _createdIsbns.Add(book.Isbn);
        
        //act
        var result = await httpClient.GetAsync($"/books/{book.Isbn}");
        var existingBook = await result.Content.ReadFromJsonAsync<Book>();
        
        //asert
        existingBook.Should().BeEquivalentTo(book);
        result.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task GetBook_ReturnsNotFound_WhenBookDoesNotExists()
    {
        //arrange
        var httpClient = _factory.CreateClient();
        var isbn = GenerateIsbn();
        
        //act
        var result = await httpClient.GetAsync($"/books/{isbn}");
        
        //asert
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetAllBooks_ReturnsAllBooks_WhenBooksExist()
    {
        //arrange
        var httpClient = _factory.CreateClient();
        var book = GenerateBook();
        await httpClient.PostAsJsonAsync("/books", book);
        _createdIsbns.Add(book.Isbn);
        var books = new List<Book> { book };
        //act
        var result = await httpClient.GetAsync($"/books");
        var returnedBooks = await result.Content.ReadFromJsonAsync<List<Book>>();

        //assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        returnedBooks.Should().BeEquivalentTo(books);

    }
    
   [Fact]
    public async Task GetAllBooks_ReturnsNoBooks_WhenNoBooksExist()
    {
        //arrange
        var httpClient = _factory.CreateClient();
      
        //act
        var result = await httpClient.GetAsync($"/books");
        var returnedBooks = await result.Content.ReadFromJsonAsync<List<Book>>();

        //assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        returnedBooks.Should().BeEmpty();

    }
    
    [Fact]
    public async Task SearchBooks_ReturnsBooks_WhenTitleMatches()
    {
        //arrange
        var httpClient = _factory.CreateClient();
        var book = GenerateBook();
        await httpClient.PostAsJsonAsync("/books", book);
        _createdIsbns.Add(book.Isbn);
        var books = new List<Book> { book };
        //act
        var result = await httpClient.GetAsync($"/books?searchTerm=oder");
        var returnedBooks = await result.Content.ReadFromJsonAsync<List<Book>>();

        //assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        returnedBooks.Should().BeEquivalentTo(books);

    }

    [Fact]
    public async Task UpdateBook_UpdatesBook_WhenDataIsCorrect()
    {
        //arange
        var httpClient = _factory.CreateClient();
        var book = GenerateBook();
        await httpClient.PostAsJsonAsync("/books", book);
        _createdIsbns.Add(book.Isbn);
        
        //act
        book.PageCount = 80;
        var result = await httpClient.PutAsJsonAsync($"/books/{book.Isbn}", book);
        var updatedBook = await result.Content.ReadFromJsonAsync<Book>();
        
        //assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        updatedBook.Should().BeEquivalentTo(book);
    }
    
    [Fact]
    public async Task UpdateBook_DoesNotUpdatesBook_WhenDataIsInCorrect()
    {
        //arange
        var httpClient = _factory.CreateClient();
        var book = GenerateBook();
        await httpClient.PostAsJsonAsync("/books", book);
        _createdIsbns.Add(book.Isbn);
        
        //act
        book.Title = string.Empty;
        var result = await httpClient.PutAsJsonAsync($"/books/{book.Isbn}", book);
        var errors = await result.Content.ReadFromJsonAsync<IEnumerable<ValidationError>>();
        var error = errors!.Single();
        
        //assert
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        error.PropertyName.Should().Be("Title");
        error.ErrorMessage.Should().Be("Title is required");
    }

    [Fact]
    public async Task UpdateBook_ReturnsNotFound_WhenBookDoesNotExists()
    {
        //arange
        var httpClient = _factory.CreateClient();
        var book = GenerateBook();
        
        //act
        var result = await httpClient.PutAsJsonAsync($"/books/{book.Isbn}", book);
        
        //assert
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteBook_ReturnsNotFound_WhenBookDoesNotExists()
    {
        //arrange
        var httpClient = _factory.CreateClient();
        var isbn = GenerateIsbn();
        
        //act
        var result = await httpClient.DeleteAsync($"/books/{isbn}");
        
        //asert
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task DeleteBook_ReturnsNoContent_WhenBookDoesExists()
    {
        //arrange
        var httpClient = _factory.CreateClient();
        var book = GenerateBook();
        await httpClient.PostAsJsonAsync($"/books", book);
        _createdIsbns.Add(book.Isbn);
        
        //act
        var result = await httpClient.DeleteAsync($"/books/{book.Isbn}");
        
        //asert
        result.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    private Book GenerateBook(string title = "The dirty Coder")
    {
        return new Book
        {
            Isbn = GenerateIsbn(),
            Title = title,
            Author = "Author",
            PageCount = 400,
            ShortDescription = "Short Description",
            ReleaseDate = new DateTime(1980, 1, 1),
        };
    }

    public string GenerateIsbn()
    {
        // Generate the first 12 digits randomly
        string first12Digits = string.Empty;
        for (int i = 0; i < 12; i++)
        {
            first12Digits += random.Next(0, 10);
        }

        // Calculate the check digit
        int checkDigit = CalculateCheckDigit(first12Digits);

        // Combine the first 12 digits with the check digit
        return $"{first12Digits}{checkDigit}";
    }
    private int CalculateCheckDigit(string first12Digits)
    {
        int sum = 0;
        for (int i = 0; i < first12Digits.Length; i++)
        {
            int digit = int.Parse(first12Digits[i].ToString());
            sum += (i % 2 == 0) ? digit : digit * 3;
        }

        int mod = sum % 10;
        return mod == 0 ? 0 : 10 - mod;
    }

    public bool ValidateIsbn(string isbn)
    {
        // Regular expression to match an ISBN-13
        string pattern = @"^\d{13}$";
        return Regex.IsMatch(isbn, pattern);
    }

    public Task InitializeAsync() => Task.CompletedTask;

    // Delete created books for testing reason
    public async Task DisposeAsync()
    {
        var httpClient = _factory.CreateClient();
        foreach (var isbn in _createdIsbns)
        {
            var response = await httpClient.DeleteAsync($"/books/{isbn}");
            if (!response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Failed to delete book with ISBN {isbn}. " +
                                  $"Status code: {response.StatusCode}." +
                                  $" Response: {responseBody}");
            }
            else
            {
                Console.WriteLine($"Successfully deleted book with ISBN {isbn}.");
            }
        }
        _createdIsbns.Clear();
    }

}