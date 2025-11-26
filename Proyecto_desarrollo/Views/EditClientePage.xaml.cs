using Proyecto_desarrollo.Models;
using Proyecto_desarrollo.Services;

namespace Proyecto_desarrollo.Views;

public partial class EditClientePage : ContentPage
{
    private Cliente _cliente;
    private readonly ApiService _apiService = new ApiService();

    public EditClientePage(Cliente cliente)
    {
        InitializeComponent();
        _cliente = cliente;
        CargarDatosCliente();
    }

    private void CargarDatosCliente()
    {
        if (_cliente != null)
        {
            NombreEntry.Text = _cliente.Nombre;
            EmailEntry.Text = _cliente.Email;
            TelefonoEntry.Text = _cliente.Telefono;
            DireccionEditor.Text = _cliente.Direccion;
        }
    }

    private async void OnCancelarClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private async void OnGuardarCambiosClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(NombreEntry.Text) ||
            string.IsNullOrWhiteSpace(EmailEntry.Text))
        {
            await DisplayAlert("Error", "Por favor completa los campos requeridos (Nombre y Email)", "OK");
            return;
        }

        if (!EmailEntry.Text.Contains("@"))
        {
            await DisplayAlert("Error", "Por favor ingresa un email válido", "OK");
            return;
        }

        _cliente.Nombre = NombreEntry.Text;
        _cliente.Email = EmailEntry.Text;
        _cliente.Telefono = TelefonoEntry.Text ?? string.Empty;
        _cliente.Direccion = DireccionEditor.Text ?? string.Empty;

        var resultado = await _apiService.UpdateClienteAsync(_cliente);

        if (resultado)
        {
            await DisplayAlert("✅ Éxito",
                $"Cliente actualizado en el servidor:\n\n" +
                $"{_cliente.Nombre}\n" +
                $"Email: {_cliente.Email}\n" +
                $"Teléfono: {_cliente.Telefono}",
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