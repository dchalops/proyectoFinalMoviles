using System.Collections.ObjectModel;
using Microsoft.Maui.Controls;
using proyectoFinalMoviles.Models;

namespace proyectoFinalMoviles.Views
{
    public partial class SupportEntryPage : ContentPage
    {
        public ObservableCollection<Marcacion> Marcaciones { get; set; }

        public SupportEntryPage()
        {
            InitializeComponent();
            Marcaciones = new ObservableCollection<Marcacion>
            {
                new Marcacion { Nombre = "Jose Perez", Codigo = "101715", Hora = "8:30" },
                new Marcacion { Nombre = "Maria Lopez", Codigo = "102315", Hora = "8:45" },
                new Marcacion { Nombre = "Luis Garcia", Codigo = "103115", Hora = "9:00" },
                new Marcacion { Nombre = "Ana Torres", Codigo = "104215", Hora = "9:15" },
                new Marcacion { Nombre = "Carlos Sanchez", Codigo = "105515", Hora = "9:30" }
            };
            BindingContext = this;
        }
    }

}
