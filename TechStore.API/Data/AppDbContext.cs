using Microsoft.EntityFrameworkCore;
using TechStore.API.Models;

namespace TechStore.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Producto> Productos { get; set; }
    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<Pedido> Pedidos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configurar datos iniciales (seed data)
        modelBuilder.Entity<Producto>().HasData(
            new Producto
            {
                Id = 1,
                Nombre = "Laptop Gamer Pro",
                Descripcion = "Intel i7, 16GB RAM, 1TB SSD, RTX 4060",
                Precio = 1299.99m,
                Stock = 5,
                Procesador = "Intel Core i7",
                RAM = "16GB DDR5",
                Almacenamiento = "1TB NVMe SSD",
                TarjetaVideo = "NVIDIA RTX 4060"
            },
            new Producto
            {
                Id = 2,
                Nombre = "Laptop Business",
                Descripcion = "Intel i5, 8GB RAM, 512GB SSD",
                Precio = 899.99m,
                Stock = 8,
                Procesador = "Intel Core i5",
                RAM = "8GB DDR4",
                Almacenamiento = "512GB SSD",
                TarjetaVideo = "Gráficos Integrados"
            }
        );

        modelBuilder.Entity<Cliente>().HasData(
            new Cliente
            {
                Id = 1,
                Nombre = "Juan Pérez",
                Email = "juan@email.com",
                Telefono = "555-1234",
                Direccion = "Av. Principal #123"
            },
            new Cliente
            {
                Id = 2,
                Nombre = "María García",
                Email = "maria@email.com",
                Telefono = "555-5678",
                Direccion = "Calle Secundaria #456"
            }
        );

        base.OnModelCreating(modelBuilder);
    }
}