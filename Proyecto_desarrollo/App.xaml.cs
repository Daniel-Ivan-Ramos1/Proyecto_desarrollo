using Proyecto_desarrollo.Views;

namespace Proyecto_desarrollo;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        Services.ThemeService.LoadSavedTheme();

        MainPage = new SplashScreen();
    }
}