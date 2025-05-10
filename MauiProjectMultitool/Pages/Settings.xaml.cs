using Microsoft.Maui.Controls;

namespace MauiProjectMultitool.Pages
{
    public partial class Settings : ContentPage
    {
        public Settings()
        {
            InitializeComponent();
            LoadThemePreference();
        }

        private void LoadThemePreference()
        {
            var theme = Preferences.Get("AppTheme", "Auto");

            switch (theme)
            {
                case "Auto":
                HighlightButton(buttAuto);
                break;
                case "Dark":
                HighlightButton(buttDark);
                break;
                case "Light":
                HighlightButton(buttLight);
                break;
            }
        }

        private void OnThemeAutoClicked(object sender, EventArgs e)
        {
            SetTheme(AppTheme.Unspecified);
            HighlightButton(buttAuto);
            Preferences.Set("AppTheme", "Auto");
        }

        private void OnThemeDarkClicked(object sender, EventArgs e)
        {
            SetTheme(AppTheme.Dark);
            HighlightButton(buttDark);
            Preferences.Set("AppTheme", "Dark");
        }

        private void OnThemeLightClicked(object sender, EventArgs e)
        {
            SetTheme(AppTheme.Light);
            HighlightButton(buttLight);
            Preferences.Set("AppTheme", "Light");
        }

        private void SetTheme(AppTheme theme)
        {
            Application.Current.UserAppTheme = theme;
        }

        private void HighlightButton(Button selectedButton)
        {
            var isDarkTheme = Application.Current.RequestedTheme == AppTheme.Dark;

            buttAuto.BackgroundColor = Colors.Transparent;
            buttAuto.TextColor = isDarkTheme ? Colors.White : Colors.Black;

            buttDark.BackgroundColor = Colors.Transparent;
            buttDark.TextColor = isDarkTheme ? Colors.White : Colors.Black;

            buttLight.BackgroundColor = Colors.Transparent;
            buttLight.TextColor = isDarkTheme ? Colors.White : Colors.Black;

            selectedButton.BackgroundColor = Colors.Purple;
            selectedButton.TextColor = Colors.White;
        }
        private async void OnReturnTapped(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}