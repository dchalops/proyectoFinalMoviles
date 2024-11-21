using Microsoft.Maui.Controls;

namespace proyectoFinalMoviles.Views;

public partial class CustomFooter : ContentView
{
	public CustomFooter()
	{
		InitializeComponent();
	}
    private void OnRefreshClicked(object sender, EventArgs e)
    {
        // Lógica para el icono de recargar
    }

    private async void OnHomeClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//HomePage");
    }

    private void OnDocumentClicked(object sender, EventArgs e)
    {
        // Lógica para el icono de documento
    }
}