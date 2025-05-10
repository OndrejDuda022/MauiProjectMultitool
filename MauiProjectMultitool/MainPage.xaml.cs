using MauiProjectMultitool.Pages;
namespace MauiProjectMultitool
{
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();
            LoadThemePreference();
            //Preferences.Clear();
        }

        private async void OnGearTapped(object sender, EventArgs e)
        {
            // Navigate to the Settings page directly
            await Navigation.PushAsync(new Settings());
        }

        private async void OnQuestionTapped(object sender, EventArgs e)
        {
            // Navigate to the Settings page directly
            await Navigation.PushAsync(new Help());
        }

        private void LoadThemePreference()
        {
            var theme = Preferences.Get("AppTheme", "Auto");

            switch (theme)
            {
                case "Auto":
                SetTheme(AppTheme.Unspecified);
                break;
                case "Dark":
                SetTheme(AppTheme.Dark);
                break;
                case "Light":
                SetTheme(AppTheme.Light);
                break;
            }
        }
        private void SetTheme(AppTheme theme)
        {
            Application.Current.UserAppTheme = theme;
        }
    }

}
