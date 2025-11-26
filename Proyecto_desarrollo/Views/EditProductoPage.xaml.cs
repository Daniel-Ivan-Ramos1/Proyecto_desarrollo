using Proyecto_desarrollo.Models;
using Proyecto_desarrollo.Services;

namespace Proyecto_desarrollo.Views;

public partial class EditProductoPage : ContentPage
{
    private Producto _producto;
    private readonly ApiService _apiService = new ApiService();

    public EditProductoPage(Producto producto)
    {
        InitializeComponent();
        _producto = producto;
        CargarDatosProducto();
    }

    private void CargarDatosProducto()
    {
        if (_producto != null)
        {
            NombreEntry.Text = _producto.Nombre;
            DescripcionEditor.Text = _producto.Descripcion;
            PrecioEntry.Text = _producto.Precio.ToString("F2");
            StockEntry.Text = _producto.Stock.ToString();
            ProcesadorEntry.Text = _producto.Procesador;
            RAMEntry.Text = _producto.RAM;
            AlmacenamientoEntry.Text = _producto.Almacenamiento;
            TarjetaVideoEntry.Text = _producto.TarjetaVideo;
        }
    }

    private async void OnCancelarClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private async void OnGuardarCambiosClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(NombreEntry.Text) ||
            string.IsNullOrWhiteSpace(DescripcionEditor.Text) ||
            string.IsNullOrWhiteSpace(PrecioEntry.Text))
        {
            await DisplayAlert("Error", "Por favor completa todos los campos requeridos", "OK");
            return;
        }

        if (!decimal.TryParse(PrecioEntry.Text, out decimal precio))
        {
            await DisplayAlert("Error", "Por favor ingresa un precio válido", "OK");
            return;
        }

        _producto.Nombre = NombreEntry.Text;
        _producto.Descripcion = DescripcionEditor.Text;
        _producto.Precio = precio;
        _producto.Stock = int.TryParse(StockEntry.Text, out int stock) ? stock : 0;
        _producto.Procesador = ProcesadorEntry.Text ?? string.Empty;
        _producto.RAM = RAMEntry.Text ?? string.Empty;
        _producto.Almacenamiento = AlmacenamientoEntry.Text ?? string.Empty;
        _producto.TarjetaVideo = TarjetaVideoEntry.Text ?? string.Empty;

        var resultado = await _apiService.UpdateProductoAsync(_producto);

        if (resultado)
        {
            await DisplayAlert("✅ Éxito",
                $"Producto actualizado en el servidor:\n\n" +
                $"{_producto.Nombre}\n" +
                $"Precio: ${_producto.Precio:F2}\n" +
                $"Stock: {_producto.Stock}",
                "OK");
        }
        else
        {
            await DisplayAlert("⚠️ Modo Local",
                "Cambios guardados localmente (sin conexión al servidor)",
                "OK");
        }

        await Navigation.PopAsync();
    }
}