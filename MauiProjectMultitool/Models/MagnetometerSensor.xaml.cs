namespace MauiProjectMultitool.Models;

public partial class MagnetometerSensor : ContentView
{
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
                // Turn on magnetometer
                Magnetometer.Default.ReadingChanged += Magnetometer_ReadingChanged;
                Magnetometer.Default.Start(SensorSpeed.Default);
            }
            else
            {
                // Turn off magnetometer
                Magnetometer.Default.Stop();
                Magnetometer.Default.ReadingChanged -= Magnetometer_ReadingChanged;
            }
        }
    }

    private void Magnetometer_ReadingChanged(object sender, MagnetometerChangedEventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            var magneticField = e.Reading.MagneticField;

            var x = magneticField.X.ToString("F2");
            var y = magneticField.Y.ToString("F2");
            var z = magneticField.Z.ToString("F2");

            MagLabel.Text = $"X: {x} µT\nY: {y} µT\nZ: {z} µT";

            var totalField = Math.Sqrt(
                Math.Pow(magneticField.X, 2) +
                Math.Pow(magneticField.Y, 2) +
                Math.Pow(magneticField.Z, 2)
            );

            var maxField = 100.0; // Max
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

    public MagnetometerSensor()
	{
		InitializeComponent();
        BindingContext = this;
        ToggleMagnetometer();
    }
}