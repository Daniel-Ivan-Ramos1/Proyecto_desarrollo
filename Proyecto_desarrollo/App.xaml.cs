using Proyecto_desarrollo.Views;

namespace Proyecto_desarrollo;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        Services.ThemeService.LoadSavedTheme();

#if DEBUG
        // Descomentar solo una línea para seleccionar el modo de inicio

        // Modo 1: Android/Real
        // Preferences.Set("ForcedMode", "android");

        // Modo 2: iOS/Emulador
        // Preferences.Set("ForcedMode", "ios");

        // Modo 3: Windows
        // Preferences.Set("ForcedMode", "windows");

        // Modo 4: Auto-detección (por defecto)
         Preferences.Remove("ForcedMode");
#endif

        MainPage = new NavigationPage(new SplashScreen());
    }
}