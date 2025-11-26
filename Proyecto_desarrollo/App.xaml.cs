namespace Proyecto_desarrollo;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        // Cargar el tema guardado al iniciar
        Services.ThemeService.LoadSavedTheme();

        MainPage = new NavigationPage(new Views.ProductosPage());
    }
}