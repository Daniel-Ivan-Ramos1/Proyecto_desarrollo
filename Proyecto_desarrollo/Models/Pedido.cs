using System;

namespace Proyecto_desarrollo.Models;

public class Pedido
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public int ProductoId { get; set; }
    public int Cantidad { get; set; }
    public decimal Total { get; set; }
    public DateTime FechaPedido { get; set; } = DateTime.Now;
    public string Estado { get; set; } = "Pendiente";
}