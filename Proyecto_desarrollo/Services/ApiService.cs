using System.Text;
using System.Text.Json;
using Proyecto_desarrollo.Models;

namespace Proyecto_desarrollo.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiBaseUrl = "https://localhost:7125/api";

    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    public ApiService()
    {
        _httpClient = new HttpClient();
        _httpClient.Timeout = TimeSpan.FromSeconds(30);
    }

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

    public async Task<bool> UpdateClienteAsync(Cliente cliente)
    {
        try
        {
            var json = JsonSerializer.Serialize(cliente, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"{_apiBaseUrl}/clientes/{cliente.Id}", content);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error al actualizar cliente: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> DeleteClienteAsync(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"{_apiBaseUrl}/clientes/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error al eliminar cliente: {ex.Message}");
            return false;
        }
    }

    public async Task<List<Pedido>> GetPedidosAsync()
    {
        try
        {
            var response = await _httpClient.GetStringAsync($"{_apiBaseUrl}/pedidos");
            var pedidos = JsonSerializer.Deserialize<List<Pedido>>(response, _jsonOptions);
            return pedidos ?? new List<Pedido>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error al obtener pedidos: {ex.Message}");
            return GetPedidosEjemplo();
        }
    }

    public async Task<Pedido> AddPedidoAsync(Pedido pedido)
    {
        try
        {
            Console.WriteLine($"🌐 ENVIANDO PEDIDO A: {_apiBaseUrl}/pedidos");
            Console.WriteLine($"📦 DATOS: ClienteId={pedido.ClienteId}, ProductoId={pedido.ProductoId}, Cantidad={pedido.Cantidad}, Total={pedido.Total}");

            var json = JsonSerializer.Serialize(pedido, _jsonOptions);
            Console.WriteLine($"📋 JSON: {json}");

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_apiBaseUrl}/pedidos", content);

            Console.WriteLine($"📡 RESPUESTA HTTP: {response.StatusCode}");

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"✅ PEDIDO CREADO: {responseContent}");
                return JsonSerializer.Deserialize<Pedido>(responseContent, _jsonOptions);
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"❌ ERROR SERVIDOR - Status: {response.StatusCode}, Mensaje: {errorContent}");
                return null;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"💥 ERROR CONEXIÓN: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"💥 DETALLES: {ex.InnerException.Message}");
            }
            return null;
        }
    }

    public async Task<bool> UpdatePedidoAsync(Pedido pedido)
    {
        try
        {
            var json = JsonSerializer.Serialize(pedido, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"{_apiBaseUrl}/pedidos/{pedido.Id}", content);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error al actualizar pedido: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> DeletePedidoAsync(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"{_apiBaseUrl}/pedidos/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error al eliminar pedido: {ex.Message}");
            return false;
        }
    }

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

    private List<Pedido> GetPedidosEjemplo()
    {
        return new List<Pedido>
        {
            new Pedido
            {
                Id = 1,
                ClienteId = 1,
                ProductoId = 1,
                Cantidad = 1,
                Total = 1299.99m,
                FechaPedido = DateTime.Now.AddDays(-5),
                Estado = "Completado (Local)"
            },
            new Pedido
            {
                Id = 2,
                ClienteId = 1,
                ProductoId = 2,
                Cantidad = 2,
                Total = 1799.98m,
                FechaPedido = DateTime.Now.AddDays(-2),
                Estado = "Pendiente (Local)"
            }
        };
    }
}