using System.Collections.ObjectModel;
using System.Windows.Input;
using Proyecto_desarrollo.Models;

namespace Proyecto_desarrollo.ViewModels;

public class ProductosViewModel : BaseViewModel
{
    public ObservableCollection<Producto> Productos { get; } = new();
    public ICommand AgregarProductoCommand { get; }
    public ICommand EditarProductoCommand { get; }
    public ICommand EliminarProductoCommand { get; }
    public ICommand VerDetallesCommand { get; }

    public ProductosViewModel()
    {
        Title = "💻 Productos";

        AgregarProductoCommand = new Command(OnAgregarProducto);
        EditarProductoCommand = new Command<Producto>(OnEditarProducto);
        EliminarProductoCommand = new Command<Producto>(OnEliminarProducto);
        VerDetallesCommand = new Command<Producto>(OnVerDetalles);

        CargarProductosEjemplo();
    }

    private void CargarProductosEjemplo()
    {
        Productos.Clear();

        Productos.Add(new Producto
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
        });

        Productos.Add(new Producto
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
        });

        // Agregar más productos de ejemplo
        Productos.Add(new Producto
        {
            Id = 3,
            Nombre = "Desktop Gaming Power",
            Descripcion = "AMD Ryzen 7, 32GB RAM, 2TB SSD, RTX 4070",
            Precio = 1899.99m,
            Stock = 3,
            Procesador = "AMD Ryzen 7",
            RAM = "32GB DDR5",
            Almacenamiento = "2TB NVMe SSD",
            TarjetaVideo = "NVIDIA RTX 4070"
        });
    }

    private async void OnAgregarProducto()
    {
        await DisplayAlert(
            "➕ Agregar Producto",
            "Funcionalidad de agregar producto:\n\n• Aquí se abriría un formulario\n• Para capturar datos del nuevo producto\n• Y enviarlo a la base de datos",
            "👌 Entendido");
    }

    private async void OnEditarProducto(Producto? producto)
    {
        if (producto == null) return;

        await DisplayAlert("✏️ Editar Producto",
            $"Editarás: {producto.Nombre}\n\nPrecio actual: ${producto.Precio:F2}\nStock actual: {producto.Stock}",
            "✅ Continuar");
    }

    private async void OnEliminarProducto(Producto? producto)
    {
        if (producto == null) return;

        bool confirmar = await DisplayAlertConfirm(
            "🗑️ Eliminar Producto",
            $"¿Estás seguro de eliminar:\n\n{producto.Nombre}?",
            "✅ Sí, eliminar",
            "❌ Cancelar");

        if (confirmar)
        {
            Productos.Remove(producto);
            await DisplayAlert("✅ Éxito",
                "Producto eliminado correctamente",
                "Aceptar");
        }
    }

    private async void OnVerDetalles(Producto? producto)
    {
        if (producto == null) return;

        await DisplayAlert(
            "🔍 Detalles del Producto",
            $"**{producto.Nombre}**\n\n📋 {producto.Descripcion}\n\n⚙️ Especificaciones:\n• Procesador: {producto.Procesador}\n• RAM: {producto.RAM}\n• Almacenamiento: {producto.Almacenamiento}\n• Tarjeta de Video: {producto.TarjetaVideo}\n\n💰 Precio: ${producto.Precio:F2}\n📦 Stock: {producto.Stock} unidades",
            "✅ Aceptar");
    }

    // Método helper para mostrar alerts normales
    private async Task DisplayAlert(string title, string message, string cancel)
    {
        if (Application.Current?.MainPage != null)
        {
            await Application.Current.MainPage.DisplayAlert(title, message, cancel);
        }
    }

    // Método helper para mostrar alerts con confirmación (2 botones)
    private async Task<bool> DisplayAlertConfirm(string title, string message, string accept, string cancel)
    {
        if (Application.Current?.MainPage != null)
        {
            return await Application.Current.MainPage.DisplayAlert(title, message, accept, cancel);
        }
        return false;
    }
}