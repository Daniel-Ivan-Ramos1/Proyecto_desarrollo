using System.Diagnostics;
using System.Timers;

namespace Proyecto_desarrollo.Views;

public partial class LoginPage : ContentPage
{
    private bool _isVerifying = false;
    private string _currentMode = "";
    private System.Timers.Timer _verificationTimer;
    private double _progress = 0;
    private bool _isUserHolding = false;
    private DateTime _holdStartTime;
    private const int REQUIRED_HOLD_SECONDS = 3;
    private bool _touchStarted = false;
    private DateTime _touchStartTime;

    public LoginPage()
    {
        InitializeComponent();

        _verificationTimer = new System.Timers.Timer(100);
        _verificationTimer.Elapsed += OnVerificationTimerElapsed;
        _verificationTimer.AutoReset = true;

        DetectAndConfigureMode();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        ResetUI();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        StopVerification();
        _verificationTimer?.Dispose();
    }

    private void DetectAndConfigureMode()
    {
        string forcedMode = Preferences.Get("ForcedMode", "auto");

        if (forcedMode != "auto")
        {
            _currentMode = forcedMode;
            Debug.WriteLine($"🔧 [FORZADO] Modo: {_currentMode.ToUpper()}");
        }
        else
        {
            _currentMode = AutoDetectMode();
            Debug.WriteLine($"📱 [AUTO] Modo detectado: {_currentMode.ToUpper()}");
        }

        ConfigureModeUI();
    }

    private string AutoDetectMode()
    {
        string platform = DeviceInfo.Platform.ToString();

        return platform.ToLower() switch
        {
            "android" => "android",
            "ios" or "maccatalyst" => "ios",
            "winui" or "windows" => "windows",
            _ => "android"
        };
    }

    private void ConfigureModeUI()
    {
        FingerprintFrame.IsVisible = false;
        WindowsButton.IsVisible = false;

        switch (_currentMode)
        {
            case "android":
                ConfigureAndroidMode();
                break;
            case "ios":
                ConfigureIOSMode();
                break;
            case "windows":
                ConfigureWindowsMode();
                break;
            default:
                ConfigureAndroidMode();
                break;
        }
    }

    private void ConfigureAndroidMode()
    {
        Title = "🔐 Android - TechStore";

        FingerprintFrame.IsVisible = true;
        WindowsButton.IsVisible = false;

        FingerprintIcon.Text = "📱";
        FingerprintText.Text = "Mantener presionado";
        IconLabel.Text = "🤖";
        StatusLabel.Text = "Mantenga presionado para verificar huella";
        FingerprintFrame.BackgroundColor = Color.FromArgb("#E8F5E9");

        ModeTitleLabel.Text = "🤖 Modo: Android";
        ModeDescriptionLabel.Text = "Presione y mantenga el círculo por 3 segundos";
        ModeHintLabel.Text = "Suelte para cancelar";

        ConfigureAndroidGestures();
    }

    private void ConfigureIOSMode()
    {
        Title = "🔐 iOS - TechStore";

        FingerprintFrame.IsVisible = true;
        WindowsButton.IsVisible = false;

        FingerprintIcon.Text = "📱";
        FingerprintText.Text = "Mantener presionado";
        IconLabel.Text = "🍎";
        StatusLabel.Text = "Mantenga presionado para verificar Face ID";
        FingerprintFrame.BackgroundColor = Color.FromArgb("#F0F8FF");

        ModeTitleLabel.Text = "🍎 Modo: iOS";
        ModeDescriptionLabel.Text = "Presione y mantenga el círculo por 3 segundos";
        ModeHintLabel.Text = "Suelte para cancelar";

        ConfigureIOSGestures();
    }

    private void ConfigureWindowsMode()
    {
        Title = "🔐 Windows - TechStore";

        FingerprintFrame.IsVisible = false;
        WindowsButton.IsVisible = true;

        WindowsButton.Text = "👆 MANTENER PRESIONADO";
        WindowsButton.BackgroundColor = Color.FromArgb("#2E86AB");
        IconLabel.Text = "🖥️";
        StatusLabel.Text = "Presione y mantenga el botón por 3 segundos";

        ModeTitleLabel.Text = "🖥️ Modo: Windows";
        ModeDescriptionLabel.Text = "Presione y mantenga el botón por 3 segundos";
        ModeHintLabel.Text = "Suelte para cancelar";

        ConfigureWindowsGestures();
    }
    private void ConfigureAndroidGestures()
    {
        FingerprintFrame.GestureRecognizers.Clear();

        var panGesture = new PanGestureRecognizer();
        panGesture.PanUpdated += OnAndroidPanUpdated;
        FingerprintFrame.GestureRecognizers.Add(panGesture);
    }

