using APIPortalKiosco.Entities;
using Newtonsoft.Json;
using Portal.Kiosco.Properties.Views;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Cartelera = Portal.Kiosco.Properties.Views.Cartelera;

namespace Portal.Kiosco
{
    /// <summary>
    /// Lógica de interacción para Scanear_documento.xaml
    /// </summary>
    public partial class Scanear_documento : Window
    {
        public Scanear_documento()
        {
            InitializeComponent();
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
            var openWindow = new Cartelera();
            DoubleAnimation fadeOutAnimation = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.5));
            this.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);
            await Task.Delay(300);
            this.Visibility = Visibility.Collapsed;
            openWindow.Background = Brushes.White;
            openWindow.Show();
            DoubleAnimation fadeInAnimation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.5));
            openWindow.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);
        }

        private async void TextDocumento_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {
                    General ob_fncgrl = new General();
                    var userDocument = new UserDocumento();
                    userDocument.Documento = TextDocumento.Text;
                    Dictionary<string, string> ob_diclst = new Dictionary<string, string>();
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

                        ob_diclst = (Dictionary<string, string>)JsonConvert.DeserializeObject(lc_result.Replace("0-[", "["), (typeof(Dictionary<string, string>)));
                    }

                    Usuario = ob_diclst["Login"].ToString();
                    if (Usuario != "")
                    {
                        if (App.IsBoleteriaConfiteria == false)
                        {
                            var openWindow = new Cartelera();
                            DoubleAnimation fadeOutAnimation = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.5));
                            this.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);
                            await Task.Delay(300);
                            this.Visibility = Visibility.Collapsed;
                            openWindow.Background = Brushes.White;
                            openWindow.Show();
                            DoubleAnimation fadeInAnimation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.5));
                            openWindow.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);
                            App.DatosCineFans.Documento = TextDocumento.Text;
                        }
                        else
                        {
                            var openWindow = new Combos();
                            DoubleAnimation fadeOutAnimation = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.5));
                            this.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);
                            await Task.Delay(300);
                            this.Visibility = Visibility.Collapsed;
                            openWindow.Background = Brushes.White;
                            openWindow.Show();
                            DoubleAnimation fadeInAnimation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.5));
                            openWindow.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);
                            App.DatosCineFans.Documento = TextDocumento.Text;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var errorWindow = new ErrorGeneral();
                errorWindow.Owner = this;
                errorWindow.Closed += (s, args) =>
                {
                    this.Visibility = Visibility.Visible;
                };
                errorWindow.ShowDialog();
            }
        }

        private async void btnVolverComoCompra_Click(object sender, RoutedEventArgs e)
        {
            var openWindow = new ComoCompra();
            DoubleAnimation fadeOutAnimation = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.5));
            this.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);
            await Task.Delay(300);
            this.Visibility = Visibility.Collapsed;
            openWindow.Background = Brushes.White;
            openWindow.Show();
            DoubleAnimation fadeInAnimation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.5));
            openWindow.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);
        }




    }
}