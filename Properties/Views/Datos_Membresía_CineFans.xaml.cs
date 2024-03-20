using APIPortalWebMed.Entities;
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

namespace Portal.Kiosco
{
    /// <summary>
    /// Lógica de interacción para Datos_Membresía_CineFans.xaml
    /// </summary>
    public partial class Datos_Membresía_CineFans : Window
    {
        public Datos_Membresía_CineFans()
        {
            InitializeComponent();
            var cineFansData = App.DatosCineFans;
            lblNombre.Content = cineFansData.Nombre;
            lblNivel.Content = cineFansData.Nivel;
            lblPuntosSiguenteLvl.Content = $"Pronto alcanzarás tu siguiente nivel, {cineFansData.Nivel + 1}";

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnSalir_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
