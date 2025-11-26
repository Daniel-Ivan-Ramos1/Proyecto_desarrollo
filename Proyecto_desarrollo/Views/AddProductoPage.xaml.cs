using Proyecto_desarrollo.Models;
using Proyecto_desarrollo.Services;

namespace Proyecto_desarrollo.Views;

public partial class AddProductoPage : ContentPage
{
    public AddProductoPage()
    {
        InitializeComponent();
    }

    private async void OnCancelarClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private async void OnGuardarClicked(object sender, EventArgs e)
    {
        // Validar campos
        if (string.IsNullOrWhiteSpace(NombreEntry.Text) ||
            string.IsNullOrWhiteSpace(DescripcionEditor.Text) ||
            string.IsNullOrWhiteSpace(PrecioEntry.Text))
        {
            await DisplayAlert("Error", "Por favor completa todos los campos requeridos", "OK");
            return;
        }

        // Validar que el precio sea un número válido
        if (!decimal.TryParse(PrecioEntry.Text, out decimal precio))
        {
            await DisplayAlert("Error", "Por favor ingresa un precio válido", "OK");
            return;
        }

        // Crear nuevo producto
        var nuevoProducto = new Producto
        {
            Nombre = NombreEntry.Text,
            Descripcion = DescripcionEditor.Text,
            Precio = precio,
            Stock = int.TryParse(StockEntry.Text, out int stock) ? stock : 0,
            Procesador = ProcesadorEntry.Text ?? string.Empty,
            RAM = RAMEntry.Text ?? string.Empty,
            Almacenamiento = AlmacenamientoEntry.Text ?? string.Empty,
            TarjetaVideo = TarjetaVideoEntry.Text ?? string.Empty
        };

        // Guardar en el API
        var apiService = new ApiService();
        var productoGuardado = await apiService.AddProductoAsync(nuevoProducto);

        if (productoGuardado != null)
        {
            await DisplayAlert("✅ Éxito",
                $"Producto guardado en el servidor:\n\n" +
                $"{productoGuardado.Nombre}\n" +
                $"Precio: ${productoGuardado.Precio:F2}\n" +
                $"Stock: {productoGuardado.Stock}",
                "OK");
        }
        else
        {
            await DisplayAlert("⚠️ Modo Local",
                "Producto guardado localmente (sin conexión al servidor)",
                "OK");
        }

        // Regresar a la página anterior
        await Navigation.PopAsync();
    }
}