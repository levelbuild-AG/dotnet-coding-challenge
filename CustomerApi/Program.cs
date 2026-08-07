using CustomerApi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.OData;
using CustomerApi.Middleware;

var builder = WebApplication.CreateBuilder(args);

// This line connects our app to the database file.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// This lets the app use "Controllers" - the files where we'll write
// our Customer and User code (create, read, update, delete).
builder.Services.AddControllers().AddOData(options =>
    options.Select().Filter().OrderBy().SetMaxTop(100).SkipToken());

// This adds Swagger - a webpage that lets you test the API by clicking buttons,
// instead of typing commands.
builder.Services.AddOpenApi();

var app = builder.Build();
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "CustomerApi v1");
    });
}

app.UseHttpsRedirection();

// This tells the app: "go find all the Controllers and turn them into real web addresses"
app.MapControllers();

app.Run();