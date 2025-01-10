namespace Web.API.Repositories.Interfaces;

public interface IBookRepository
{
    Task<bool> ExistsAsync(string isbn);
    Task AddAsync(Book book);
    Task<Book?> GetByIsbnAsync(string isbn);
    Task<IEnumerable<Book>> GetAllAsync();
    Task<IEnumerable<Book>> SearchByTitleAsync(string searchTerm);
    Task UpdateAsync(Book book);
    Task DeleteAsync(Book book);
}
