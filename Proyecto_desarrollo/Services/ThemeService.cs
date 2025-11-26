using Microsoft.Maui.Controls;

namespace Proyecto_desarrollo.Services;

public static class ThemeService
{
    private const string ThemeKey = "CurrentTheme";

    public static event Action<Theme> ThemeChanged;

    public enum Theme
    {
        Azul,
        Oscuro
    }

    public static void ApplyTheme(Theme theme)
    {
        Preferences.Set(ThemeKey, theme.ToString());

        ApplyThemeColors(theme);

        ThemeChanged?.Invoke(theme);

        ForceUIUpdate();
    }

    private static void ApplyThemeColors(Theme theme)
    {
        var colors = GetThemeColors(theme);

        foreach (var color in colors)
        {
            if (Application.Current.Resources.ContainsKey(color.Key))
            {
                Application.Current.Resources[color.Key] = color.Value;
            }
            else
            {
                Application.Current.Resources.Add(color.Key, color.Value);
            }
        }
    }

    private static void ForceUIUpdate()
    {
        if (Application.Current?.MainPage is not null)
        {
            var currentPage = Application.Current.MainPage;

            UpdatePageAppearance(currentPage);
        }
    }

    private static void UpdatePageAppearance(Page page)
    {
        try
        {
            if (page is ContentPage contentPage)
            {
                var bgColor = contentPage.BackgroundColor;
                contentPage.BackgroundColor = Colors.Transparent;
                contentPage.BackgroundColor = bgColor;
            }

            if (page is NavigationPage navPage && navPage.CurrentPage != null)
            {
                UpdatePageAppearance(navPage.CurrentPage);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error actualizando apariencia: {ex.Message}");
        }
    }

    private static Dictionary<string, Color> GetThemeColors(Theme theme)
    {
        return theme switch
        {
            Theme.Azul => new Dictionary<string, Color>
            {
                ["PrimaryColor"] = Color.FromArgb("#2E86AB"),
                ["SecondaryColor"] = Color.FromArgb("#A23B72"),
                ["AccentColor"] = Color.FromArgb("#F18F01"),
                ["BackgroundColor"] = Color.FromArgb("#F8F9FA"),
                ["CardColor"] = Color.FromArgb("#FFFFFF"),
                ["TextColor"] = Color.FromArgb("#212529")
            },

            Theme.Oscuro => new Dictionary<string, Color>
            {
                ["PrimaryColor"] = Color.FromArgb("#BB86FC"),
                ["SecondaryColor"] = Color.FromArgb("#03DAC6"),
                ["AccentColor"] = Color.FromArgb("#CF6679"),
                ["BackgroundColor"] = Color.FromArgb("#121212"),
                ["CardColor"] = Color.FromArgb("#1E1E1E"),
                ["TextColor"] = Color.FromArgb("#FFFFFF")
            },

            _ => GetThemeColors(Theme.Azul)
        };
    }

    public static void LoadSavedTheme()
    {
        var savedTheme = Preferences.Get(ThemeKey, "Azul");
        if (Enum.TryParse<Theme>(savedTheme, out var theme))
        {
            ApplyThemeColors(theme);
        }
    }

    public static Theme GetCurrentTheme()
    {
        var savedTheme = Preferences.Get(ThemeKey, "Azul");
        return Enum.TryParse<Theme>(savedTheme, out var theme) ? theme : Theme.Azul;
    }
}