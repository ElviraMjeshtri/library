using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Web.API.Auth;
using Web.API.Data;
using Web.API.Endpoints.Internal;
using Web.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure Entity Framework Core with PostgreSQL
builder.Services.AddDbContext<LibraryDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register application services
builder.Services.AddScoped<IBookService, BookService>();

// Register FluentValidation
builder.Services
    .AddValidatorsFromAssemblyContaining<Program>();

// Register custom endpoints
builder.Services.AddEndpoints<Program>(builder.Configuration);

// Configure authentication and authorization
builder.Services.AddAuthentication(ApiKeySchemeConstants.SchemeName)
    .AddScheme<ApiKeyAuthSchemeOptions, ApiKeyAuthHandler>(ApiKeySchemeConstants.SchemeName, _ => { });
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication(); // Ensure authentication middleware is used
app.UseAuthorization();

app.UseEndpoints<Program>();

// Run the application
app.Run();