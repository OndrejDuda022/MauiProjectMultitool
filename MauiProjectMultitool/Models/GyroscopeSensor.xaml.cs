namespace MauiProjectMultitool.Models;

public partial class GyroscopeSensor : ContentView
{
    private const string GyroscopeCheckboxKey = "GyroscopeCheckboxState";

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
                Gyroscope.Default.ReadingChanged += Gyroscope_ReadingChanged;
                Gyroscope.Default.Start(SensorSpeed.Default);
                GyroLabel.FontSize = 20;
                AbsoluteLayout.SetLayoutBounds(GyroLabel, new Rect(0.5, 0.5, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize));
            }
            else
            {
                Gyroscope.Default.Stop();
                Gyroscope.Default.ReadingChanged -= Gyroscope_ReadingChanged;
                DisableGyroscope();
            }
        }
    }

    private void Gyroscope_ReadingChanged(object sender, GyroscopeChangedEventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            var angularVelocity = e.Reading.AngularVelocity;

            var x = Math.Max(0, angularVelocity.X).ToString("F2");
            var y = Math.Max(0, angularVelocity.Y).ToString("F2");
            var z = Math.Max(0, angularVelocity.Z).ToString("F2");

            GyroLabel.Text = $"X: {x} rad/s\nY: {y} rad/s\nZ: {z} rad/s";

            var maxAngularVelocity = 10.0;
            var normalizedValue = Math.Min(1.0, Math.Sqrt(
                Math.Pow(Math.Max(0, angularVelocity.X), 2) +
                Math.Pow(Math.Max(0, angularVelocity.Y), 2) +
                Math.Pow(Math.Max(0, angularVelocity.Z), 2)
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

    private void DisableGyroscope()
    {
        sensorFrame.BorderColor = Colors.Gray;
        GyroLabel.Text = "Gyroscope not active";
        GyroLabel.FontSize = 15;
        AbsoluteLayout.SetLayoutBounds(GyroLabel, new Rect(0.5, 0.4, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize));
    }

    private void ToggleCheckBox_Changed(object sender, CheckedChangedEventArgs e)
    {
        if (e.Value)
        {
            ToggleGyroscope();
        }
        else
        {
            ToggleGyroscope();
        }

        Preferences.Set(GyroscopeCheckboxKey, e.Value);
    }

    public GyroscopeSensor()
    {
        InitializeComponent();
        BindingContext = this;

        if (Preferences.ContainsKey(GyroscopeCheckboxKey))
        {
            var isChecked = Preferences.Get(GyroscopeCheckboxKey, true);

            if (isChecked)
            {
                ToggleCheckBox.IsChecked = isChecked;
            }
            else
            {
                DisableGyroscope();
            }
        }
        else
        {
            ToggleCheckBox.IsChecked = true;
            Preferences.Set(GyroscopeCheckboxKey, true);
        }
    }
}
