using FluentValidation;
using FluentValidation.Results;
using Web.API.Endpoints.Internal;
using Web.API.Services;

namespace Web.API.Endpoints;

public class LibraryEndpoints : IEndpoints
{
    private const string ContentType = "application/json";
    private const string Tag = "Books";
    private const string BaseRoute = "books";

    public static void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost(BaseRoute, CreateBookAsync)
            .WithName("CreateBook")
            .Accepts<Book>(ContentType)
            .Produces<Book>(201)
            .Produces<IEnumerable<ValidationFailure>>(400)
            .WithTags(Tag);

        app.MapPut($"{BaseRoute}/{{isbn}}", UpdateBookAsync)
            .WithName("UpdateBook")
            .Accepts<Book>(ContentType)
            .Produces<Book>(200)
            .Produces<IEnumerable<ValidationFailure>>(400)
            .WithTags(Tag);

        app.MapGet(BaseRoute, GetAlBooksAsync)
            .WithName("GetBooks")
            .Produces<IEnumerable<Book>>(200)
            .WithTags(Tag);

        app.MapGet($"{BaseRoute}/{{isbn}}", GetBookByIsbnAsync)
            .WithName("GetBook")
            .Produces<Book>(200)
            .Produces(404)
            .WithTags(Tag);

        app.MapDelete($"{BaseRoute}/{{isbn}}", DeleteBookAsync)
            .WithName("DeleteBook")
            .Produces(204)
            .Produces(404)
            .WithTags(Tag);
    }

    internal static async Task<IResult> CreateBookAsync(
        Book book,
        IBookService bookService,
        IValidator<Book> validator,
        LinkGenerator linker,
        HttpContext context)
    {
        var validatorResult = await validator.ValidateAsync(book);
        if (!validatorResult.IsValid)
        {
            return Results.BadRequest(validatorResult.Errors);
        }

        var created = await bookService.CreateAsync(book);
        if (!created)
        {
            return Results.BadRequest(new List<ValidationFailure>
            {
                new("Isbn", "A book with this ISBN-13 already exists.")
            });
        }

        var locationUri = linker.GetUriByName(context, "GetBook", new { isbn = book.Isbn })!;
        return Results.Created(locationUri, book);
    }

    internal static async Task<IResult> GetAlBooksAsync(IBookService bookService, string? searchTerm)
    {
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var matchedBooks = await bookService.SearchByTitleAsync(searchTerm);
            return Results.Ok(matchedBooks);
        }

        var books = await bookService.GetAllAsync();
        return Results.Ok(books);
    }

    internal static async Task<IResult> GetBookByIsbnAsync(string isbn, IBookService bookService)
    {
        var book = await bookService.GetByIsbnAsync(isbn);
        return book is not null ? Results.Ok(book) : Results.NotFound();
    }

    internal static async Task<IResult> UpdateBookAsync(
        Book book,
        string isbn,
        IBookService bookService,
        IValidator<Book> validator)
    {
        book.Isbn = isbn;
        var validatorResult = await validator.ValidateAsync(book);
        if (!validatorResult.IsValid)
        {
            return Results.BadRequest(validatorResult.Errors);
        }

        var updated = await bookService.UpdateAsync(book);
        return updated ? Results.Ok(book) : Results.NotFound();
    }

    internal static async Task<IResult> DeleteBookAsync(string isbn, IBookService bookService)
    {
        var result = await bookService.DeleteAsync(isbn);
        return result ? Results.NoContent() : Results.NotFound();
    }

    public static void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IBookService, BookService>(); // Updated from Singleton to Scoped
    }
}
