namespace MauiProjectMultitool.Pages;

public partial class Help : ContentPage
{
	public Help()
	{
		InitializeComponent();
	}
    private async void OnReturnTapped(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}