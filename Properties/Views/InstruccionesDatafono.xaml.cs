using APIPortalKiosco.Entities;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Media.Animation;

namespace Portal.Kiosco.Properties.Views
{
    public partial class InstruccionesDatafono : Window
    {
        private bool isThreadActive = true;
        private System.Timers.Timer timer;
        public int validador = 0;
        private readonly IOptions<MyConfig> config;

        public InstruccionesDatafono()
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
            validador = 0;
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

            Loaded += InstruccionesDatafono_Loaded;
        }

        private void InstruccionesDatafono_Loaded(object sender, RoutedEventArgs e)
        {
            timer = new System.Timers.Timer(3000);
            timer.Elapsed += Timer_Elapsed;
            timer.AutoReset = false;
            timer.Start();
        }


        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                LLamadoDatafono();
            });
        }


        public void LLamadoDatafono()
        {
            
                Producto producto = new Producto
                {
                    TipoCompra = App.TipoCompra,
                    KeySecuencia = App.Secuencia,
                    SwtVenta = "V",
                    SwitchCashback = "N",
                    Valor = App.TotalPagar,
                };


                App.Payment(producto);
                App.validadorVenta = 1;
            
            if (App.EstadoScore == "0")
            {
                var subtotal = 0;
                var IAC = 0;
                var total = Convert.ToInt64(App.TotalPagar);
                var subtotalString = "";
            ;
                // Si App.IVA es null, se utiliza "0"
                string ivaString = App.IVA ?? "0";

                string icaString = App.IVC ?? "0";
                // Convertir el valor a decimal
                decimal ivaDecimal;
                if (decimal.TryParse(ivaString, out ivaDecimal))
                {
                    // Redondear el valor
                     subtotal = Convert.ToInt32(Math.Round(ivaDecimal));

                    // Convertir a string si es necesario
                     subtotalString = subtotal.ToString();
                    App.IVA = subtotalString;
                }
                else
                {
                    // Manejar el caso en que la conversión falle
                    throw new FormatException("El valor de IVA no tiene un formato correcto.");
                }
                decimal icaDecimal;
                if (decimal.TryParse(icaString, out icaDecimal))
                {
                    // Redondear el valor
                    IAC = Convert.ToInt32(Math.Round(icaDecimal));

                    // Convertir a string si es necesario
                    icaString = IAC.ToString();
                    App.IVC = icaString;
                }
                else
                {
                    // Manejar el caso en que la conversión falle
                    throw new FormatException("El valor de IVA no tiene un formato correcto.");
                }


              


                var responseSection = "";


                String Trama = "01," + total.ToString() + "," + subtotalString + "," + App.PuntoVenta + ","+ App.Secuencia.ToString() +", 0 ," + icaString + ",KIOSCO,0,0,";

                var respuesta =  App.RunProgramAndWait(Trama);

                int start = respuesta.IndexOf("Response:");

                int end = respuesta.IndexOf("*", start);

                if (start != -1 && end != -1)
                {
                    responseSection = respuesta.Substring(start, end - start);

                    App.respuestagenerica = responseSection.Contains("Response:00") ? "00" : "Error";
                    Console.WriteLine(responseSection);
                }
                else
                {
                    MessageBox.Show("No se encontró la sección de respuesta en el texto proporcionado.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                if (App.respuestagenerica == "Error" || App.respuestagenerica == "")
                {
                    MessageBox.Show("Error al procesar el pago " + App.Secuencia + "-PUNTOVTA: " + App.PuntoVenta, "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                    var openWindows = new ResumenCompra(config);
                    openWindows.Show();
                    this.Close();
                }
                else
                {
                    App.ResponseDatafono = responseSection.Substring(0, 3).Replace(",", "");
                    var openWindows = new BoletasGafasAlimentos();
                    openWindows.Show();
                    this.Close();
                }

            }
            else
            {
                MessageBox.Show("Error al procesar el pago en SCORE" + App.Secuencia + "-PUNTOVTA: " + App.PuntoVenta, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                var openWindows = new ResumenCompra(config);
                openWindows.Show();
                this.Close();

            }

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


        private async void btnSiguiente_Click(object sender, RoutedEventArgs e)
        {
            isThreadActive = false;
            BoletasGafasAlimentos openWindows = new BoletasGafasAlimentos();
            this.Close();
            openWindows.Show();
        }

        private void btnVolver_Click(object sender, RoutedEventArgs e)
        {
            //PagoCashback openWindows = new PagoCashback();
            //this.Close();
            //openWindows.Show();
        }

        private async void btnSalir_Click(object sender, RoutedEventArgs e)
        {
            isThreadActive = false;
            Principal openWindows = new Principal();
            this.Close();
            openWindows.Show();
        }
    }
}