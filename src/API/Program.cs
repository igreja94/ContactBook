using Infrastructure; // Import your extension namespace

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.Services.ApplyMigrations();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();