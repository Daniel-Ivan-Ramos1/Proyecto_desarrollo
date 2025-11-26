using System.Collections.ObjectModel;
using System.Windows.Input;
using Proyecto_desarrollo.Models;

namespace Proyecto_desarrollo.ViewModels;

public class PedidosViewModel : BaseViewModel
{
    public ObservableCollection<Pedido> Pedidos { get; } = new();
    public ICommand AgregarPedidoCommand { get; }

    public PedidosViewModel()
    {
        Title = "📦 Pedidos";
        AgregarPedidoCommand = new Command(OnAgregarPedido);
        CargarPedidosEjemplo();
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
            Estado = "Completado",
            FechaPedido = DateTime.Now.AddDays(-2)
        });

        Pedidos.Add(new Pedido
        {
            Id = 2,
            ClienteId = 2,
            ProductoId = 2,
            Cantidad = 2,
            Total = 1799.98m,
            Estado = "Pendiente",
            FechaPedido = DateTime.Now.AddDays(-1)
        });
    }

    private async void OnAgregarPedido()
    {
        await DisplayAlert(
            "📦 Nuevo Pedido",
            "Funcionalidad para agregar pedido:\n\n" +
            "• Seleccionar cliente\n" +
            "• Elegir productos\n" +
            "• Calcular total\n" +
            "• Confirmar pedido",
            "✅ Entendido");
    }

    private async Task DisplayAlert(string title, string message, string cancel)
    {
        if (Application.Current?.MainPage != null)
        {
            await Application.Current.MainPage.DisplayAlert(title, message, cancel);
        }
    }
}