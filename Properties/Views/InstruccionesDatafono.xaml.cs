using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Media.Animation;

namespace Portal.Kiosco.Properties.Views
{
    /// <summary>
    /// Lógica de interacción para Frame15.xaml
    /// </summary>
    public partial class InstruccionesDatafono : Window
    {
        private bool isThreadActive = true;
        private System.Timers.Timer timer;

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

            Loaded += InstruccionesDatafono_Loaded; // Suscribir al evento Loaded

        }

        private void InstruccionesDatafono_Loaded(object sender, RoutedEventArgs e)
        {
            timer = new System.Timers.Timer(3000); // 10000 milisegundos = 10 segundos
            timer.Elapsed += Timer_Elapsed;
            timer.AutoReset = false; // Para que solo se dispare una vez
            timer.Start();
        }


        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // Llamar al método LLamadoDatafono después de que haya pasado el tiempo de espera
            Dispatcher.Invoke(() =>
            {
                LLamadoDatafono();
            });
        }


        public void LLamadoDatafono()
        {
            // Construir la trama de entrada para la función 01 (Compra con tarjeta)

            var subtotal = Convert.ToInt64(Convert.ToInt64(App.TotalPagar) * 0.19);
            var total = Convert.ToInt64(App.TotalPagar) - subtotal;
            var responseSection = "";
            var respuesta = App.RunProgramAndWait("01," + total.ToString() + ",100,CAJA" + App.PuntoVenta + ",TRX" + App.Secuencia + "," + subtotal + ",0,KIOSCO,0,0,");
            int start = respuesta.IndexOf("Response:");

            // Encontrar la posición de "*"
            int end = respuesta.IndexOf("*", start);

            if (start != -1 && end != -1)
            {
                // Extraer la subcadena desde "Response:" hasta "*"
                responseSection = respuesta.Substring(start, end - start);
                Console.WriteLine(responseSection);
            }
            else
            {
                MessageBox.Show("No se encontró la sección de respuesta en el texto proporcionado.");
            }
            if (responseSection.Replace("Response:","").Substring(0, 3).Replace(",", "") == "00")
            {
                App.ResponseDatafono = responseSection.Substring(0, 3).Replace(",", "");
                var openWindows = new BoletasGafasAlimentos();
                openWindows.Show();
                this.Close();
            }
            else 
            {
                this.Close();
            }
           
        }


        private bool ComprobarTiempo()
        {
            bool isMainWindowOpen = false; // Variable local para indicar si la ventana principal está abierta

            if (App._tiempoRestanteGlobal == "00:00")
            {
                this.Dispatcher.Invoke(() =>
                {
                    Principal principal = Application.Current.Windows.OfType<Principal>().FirstOrDefault();
                    if (principal != null && principal.Visibility == Visibility.Visible)
                    {
                        // Enfocar la ventana principal si está abierta y visible
                        principal.Activate();
                        isMainWindowOpen = true; // Marcar que la ventana principal está abierta
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
                            // Cerrar todas las demás ventanas excepto la ventana principal
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

            return isMainWindowOpen; // Devolver el valor booleano
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