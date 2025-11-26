namespace Proyecto_desarrollo.Views;

public partial class SettingsPage : ContentPage
{
    public SettingsPage()
    {
        InitializeComponent();
    }

    private async void OnTemaAzulClicked(object sender, EventArgs e)
    {
        await DisplayAlert("🎨 Tema", "Tema Azul Tech aplicado", "✅ OK");
    }

    private async void OnTemaOscuroClicked(object sender, EventArgs e)
    {
        await DisplayAlert("🎨 Tema", "Tema Oscuro aplicado", "✅ OK");
    }

    private async void OnRecargarDatosClicked(object sender, EventArgs e)
    {
        await DisplayAlert("🔄 Recargar", "Datos recargados correctamente", "✅ OK");
    }
}