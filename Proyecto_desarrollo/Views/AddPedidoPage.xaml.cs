using Proyecto_desarrollo.Models;
using Proyecto_desarrollo.Services;

namespace Proyecto_desarrollo.Views;

public partial class AddPedidoPage : ContentPage
{
    private readonly ApiService _apiService = new ApiService();
    private List<Cliente> _clientes = new();
    private List<Producto> _productos = new();
    private Producto _productoSeleccionado;

    public AddPedidoPage()
    {
        InitializeComponent();
        EstadoPicker.SelectedIndex = 0;
        _ = CargarDatosAsync();
    }

    private async Task CargarDatosAsync()
    {
        try
        {
            _clientes = await _apiService.GetClientesAsync();
            _productos = await _apiService.GetProductosAsync();

            ClientePicker.ItemsSource = _clientes;
            ProductoPicker.ItemsSource = _productos;

            if (_clientes.Any())
                ClientePicker.SelectedIndex = 0;
            if (_productos.Any())
                ProductoPicker.SelectedIndex = 0;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se pudieron cargar los datos: {ex.Message}", "OK");
        }
    }

    private void OnProductoSeleccionado(object sender, EventArgs e)
    {
        if (ProductoPicker.SelectedItem is Producto producto)
        {
            _productoSeleccionado = producto;
            CalcularTotal();
        }
    }

    private void OnCantidadCambiada(object sender, TextChangedEventArgs e)
    {
        CalcularTotal();
    }

    private void CalcularTotal()
    {
        if (_productoSeleccionado != null && int.TryParse(CantidadEntry.Text, out int cantidad) && cantidad > 0)
        {
            var total = _productoSeleccionado.Precio * cantidad;
            TotalEntry.Text = total.ToString("F2");
        }
        else
        {
            TotalEntry.Text = "0.00";
        }
    }

    private async void OnCancelarClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private async void OnGuardarClicked(object sender, EventArgs e)
    {
        if (ClientePicker.SelectedItem == null ||
            ProductoPicker.SelectedItem == null ||
            string.IsNullOrWhiteSpace(CantidadEntry.Text))
        {
            await DisplayAlert("Error", "Por favor completa todos los campos requeridos", "OK");
            return;
        }

        if (!int.TryParse(CantidadEntry.Text, out int cantidad) || cantidad <= 0)
        {
            await DisplayAlert("Error", "Por favor ingresa una cantidad válida mayor a 0", "OK");
            return;
        }

        var clienteSeleccionado = (Cliente)ClientePicker.SelectedItem;
        var productoSeleccionado = (Producto)ProductoPicker.SelectedItem;

        var nuevoPedido = new Pedido
        {
            ClienteId = clienteSeleccionado.Id,
            ProductoId = productoSeleccionado.Id,
            Cantidad = cantidad,
            Total = productoSeleccionado.Precio * cantidad,
            FechaPedido = DateTime.Now,
            Estado = EstadoPicker.SelectedItem?.ToString() ?? "Pendiente"
        };

        var pedidoGuardado = await _apiService.AddPedidoAsync(nuevoPedido);

        if (pedidoGuardado != null)
        {
            await DisplayAlert("✅ Éxito",
                $"Pedido creado en el servidor:\n\n" +
                $"📦 ID: {pedidoGuardado.Id}\n" +
                $"👤 Cliente: {clienteSeleccionado.Nombre}\n" +
                $"💻 Producto: {productoSeleccionado.Nombre}\n" +
                $"📦 Cantidad: {pedidoGuardado.Cantidad}\n" +
                $"💰 Total: ${pedidoGuardado.Total:F2}\n" +
                $"📋 Estado: {pedidoGuardado.Estado}",
                "OK");
        }
        else
        {
            await DisplayAlert("⚠️ Modo Local",
                "Pedido guardado localmente (sin conexión al servidor)",
                "OK");
        }

        await Navigation.PopAsync();
    }
}