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
                sensorFrame.BorderColor = Colors.LightGray;
                OriLabel.FontSize = 20;
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

            double pitch = Math.Asin(2 * (orientation.W * orientation.Y - orientation.Z * orientation.X)) * (180 / Math.PI);
            double roll = Math.Atan2(2 * (orientation.W * orientation.X + orientation.Y * orientation.Z), 1 - 2 * (orientation.X * orientation.X + orientation.Y * orientation.Y)) * (180 / Math.PI);
            double yaw = Math.Atan2(2 * (orientation.W * orientation.Z + orientation.X * orientation.Y), 1 - 2 * (orientation.Y * orientation.Y + orientation.Z * orientation.Z)) * (180 / Math.PI);

            OriLabel.Text = $"Pitch: {pitch:F1}°\nRoll: {roll:F1}°\nYaw: {yaw:F1}°";
        });
    }


    private void DisableOrientation()
    {
        sensorFrame.BorderColor = Colors.Gray;
        OriLabel.Text = "Orientation not active";
        OriLabel.FontSize = 15;
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
