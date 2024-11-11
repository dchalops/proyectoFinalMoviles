using System.Net.Http;
using System.Threading.Tasks;
using proyectoFinalMoviles.Services;
using Microsoft.Maui.Controls;

namespace LoginFlow.Views;

public partial class LoginPage : ContentPage
{
    private readonly AuthService _authService;

    public LoginPage()
    {
        InitializeComponent();
        _authService = new AuthService();
    }

    protected override bool OnBackButtonPressed()
    {
        Application.Current.Quit();
        return true;
    }

    private async void LoginButton_Clicked(object sender, EventArgs e)
    {
        LoadingIndicator.IsVisible = true;
        LoadingIndicator.IsRunning = true;

        LoginButton.IsEnabled = false;

        await Task.Delay(1);

        try
        {
            var (isAuthenticated, errorMessage) = await _authService.AuthenticateAsync(Username.Text, Password.Text);

            if (isAuthenticated)
            {
                await Navigation.PushAsync(new HomePage());
            }
            else
            {
                await DisplayAlert("Login failed", errorMessage ?? "Username or password is invalid", "OK");
            }
        }
        finally
        {
            LoadingIndicator.IsVisible = false;
            LoadingIndicator.IsRunning = false;
            LoginButton.IsEnabled = true;
        }
    }
}
