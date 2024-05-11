using System.Threading.Tasks;
using System;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Media;
using APIPortalKiosco.Entities;
using Microsoft.Extensions.Options;

namespace Portal.Kiosco.Properties.Views
{
    public partial class PagoCashback : Window
    {
        private readonly IOptions<MyConfig> config;
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
            DoubleAnimation fadeInAnimation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.5));
            gridPrincipal.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);

        }

        private async void btnSiguiente_Click(object sender, RoutedEventArgs e)
        {
            var openWindow = new InstruccionesDatafono();
            openWindow.Show();
            this.Close();
       
        }

        private async void btnVolver_Click(object sender, RoutedEventArgs e)
        {
            var openWindow = new ResumenCompra(config);
            openWindow.Show();
            this.Close();
           
        }

        private async void btnCambiar_Click(object sender, RoutedEventArgs e)
        {
            var openWindow = new InstruccionesDatafono();
         
            openWindow.Show();
            this.Close();
         
        }

        private void btnSalir_Click(object sender, RoutedEventArgs e)
        {
            App.RoomReverse();
            var openWindow = new Principal();
            openWindow.Show();
            this.Close();
        }
    }
}
