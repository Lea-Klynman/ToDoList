using Microsoft.EntityFrameworkCore;
using TodoApi;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer(); 
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

builder.Services.AddDbContext<ToDoDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 0))
    ));
var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors();


app.MapGet("/", () => "TodoList API is running");

app.MapGet("/items", async (ToDoDbContext db) =>
{
    var items = await db.Items.ToListAsync();
    return Results.Ok(items);
});

app.MapGet("/items/{id}", async (int id, ToDoDbContext db) =>
    await db.Items.FindAsync(id) is Item item ? Results.Ok(item) : Results.NotFound());


app.MapPost("/", async (Item item, ToDoDbContext db) =>
{
    db.Add(item);
    await db.SaveChangesAsync();
    return Results.Created($"/items/{item.Id}", item);

});

app.MapPut("/items/{id}", async (int id, bool iscomplete, ToDoDbContext db) =>
{
    var item = await db.Items.FindAsync(id);
    if (item is null) return Results.NotFound();

    item.IsComplete = iscomplete;

    await db.SaveChangesAsync();
    return Results.NoContent();
});


app.MapDelete("/items/{id}", async (int id, ToDoDbContext db) =>
{
    var item = await db.Items.FindAsync(id);
    if (item is null) return Results.NotFound();

    db.Items.Remove(item);
    await db.SaveChangesAsync();
    return Results.NoContent();
});
app.Run();
// using Microsoft.EntityFrameworkCore;
// using TodoApi;

// var builder = WebApplication.CreateBuilder(args);

// // Injecting the DbContext
// builder.Services.AddDbContext<ToDoDbContext>(/*options =>
//     options.UseMySql("name=ToDoDB", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.41-mysql"))
// */);

// builder.Services.AddCors(options =>
// {
//     options.AddPolicy("AllowAllOrigins",
//         builder => builder.AllowAnyOrigin()
//                           .AllowAnyMethod()
//                           .AllowAnyHeader());
// });

// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();

// var app = builder.Build();

// // Use CORS before other middleware
// app.UseCors("AllowAllOrigins");

// app.UseSwagger();
// app.UseSwaggerUI(/*c =>
// {
//     c.SwaggerEndpoint("/swagger/v1/swagger.json", "ToDo API V1");
//     c.RoutePrefix = string.Empty; // This allows you to access Swagger at the root address
// }*/);

// // Get all items
// app.MapGet("/items", async (ToDoDbContext db) =>
// {
//     return await db.Items.ToListAsync();
// });

// // Add a new item
// app.MapPost("/items", async (ToDoDbContext db, Item newItem) =>
// {
//     try
//     {
//         db.Items.Add(newItem);
//         await db.SaveChangesAsync();
//         return Results.Created($"/items/{newItem.Id}", newItem);
//     }
//     catch (Exception ex)
//     {
//         return Results.Problem(ex.Message);
//     }
// });

// // Update an item
// app.MapPut("/items/{id}", async (ToDoDbContext db, int id, Item updatedItem) =>
// {
//     var existingItem = await db.Items.FindAsync(id);
//     if (existingItem is null) return Results.NotFound();

//     existingItem.IsComplete = updatedItem.IsComplete;

//     await db.SaveChangesAsync();
//     return Results.NoContent();
// });

// // Delete an item
// app.MapDelete("/items/{id}", async (ToDoDbContext db, int id) =>
// {
//     var item = await db.Items.FindAsync(id);
//     if (item is null) return Results.NotFound();

//     db.Items.Remove(item);
//     await db.SaveChangesAsync();
//     return Results.NoContent();
// });

// app.Run();
