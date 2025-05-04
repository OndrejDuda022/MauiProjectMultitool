namespace MauiProjectMultitool.Models;

public partial class AccelerometerSensor : ContentView
{
    private const string AccelerometerCheckboxKey = "AccelerometerCheckboxState"; // Key for storing the checkbox state

    public static readonly BindableProperty NameProperty =
        BindableProperty.Create(nameof(Name), typeof(string), typeof(AccelerometerSensor), default(string));

    public string Name
    {
        get => (string)GetValue(NameProperty);
        set => SetValue(NameProperty, value);
    }

    private void ToggleAccelerometer()
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
                    AccelLabel.FontSize = 30;
                }
                else
                {
                    // Turn off accelerometer
                    Accelerometer.Default.Stop();
                    Accelerometer.Default.ReadingChanged -= Accelerometer_ReadingChanged;
                    DisableAccelerometer();
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

            speed--; // Remove gravity effect
            speed = Math.Max(0, speed);

            var minSpeed = 1.0; // Minimum speed (green)
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

    private void DisableAccelerometer()
    {
        sensorFrame.BorderColor = Colors.Gray;
        AccelLabel.Text = "Accelerometer not active";
        AccelLabel.FontSize = 15;
    }

    private void ToggleCheckBox_Changed(object sender, CheckedChangedEventArgs e)
    {
        if (e.Value)
        {
            ToggleAccelerometer();
            AccelLabel.FontSize = 30;
        }
        else
        {
            ToggleAccelerometer();
        }

        Preferences.Set(AccelerometerCheckboxKey, e.Value);
    }

    public AccelerometerSensor()
    {
        InitializeComponent();
        BindingContext = this;

        if (Preferences.ContainsKey(AccelerometerCheckboxKey))
        {
            var isChecked = Preferences.Get(AccelerometerCheckboxKey, true);

            if (isChecked)
            {
                ToggleCheckBox.IsChecked = isChecked;
            }
            else
            {
                DisableAccelerometer();
            }
        }
        else
        {
            ToggleCheckBox.IsChecked = true;
            Preferences.Set(AccelerometerCheckboxKey, true);
        }
    }
}
