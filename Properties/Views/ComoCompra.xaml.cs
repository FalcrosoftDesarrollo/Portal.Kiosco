using APIPortalKiosco.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Media.Animation;

namespace Portal.Kiosco.Properties.Views
{
    /// <summary>
    /// Lógica de interacción para ComoCompra.xaml
    /// </summary>
    public partial class ComoCompra : Window
    {
        private bool isThreadActive = true;

        public ComoCompra()
        {
            InitializeComponent();
            App.ob_diclst = new System.Collections.Generic.Dictionary<string, string>();
            App.IsFecha = false;
            App.ProductosSeleccionados = new List<Producto>();

            DoubleAnimation fadeInAnimation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.5));
            gridPrincipal.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);
            Thread thread = new Thread(() =>
            {
                while (isThreadActive)
                {
                    ComprobarTiempo();
                }
            });
            thread.IsBackground = true;
            thread.Start();
        }

        private void ComprobarTiempo()
        {
            if (App._tiempoRestanteGlobal == "00:00")
            {
                this.Dispatcher.Invoke(() =>
                {
                    Principal principal = Application.Current.Windows.OfType<Principal>().FirstOrDefault();
                    if (principal != null)
                    {
                        this.Close();
                        principal.Show();
                    }
                    else
                    {

                        Principal p = new Principal();
                        this.Close();
                        p.Show();
                    }
                });
            }
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
                    Cartelera w = new Cartelera();
                    this.Close();
                    w.ShowDialog();
                }
                else
                {
                    isThreadActive = false;
                    App.IsCinefans = true;
                    Combos w = new Combos();
                    this.Close();
                    w.ShowDialog();

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
            Principal w = new Principal();
            this.Close();
            w.ShowDialog();
        }

    }   
}