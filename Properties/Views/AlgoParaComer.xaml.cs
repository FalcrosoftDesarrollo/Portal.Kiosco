using APIPortalKiosco.Entities;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Media.Animation;

namespace Portal.Kiosco.Properties.Views
{
    /// <summary>
    /// Lógica de interacción para Frame9.xaml
    /// </summary>
    public partial class AlgoParaComer : Window
    {
        private readonly IOptions<MyConfig> config;
        private bool isThreadActive = true;

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

        private async void btnSalir_Click(object sender, RoutedEventArgs e)
        {
            isThreadActive = false;
            Principal w = new Principal();
            this.Close();
            w.ShowDialog();

        }

        private async void btnVolver_Click(object sender, RoutedEventArgs e)
        {
            isThreadActive = false;
            AlgoParaComer w = new AlgoParaComer();
            this.Close();
            w.ShowDialog();
        }

        private async void btnSiguiente_Click(object sender, RoutedEventArgs e)
        {
            isThreadActive = false;
            Combos w = new Combos();
            this.Close();
            w.ShowDialog();

        }

        private async void btnCombos_Click(object sender, RoutedEventArgs e)
        {
            isThreadActive = false;
            Combos w = new Combos();
            this.Close();
            w.ShowDialog();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void btnResumen_Click(object sender, RoutedEventArgs e)
        {
            isThreadActive = false;
            ResumenCompra w = new ResumenCompra(config);
            this.Close();
            w.ShowDialog();
        }
    }
}