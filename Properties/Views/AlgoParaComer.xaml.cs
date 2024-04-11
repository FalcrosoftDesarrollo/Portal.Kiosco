using System.Threading.Tasks;
using System;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Media;

namespace Portal.Kiosco.Properties.Views
{
    /// <summary>
    /// Lógica de interacción para Frame9.xaml
    /// </summary>
    public partial class AlgoParaComer : Window
    {
        public AlgoParaComer()
        {
            InitializeComponent();
        }

        private async void btnSalir_Click(object sender, RoutedEventArgs e)
        {
            await App.OpenWindow("Principal");
        }

        private void btnVolver_Click(object sender, RoutedEventArgs e)
        {
            Gafas3D w = new Gafas3D();
            this.Close();
            w.ShowDialog();
        }

        private async void btnSiguiente_Click(object sender, RoutedEventArgs e)
        {
            await App.OpenWindow("Combos");
        }

        private async void btnCombos_Click(object sender, RoutedEventArgs e)
        {   
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void btnResumen_Click(object sender, RoutedEventArgs e)
        {
            await App.OpenWindow("Combos");
        }
    }
}