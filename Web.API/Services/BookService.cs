using Microsoft.EntityFrameworkCore;
using Web.API.Data;

namespace Web.API.Services;

public class BookService : IBookService
{
    private readonly LibraryDbContext _context;

    public BookService(LibraryDbContext context)
    {
        _context = context;
    }
    
    public async Task<bool> CreateAsync(Book book)
    {
        // Check if the book already exists
        var existingBook = await _context.Books.AsNoTracking().FirstOrDefaultAsync(b => b.Isbn == book.Isbn);
        if (existingBook is not null)
        {
            return false;
        }

        // Add and save the new book
        _context.Books.Add(book);
        await _context.SaveChangesAsync();
        return true;
    }
    
    public async Task<Book?> GetByIsbnAsync(string isbn)
    {
        // Fetch a single book by ISBN
        return await _context.Books.FirstOrDefaultAsync(b => b.Isbn == isbn);
    }

    public async Task<IEnumerable<Book>> GetAllAsync()
    {
        // Fetch all books
        return await _context.Books.AsNoTracking().ToListAsync();
    }

    public async Task<IEnumerable<Book>> SearchByTitleAsync(string searchTerm)
    {
        // Search books by title
        return await _context.Books
            .Where(b => EF.Functions.ILike(b.Title, $"%{searchTerm}%")) // Use ILIKE for case-insensitive search in PostgreSQL
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<bool> UpdateAsync(Book book)
    {
        // Check if the book exists
        var existingBook = await _context.Books.FirstOrDefaultAsync(b => b.Isbn == book.Isbn);
        if (existingBook is null)
        {
            return false;
        }

        // Update the book details
        _context.Entry(existingBook).CurrentValues.SetValues(book);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(string isbn)
    {
        // Find and delete the book
        var book = await _context.Books.FirstOrDefaultAsync(b => b.Isbn == isbn);
        if (book is null)
        {
            return false;
        }

        _context.Books.Remove(book);
        await _context.SaveChangesAsync();
        return true;
    }

    // public async Task<bool> CreateAsync(Book book)
    // {
    //     var existingBook = await GetByIsbnAsync(book.Isbn);
    //     if (existingBook is not null)
    //     {
    //         return false;
    //     }
    //
    //     using var connection = await _connectionFactory.GetConnectionAsync();
    //     var result = await connection.ExecuteAsync(
    //         @"INSERT INTO Books(Isbn, Title, Author, ShortDescription, PageCount, ReleaseDate)
    //             VALUES (@Isbn, @Title, @Author, @ShortDescription, @PageCount, @ReleaseDate)", book
    //     );
    //     return result > 0;
    // }
    //
    // public async Task<Book?> GetByIsbnAsync(string isbn)
    // {
    //     using var connection = await _connectionFactory.GetConnectionAsync();
    //     return connection.QuerySingleOrDefault<Book>("SELECT * FROM Books WHERE Isbn = @isbn LIMIT 1", new { isbn });
    // }
    //
    // public async Task<IEnumerable<Book>> GetAllAsync()
    // {
    //     using var connection = await _connectionFactory.GetConnectionAsync();
    //     return await connection.QueryAsync<Book>("SELECT * FROM Books");
    // }
    //
    // public async Task<IEnumerable<Book>> SearchByTitleAsync(string searchTerm)
    // {
    //     using var connection = await _connectionFactory.GetConnectionAsync();
    //     return await connection.QueryAsync<Book>(
    //         "SELECT * FROM Books WHERE Title LIKE '%' || @SearchTerm || '%'",
    //         new { SearchTerm = searchTerm });
    // }
    //
    // public async Task<bool> UpdateAsync(Book book)
    // {
    //     using var connection = await _connectionFactory.GetConnectionAsync();
    //     var existingBook = await GetByIsbnAsync(book.Isbn);
    //     if (existingBook is null)
    //     {
    //         return false;
    //     }
    //
    //     var result = await connection.ExecuteAsync(
    //         @"UPDATE Books set Title =@Title,
    //              Author = @Author, 
    //              ShortDescription = @ShortDescription,
    //              PageCount = @PageCount,
    //              ReleaseDate = @ReleaseDate WHERE Isbn=@Isbn", book
    //     );
    //     return result > 0;
    // }
    //
    // public async Task<bool> DeleteAsync(string isbn)
    // {
    //     using var connection = await _connectionFactory.GetConnectionAsync();
    //     var existingBook = await GetByIsbnAsync(isbn);
    //     if (existingBook is null)
    //     {
    //         return false;
    //     }
    //
    //     var result = connection.Execute("DELETE FROM Books WHERE Isbn=@Isbn", new { Isbn = isbn });
    //     return result > 0;
    // }
    
}