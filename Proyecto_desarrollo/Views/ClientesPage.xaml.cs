namespace Proyecto_desarrollo.Views;

public partial class ClientesPage : ContentPage
{
    public ClientesPage()
    {
        InitializeComponent();
    }

    private async void OnAgregarClienteClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Próximamente", "Aquí podrás agregar nuevos clientes", "OK");
    }
}