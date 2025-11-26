using System.Collections.ObjectModel;
using System.Windows.Input;
using Proyecto_desarrollo.Models;
using Proyecto_desarrollo.Services;
using Proyecto_desarrollo.Views;

namespace Proyecto_desarrollo.ViewModels;

public class ClientesViewModel : BaseViewModel
{
    private readonly ApiService _apiService = new ApiService();
    public ObservableCollection<Cliente> Clientes { get; } = new();
    public ICommand AgregarClienteCommand { get; }
    public ICommand EditarClienteCommand { get; }
    public ICommand EliminarClienteCommand { get; }
    public ICommand VerDetallesCommand { get; }
    public ICommand RefreshCommand { get; }

    public ClientesViewModel()
    {
        Title = "👥 Clientes";

        AgregarClienteCommand = new Command(OnAgregarCliente);
        EditarClienteCommand = new Command<Cliente>(OnEditarCliente);
        EliminarClienteCommand = new Command<Cliente>(OnEliminarCliente);
        VerDetallesCommand = new Command<Cliente>(OnVerDetalles);
        RefreshCommand = new Command(async () => await LoadClientesFromApi());

        MessagingCenter.Subscribe<SettingsPage>(this, "RecargarClientes", async (sender) =>
        {
            await LoadClientesFromApi();
            await DisplayAlert("🔄 Actualizado", "Clientes recargados desde Azure SQL", "OK");
        });

        _ = LoadClientesFromApi();
    }

    private async Task LoadClientesFromApi()
    {
        IsBusy = true;
        try
        {
            var clientes = await _apiService.GetClientesAsync();
            Clientes.Clear();
            foreach (var cliente in clientes)
                Clientes.Add(cliente);

            if (clientes.Any(c => c.Nombre?.Contains("(Local)") == true))
            {
                await DisplayAlert("ℹ️ Modo Local",
                    "Datos cargados en modo local (sin conexión al servidor)", "OK");
            }
            else
            {
                await DisplayAlert("✅ Conectado",
                    $"Clientes cargados desde el servidor: {clientes.Count} elementos", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("⚠️ Error de Conexión",
                $"No se pudo conectar al servidor: {ex.Message}\n\nUsando datos locales.", "OK");

            CargarClientesEjemplo();
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void CargarClientesEjemplo()
    {
        Clientes.Clear();

        Clientes.Add(new Cliente
        {
            Id = 1,
            Nombre = "Juan Pérez (Local)",
            Email = "juan@email.com",
            Telefono = "555-1234",
            Direccion = "Av. Principal 123",
            FechaRegistro = DateTime.Now.AddDays(-30)
        });

        Clientes.Add(new Cliente
        {
            Id = 2,
            Nombre = "María García (Local)",
            Email = "maria@email.com",
            Telefono = "555-5678",
            Direccion = "Calle Secundaria 456",
            FechaRegistro = DateTime.Now.AddDays(-15)
        });
    }

    private async void OnAgregarCliente()
    {
        if (Application.Current?.MainPage != null)
        {
            await Application.Current.MainPage.Navigation.PushAsync(new Views.AddClientePage());
        }
    }

    private async void OnEditarCliente(Cliente? cliente)
    {
        if (cliente == null) return;

        if (Application.Current?.MainPage != null)
        {
            await Application.Current.MainPage.Navigation.PushAsync(new EditClientePage(cliente));
        }
    }

    private async void OnEliminarCliente(Cliente? cliente)
    {
        if (cliente == null) return;

        bool confirmar = await DisplayAlertConfirm(
            "🗑️ Eliminar Cliente",
            $"¿Estás seguro de eliminar permanentemente:\n\n" +
            $"**{cliente.Nombre}**\n\n" +
            $"📧 Email: {cliente.Email}\n" +
            $"📞 Teléfono: {cliente.Telefono}",
            "✅ Sí, eliminar",
            "❌ Cancelar");

        if (confirmar)
        {
            var clienteEliminado = cliente;

            Clientes.Remove(cliente);

            var resultado = await _apiService.DeleteClienteAsync(cliente.Id);

            if (resultado)
            {
                await DisplayAlert("✅ Éxito",
                    $"Cliente eliminado del servidor:\n{clienteEliminado.Nombre}", "OK");
            }
            else
            {
                await DisplayAlert("⚠️ Modo Local",
                    $"Cliente eliminado localmente:\n{clienteEliminado.Nombre}", "OK");
            }
        }
    }

    private async void OnVerDetalles(Cliente? cliente)
    {
        if (cliente == null) return;

        await DisplayAlert(
            "🔍 Detalles del Cliente",
            $"**{cliente.Nombre}**\n\n" +
            $"📧 Email: {cliente.Email}\n" +
            $"📞 Teléfono: {cliente.Telefono}\n" +
            $"🏠 Dirección: {cliente.Direccion}\n" +
            $"📅 Fecha de Registro: {cliente.FechaRegistro:dd/MM/yyyy}\n\n" +
            $"🌐 Fuente: {(cliente.Nombre?.Contains("(Local)") == true ? "Datos Locales" : "Servidor Azure")}",
            "✅ Aceptar");
    }

    private async Task DisplayAlert(string title, string message, string cancel)
    {
        if (Application.Current?.MainPage != null)
        {
            await Application.Current.MainPage.DisplayAlert(title, message, cancel);
        }
    }

    private async Task<bool> DisplayAlertConfirm(string title, string message, string accept, string cancel)
    {
        if (Application.Current?.MainPage != null)
        {
            return await Application.Current.MainPage.DisplayAlert(title, message, accept, cancel);
        }
        return false;
    }
}