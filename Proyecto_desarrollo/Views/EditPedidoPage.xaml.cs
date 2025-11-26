using Proyecto_desarrollo.Models;
using Proyecto_desarrollo.Services;

namespace Proyecto_desarrollo.Views;

public partial class EditPedidoPage : ContentPage
{
    private Pedido _pedido;
    private readonly ApiService _apiService = new ApiService();
    private List<Cliente> _clientes = new();
    private List<Producto> _productos = new();
    private Producto _productoSeleccionado;

    public EditPedidoPage(Pedido pedido)
    {
        InitializeComponent();
        _pedido = pedido;
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

            var clienteActual = _clientes.FirstOrDefault(c => c.Id == _pedido.ClienteId);
            var productoActual = _productos.FirstOrDefault(p => p.Id == _pedido.ProductoId);

            if (clienteActual != null)
                ClientePicker.SelectedItem = clienteActual;
            if (productoActual != null)
            {
                ProductoPicker.SelectedItem = productoActual;
                _productoSeleccionado = productoActual;
                PrecioUnitarioLabel.Text = $"${productoActual.Precio:F2}";
            }

            CantidadEntry.Text = _pedido.Cantidad.ToString();
            TotalEntry.Text = _pedido.Total.ToString("F2");

            var estados = new List<string> { "Pendiente", "Procesando", "Enviado", "Completado", "Cancelado" };
            var index = estados.IndexOf(_pedido.Estado);
            EstadoPicker.SelectedIndex = index >= 0 ? index : 0;
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
            PrecioUnitarioLabel.Text = $"${producto.Precio:F2}";
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

    private async void OnGuardarCambiosClicked(object sender, EventArgs e)
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

        _pedido.ClienteId = clienteSeleccionado.Id;
        _pedido.ProductoId = productoSeleccionado.Id;
        _pedido.Cantidad = cantidad;
        _pedido.Total = productoSeleccionado.Precio * cantidad;
        _pedido.Estado = EstadoPicker.SelectedItem?.ToString() ?? "Pendiente";

        var resultado = await _apiService.UpdatePedidoAsync(_pedido);

        if (resultado)
        {
            await DisplayAlert("✅ Éxito",
                $"Pedido actualizado en el servidor:\n\n" +
                $"📦 ID: {_pedido.Id}\n" +
                $"👤 Cliente: {clienteSeleccionado.Nombre}\n" +
                $"💻 Producto: {productoSeleccionado.Nombre}\n" +
                $"📦 Cantidad: {_pedido.Cantidad}\n" +
                $"💰 Total: ${_pedido.Total:F2}\n" +
                $"📋 Estado: {_pedido.Estado}",
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