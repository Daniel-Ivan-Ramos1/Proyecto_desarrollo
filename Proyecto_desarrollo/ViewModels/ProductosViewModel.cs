using System.Collections.ObjectModel;
using System.Windows.Input;
using Proyecto_desarrollo.Models;

namespace Proyecto_desarrollo.ViewModels;

public class ProductosViewModel : BaseViewModel
{
    private readonly ObservableCollection<Producto> _productos;

    public ObservableCollection<Producto> Productos { get; }

    public ICommand AgregarProductoCommand { get; }
    public ICommand EditarProductoCommand { get; }
    public ICommand EliminarProductoCommand { get; }

    public ProductosViewModel()
    {
        Title = "💻 Productos";
        Productos = new ObservableCollection<Producto>();
        AgregarProductoCommand = new Command(OnAgregarProducto);

        // Datos de ejemplo
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
        await Application.Current.MainPage.DisplayAlert("MVVM", "Agregar producto con MVVM", "OK");
    }
}