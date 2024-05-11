using System;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Media.Animation;
//using TEFII_NET;

namespace Portal.Kiosco.Properties.Views
{
    /// <summary>
    /// Lógica de interacción para Frame15.xaml
    /// </summary>
    public partial class InstruccionesDatafono : Window
    {
        private bool isThreadActive = true;

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

        private async void btnSiguiente_Click(object sender, RoutedEventArgs e)
        {
            isThreadActive = false;
            BoletasGafasAlimentos w = new BoletasGafasAlimentos();
            this.Close();
            w.ShowDialog();
        }

        private void btnVolver_Click(object sender, RoutedEventArgs e)
        {
            //PagoCashback w = new PagoCashback();
            //this.Close();
            //w.ShowDialog();
        }

        private async void btnSalir_Click(object sender, RoutedEventArgs e)
        {
            isThreadActive = false;
            Principal w = new Principal();
            this.Close();
            w.ShowDialog();
        }
    }
}