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
    }

    private async void OnAgregarProducto()
    {
        if (Application.Current?.Windows[0].Page != null)
        {
            await Application.Current.Windows[0].Page.DisplayAlert("MVVM", "Agregar producto con MVVM", "OK");
        }
    }

    private async void OnEditarProducto(Producto? producto)
    {
        if (producto == null) return;

        if (Application.Current?.Windows[0].Page != null)
        {
            await Application.Current.Windows[0].Page.DisplayAlert("Editar",
                $"Editarás: {producto.Nombre}", "OK");
        }
    }

    private async void OnEliminarProducto(Producto? producto)
    {
        if (producto == null) return;

        if (Application.Current?.Windows[0].Page != null)
        {
            bool confirmar = await Application.Current.Windows[0].Page.DisplayAlert(
                "Eliminar Producto",
                $"¿Estás seguro de eliminar {producto.Nombre}?",
                "Sí, eliminar", "Cancelar");

            if (confirmar)
            {
                Productos.Remove(producto);
                await Application.Current.Windows[0].Page.DisplayAlert("Éxito",
                    "Producto eliminado", "OK");
            }
        }
    }

    private async void OnVerDetalles(Producto? producto)
    {
        if (producto == null) return;

        if (Application.Current?.Windows[0].Page != null)
        {
            await Application.Current.Windows[0].Page.DisplayAlert("Detalles",
                $"{producto.Nombre}\n\n{producto.Descripcion}\n\nPrecio: ${producto.Precio:F2}\nStock: {producto.Stock}",
                "Cerrar");
        }
    }
}