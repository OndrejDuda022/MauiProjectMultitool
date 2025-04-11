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
                AccelLabel.TextColor = Colors.LightGray;
                AccelLabel.Text = "Accelerometer not supported on this device.";
            }
        }
        catch (Exception ex)
        {
            AccelLabel.TextColor = Colors.LightGray;
            AccelLabel.Text = $"Error: {ex.Message}";
        }
    }

    private void Accelerometer_ReadingChanged(object sender, AccelerometerChangedEventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            var acceleration = e.Reading.Acceleration;
            var speedThreshold = 1.5; // Adjust this threshold as needed

            if (Math.Abs(acceleration.X) > speedThreshold ||
                Math.Abs(acceleration.Y) > speedThreshold ||
                Math.Abs(acceleration.Z) > speedThreshold)
            {
                AccelLabel.TextColor = Colors.Pink; // Change text color to pink if speed increases
            }
            else
            {
                AccelLabel.TextColor = Colors.Lavender; // Default color
            }

            AccelLabel.Text = $"Accel: X={acceleration.X}, Y={acceleration.Y}, Z={acceleration.Z}";
        });
    }


    public AccelerometerSensor()
    {
        InitializeComponent();
        BindingContext = this;
        ToggleAccelerometer();
    }
}