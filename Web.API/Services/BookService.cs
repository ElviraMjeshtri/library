using Web.API.Repositories.Interfaces;

namespace Web.API.Services;

public class BookService : IBookService
{
    private readonly IBookRepository _repository;

    public BookService(IBookRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> CreateAsync(Book book)
    {
        if (await _repository.ExistsAsync(book.Isbn))
        {
            return false;
        }

        await _repository.AddAsync(book);
        return true;
    }

    public async Task<Book?> GetByIsbnAsync(string isbn)
    {
        return await _repository.GetByIsbnAsync(isbn);
    }

    public async Task<IEnumerable<Book>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<IEnumerable<Book>> SearchByTitleAsync(string searchTerm)
    {
        return await _repository.SearchByTitleAsync(searchTerm);
    }

    public async Task<bool> UpdateAsync(Book book)
    {
        if (!await _repository.ExistsAsync(book.Isbn))
        {
            return false;
        }

        await _repository.UpdateAsync(book);
        return true;
    }

    public async Task<bool> DeleteAsync(string isbn)
    {
        var book = await _repository.GetByIsbnAsync(isbn);
        if (book is null)
        {
            return false;
        }

        await _repository.DeleteAsync(book);
        return true;
    }
}
