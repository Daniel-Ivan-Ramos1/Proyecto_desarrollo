using Proyecto_desarrollo.Services;

namespace Proyecto_desarrollo.Views;

public partial class SettingsPage : ContentPage
{
    public SettingsPage()
    {
        InitializeComponent();
        UpdateButtonStates();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        UpdateButtonStates();
    }

    private void UpdateButtonStates()
    {
        var currentTheme = ThemeService.GetCurrentTheme();

        TemaAzulButton.BackgroundColor = currentTheme == ThemeService.Theme.Azul ?
            Color.FromArgb("#1E88E5") : Color.FromArgb("#2E86AB");

        TemaOscuroButton.BackgroundColor = currentTheme == ThemeService.Theme.Oscuro ?
            Color.FromArgb("#BB86FC") : Color.FromArgb("#333333");
    }

    private async void OnTemaAzulClicked(object sender, EventArgs e)
    {
        ThemeService.ApplyTheme(ThemeService.Theme.Azul);
        UpdateButtonStates();

        var restart = await DisplayAlert("🎨 Tema Azul Aplicado",
            "El tema azul se ha aplicado.\n\n¿Reiniciar la aplicación para ver todos los cambios?",
            "✅ Sí, reiniciar", "➡️ Continuar");

        if (restart)
        {
            Application.Current.MainPage = new NavigationPage(new ProductosPage());
        }
    }

    private async void OnTemaOscuroClicked(object sender, EventArgs e)
    {
        ThemeService.ApplyTheme(ThemeService.Theme.Oscuro);
        UpdateButtonStates();

        var restart = await DisplayAlert("🌙 Tema Oscuro Aplicado",
            "El tema oscuro se ha aplicado.\n\n¿Reiniciar la aplicación para ver todos los cambios?",
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
            MessagingCenter.Send(this, "RecargarClientes");
            MessagingCenter.Send(this, "RecargarPedidos");

            await DisplayAlert("✅ Éxito",
                "Datos recargados correctamente desde Azure SQL:\n\n" +
                "• Productos actualizados\n" +
                "• Clientes actualizados\n" +
                "• Pedidos actualizados", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("❌ Error",
                $"No se pudieron recargar los datos: {ex.Message}", "OK");
        }
    }
}