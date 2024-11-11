namespace LoginFlow.Views;

public partial class LoadingPage : ContentPage
{
    public LoadingPage()
    {
        InitializeComponent();

    }
    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        if (await isAuthenticated())
        {
            await Navigation.PushAsync(new LoginPage());
        }
        else
        {
            await Navigation.PushAsync(new LoginPage());
        }
        base.OnNavigatedTo(args);
    }

    async Task<bool> isAuthenticated()
    {
        await Task.Delay(5000);
        var hasAuth = await SecureStorage.GetAsync("hasAuth");
        return !(hasAuth == null);
    }
}