    private void OnAndroidPanUpdated(object sender, PanUpdatedEventArgs e)
    {
        switch (e.StatusType)
        {
            case GestureStatus.Started:
                OnTouchStarted();
                break;

            case GestureStatus.Completed:
            case GestureStatus.Canceled:
                OnTouchEnded();
                break;
        }
    }

    private void ConfigureIOSGestures()
    {
        FingerprintFrame.GestureRecognizers.Clear();

        var panGesture = new PanGestureRecognizer();
        panGesture.PanUpdated += OnIOSPanUpdated;
        FingerprintFrame.GestureRecognizers.Add(panGesture);
    }

    private void OnIOSPanUpdated(object sender, PanUpdatedEventArgs e)
    {
        switch (e.StatusType)
        {
            case GestureStatus.Started:
                OnTouchStarted();
                break;

            case GestureStatus.Completed:
            case GestureStatus.Canceled:
                OnTouchEnded();
                break;
        }
    }

    private void ConfigureWindowsGestures()
    {
        WindowsButton.Pressed -= OnWindowsButtonPressed;
        WindowsButton.Released -= OnWindowsButtonReleased;

        WindowsButton.Pressed += OnWindowsButtonPressed;
        WindowsButton.Released += OnWindowsButtonReleased;
    }

    private void OnWindowsButtonPressed(object sender, EventArgs e)
    {
        OnTouchStarted();
    }

    private void OnWindowsButtonReleased(object sender, EventArgs e)
    {
        OnTouchEnded();
    }

    private void OnTouchStarted()
    {
        if (_isVerifying || _isUserHolding) return;

        _touchStarted = true;
        _touchStartTime = DateTime.Now;

        if (_currentMode == "windows")
        {
            WindowsButton.ScaleTo(0.95, 50);
        }
        else
        {
            FingerprintFrame.ScaleTo(0.95, 50);
        }

        Device.StartTimer(TimeSpan.FromMilliseconds(300), () =>
        {
            if (_touchStarted && !_isUserHolding && !_isVerifying)
            {
                StartHoldVerification();
            }
            return false;
        });
    }

    private void OnTouchEnded()
    {
        _touchStarted = false;

        if (!_isUserHolding)
        {
            if (_currentMode == "windows")
            {
                WindowsButton.ScaleTo(1.0, 50);
            }
            else
            {
                FingerprintFrame.ScaleTo(1.0, 50);
            }
        }
        else if (_isUserHolding && !_isVerifying)
        {
            CancelVerification();
        }
    }

    private void StartHoldVerification()
    {
        if (_isVerifying || _isUserHolding) return;

        _isUserHolding = true;
        _holdStartTime = DateTime.Now;
        _progress = 0;

        if (_currentMode == "windows")
        {
            WindowsButton.Text = "⏳ VERIFICANDO...";
            WindowsButton.BackgroundColor = Color.FromArgb("#FF9800");
            WindowsButton.ScaleTo(1.0, 50);
        }
        else
        {
            FingerprintIcon.Text = "⏳";
            FingerprintIcon.TextColor = Color.FromArgb("#FF9800");
            FingerprintText.Text = "Verificando...";
            VerificationProgress.IsVisible = true;
            FingerprintFrame.ScaleTo(1.0, 50);

            if (_currentMode == "android")
                FingerprintFrame.BackgroundColor = Color.FromArgb("#FFF3E0");
            else
                FingerprintFrame.BackgroundColor = Color.FromArgb("#FFF0F5");
        }

        TimerLabel.IsVisible = true;
        TimerLabel.Text = $"0.0s / {REQUIRED_HOLD_SECONDS}.0s";
        StatusLabel.Text = "Mantenga presionado...";

        _verificationTimer.Start();
    }

