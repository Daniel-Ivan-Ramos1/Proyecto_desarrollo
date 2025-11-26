using System.Text;
using System.Text.Json;
using Proyecto_desarrollo.Models;

namespace Proyecto_desarrollo.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiBaseUrl = "https://localhost:7125/api"; // URL de tu API local

    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    public ApiService()
    {
        _httpClient = new HttpClient();

        // Configurar timeout
        _httpClient.Timeout = TimeSpan.FromSeconds(30);
    }

    // PRODUCTOS
    public async Task<List<Producto>> GetProductosAsync()
    {
        try
        {
            var response = await _httpClient.GetStringAsync($"{_apiBaseUrl}/productos");
            var productos = JsonSerializer.Deserialize<List<Producto>>(response, _jsonOptions);
            return productos ?? new List<Producto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error al obtener productos: {ex.Message}");
            // Fallback a datos de ejemplo si no hay conexión
            return GetProductosEjemplo();
        }
    }

    public async Task<Producto> AddProductoAsync(Producto producto)
    {
        try
        {
            var json = JsonSerializer.Serialize(producto, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_apiBaseUrl}/productos", content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<Producto>(responseContent, _jsonOptions);
            }
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error al agregar producto: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> UpdateProductoAsync(Producto producto)
    {
        try
        {
            var json = JsonSerializer.Serialize(producto, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"{_apiBaseUrl}/productos/{producto.Id}", content);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error al actualizar producto: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> DeleteProductoAsync(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"{_apiBaseUrl}/productos/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error al eliminar producto: {ex.Message}");
            return false;
        }
    }

    // CLIENTES
    public async Task<List<Cliente>> GetClientesAsync()
    {
        try
        {
            var response = await _httpClient.GetStringAsync($"{_apiBaseUrl}/clientes");
            var clientes = JsonSerializer.Deserialize<List<Cliente>>(response, _jsonOptions);
            return clientes ?? new List<Cliente>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error al obtener clientes: {ex.Message}");
            return GetClientesEjemplo();
        }
    }

    public async Task<Cliente> AddClienteAsync(Cliente cliente)
    {
        try
        {
            var json = JsonSerializer.Serialize(cliente, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_apiBaseUrl}/clientes", content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<Cliente>(responseContent, _jsonOptions);
            }
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error al agregar cliente: {ex.Message}");
            return null;
        }
    }

    // Datos de ejemplo para desarrollo (fallback)
    private List<Producto> GetProductosEjemplo()
    {
        return new List<Producto>
        {
            new Producto
            {
                Id = 1,
                Nombre = "📱 Laptop Gamer Pro (Local)",
                Descripcion = "Intel i7, 16GB RAM, 1TB SSD, RTX 4060",
                Precio = 1299.99m,
                Stock = 5,
                Procesador = "Intel Core i7",
                RAM = "16GB DDR5",
                Almacenamiento = "1TB NVMe SSD",
                TarjetaVideo = "NVIDIA RTX 4060"
            }
        };
    }

    private List<Cliente> GetClientesEjemplo()
    {
        return new List<Cliente>
        {
            new Cliente
            {
                Id = 1,
                Nombre = "👤 Cliente Local",
                Email = "local@email.com",
                Telefono = "555-0000",
                Direccion = "Dirección local"
            }
        };
    }
}