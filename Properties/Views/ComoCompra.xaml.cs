using APIPortalKiosco.Entities;
using APIPortalWebMed.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;


namespace Portal.Kiosco.Properties.Views
{
    /// <summary>
    /// Lógica de interacción para ComoCompra.xaml
    /// </summary>
    public partial class ComoCompra : Window
    {
        public ComoCompra()
        {
            InitializeComponent();
            App.ob_diclst = new System.Collections.Generic.Dictionary<string, string>();
            App.IsFecha = false;
            App.ProductosSeleccionados = new List<Producto>();

            DoubleAnimation fadeInAnimation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.5));
            gridPrincipal.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);
        }

        private async void btnCinefans_Click(object sender, RoutedEventArgs e)
        {
            App.IsCinefans = false;
            var openWindow = new Scanear_documento();
            openWindow.Show();
            this.Close();
        }
        private bool procesoEnCurso = false;

        private async void btnInvitado_Click(object sender, RoutedEventArgs e)
        {
            if (procesoEnCurso)
                return;

            procesoEnCurso = true;

            try
            {
                if (App.IsBoleteriaConfiteria == false)
                {
                    App.IsCinefans = true;
                    var openWindow = new Cartelera();
                    DoubleAnimation fadeOutAnimation = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.5));
                    this.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);
                    await Task.Delay(300);
                    this.Visibility = Visibility.Collapsed;
                    openWindow.Background = Brushes.White;
                    openWindow.Show();
                    this.Close();
                    DoubleAnimation fadeInAnimation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.5));
                    openWindow.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);
                    ;
                }
                else
                {
                    App.IsCinefans = true;
                    var openWindow = new Combos();
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
            finally
            {
                procesoEnCurso = false;
            }
        }


        private async void btnVolverComoCompra_Click(object sender, RoutedEventArgs e)
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
