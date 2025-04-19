namespace MauiProjectMultitool.Models;

public partial class AccelerometerSensor : ContentView
{
    public static readonly BindableProperty NameProperty =
        BindableProperty.Create(nameof(Name), typeof(string), typeof(AccelerometerSensor), default(string));

    public string Name
    {
        get => (string)GetValue(NameProperty);
        set => SetValue(NameProperty, value);
    }

    public void ToggleAccelerometer()
    {
        try
        {
            if (Accelerometer.Default.IsSupported)
            {
                if (!Accelerometer.Default.IsMonitoring)
                {
                    // Turn on accelerometer
                    Accelerometer.Default.ReadingChanged += Accelerometer_ReadingChanged;
                    Accelerometer.Default.Start(SensorSpeed.Default);
                }
                else
                {
                    // Turn off accelerometer
                    Accelerometer.Default.Stop();
                    Accelerometer.Default.ReadingChanged -= Accelerometer_ReadingChanged;
                }
            }
            else
            {
                AccelLabel.Text = "Accelerometer not supported on this device.";
            }
        }
        catch (Exception ex)
        {
            AccelLabel.Text = $"Error: {ex.Message}";
        }
    }

    private void Accelerometer_ReadingChanged(object sender, AccelerometerChangedEventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            var acceleration = e.Reading.Acceleration;

            var speed = Math.Sqrt(
                Math.Pow(acceleration.X, 2) +
                Math.Pow(acceleration.Y, 2) +
                Math.Pow(acceleration.Z, 2)
            );

            var minSpeed = 2.0; // Minimum speed (green)
            var maxSpeed = 5.0; // Maximum speed (red)

            var normalizedSpeed = Math.Min(1.0, Math.Max(0.0, (speed - minSpeed) / (maxSpeed - minSpeed)));
            var borderColor = InterpolateColor(Colors.Green, Colors.Red, normalizedSpeed);

            sensorFrame.BorderColor = borderColor;

            AccelLabel.Text = $"{speed:F2} m/s";
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

    public AccelerometerSensor()
    {
        InitializeComponent();
        BindingContext = this;
        ToggleAccelerometer();
    }
}
