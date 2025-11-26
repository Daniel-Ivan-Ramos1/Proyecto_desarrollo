using Proyecto_desarrollo.Models;
using Proyecto_desarrollo.Services;

namespace Proyecto_desarrollo.Views;

public partial class AddClientePage : ContentPage
{
    public AddClientePage()
    {
        InitializeComponent();
    }

    private async void OnCancelarClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private async void OnGuardarClicked(object sender, EventArgs e)
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

        var nuevoCliente = new Cliente
        {
            Nombre = NombreEntry.Text,
            Email = EmailEntry.Text,
            Telefono = TelefonoEntry.Text ?? string.Empty,
            Direccion = DireccionEditor.Text ?? string.Empty,
            FechaRegistro = DateTime.UtcNow
        };

        var apiService = new ApiService();
        var clienteGuardado = await apiService.AddClienteAsync(nuevoCliente);

        if (clienteGuardado != null)
        {
            await DisplayAlert("✅ Éxito",
                $"Cliente guardado en el servidor:\n\n" +
                $"{clienteGuardado.Nombre}\n" +
                $"Email: {clienteGuardado.Email}\n" +
                $"Teléfono: {clienteGuardado.Telefono}",
                "OK");
        }
        else
        {
            await DisplayAlert("⚠️ Modo Local",
                "Cliente guardado localmente (sin conexión al servidor)",
                "OK");
        }

        await Navigation.PopAsync();
    }
}