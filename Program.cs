using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using TodoApi;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("tododbclevercloud");

builder.Services.AddDbContext<ToDoDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder
            .AllowAnyOrigin() //  לקבוע רק דומיינים מסוימים
            .AllowAnyMethod()
            .AllowAnyHeader());
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}
app.UseCors("AllowAll");


app.MapGet("/", () => "Hello World!");
app.MapPost("/login",async (string userName, string password,ToDoDbContext db) => await db.Users.FindAsync(password));
app.MapPost("/register",async (string userName, string password,ToDoDbContext db) => await db.Users.FindAsync(password));

app.MapGet("/items",  async (ToDoDbContext db) =>  await db.Items.ToListAsync());
app.MapPost("/items/{name}", async (ToDoDbContext db,string name) =>{
    var item = new Item(){Name=name,IsComplete=false};
    db.Items.Add(item);
    await db.SaveChangesAsync();
    return Results.Ok($"item created successfully");
});
app.MapPut("/items/{id}&{isComplete}", async (ToDoDbContext db,int id,bool isComplete) => {
    var existingEntity = await db.Items.FindAsync(id);
     if (existingEntity is null)
    {
        return Results.NotFound();
    }
    existingEntity.IsComplete=isComplete;
    await db.SaveChangesAsync();
    return Results.Ok($"item {id} update successfully");    

});
app.MapDelete("/items/{id}", async (ToDoDbContext db,int id) => {
    var existingEntity = await db.Items.FindAsync(id);
     if (existingEntity is null)
    {
        return Results.NotFound();
    }
    db.Items.Remove(existingEntity);
    await db.SaveChangesAsync();
    return Results.Ok($"item {id} deleted successfully");
});


app.Run();






// app.UseAuthentication();
// app.UseAuthorization();
// app.MapMethods("/options-or-head", new[] { "OPTIONS", "HEAD" }, 
//                           () => "This is an options or head request ");
// .WithOrigins("https://example.com") // דומיין מורשה
//         .WithMethods("GET", "POST") // שיטות מורשות
//         .WithHeaders("content-type")); // כותרות מורשות

//dotnet watch run  //הרצה