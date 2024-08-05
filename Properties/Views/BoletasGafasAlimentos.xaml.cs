using APIPortalKiosco.Entities;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Animation;

namespace Portal.Kiosco.Properties.Views
{
    public partial class BoletasGafasAlimentos : Window
    {
        private readonly IOptions<MyConfig> config;
        private bool isThreadActive = true;

        public BoletasGafasAlimentos()
        {
            InitializeComponent();
            DataContext = ((App)Application.Current);
            if (App.ob_diclst.Count > 0)
            {
                lblnombre.Content = "¡HOLA " + App.ob_diclst["Nombre"].ToString() + " " + App.ob_diclst["Apellido"].ToString() + "!";
            }
            else
            {
                lblnombre.Content = "¡HOLA INVITADO!";
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

             
            var nuevaVista = new BoletaFactura(config); 
             
            resultImprecion.Content = nuevaVista;
        }

        public async Task LoadDataAsync()
        {
            await Task.Run(() =>
            {
                System.Threading.Thread.Sleep(3000);
            });
        }

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


        private void btnImprimir_Click(object sender, RoutedEventArgs e)
        {
            var compra = App.TipoCompra;
            var ventanaSecundaria = new BoletaFactura(config);

            UIElement contenidoVisual = ventanaSecundaria.Content as UIElement;

            ventanaSecundaria.Content = null;

            ImpresionDirectaWPF impresion = new ImpresionDirectaWPF(contenidoVisual);

            impresion.ImprimirDirecto();

            var gracias = new GraciasXcomprar();
            gracias.Show();
            this.Close();
        } 

        private void btnSiguiente_Click(object sender, RoutedEventArgs e)
        {
            isThreadActive = false;
            CorreoTecladoFlotante openWindows = new CorreoTecladoFlotante(config);
            openWindows.Show();
            this.Close();
        }

        private void btnVolver_Click(object sender, RoutedEventArgs e)
        {
            isThreadActive = false;
            InstruccionesDatafono openWindows = new InstruccionesDatafono();
            openWindows.Show();
            this.Close();
        }

        private async void btnSalir_Click(object sender, RoutedEventArgs e)
        {
            isThreadActive = false;
            Principal openWindows = new Principal();
            openWindows.Show();
            this.Close();

        }

        private async void btnEnviar_Click(object sender, RoutedEventArgs e)
        {
            isThreadActive = false;
            CorreoTecladoFlotante openWindows = new CorreoTecladoFlotante(config);
            openWindows.Show();
            this.Close();
        }
    }
}