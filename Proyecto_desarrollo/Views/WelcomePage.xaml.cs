namespace Proyecto_desarrollo.Views;

public partial class WelcomePage : ContentPage
{
    public WelcomePage()
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

    private async void OnPedidosClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new PedidosPage());
    }

    private async void OnSettingsClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new SettingsPage());
    }
}