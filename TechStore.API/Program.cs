using Microsoft.EntityFrameworkCore;
using TechStore.API.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configurar Entity Framework con Azure SQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configurar CORS para permitir requests desde MAUI
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowMAUI", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowMAUI");
app.UseAuthorization();
app.MapControllers();

// Crear y sembrar la base de datos automáticamente en Azure SQL
using (var scope = app.Services.CreateScope())
{
    try
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.Database.EnsureCreated(); // Crea las tablas si no existen
        Console.WriteLine("✅ Base de datos Azure SQL configurada correctamente");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error al conectar con Azure SQL: {ex.Message}");
        throw;
    }
}

app.Run();