namespace MauiProjectMultitool.Models;

public partial class GyroscopeSensor : ContentView
{
    public static readonly BindableProperty NameProperty =
        BindableProperty.Create(nameof(Name), typeof(string), typeof(GyroscopeSensor), default(string));

    public string Name
    {
        get => (string)GetValue(NameProperty);
        set => SetValue(NameProperty, value);
    }
    private void ToggleGyroscope()
    {
        if (Gyroscope.Default.IsSupported)
        {
            if (!Gyroscope.Default.IsMonitoring)
            {
                // Turn on gyroscope
                Gyroscope.Default.ReadingChanged += Gyroscope_ReadingChanged;
                Gyroscope.Default.Start(SensorSpeed.Default);
            }
            else
            {
                // Turn off gyroscope
                Gyroscope.Default.Stop();
                Gyroscope.Default.ReadingChanged -= Gyroscope_ReadingChanged;
            }
        }
    }

    private void Gyroscope_ReadingChanged(object sender, GyroscopeChangedEventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            var angularVelocity = e.Reading.AngularVelocity;

            var x = angularVelocity.X.ToString("F2");
            var y = angularVelocity.Y.ToString("F2");
            var z = angularVelocity.Z.ToString("F2");

            GyroLabel.Text = $"X: {x} rad/s\nY: {y} rad/s\nZ: {z} rad/s";

            var maxAngularVelocity = 10.0; // Max
            var normalizedValue = Math.Min(1.0, Math.Sqrt(
                Math.Pow(angularVelocity.X, 2) +
                Math.Pow(angularVelocity.Y, 2) +
                Math.Pow(angularVelocity.Z, 2)
            ) / maxAngularVelocity);

            var borderColor = InterpolateColor(Colors.Green, Colors.Red, normalizedValue);
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

    public GyroscopeSensor()
	{
		InitializeComponent();
        BindingContext = this;
        ToggleGyroscope();
    }
}