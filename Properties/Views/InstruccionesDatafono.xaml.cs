using APIPortalKiosco.Entities;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

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
            botones.Visibility = Visibility.Hidden;
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

        private async void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Dispatcher.Invoke(async () =>
            {
               await LLamadoDatafonoAsync();
            });
        }

        public async Task LoadDataAsync()
        {
            await Task.Run(() =>
            {
                System.Threading.Thread.Sleep(3000);
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
                string ivaString = App.IVA ?? "0";
                string icaString = App.IVC ?? "0";
                decimal ivaDecimal;
                if (decimal.TryParse(ivaString, out ivaDecimal))
                {
                    subtotal = Convert.ToInt32(Math.Round(ivaDecimal));
                    subtotalString = subtotal.ToString();
                    App.IVA = subtotalString;
                }
                else
                {
                    throw new FormatException("El valor de IVA no tiene un formato correcto.");
                }
                decimal icaDecimal;
                if (decimal.TryParse(icaString, out icaDecimal))
                {
                    IAC = Convert.ToInt32(Math.Round(icaDecimal));
                    icaString = IAC.ToString();
                    App.IVC = icaString;
                }
                else
                {
                    throw new FormatException("El valor de IVA no tiene un formato correcto.");
                }

                var responseSection = "";


                String Trama = "01," + total.ToString() + "," + subtotalString + "," + App.PuntoVenta + "," + App.Secuencia.ToString() + ", 0 ," + icaString + ",KIOSCO,0,0,";

                var respuesta = App.RunProgramAndWait(Trama);

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

                    imgdatafono.Source = new BitmapImage(new Uri("C:\\FALCROSOFT\\PROCINAL\\Portal.Kiosco\\Properties\\Resources\\ErrorDatafono.png"));
                    borderInstrucciones.Visibility = Visibility.Hidden;
                    TextTitulo.Text = "¡Ups! La transación no se pudo realizar";
                    botones.Visibility = Visibility.Visible; 
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

                imgdatafono.Source = new BitmapImage(new Uri("C:\\FALCROSOFT\\PROCINAL\\Portal.Kiosco\\Properties\\Resources\\ErrorDatafono.png"));
                borderInstrucciones.Visibility = Visibility.Hidden;
                TextTitulo.Text = "¡Ups! La transación no se pudo realizar";
                botones.Visibility = Visibility.Visible;
            }

        }
        public async Task LLamadoDatafonoAsync()
        {
            Producto producto = new Producto
            {
                TipoCompra = App.TipoCompra,
                KeySecuencia = App.Secuencia,
                SwtVenta = "V",
                SwitchCashback = "N",
                Valor = App.TotalPagar,
            };

            await Task.Run(() => App.Payment(producto));
            App.validadorVenta = 1;

            if (App.EstadoScore == "0")
            {
                int subtotal = 0;
                int IAC = 0;
                long total = Convert.ToInt64(App.TotalPagar);
                string subtotalString = "";
                string ivaString = App.IVA ?? "0";
                string icaString = App.IVC ?? "0";
                if (decimal.TryParse(ivaString, out decimal ivaDecimal))
                {
                    subtotal = Convert.ToInt32(Math.Round(ivaDecimal));
                    subtotalString = subtotal.ToString();
                    App.IVA = subtotalString;
                }
                else
                {
                    throw new FormatException("El valor de IVA no tiene un formato correcto.");
                }
                if (decimal.TryParse(icaString, out decimal icaDecimal))
                {
                    IAC = Convert.ToInt32(Math.Round(icaDecimal));
                    icaString = IAC.ToString();
                    App.IVC = icaString;
                }
                else
                {
                    throw new FormatException("El valor de IVA no tiene un formato correcto.");
                }

                string responseSection = "";

                string Trama = $"01,{total},{subtotalString},{App.PuntoVenta},{App.Secuencia},0,{icaString},KIOSCO,0,0,";
                string respuesta = await Task.Run(() => App.RunProgramAndWait(Trama));

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
                    imgdatafono.Source = new BitmapImage(new Uri("C:\\FALCROSOFT\\PROCINAL\\Portal.Kiosco\\Properties\\Resources\\ErrorDatafono.png"));
                    borderInstrucciones.Visibility = Visibility.Hidden;
                    TextTitulo.Text = "¡Ups! La transación no se pudo realizar";
                    botones.Visibility = Visibility.Visible;
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
                imgdatafono.Source = new BitmapImage(new Uri("C:\\FALCROSOFT\\PROCINAL\\Portal.Kiosco\\Properties\\Resources\\ErrorDatafono.png"));
                borderInstrucciones.Visibility = Visibility.Hidden;
                TextTitulo.Text = "¡Ups! La transación no se pudo realizar";
                botones.Visibility = Visibility.Visible;
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

        private void btnReintentar_Click(object sender, RoutedEventArgs e)
        {
            timer = new System.Timers.Timer(3000);
            timer.Elapsed += Timer_Elapsed;
            timer.AutoReset = false;
            timer.Start();
        }
    }
}