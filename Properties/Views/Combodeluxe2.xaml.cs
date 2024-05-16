using System.Windows;

namespace Portal.Kiosco.Properties.Views
{

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
            Principal openWindows = new Principal();
            this.Close();
            openWindows.Show();
        }
    }
}