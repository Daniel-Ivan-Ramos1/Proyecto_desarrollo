namespace Proyecto_desarrollo;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        // Usar NavigationPage para habilitar la navegación
        MainPage = new NavigationPage(new Views.ProductosPage());
    }
}