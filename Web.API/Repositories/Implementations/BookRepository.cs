using Web.API.Data;
using Web.API.Repositories.Interfaces;

namespace Web.API.Repositories.Implementations;

using Microsoft.EntityFrameworkCore;

public class BookRepository : IBookRepository
{
    private readonly LibraryDbContext _context;

    public BookRepository(LibraryDbContext context)
    {
        _context = context;
    }

    public async Task<bool> ExistsAsync(string isbn)
    {
        return await _context
            .Books
            .AsNoTracking()
            .AnyAsync(b => b.Isbn == isbn);
    }

    public async Task AddAsync(Book book)
    {
        _context.Books.Add(book);
        await _context.SaveChangesAsync();
    }

    public async Task<Book?> GetByIsbnAsync(string isbn)
    {
        return await _context
            .Books
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Isbn == isbn);
    }

    public async Task<IEnumerable<Book>> GetAllAsync()
    {
        return await _context
            .Books
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Book>> SearchByTitleAsync(string searchTerm)
    {
        return await _context.Books
            .Where(b => EF.Functions.ILike(b.Title, $"%{searchTerm}%")) // PostgreSQL case-insensitive search
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task UpdateAsync(Book book)
    {
        _context.Books.Update(book);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Book book)
    {
        _context.Books.Remove(book);
        await _context.SaveChangesAsync();
    }
}
