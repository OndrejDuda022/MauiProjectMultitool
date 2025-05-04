namespace MauiProjectMultitool.Models;

public partial class MagnetometerSensor : ContentView
{
    private const string MagnetometerCheckboxKey = "MagnetometerCheckboxState";

    public static readonly BindableProperty NameProperty =
        BindableProperty.Create(nameof(Name), typeof(string), typeof(MagnetometerSensor), default(string));

    public string Name
    {
        get => (string)GetValue(NameProperty);
        set => SetValue(NameProperty, value);
    }

    private void ToggleMagnetometer()
    {
        if (Magnetometer.Default.IsSupported)
        {
            if (!Magnetometer.Default.IsMonitoring)
            {
                Magnetometer.Default.ReadingChanged += Magnetometer_ReadingChanged;
                Magnetometer.Default.Start(SensorSpeed.Default);
                MagLabel.FontSize = 30;
            }
            else
            {
                Magnetometer.Default.Stop();
                Magnetometer.Default.ReadingChanged -= Magnetometer_ReadingChanged;
                DisableMagnetometer();
            }
        }
    }

    private void Magnetometer_ReadingChanged(object sender, MagnetometerChangedEventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            var magneticField = e.Reading.MagneticField;

            var totalField = Math.Sqrt(
                Math.Pow(magneticField.X, 2) +
                Math.Pow(magneticField.Y, 2) +
                Math.Pow(magneticField.Z, 2)
            );

            MagLabel.Text = $"{totalField:F2} µT";

            var maxField = 100.0;
            var normalizedValue = Math.Min(1.0, totalField / maxField);

            var borderColor = InterpolateColor(Colors.Blue, Colors.Red, normalizedValue);
            sensorFrame.BorderColor = borderColor;
        });
    }


    private Color InterpolateColor(Color startColor, Color endColor, double t)
    {
        return new Color(
            (float)(startColor.Red + (endColor.Red - startColor.Red) * t),
            (float)(startColor.Green + (endColor.Green - startColor.Green) * t),
            (float)(startColor.Blue + (endColor.Blue - startColor.Blue) * t)
        );
    }

    private void DisableMagnetometer()
    {
        sensorFrame.BorderColor = Colors.Gray;
        MagLabel.Text = "Magnetometer not active";
        MagLabel.FontSize = 15;
    }

    private void ToggleCheckBox_Changed(object sender, CheckedChangedEventArgs e)
    {
        if (e.Value)
        {
            ToggleMagnetometer();
        }
        else
        {
            ToggleMagnetometer();
        }

        Preferences.Set(MagnetometerCheckboxKey, e.Value);
    }

    public MagnetometerSensor()
    {
        InitializeComponent();
        BindingContext = this;

        if (Preferences.ContainsKey(MagnetometerCheckboxKey))
        {
            var isChecked = Preferences.Get(MagnetometerCheckboxKey, true);

            if (isChecked)
            {
                ToggleCheckBox.IsChecked = isChecked;
            }
            else
            {
                DisableMagnetometer();
            }
        }
        else
        {
            ToggleCheckBox.IsChecked = true;
            Preferences.Set(MagnetometerCheckboxKey, true);
        }
    }
}
