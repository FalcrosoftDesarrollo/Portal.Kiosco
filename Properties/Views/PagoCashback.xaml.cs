using System.Threading.Tasks;
using System;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Media;

namespace Portal.Kiosco.Properties.Views
{
    public partial class PagoCashback : Window
    {
        public PagoCashback()
        {
            InitializeComponent();
            if (App.ob_diclst.Count > 0)
            {
                lblnombre.Content = "!HOLA " + App.ob_diclst["Nombre"].ToString() + " " + App.ob_diclst["Apellido"].ToString();
            }
            else
            {
                lblnombre.Content = "!HOLA INVITADO";
            }
            DataContext = ((App)Application.Current);
        }

        private void btnSiguiente_Click(object sender, RoutedEventArgs e)
        {
            InstruccionesDatafono w = new InstruccionesDatafono();
            this.Close();
            w.ShowDialog();
        }

        private void btnVolver_Click(object sender, RoutedEventArgs e)
        {
            ResumenCompra w = new ResumenCompra();
            this.Close();
            w.ShowDialog();
        }

        private async void btnCambiar_Click(object sender, RoutedEventArgs e)
        {
            var openWindow = new InstruccionesDatafono();
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
