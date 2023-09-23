using FullstackBookshelf;
using FullstackBookshelf.Model;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var databaseName = "bookshelfDB";
var collectionName = "books";

var mongoDBConnect = new MongoDBConnect(databaseName, collectionName);

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("/bookshelf", GetAllBooksAsync);

async Task<IEnumerable<Book>> GetAllBooksAsync()
{
    var books = await mongoDBConnect.GetAllBooksAsync();
    return books;
}

app.MapPost("/bookshelf", async (HttpContext context) =>
{
    var book = await context.Request.ReadFromJsonAsync<Book>();
    if (book != null)
    {
        await mongoDBConnect.InsertBookAsync(book);
        context.Response.StatusCode = StatusCodes.Status201Created;
    }
    else
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
    }
});

app.MapDelete("/bookshelf/{id}", async (HttpContext context, int id) =>
{
    if (id > 0)
    {
        await mongoDBConnect.DeleteBookAsync(id);
        context.Response.StatusCode = StatusCodes.Status204NoContent;
    }
    else
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
    }
});

app.Run();