using Portal.Kiosco.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Portal.Kiosco.Helpers;
using System;
using System.Windows;
using System.Windows.Controls;
using APIPortalWebMed.Entities;
using Newtonsoft.Json;
 
using Portal.Kiosco.Properties.Views;
using APIPortalKiosco.Entities;

namespace Portal.Kiosco
{
    /// <summary>
    /// Lógica de interacción para IngresoDeDocumento.xaml
    /// </summary>
    public partial class IngresoDeDocumento : Window
    {

        public IngresoDeDocumento()
        {
            InitializeComponent();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        private async void btnObtenerDatos_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var _apiService = new ApiService(); 
                string documento = TextDocumento.Text;
                string passwrd = TextContraseña.Text;  
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
    }
}