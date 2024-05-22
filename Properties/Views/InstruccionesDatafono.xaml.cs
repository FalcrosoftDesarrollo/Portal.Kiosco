﻿using APIPortalKiosco.Entities;
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

        public InstruccionesDatafono()
        {
            InitializeComponent();

            DataContext = ((App)Application.Current);
            if (App.ob_diclst.Count > 0)
            {
                lblnombre.Content = "!HOLA " + App.ob_diclst["Nombre"].ToString() + " " + App.ob_diclst["Apellido"].ToString() + "!";
            }
            else
            {
                lblnombre.Content = "!HOLA INVITADO¡";
            }

            DoubleAnimation fadeInAnimation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.5));
            gridPrincipal.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);

            //var consumo = new TEFII_NET.trx.TEFTransactionManager();
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
            var subtotal = Convert.ToInt64(Convert.ToInt64(App.TotalPagar) * 0.19);
            var total = Convert.ToInt64(App.TotalPagar) - subtotal;
            var responseSection = "";
            var respuesta = App.RunProgramAndWait("01," + total.ToString() + ",100,CAJA" + App.PuntoVenta + ",TRX" + App.Secuencia + "," + subtotal + ",0,KIOSCO,0,0,");
            int start = respuesta.IndexOf("Response:");

            int end = respuesta.IndexOf("*", start);

            if (start != -1 && end != -1)
            {
                // Extraer la subcadena desde "Response:" hasta "*"
                responseSection = respuesta.Substring(start, end - start);
                Console.WriteLine(responseSection);
            }
            else
            {
                MessageBox.Show("No se encontró la sección de respuesta en el texto proporcionado.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            if (responseSection.Replace("Response:","").Substring(0, 3).Replace(",", "") == "00")
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

                if (App.respuestagenerica == "Error")
                {
                    MessageBox.Show("Error al procesar el pago " + App.Secuencia + "-PUNTOVTA: " + App.PuntoVenta, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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