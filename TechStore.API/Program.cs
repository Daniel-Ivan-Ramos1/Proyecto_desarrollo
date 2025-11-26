using Microsoft.EntityFrameworkCore;
using TechStore.API.Data;
using TechStore.API.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
Console.WriteLine($"🔗 Conectando a Azure SQL: {connectionString}");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowMAUI");
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    try
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        Console.WriteLine("🚀 Configurando base de datos Azure SQL...");

        var created = await dbContext.Database.EnsureCreatedAsync();
        Console.WriteLine($"✅ Base de datos creada: {created}");

        var productosCount = await dbContext.Productos.CountAsync();
        var clientesCount = await dbContext.Clientes.CountAsync();
        var pedidosCount = await dbContext.Pedidos.CountAsync();

        Console.WriteLine($"📊 Productos en BD: {productosCount}");
        Console.WriteLine($"👥 Clientes en BD: {clientesCount}");
        Console.WriteLine($"📦 Pedidos en BD: {pedidosCount}");

        if (pedidosCount == 0 && productosCount > 0 && clientesCount > 0)
        {
            Console.WriteLine("🌱 Insertando pedidos de ejemplo...");

            var producto1 = await dbContext.Productos.FindAsync(1);
            var producto2 = await dbContext.Productos.FindAsync(2);
            var cliente1 = await dbContext.Clientes.FindAsync(1);
            var cliente2 = await dbContext.Clientes.FindAsync(2);

            if (producto1 != null && cliente1 != null)
            {
                dbContext.Pedidos.AddRange(
                    new Pedido
                    {
                        ClienteId = cliente1.Id,
                        ProductoId = producto1.Id,
                        Cantidad = 1,
                        Total = producto1.Precio * 1,
                        Estado = "Completado",
                        FechaPedido = DateTime.UtcNow.AddDays(-2)
                    },
                    new Pedido
                    {
                        ClienteId = cliente2.Id,
                        ProductoId = producto2.Id,
                        Cantidad = 2,
                        Total = producto2.Precio * 2,
                        Estado = "Pendiente",
                        FechaPedido = DateTime.UtcNow.AddDays(-1)
                    }
                );

                await dbContext.SaveChangesAsync();
                Console.WriteLine("✅ Pedidos de ejemplo insertados");
            }
        }

        Console.WriteLine("🎯 API lista para recibir requests");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error al configurar la BD: {ex.Message}");
        if (ex.InnerException != null)
        {
            Console.WriteLine($"❌ Detalles: {ex.InnerException.Message}");
        }
    }
}

app.Run();