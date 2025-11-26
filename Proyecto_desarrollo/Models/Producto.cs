using System;

namespace Proyecto_desarrollo.Models;

public class Producto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public int Stock { get; set; }
    public string Categoria { get; set; } = "Computadora";
    public string Procesador { get; set; } = string.Empty;
    public string RAM { get; set; } = string.Empty;
    public string Almacenamiento { get; set; } = string.Empty;
    public string TarjetaVideo { get; set; } = string.Empty;
    public string ImagenUrl { get; set; } = "laptop_default.png";
    public DateTime FechaCreacion { get; set; } = DateTime.Now;
}