    private void OnVerificationTimerElapsed(object sender, ElapsedEventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            if (!_isUserHolding)
            {
                CancelVerification();
                return;
            }

            var elapsed = (DateTime.Now - _holdStartTime).TotalSeconds;
            _progress = Math.Min(elapsed / REQUIRED_HOLD_SECONDS, 1.0);

            TimerLabel.Text = $"{elapsed:F1}s / {REQUIRED_HOLD_SECONDS}.0s";

            if (_currentMode == "windows")
            {
                int percentage = (int)(_progress * 100);
                WindowsButton.Text = $"⏳ {percentage}%";

                if (_progress < 0.5)
                    WindowsButton.BackgroundColor = Color.FromArgb("#FF9800");
                else if (_progress < 0.8)
                    WindowsButton.BackgroundColor = Color.FromArgb("#FFC107");
                else
                    WindowsButton.BackgroundColor = Color.FromArgb("#4CAF50");
            }
            else
            {
                VerificationProgress.Progress = _progress;

                if (_progress < 0.33)
                    FingerprintIcon.Text = "▫️";
                else if (_progress < 0.66)
                    FingerprintIcon.Text = "▪️▫️";
                else if (_progress < 1.0)
                    FingerprintIcon.Text = "▪️▪️▪️";

                var hue = 120 * _progress;
                FingerprintIcon.TextColor = Color.FromHsla(hue / 360, 1.0, 0.5);
            }

            if (_progress >= 1.0)
            {
                CompleteVerification();
            }
        });
    }

    private void CompleteVerification()
    {
        StopVerification();

        if (_currentMode == "windows")
        {
            WindowsButton.Text = "✅ VERIFICADO";
            WindowsButton.BackgroundColor = Color.FromArgb("#4CAF50");
            StatusLabel.Text = "✅ Acceso concedido exitosamente";
        }
        else
        {
            FingerprintIcon.Text = "✅";
            FingerprintIcon.TextColor = Color.FromArgb("#4CAF50");
            FingerprintText.Text = "Verificado";
            VerificationProgress.ProgressColor = Color.FromArgb("#4CAF50");

            if (_currentMode == "android")
            {
                FingerprintFrame.BackgroundColor = Color.FromArgb("#C8E6C9");
                StatusLabel.Text = "✅ Huella verificada correctamente";
            }
            else
            {
                FingerprintFrame.BackgroundColor = Color.FromArgb("#D5F4E6");
                StatusLabel.Text = "✅ Face ID verificado correctamente";
            }
        }

        TimerLabel.Text = "✅ Completado";

        Device.StartTimer(TimeSpan.FromSeconds(1), () =>
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await GrantAccess();
            });
            return false;
        });
    }

    private void CancelVerification()
    {
        if (!_isUserHolding) return;

        StopVerification();

        if (_currentMode == "windows")
        {
            WindowsButton.Text = "❌ CANCELADO";
            WindowsButton.BackgroundColor = Color.FromArgb("#F44336");
            StatusLabel.Text = "❌ Verificación cancelada";
        }
        else
        {
            FingerprintIcon.Text = "❌";
            FingerprintIcon.TextColor = Color.FromArgb("#F44336");
            FingerprintText.Text = "Cancelado";
            VerificationProgress.ProgressColor = Color.FromArgb("#F44336");

            if (_currentMode == "android")
                FingerprintFrame.BackgroundColor = Color.FromArgb("#FFEBEE");
            else
                FingerprintFrame.BackgroundColor = Color.FromArgb("#FFE4E1");

            StatusLabel.Text = "❌ Verificación cancelada";
        }

        TimerLabel.Text = "❌ Cancelado";

        Device.StartTimer(TimeSpan.FromSeconds(1), () =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                ResetUI();
            });
            return false;
        });
    }

    private void StopVerification()
    {
        _isVerifying = false;
        _isUserHolding = false;
        _verificationTimer.Stop();
    }

    private void ResetUI()
    {
        _isVerifying = false;
        _isUserHolding = false;
        _touchStarted = false;
        _progress = 0;

        StopVerification();

        if (_currentMode == "windows")
        {
            WindowsButton.IsEnabled = true;
            WindowsButton.Opacity = 1.0;
            WindowsButton.Text = "👆 MANTENER PRESIONADO";
            WindowsButton.BackgroundColor = Color.FromArgb("#2E86AB");
            WindowsButton.Scale = 1.0;
            TimerLabel.IsVisible = false;
            StatusLabel.Text = "Presione y mantenga el botón por 3 segundos";
        }
        else
        {
            FingerprintFrame.Opacity = 1.0;
            FingerprintIcon.Text = "📱";
            FingerprintIcon.TextColor = Color.FromArgb("#1E88E5");
            FingerprintText.Text = "Mantener presionado";
            VerificationProgress.IsVisible = false;
            VerificationProgress.Progress = 0;
            TimerLabel.IsVisible = false;
            StatusLabel.Text = "Mantenga presionado para verificar";
            FingerprintFrame.Scale = 1.0;

            if (_currentMode == "android")
                FingerprintFrame.BackgroundColor = Color.FromArgb("#E8F5E9");
            else
                FingerprintFrame.BackgroundColor = Color.FromArgb("#F0F8FF");
        }
    }

    private async Task GrantAccess()
    {
        StatusLabel.Text = "🚀 Accediendo al sistema...";
        await Task.Delay(500);
        Debug.WriteLine("🏠 Navegando a WelcomePage...");
        await Navigation.PushAsync(new WelcomePage());
        Navigation.RemovePage(this);
    }
}