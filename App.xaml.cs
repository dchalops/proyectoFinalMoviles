using LoginFlow.Views;

namespace proyectoFinalMoviles
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            //MainPage = new AppShell();
            MainPage = new NavigationPage(new LoadingPage());
        }
    }
}
