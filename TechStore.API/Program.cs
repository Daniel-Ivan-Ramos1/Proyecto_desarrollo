using Microsoft.EntityFrameworkCore;
using TechStore.API.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS - PERMITE TODOS LOS ORIGENES
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// 🔥 CONFIGURACIÓN CRÍTICA: Escuchar en TODAS las interfaces
app.Urls.Clear();
app.Urls.Add("http://0.0.0.0:5259");   // ← ESTA LÍNEA ES CLAVE
app.Urls.Add("https://0.0.0.0:7125");  // ← Y ESTA TAMBIÉN

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");  // ← USA CORS
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

Console.WriteLine("🌐 API configurada para escuchar en:");
Console.WriteLine("   http://0.0.0.0:5259");
Console.WriteLine("   https://0.0.0.0:7125");
Console.WriteLine("📡 URLs accesibles:");
Console.WriteLine($"   http://localhost:5259");
Console.WriteLine($"   http://192.168.0.2:5259");
Console.WriteLine($"   http://[TU_IP_LOCAL]:5259");

app.Run();