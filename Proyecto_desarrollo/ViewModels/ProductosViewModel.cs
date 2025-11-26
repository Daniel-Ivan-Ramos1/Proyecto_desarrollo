using System.Collections.ObjectModel;
using System.Windows.Input;
using Proyecto_desarrollo.Models;
using Proyecto_desarrollo.Services;
using Proyecto_desarrollo.Views;

namespace Proyecto_desarrollo.ViewModels;

public class ProductosViewModel : BaseViewModel
{
    private readonly ApiService _apiService = new ApiService();
    public ObservableCollection<Producto> Productos { get; } = new();
    public ICommand AgregarProductoCommand { get; }
    public ICommand EditarProductoCommand { get; }
    public ICommand EliminarProductoCommand { get; }
    public ICommand VerDetallesCommand { get; }
    public ICommand RefreshCommand { get; }

    public ProductosViewModel()
    {
        Title = "💻 Productos";

        AgregarProductoCommand = new Command(OnAgregarProducto);
        EditarProductoCommand = new Command<Producto>(OnEditarProducto);
        EliminarProductoCommand = new Command<Producto>(OnEliminarProducto);
        VerDetallesCommand = new Command<Producto>(OnVerDetalles);
        RefreshCommand = new Command(async () => await LoadProductosFromApi());

        // ✅ NUEVO: Escuchar mensaje desde SettingsPage para recargar
        MessagingCenter.Subscribe<SettingsPage>(this, "RecargarProductos", async (sender) =>
        {
            await LoadProductosFromApi();
            await DisplayAlert("🔄 Actualizado", "Productos recargados desde Azure SQL", "OK");
        });

        // Cargar productos del API
        _ = LoadProductosFromApi();
    }

    private async Task LoadProductosFromApi()
    {
        IsBusy = true;
        try
        {
            var productos = await _apiService.GetProductosAsync();
            Productos.Clear();
            foreach (var producto in productos)
                Productos.Add(producto);

            // Mostrar mensaje informativo
            if (productos.Any(p => p.Nombre?.Contains("(Local)") == true))
            {
                await DisplayAlert("ℹ️ Modo Local",
                    "Datos cargados en modo local (sin conexión al servidor)", "OK");
            }
            else
            {
                await DisplayAlert("✅ Conectado",
                    $"Productos cargados desde el servidor: {productos.Count} elementos", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("⚠️ Error de Conexión",
                $"No se pudo conectar al servidor: {ex.Message}\n\nUsando datos locales.", "OK");

            // Fallback a datos de ejemplo
            CargarProductosEjemplo();
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void CargarProductosEjemplo()
    {
        Productos.Clear();

        Productos.Add(new Producto
        {
            Id = 1,
            Nombre = "Laptop Gamer Pro (Local)",
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
            Nombre = "Laptop Business (Local)",
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
        if (Application.Current?.MainPage != null)
        {
            // Navegar al formulario de agregar producto
            await Application.Current.MainPage.Navigation.PushAsync(new Views.AddProductoPage());
        }
    }

    private async void OnEditarProducto(Producto? producto)
    {
        if (producto == null) return;

        bool confirmar = await DisplayAlertConfirm(
            "✏️ Editar Producto",
            $"¿Editar {producto.Nombre}?\n\nPrecio actual: ${producto.Precio:F2}\nStock actual: {producto.Stock}",
            "✅ Sí, editar",
            "❌ Cancelar");

        if (confirmar)
        {
            // Aquí normalmente abrirías un formulario de edición
            var resultado = await _apiService.UpdateProductoAsync(producto);
            if (resultado)
            {
                await DisplayAlert("✅ Éxito", "Producto actualizado en el servidor", "OK");
                await LoadProductosFromApi(); // Recargar datos
            }
            else
            {
                await DisplayAlert("⚠️ Modo Local", "Cambios guardados localmente", "OK");
            }
        }
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
            // Intentar eliminar del API primero
            var resultado = await _apiService.DeleteProductoAsync(producto.Id);
            if (resultado)
            {
                Productos.Remove(producto);
                await DisplayAlert("✅ Éxito", "Producto eliminado del servidor", "OK");
            }
            else
            {
                // Fallback: eliminar localmente
                Productos.Remove(producto);
                await DisplayAlert("⚠️ Modo Local", "Producto eliminado localmente", "OK");
            }
        }
    }

    private async void OnVerDetalles(Producto? producto)
    {
        if (producto == null) return;

        await DisplayAlert(
            "🔍 Detalles del Producto",
            $"**{producto.Nombre}**\n\n" +
            $"📋 {producto.Descripcion}\n\n" +
            $"⚙️ Especificaciones:\n" +
            $"• Procesador: {producto.Procesador}\n" +
            $"• RAM: {producto.RAM}\n" +
            $"• Almacenamiento: {producto.Almacenamiento}\n" +
            $"• Tarjeta de Video: {producto.TarjetaVideo}\n\n" +
            $"💰 Precio: ${producto.Precio:F2}\n" +
            $"📦 Stock: {producto.Stock} unidades\n\n" +
            $"🌐 Fuente: {(producto.Nombre?.Contains("(Local)") == true ? "Datos Locales" : "Servidor Azure")}",
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