namespace MauiProjectMultitool.Models;

public partial class Sensor : ContentView
{
    public partial class Sensor : ContentView
    {
        public static readonly BindableProperty NameProperty =
            BindableProperty.Create(nameof(Name), typeof(string), typeof(Sensor), default(string));

        public string Name
        {
            get => (string)GetValue(NameProperty);
            set => SetValue(NameProperty, value);
        }

        public Sensor()
        {
            InitializeComponent();
            BindingContext = this;
        }
    }
}