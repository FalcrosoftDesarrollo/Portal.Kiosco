using APIPortalKiosco.Entities;
using Newtonsoft.Json;
using Portal.Kiosco.Helpers;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Portal.Kiosco.Properties.Views
{
    /// <summary>
    /// Lógica de interacción para Frame17.xaml
    /// </summary>
    public partial class CorreoTecladoFlotante : Window
    {
        public CorreoTecladoFlotante()
        {
            InitializeComponent();
        }

        private async void btnObtenerDatos_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var _apiService = new ApiService();
                string documento = App.DatosCineFans.Documento;
                string correoelectronico = TextCorreoElectronico.Text;
                string datosCineFans = await _apiService.ObtenerDatosCineFans(documento, correoelectronico);

                if (!string.IsNullOrEmpty(datosCineFans))
                {
                    App.IsUserAuthenticated = true;

                    App.DatosCineFans = JsonConvert.DeserializeObject<CineFansData>(datosCineFans); ;

                    this.Close();
                }
                else
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

        private void KeyButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button != null)
            {
                string character = button.Content.ToString();

                if (character == "Del")
                {
                    if (TextCorreoElectronico.Text.Length > 0)
                    {
                        TextCorreoElectronico.Text = TextCorreoElectronico.Text.Substring(0, TextCorreoElectronico.Text.Length - 1);
                    }
                }
                else
                {
                    TextCorreoElectronico.Text += character;
                }
            }
        }


        private void btnVolver_Click(object sender, RoutedEventArgs e)
        {
            BoletasGafasAlimentos w = new BoletasGafasAlimentos();
            this.Close();
            w.ShowDialog();
        }
    }
}