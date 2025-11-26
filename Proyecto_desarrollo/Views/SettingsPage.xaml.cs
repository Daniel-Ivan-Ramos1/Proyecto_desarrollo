using Proyecto_desarrollo.Services;

namespace Proyecto_desarrollo.Views;

public partial class SettingsPage : ContentPage
{
    public SettingsPage()
    {
        InitializeComponent();
    }

    private async void OnTemaAzulClicked(object sender, EventArgs e)
    {
        ThemeService.ApplyTheme(ThemeService.Theme.Azul);

        var restart = await DisplayAlert("🎨 Tema Azul Aplicado",
            "El tema azul se ha aplicado.\n\n¿Reiniciar la aplicación?",
            "✅ Sí, reiniciar", "➡️ Continuar");

        if (restart)
        {
            Application.Current.MainPage = new NavigationPage(new ProductosPage());
        }
    }

    private async void OnTemaOscuroClicked(object sender, EventArgs e)
    {
        ThemeService.ApplyTheme(ThemeService.Theme.Oscuro);

        var restart = await DisplayAlert("🌙 Tema Oscuro Aplicado",
            "El tema oscuro se ha aplicado.\n\n¿Reiniciar la aplicación?",
            "✅ Sí, reiniciar", "➡️ Continuar");

        if (restart)
        {
            Application.Current.MainPage = new NavigationPage(new ProductosPage());
        }
    }

    private async void OnRecargarDatosClicked(object sender, EventArgs e)
    {
        try
        {
            bool confirmar = await DisplayAlert("🔄 Recargar Datos",
                "¿Recargar datos desde Azure SQL Database?",
                "✅ Sí, recargar", "❌ Cancelar");

            if (!confirmar) return;

            MessagingCenter.Send(this, "RecargarProductos");
            await DisplayAlert("✅ Éxito", "Datos recargados correctamente desde Azure SQL", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("❌ Error",
                $"No se pudieron recargar los datos: {ex.Message}", "OK");
        }
    }
}