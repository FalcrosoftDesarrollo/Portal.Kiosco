using APIPortalKiosco.Entities;
using Newtonsoft.Json;
using Portal.Kiosco.Properties.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;
using Cartelera = Portal.Kiosco.Properties.Views.Cartelera;

namespace Portal.Kiosco
{
    public partial class Scanear_documento : Window
    {
        private bool isThreadActive = true;
        private bool isError = false;

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

            await Dispatcher.InvokeAsync(() =>
            {
                lastText = TextDocumento.Text;
            });

            while (shouldMonitorTextChanged)
            {
                try
                {
                    string scannedText = string.Empty;
                    await Dispatcher.InvokeAsync(() =>
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
                            var userDocument = new UserDocumento
                            {
                                Documento = scannedText.Substring(0, indiceComa),
                                tercero = "1"
                            };

                            string lc_srvpar = JsonConvert.SerializeObject(userDocument);
                            lc_srvpar = ob_fncgrl.EncryptStringAES(lc_srvpar);

                            string lc_result = await ob_fncgrl.WebServicesAsync($"{App.ScoreServices}scoced/", lc_srvpar);

                            if (lc_result.StartsWith("0"))
                            {
                                lc_result = lc_result.Substring(2).Trim('[', ']');
                                var respuesta = JsonConvert.DeserializeObject<Dictionary<string, string>>(lc_result);
                                App.ob_diclst = respuesta;

                                App.EmailEli = respuesta["Login"];
                                App.NombreEli = respuesta["Nombre"];
                                App.ApellidoEli = respuesta["Apellido"];
                                App.NroDocumento = respuesta["Documento"];
                                App.Direccion = respuesta["Direccion"];
                                App.TelefonoEli = respuesta["Celular"];
                                App.Clave = respuesta["Clave"];

                                string usuario = $"{respuesta["Nombre"]} {respuesta["Apellido"]}";
                                if (!string.IsNullOrEmpty(usuario))
                                {
                                    getnuevapantalla();
                                }

                                lastText = scannedText;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    await Dispatcher.InvokeAsync(() => limpiar());
                    isError = true;
                    System.Windows.MessageBox.Show("Usuario no registrado en el sistema", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    break;
                }

                await Task.Delay(100); // Reemplazar Thread.Sleep con Task.Delay
            }
        }

        public void limpiar()
        {
            Dispatcher.Invoke(() => TextDocumento.Text = string.Empty);
        }

    }
}