namespace MauiProjectMultitool.Models;

public partial class CompassSensor : ContentView
{
    private const string CompassCheckboxKey = "CompassCheckboxState";

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
                compassSphere.Stroke = Colors.Azure;
                sensorFrame.BorderColor = Colors.LightGrey;
            }
            else
            {
                // Turn off compass
                Compass.Default.Stop();
                Compass.Default.ReadingChanged -= Compass_ReadingChanged;
                DisableCompass();
            }
        }
    }

    private void Compass_ReadingChanged(object sender, CompassChangedEventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            var azimuth = e.Reading.HeadingMagneticNorth;

            RotateCompass(azimuth);

            var direction = GetDirectionFromAzimuth(azimuth);

            compassLabel.Text = $"Azimuth: {azimuth:F2}°\nDirection: {direction}";
        });
    }

    private string GetDirectionFromAzimuth(double azimuth)
    {
        switch (azimuth)
        {
            case >= 337.5 or < 22.5:
            return "North";
            case >= 22.5 and < 67.5:
            return "Northeast";
            case >= 67.5 and < 112.5:
            return "East";
            case >= 112.5 and < 157.5:
            return "Southeast";
            case >= 157.5 and < 202.5:
            return "South";
            case >= 202.5 and < 247.5:
            return "Southwest";
            case >= 247.5 and < 292.5:
            return "West";
            case >= 292.5 and < 337.5:
            return "Northwest";
            default:
            return "Unknown";
        }
    }

    private void RotateCompass(double azimuth)
    {
        CompassGrid.Rotation = -azimuth;
        CompassArrow.Rotation = azimuth;
        North.Rotation = azimuth;
        South.Rotation = azimuth;
        East.Rotation = azimuth;
        West.Rotation = azimuth;
    }

    private void DisableCompass()
    {
        RotateCompass(0);
        compassSphere.Stroke = Colors.Gray;
        sensorFrame.BorderColor = Colors.Gray;
        compassLabel.Text = "Compass not active";
    }

    private void ToggleCheckBox_Changed(object sender, CheckedChangedEventArgs e)
    {
        if (e.Value)
        {
            ToggleCompass();
        }
        else
        {
            ToggleCompass();
        }

        Preferences.Set(CompassCheckboxKey, e.Value);
    }

    public CompassSensor()
    {
        InitializeComponent();
        BindingContext = this;

        if (Preferences.ContainsKey(CompassCheckboxKey))
        {
            var isChecked = Preferences.Get(CompassCheckboxKey, true);

            if (isChecked)
            {
                ToggleCheckBox.IsChecked = isChecked;
            }
            else
            {
                DisableCompass();
            }
        }
        else
        {
            ToggleCheckBox.IsChecked = true;
            Preferences.Set(CompassCheckboxKey, true);
        }
    }
}
