using System.Threading.Tasks;
using System;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Media;

namespace Portal.Kiosco.Properties.Views
{
    /// <summary>
    /// Lógica de interacción para Frame12.xaml
    /// </summary>
    public partial class Combodeluxe2 : Window
    {
        public Combodeluxe2()
        {
            InitializeComponent();
            if (App.ob_diclst.Count > 0)
            {
                lblnombre.Content = "!HOLA " + App.ob_diclst["Nombre"].ToString() + " " + App.ob_diclst["Apellido"].ToString();
            }
            else
            {
                lblnombre.Content = "!HOLA INVITADO";
            }
        }

        private async void btnSalir_Click(object sender, RoutedEventArgs e)
        {
            Principal w = new Principal();
            this.Close();
            w.ShowDialog();
        }
    }
}