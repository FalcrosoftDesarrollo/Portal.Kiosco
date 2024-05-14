using APIPortalKiosco.Entities;
using Newtonsoft.Json;
using Portal.Kiosco.Properties.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Media.Animation;
using Cartelera = Portal.Kiosco.Properties.Views.Cartelera;

namespace Portal.Kiosco
{
    /// <summary>
    /// Lógica de interacción para Scanear_documento.xaml
    /// </summary>
    public partial class Scanear_documento : Window
    {
        private bool isThreadActive = true;

        public Scanear_documento()
        {
            InitializeComponent();
            TextDocumento.Focus();
            TextDocumento.Text = "";

            var monitorThread = new Thread(MonitorTextChanged);
            monitorThread.IsBackground = true;
            monitorThread.Start();


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


        private void Scanear_documento_Loaded(object sender, RoutedEventArgs e)
        {
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void btnIngresaDocumento_Click(object sender, RoutedEventArgs e)
        {
            if (App.IsBoleteriaConfiteria == false)
            {
                isThreadActive = false;
                Cartelera openWindows = new Cartelera();
                openWindows.Show();
                this.Close();
            }
            else
            {
                isThreadActive = false;
                Combos openWindows = new Combos();
                openWindows.Show();
                this.Close();
            }
        }

        //private async void TextDocumento_PreviewKeyDown(object sender, KeyEventArgs e)
        //{
        //    try
        //    {
        //        if (e.Key == Key.Enter)
        //        {
        //            General ob_fncgrl = new General();
        //            var userDocument = new UserDocumento();
        //            userDocument.Documento = TextDocumento.Text;
        //            userDocument.tercero = "1";
        //            string lc_srvpar = string.Empty;
        //            string lc_result = string.Empty;
        //            //Generar y encriptar JSON para servicio
        //            lc_srvpar = JsonConvert.SerializeObject(userDocument);
        //            string Usuario = string.Empty;
        //            //Encriptar Json
        //            lc_srvpar = ob_fncgrl.EncryptStringAES(lc_srvpar);

        //            //Consumir servicio
        //            lc_result = ob_fncgrl.WebServices(string.Concat(App.ScoreServices, "scoced/"), lc_srvpar);
        //            if (lc_result.Substring(0, 1) == "0")
        //            {
        //                lc_result = lc_result.Replace("0-", "");
        //                lc_result = lc_result.Replace("[", "");
        //                lc_result = lc_result.Replace("]", "");

        //                App.ob_diclst = (Dictionary<string, string>)JsonConvert.DeserializeObject(lc_result.Replace("0-[", "["), (typeof(Dictionary<string, string>)));
        //            }

        //            Usuario = App.ob_diclst["Nombre"].ToString() + " " + App.ob_diclst["Apellido"].ToString();

        //            if (Usuario != "")
        //            {
        //                if (App.IsBoleteriaConfiteria == false)
        //                {
        //                    var openWindow = new Cartelera();
        //                    DoubleAnimation fadeOutAnimation = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.5));
        //                    this.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);
        //                    await Task.Delay(300);
        //                    this.Visibility = Visibility.Collapsed;
        //                    openWindow.Background = Brushes.White;
        //                    openWindow.Show();
        //                    DoubleAnimation fadeInAnimation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.5));
        //                    openWindow.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);
        //                    App.DatosCineFans.Documento = TextDocumento.Text;
        //                }
        //                else
        //                {
        //                    var openWindow = new Combos();
        //                    DoubleAnimation fadeOutAnimation = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.5));
        //                    this.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);
        //                    await Task.Delay(300);
        //                    this.Visibility = Visibility.Collapsed;
        //                    openWindow.Background = Brushes.White;
        //                    openWindow.Show();
        //                    DoubleAnimation fadeInAnimation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.5));
        //                    openWindow.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);
        //                    App.DatosCineFans.Documento = TextDocumento.Text;
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        var errorWindow = new ErrorGeneral();
        //        errorWindow.Owner = this;
        //        errorWindow.Closed += (s, args) =>
        //        {
        //            this.Visibility = Visibility.Visible;
        //        };
        //        errorWindoopenWindows.Show();
        //    }
        //}

        private async void btnVolverComoCompra_Click(object sender, RoutedEventArgs e)
        {
            isThreadActive = false;
            ComoCompra openWindows = new ComoCompra();
            openWindows.Show();
            this.Close();
        }

        private async void TextDocumento_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {


        }
        private bool shouldMonitorTextChanged = true;

      // Asegura que este método se ejecute en un subproceso STA
        private bool carteleraWindowLoaded = false;
        [STAThread]
        public async void getnuevapantalla()
        {
            await Dispatcher.InvokeAsync(() =>
            {
                if (!carteleraWindowLoaded && App.IsBoleteriaConfiteria == false)
                {
                    isThreadActive = false;
                    Cartelera openWindows = new Cartelera();
                    openWindows.Show();
                    this.Close();
                }
                else if (App.IsBoleteriaConfiteria)
                {
                    isThreadActive = false;
                    Combos openWindows = new Combos();
                    openWindows.Show();
                    this.Close();
                }
            });
        }

        private async void MonitorTextChanged()
        {
            string lastText = string.Empty;

            Dispatcher.Invoke(() =>
            {
                lastText = TextDocumento.Text;
            });

            while (shouldMonitorTextChanged)
            {
                try
                {
                    // Acceder al TextBox desde el hilo de la interfaz de usuario usando Dispatcher.Invoke
                    string scannedText = string.Empty;
                    Dispatcher.Invoke(() =>
                    {
                        scannedText = TextDocumento.Text;
                    });

                    // Verificar si el texto ha cambiado desde la última verificación
                    if (scannedText != lastText)
                    {
                        string texto = scannedText;
                        int indiceComa = texto.IndexOf(',');
                        if (indiceComa != -1)
                        {
                            General ob_fncgrl = new General();
                            var userDocument = new UserDocumento();
                            userDocument.Documento = scannedText.Substring(0, scannedText.IndexOf(','));
                            userDocument.tercero = "1";
                            string lc_srvpar = string.Empty;
                            string lc_result = string.Empty;
                            //Generar y encriptar JSON para servicio
                            lc_srvpar = JsonConvert.SerializeObject(userDocument);
                            string Usuario = string.Empty;
                            //Encriptar Json
                            lc_srvpar = ob_fncgrl.EncryptStringAES(lc_srvpar);

                            //Consumir servicio
                            lc_result = ob_fncgrl.WebServices(string.Concat(App.ScoreServices, "scoced/"), lc_srvpar);
                            if (lc_result.Substring(0, 1) == "0")
                            {
                                lc_result = lc_result.Replace("0-", "");
                                lc_result = lc_result.Replace("[", "");
                                lc_result = lc_result.Replace("]", "");

                                App.ob_diclst = (Dictionary<string, string>)JsonConvert.DeserializeObject(lc_result.Replace("0-[", "["), (typeof(Dictionary<string, string>)));
                            }

                            Usuario = App.ob_diclst["Nombre"].ToString() + " " + App.ob_diclst["Apellido"].ToString();

                            if (Usuario != "")
                            {
                                getnuevapantalla();
                            }
                            lastText = scannedText;
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                // Esperar un breve período antes de verificar nuevamente
                Thread.Sleep(100); // Puedes ajustar el intervalo según tus necesidades
            }
        }

    }
}