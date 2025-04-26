namespace MauiProjectMultitool.Models;

public partial class Sensor : ContentView
{
    private const string SensorCheckboxKey = "SensorCheckboxState";

    public static readonly BindableProperty NameProperty =
        BindableProperty.Create(nameof(Name), typeof(string), typeof(Sensor), default(string));

    public string Name
    {
        get => (string)GetValue(NameProperty);
        set => SetValue(NameProperty, value);
    }

    int count = 0;
    private void Button_Clicked(object sender, EventArgs e)
    {
        count++;
        SensorButton.Text = $"Clicked {count} times";
    }

    private void ToggleCheckBox_Changed(object sender, CheckedChangedEventArgs e)
    {
        SensorButton.IsEnabled = e.Value;
        Preferences.Set(SensorCheckboxKey, e.Value);
    }

    public Sensor()
    {
        InitializeComponent();
        BindingContext = this;

        if (Preferences.ContainsKey(SensorCheckboxKey))
        {
            var isChecked = Preferences.Get(SensorCheckboxKey, true);
            ToggleCheckBox.IsChecked = isChecked;
            SensorButton.IsEnabled = isChecked;
        }
        else
        {
            ToggleCheckBox.IsChecked = true;
            Preferences.Set(SensorCheckboxKey, true);
        }
    }
}
