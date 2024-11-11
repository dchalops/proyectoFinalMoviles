using LoginFlow.Views;
using proyectoFinalMoviles.Views;

namespace proyectoFinalMoviles
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute("login", typeof(LoginPage));
            Routing.RegisterRoute("main", typeof(MainPage));
            Routing.RegisterRoute("home", typeof(HomePage));
            Routing.RegisterRoute("settings", typeof(SettingsPage));
            Routing.RegisterRoute("SupportEntryPage", typeof(SupportEntryPage));
        }
    }
}
