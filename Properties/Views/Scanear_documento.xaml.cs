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
                    string scannedText = string.Empty;
                    Dispatcher.Invoke(() =>
                    {
                        scannedText = TextDocumento.Text;
                    });

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
                            lc_srvpar = JsonConvert.SerializeObject(userDocument);
                            string Usuario = string.Empty;
                            lc_srvpar = ob_fncgrl.EncryptStringAES(lc_srvpar);

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

                Thread.Sleep(100); 
            }
        }
    }
}