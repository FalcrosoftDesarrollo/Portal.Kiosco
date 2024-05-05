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
            DataContext = ((App)Application.Current);
            if (App.ob_diclst.Count > 0)
            {
                lblnombre.Content = "!HOLA " + App.ob_diclst["Nombre"].ToString() + " " + App.ob_diclst["Apellido"].ToString();
            }
            else
            {
                lblnombre.Content = "!HOLA INVITADO";
            }
        }



        private async void btnSalir_Click(object sender, RoutedEventArgs e)
        {   
            var openWindow = new Principal();
            openWindow.Show();
            this.Close();
             
        }

        private async void btnVolver_Click(object sender, RoutedEventArgs e)
        {
            var openWindow = new AlgoParaComer();
            openWindow.Show();
            this.Close();
        }

        private async void btnSiguiente_Click(object sender, RoutedEventArgs e)
        {
            var openWindow = new Combos();
            openWindow.Show();
            this.Close();

        }

        private async void btnCombos_Click(object sender, RoutedEventArgs e)
        {
            var openWindow = new Combos();
            openWindow.Show();
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void btnResumen_Click(object sender, RoutedEventArgs e)
        {
            var openWindow = new ResumenCompra();
            openWindow.Show();
            this.Close();

        }
    }
}