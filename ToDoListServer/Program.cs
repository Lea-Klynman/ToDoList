using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

using TodoApi;

var builder = WebApplication.CreateBuilder(args);

// Get the connection string for the database

// Configure the DbContext with MySQL
builder.Services.AddDbContext<ToDoDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("ToDoDB"),
    Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.41-mysql"))
);

// Configure CORS to allow all origins
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});


// Add services for Swagger
// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen(options =>
// {
//     options.SwaggerDoc("v1", new OpenApiInfo
//     {
//         Title = "ToDo API",
//         Version = "v1",
//         Description = "A simple ToDo API to manage tasks."
//     });
// });

var app = builder.Build();

// Use CORS policy before any other middleware that handles requests
app.UseCors("AllowAllOrigins");

// Enable Swagger UI
// app.UseSwagger();
// app.UseSwaggerUI(c =>
// {
//     c.SwaggerEndpoint("/swagger/v1/swagger.json", "ToDo API V1");
//     c.RoutePrefix = string.Empty; 
// });

// Define endpoint to get all items
app.MapGet("/items", async (ToDoDbContext context) =>
{
    var items = await context.Items.ToListAsync();
    return items.Any() ? Results.Ok(items) : Results.NoContent();
});

// Define a simple health check endpoint
app.MapGet("/", () => "ToDo API is running");

// Define endpoint to create a new item
app.MapPost("/items", async ( Item newItem,ToDoDbContext db) =>
{
    db.Items.Add(newItem);
    await db.SaveChangesAsync();
    return Results.Created($"/items/{newItem.Id}", newItem);
});




// Define endpoint to update an existing item
app.MapPut("/items/{id}", async (int id, Item updatedItem, ToDoDbContext db) =>
{
    var item = await db.Items.FindAsync(id);
    if (item is null) return Results.NotFound();

    item.IsComplete = updatedItem.IsComplete; // Update the IsComplete property
    await db.SaveChangesAsync();
    return Results.NoContent();
});

// Define endpoint to delete an item
app.MapDelete("/items/{id}", async (ToDoDbContext db, int id) =>
{
    var item = await db.Items.FindAsync(id);
    if (item is null) return Results.NotFound();

    db.Items.Remove(item);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

// Run the application
app.Run();