using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechStore.API.Data;
using TechStore.API.Models;

namespace TechStore.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PedidosController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<PedidosController> _logger;

    public PedidosController(AppDbContext context, ILogger<PedidosController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Pedido>>> GetPedidos()
    {
        _logger.LogInformation("📦 GET Pedidos - Obteniendo todos los pedidos");
        var pedidos = await _context.Pedidos.ToListAsync();
        _logger.LogInformation($"📦 GET Pedidos - Encontrados {pedidos.Count} pedidos");
        return pedidos;
    }

    [HttpPost]
    public async Task<ActionResult<Pedido>> PostPedido(Pedido pedido)
    {
        try
        {
            _logger.LogInformation($"📦 POST Pedido - Creando nuevo pedido: ClienteId={pedido.ClienteId}, ProductoId={pedido.ProductoId}, Cantidad={pedido.Cantidad}");

            _context.Pedidos.Add(pedido);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"✅ POST Pedido - Pedido creado exitosamente: Id={pedido.Id}");

            return CreatedAtAction(nameof(GetPedido), new { id = pedido.Id }, pedido);
        }
        catch (Exception ex)
        {
            _logger.LogError($"❌ POST Pedido - Error: {ex.Message}");
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutPedido(int id, Pedido pedido)
    {
        _logger.LogInformation($"📦 PUT Pedido - Actualizando pedido {id}");

        if (id != pedido.Id)
        {
            _logger.LogWarning($"❌ PUT Pedido - ID mismatch: {id} != {pedido.Id}");
            return BadRequest();
        }

        _context.Entry(pedido).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        _logger.LogInformation($"✅ PUT Pedido - Pedido {id} actualizado exitosamente");
        return NoContent();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Pedido>> GetPedido(int id)
    {
        var pedido = await _context.Pedidos.FindAsync(id);
        if (pedido == null)
        {
            _logger.LogWarning($"❌ GET Pedido {id} - No encontrado");
            return NotFound();
        }

        _logger.LogInformation($"✅ GET Pedido {id} - Encontrado");
        return pedido;
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePedido(int id)
    {
        _logger.LogInformation($"📦 DELETE Pedido - Eliminando pedido {id}");

        var pedido = await _context.Pedidos.FindAsync(id);
        if (pedido == null)
        {
            _logger.LogWarning($"❌ DELETE Pedido {id} - No encontrado");
            return NotFound();
        }

        _context.Pedidos.Remove(pedido);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"✅ DELETE Pedido - Pedido {id} eliminado exitosamente");
        return NoContent();
    }
}