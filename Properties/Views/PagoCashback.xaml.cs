using APIPortalKiosco.Entities;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Media.Animation;

namespace Portal.Kiosco.Properties.Views
{
    public partial class PagoCashback : Window
    {
        private readonly IOptions<MyConfig> config;
        private bool isThreadActive = true;

        public PagoCashback()
        {
            InitializeComponent();
            DataContext = ((App)Application.Current);
            lblTotalPagarCash.Content = Convert.ToDecimal(App.TotalPagar).ToString("C0");
            lblCashDisp.Content = Convert.ToDecimal(App.Saldo).ToString("C0");

            if (App.ob_diclst.Count > 0)
            {
                lblnombre.Content = "!HOLA " + App.ob_diclst["Nombre"].ToString() + " " + App.ob_diclst["Apellido"].ToString();
            }
            else
            {
                lblnombre.Content = "!HOLA INVITADO¡";
            }
            
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

        private bool ComprobarTiempo()
        {
            bool isMainWindowOpen = false; 

            if (App._tiempoRestanteGlobal == "00:00")
            {
                this.Dispatcher.Invoke(() =>
                {
                    Principal principal = Application.Current.Windows.OfType<Principal>().FirstOrDefault();
                    if (principal != null && principal.Visibility == Visibility.Visible)
                    {
                        principal.Activate();
                        isMainWindowOpen = true;
                    }
                    else
                    {
                        if (!isMainWindowOpen)
                        {
                            if (principal == null)
                            {
                                principal = new Principal();
                                principal.Show();
                                isMainWindowOpen = true;
                            }
                            
                            foreach (Window window in Application.Current.Windows)
                            {
                                if (window != principal && window != this)
                                {
                                    window.Close();
                                }
                            }
                        }
                    }

                });
            }

            return isMainWindowOpen; 
        }

        private async void btnSiguiente_Click(object sender, RoutedEventArgs e)
        {
            isThreadActive = false;
            InstruccionesDatafono openWindows = new InstruccionesDatafono();
            openWindows.Show();
            this.Close();
        }

        private async void btnVolver_Click(object sender, RoutedEventArgs e)
        {
            isThreadActive = false;
            ResumenCompra openWindows = new ResumenCompra(config);
            openWindows.Show();
            this.Close();
        }

        private async void btnCambiar_Click(object sender, RoutedEventArgs e)
        {
            isThreadActive = false;
            InstruccionesDatafono openWindows = new InstruccionesDatafono();
            var openWindow = new InstruccionesDatafono();
            openWindows.Show();
            this.Close();
        }

        private void btnSalir_Click(object sender, RoutedEventArgs e)
        {
            App.RoomReverse();
            var openWindows = new Principal();
            openWindows.Show();
            this.Close();
        }

        private void PagoConCash_Click(object sender, RoutedEventArgs e)
        {
            Producto producto = new Producto
            {
                TipoCompra = App.TipoCompra,
                KeySecuencia = App.Secuencia,
                SwtVenta = "V",
                SwitchCashback = "S",
                Valor = App.TotalPagar,
            };
            App.Payment(producto);
        }
    }
}