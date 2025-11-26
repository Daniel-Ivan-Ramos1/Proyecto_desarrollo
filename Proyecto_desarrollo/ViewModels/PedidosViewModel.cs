using System.Collections.ObjectModel;
using System.Windows.Input;
using Proyecto_desarrollo.Models;
using Proyecto_desarrollo.Services;
using Proyecto_desarrollo.Views;

namespace Proyecto_desarrollo.ViewModels;

public class PedidosViewModel : BaseViewModel
{
    private readonly ApiService _apiService = new ApiService();
    public ObservableCollection<Pedido> Pedidos { get; } = new();
    public ICommand AgregarPedidoCommand { get; }
    public ICommand EditarPedidoCommand { get; }
    public ICommand EliminarPedidoCommand { get; }
    public ICommand VerDetallesCommand { get; }
    public ICommand RefreshCommand { get; }

    public PedidosViewModel()
    {
        Title = "📦 Pedidos";

        AgregarPedidoCommand = new Command(OnAgregarPedido);
        EditarPedidoCommand = new Command<Pedido>(OnEditarPedido);
        EliminarPedidoCommand = new Command<Pedido>(OnEliminarPedido);
        VerDetallesCommand = new Command<Pedido>(OnVerDetalles);
        RefreshCommand = new Command(async () => await LoadPedidosFromApi());

        MessagingCenter.Subscribe<SettingsPage>(this, "RecargarPedidos", async (sender) =>
        {
            await LoadPedidosFromApi();
            await DisplayAlert("🔄 Actualizado", "Pedidos recargados desde Azure SQL", "OK");
        });

        _ = LoadPedidosFromApi();
    }

    private async Task LoadPedidosFromApi()
    {
        IsBusy = true;
        try
        {
            var pedidos = await _apiService.GetPedidosAsync();
            Pedidos.Clear();
            foreach (var pedido in pedidos)
                Pedidos.Add(pedido);

            if (pedidos.Any(p => p.Estado?.Contains("(Local)") == true))
            {
                await DisplayAlert("ℹ️ Modo Local",
                    "Datos cargados en modo local (sin conexión al servidor)", "OK");
            }
            else
            {
                await DisplayAlert("✅ Conectado",
                    $"Pedidos cargados desde el servidor: {pedidos.Count} elementos", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("⚠️ Error de Conexión",
                $"No se pudo conectar al servidor: {ex.Message}\n\nUsando datos locales.", "OK");

            CargarPedidosEjemplo();
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void CargarPedidosEjemplo()
    {
        Pedidos.Clear();

        Pedidos.Add(new Pedido
        {
            Id = 1,
            ClienteId = 1,
            ProductoId = 1,
            Cantidad = 1,
            Total = 1299.99m,
            FechaPedido = DateTime.Now.AddDays(-5),
            Estado = "Completado (Local)"
        });

        Pedidos.Add(new Pedido
        {
            Id = 2,
            ClienteId = 2,
            ProductoId = 2,
            Cantidad = 2,
            Total = 1799.98m,
            FechaPedido = DateTime.Now.AddDays(-2),
            Estado = "Pendiente (Local)"
        });
    }

    private async void OnAgregarPedido()
    {
        if (Application.Current?.MainPage != null)
        {
            await Application.Current.MainPage.Navigation.PushAsync(new Views.AddPedidoPage());
        }
    }

    private async void OnEditarPedido(Pedido? pedido)
    {
        if (pedido == null) return;

        if (Application.Current?.MainPage != null)
        {
            await Application.Current.MainPage.Navigation.PushAsync(new EditPedidoPage(pedido));
        }
    }

    private async void OnEliminarPedido(Pedido? pedido)
    {
        if (pedido == null) return;

        bool confirmar = await DisplayAlertConfirm(
            "🗑️ Eliminar Pedido",
            $"¿Estás seguro de eliminar permanentemente este pedido?\n\n" +
            $"📦 ID: {pedido.Id}\n" +
            $"📅 Fecha: {pedido.FechaPedido:dd/MM/yyyy}\n" +
            $"💰 Total: ${pedido.Total:F2}\n" +
            $"📋 Estado: {pedido.Estado}",
            "✅ Sí, eliminar",
            "❌ Cancelar");

        if (confirmar)
        {
            var pedidoEliminado = pedido;

            Pedidos.Remove(pedido);

            var resultado = await _apiService.DeletePedidoAsync(pedido.Id);

            if (resultado)
            {
                await DisplayAlert("✅ Éxito",
                    $"Pedido eliminado del servidor:\nID: {pedidoEliminado.Id}", "OK");
            }
            else
            {
                await DisplayAlert("⚠️ Modo Local",
                    $"Pedido eliminado localmente:\nID: {pedidoEliminado.Id}", "OK");
            }
        }
    }

    private async void OnVerDetalles(Pedido? pedido)
    {
        if (pedido == null) return;

        await DisplayAlert(
            "🔍 Detalles del Pedido",
            $"**Pedido #{pedido.Id}**\n\n" +
            $"📅 Fecha: {pedido.FechaPedido:dd/MM/yyyy HH:mm}\n" +
            $"👤 Cliente ID: {pedido.ClienteId}\n" +
            $"💻 Producto ID: {pedido.ProductoId}\n" +
            $"📦 Cantidad: {pedido.Cantidad}\n" +
            $"💰 Total: ${pedido.Total:F2}\n" +
            $"📋 Estado: {pedido.Estado}\n\n" +
            $"🌐 Fuente: {(pedido.Estado?.Contains("(Local)") == true ? "Datos Locales" : "Servidor Azure")}",
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