namespace MauiProjectMultitool.Models;

public partial class CompassSensor : ContentView
{
    public static readonly BindableProperty NameProperty =
        BindableProperty.Create(nameof(Name), typeof(string), typeof(CompassSensor), default(string));

    public string Name
    {
        get => (string)GetValue(NameProperty);
        set => SetValue(NameProperty, value);
    }

    private void ToggleCompass()
    {
        if (Compass.Default.IsSupported)
        {
            if (!Compass.Default.IsMonitoring)
            {
                // Turn on compass
                Compass.Default.ReadingChanged += Compass_ReadingChanged;
                Compass.Default.Start(SensorSpeed.UI, applyLowPassFilter: true);
            }
            else
            {
                // Turn off compass
                Compass.Default.Stop();
                Compass.Default.ReadingChanged -= Compass_ReadingChanged;
            }
        }
    }

    private void Compass_ReadingChanged(object sender, CompassChangedEventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            var azimuth = e.Reading.HeadingMagneticNorth;

            CompassArrow.Rotation = azimuth;

            var direction = GetDirectionFromAzimuth(azimuth);

            CompassLabel.Text = $"Azimuth: {azimuth:F2}°\nDirection: {direction}";
        });
    }

    private string GetDirectionFromAzimuth(double azimuth)
    {
        if (azimuth >= 337.5 || azimuth < 22.5)
            return "North";
        if (azimuth >= 22.5 && azimuth < 67.5)
            return "Northeast";
        if (azimuth >= 67.5 && azimuth < 112.5)
            return "East";
        if (azimuth >= 112.5 && azimuth < 157.5)
            return "Southeast";
        if (azimuth >= 157.5 && azimuth < 202.5)
            return "South";
        if (azimuth >= 202.5 && azimuth < 247.5)
            return "Southwest";
        if (azimuth >= 247.5 && azimuth < 292.5)
            return "West";
        if (azimuth >= 292.5 && azimuth < 337.5)
            return "Northwest";
        return "Unknown";
    }

    public CompassSensor()
    {
        InitializeComponent();
        BindingContext = this;
        ToggleCompass();
    }
}
