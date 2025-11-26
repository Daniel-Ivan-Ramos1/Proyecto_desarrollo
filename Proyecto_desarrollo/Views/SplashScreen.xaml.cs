namespace Proyecto_desarrollo.Views;

public partial class SplashScreen : ContentPage
{
    public SplashScreen()
    {
        InitializeComponent();
        StartApp();
    }

    private async void StartApp()
    {
        await Task.Delay(3000);

        Application.Current.MainPage = new NavigationPage(new WelcomePage());
    }
}