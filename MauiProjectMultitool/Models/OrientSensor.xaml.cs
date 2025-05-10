using System.Diagnostics.Tracing;

namespace MauiProjectMultitool.Models;

public partial class OrientSensor : ContentView
{
    private const string OrientationCheckboxKey = "OrientationCheckboxState";

    public static readonly BindableProperty NameProperty =
        BindableProperty.Create(nameof(Name), typeof(string), typeof(OrientSensor), default(string));

    public string Name
    {
        get => (string)GetValue(NameProperty);
        set => SetValue(NameProperty, value);
    }
    private void ToggleOrientation()
    {
        if (OrientationSensor.Default.IsSupported)
        {
            if (!OrientationSensor.Default.IsMonitoring)
            {
                // Turn on orientation
                OrientationSensor.Default.ReadingChanged += Orientation_ReadingChanged;
                OrientationSensor.Default.Start(SensorSpeed.Default);
                sensorFrame.BorderColor = Colors.DarkGray;
                OriLabel.FontSize = 20;
                if (Application.Current.Resources.TryGetValue("ExpressiveBlue", out var headerTextColor) && headerTextColor is Color color)
                {
                    Bubble.Fill = color;
                }
                else
                {
                    Bubble.Fill = Colors.White; // Fallback color
                }

            }
            else
            {
                // Turn off orientation
                OrientationSensor.Default.Stop();
                OrientationSensor.Default.ReadingChanged -= Orientation_ReadingChanged;
                DisableOrientation();
            }
        }
    }

    private void Orientation_ReadingChanged(object sender, OrientationSensorChangedEventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            var orientation = e.Reading.Orientation;

            double pitch = Math.Atan2(2 * (orientation.W * orientation.X + orientation.Y * orientation.Z), 1 - 2 * (orientation.X * orientation.X + orientation.Y * orientation.Y)) * (180 / Math.PI);
            double roll = Math.Asin(2 * (orientation.W * orientation.Y - orientation.Z * orientation.X)) * (180 / Math.PI);

            OriLabel.Text = $"Pitch: {pitch:F1}°\nRoll: {roll:F1}°";

            const double maxOffset = 80;
            double xOffset = Math.Clamp(-roll / 45.0 * maxOffset, -maxOffset, maxOffset);
            double yOffset = Math.Clamp(pitch / 45.0 * maxOffset, -maxOffset, maxOffset);

            Bubble.TranslationX = xOffset;
            Bubble.TranslationY = -yOffset;
        });
    }

    private void DisableOrientation()
    {
        sensorFrame.BorderColor = Colors.Gray;
        OriLabel.Text = "Orientation not active";
        OriLabel.FontSize = 15;
        Bubble.TranslationX = 0;
        Bubble.TranslationY = 0;
        Bubble.Fill = Colors.Transparent;
    }

    private void ToggleCheckBox_Changed(object sender, CheckedChangedEventArgs e)
    {
        if (e.Value)
        {
            ToggleOrientation();
        }
        else
        {
            ToggleOrientation();
        }

        Preferences.Set(OrientationCheckboxKey, e.Value);
    }

    public OrientSensor()
    {
        InitializeComponent();
        BindingContext = this;

        if (Preferences.ContainsKey(OrientationCheckboxKey))
        {
            var isChecked = Preferences.Get(OrientationCheckboxKey, true);

            if (isChecked)
            {
                ToggleCheckBox.IsChecked = isChecked;
            }
            else
            {
                DisableOrientation();
            }
        }
        else
        {
            ToggleCheckBox.IsChecked = true;
            Preferences.Set(OrientationCheckboxKey, true);
        }
    }
}
