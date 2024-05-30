using APIPortalKiosco.Entities;
using APIPortalWebMed.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Media.Animation;

namespace Portal.Kiosco.Properties.Views
{
    public partial class ComoCompra : Window
    {
        private bool isThreadActive = true;

        public ComoCompra()
        {
            InitializeComponent();
            App.ob_diclst = new System.Collections.Generic.Dictionary<string, string>();
            App.IsFecha = false;
            App.ProductosSeleccionados = new List<Producto>();
            App.Secuencia = "0";
            App.ProductosCambiados = new List<Producto>();
            App.Pelicula = new Pelicula();
            App.CantidadBoletas = 0;
            App.ValorTarifa = 0;

            DoubleAnimation fadeInAnimation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.5));
            gridPrincipal.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);

            Thread thread = new Thread(() =>
            {
                while (isThreadActive)
                {
                    if (ComprobarTiempo())
                    {
                        break; 
                    }
                }
            });
            thread.IsBackground = true;
            thread.Start();
        }

        private bool isMainWindowOpen = false;

        private bool ComprobarTiempo()
        {
            bool isMainWindowOpen = false;

            if (App._tiempoRestanteGlobal == "00:00")
            {
                this.Dispatcher.Invoke(() =>
                {
                    Principal principal = Application.Current.Windows.OfType<Principal>().FirstOrDefault();

                    if (principal == null)
                    {
                        principal = new Principal();
                        principal.Show();
                        isMainWindowOpen = true;
                    }
                    else if (principal.Visibility == Visibility.Visible)
                    {
                        principal.Activate();
                        isMainWindowOpen = true;
                    }
                    else
                    {
                        principal.Show();
                        isMainWindowOpen = true;
                    }

                    this.Close();

                });
            }

            return isMainWindowOpen;
        }


        private async void btnCinefans_Click(object sender, RoutedEventArgs e)
        {
            isThreadActive = false;
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
                    isThreadActive = false;
                    App.IsCinefans = true;
                    Cartelera openWindows = new Cartelera();

                    openWindows.Show();
                    this.Close();
                }
                else
                {
                    isThreadActive = false;
                    App.IsCinefans = true;
                    Combos openWindows = new Combos();

                    openWindows.Show();
                    this.Close();
                }
            }
            finally
            {
                procesoEnCurso = false;
            }
        }

        private async void btnVolverComoCompra_Click(object sender, RoutedEventArgs e)
        {
            isThreadActive = false;
            Principal openWindows = new Principal();
            openWindows.Show();
            this.Close();
        }
    }
}