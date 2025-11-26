namespace Proyecto_desarrollo.Views;

public partial class PedidosPage : ContentPage
{
    public PedidosPage()
    {
        InitializeComponent();
    }

    private async void OnProductosClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ProductosPage());
    }

    private async void OnClientesClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ClientesPage());
    }

    private async void OnSettingsClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new SettingsPage());
    }
}