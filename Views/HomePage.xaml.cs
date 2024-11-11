using proyectoFinalMoviles.Views;

namespace LoginFlow.Views;

public partial class HomePage : ContentPage
{
	public HomePage()
	{
		InitializeComponent();
	}

    private async void OnEntradaTapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new SupportEntryPage());
    }

    private async void OnOrdenesTapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new OrdersWorkPage());
    }

    private async void OnNovedadesTapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new NewsPage());
    }

    private async void OnSalidaTapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AssistanceDeparturePage());
    }

}