using System.Threading.Tasks;
using System;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Media;
//using TEFII_NET;

namespace Portal.Kiosco.Properties.Views
{
    /// <summary>
    /// Lógica de interacción para Frame15.xaml
    /// </summary>
    public partial class InstruccionesDatafono : Window
    {
        public InstruccionesDatafono()
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

            // Codigo de datafono
            // 

            DoubleAnimation fadeInAnimation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.5));
            gridPrincipal.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);


            //var consumo = new TEFII_NET.trx.TEFTransactionManager();
        }

        private async void btnSiguiente_Click(object sender, RoutedEventArgs e)
        {
            var openWindow = new BoletasGafasAlimentos();
            DoubleAnimation fadeOutAnimation = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.5));
            this.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);
            await Task.Delay(300);
            this.Visibility = Visibility.Collapsed;
            openWindow.Background = Brushes.White;
            openWindow.Show();
            this.Close();
            DoubleAnimation fadeInAnimation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.5));
            openWindow.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);
        }

        private void btnVolver_Click(object sender, RoutedEventArgs e)
        {
            //PagoCashback w = new PagoCashback();
            //this.Close();
            //w.ShowDialog();
        }

        private async void btnSalir_Click(object sender, RoutedEventArgs e)
        {
            var openWindow = new Principal();
            DoubleAnimation fadeOutAnimation = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.5));
            this.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);
            await Task.Delay(300);
            this.Visibility = Visibility.Collapsed;
            openWindow.Background = Brushes.White;
            openWindow.Show();
            this.Close();
            DoubleAnimation fadeInAnimation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.5));
            openWindow.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);

        }
    }
}