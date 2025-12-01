using System.Diagnostics;

namespace Proyecto_desarrollo.Views;

public partial class SplashScreen : ContentPage
{
    public SplashScreen()
    {
        InitializeComponent();
        Debug.WriteLine("🚀 SplashScreen iniciado");
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await Task.Delay(3000);
        Debug.WriteLine("🔑 Navegando a LoginPage...");
        await Navigation.PushAsync(new LoginPage());
        Navigation.RemovePage(this);
    }
}