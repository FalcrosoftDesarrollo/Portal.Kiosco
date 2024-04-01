using APIPortalKiosco.Entities;
using Newtonsoft.Json;
using Portal.Kiosco.Helpers;
using Portal.Kiosco.Properties.Views;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Portal.Kiosco
{
    /// <summary>
    /// Lógica de interacción para IngresoDePassword.xaml
    /// </summary>
    public partial class IngresoDePassword : Window
    {
        public IngresoDePassword()
        {
            InitializeComponent();
        }

        private async void btnObtenerDatos_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var _apiService = new ApiService();
                string documento = App.DatosCineFans.Documento;
                string passwrd = TextContraseña.Password;
                string datosCineFans = await _apiService.ObtenerDatosCineFans(documento, passwrd);

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
                    if (TextContraseña.Password.Length > 0)
                    {
                        TextContraseña.Password = TextContraseña.Password.Substring(0, TextContraseña.Password.Length - 1);
                    }
                }
                else
                {
                    TextContraseña.Password += character;
                }
            }
        }

    }
}