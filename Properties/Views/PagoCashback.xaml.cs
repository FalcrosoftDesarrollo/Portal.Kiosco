using System.Windows;

namespace Portal.Kiosco.Properties.Views
{
    public partial class PagoCashback : Window
    {
        public PagoCashback()
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
            DataContext = ((App)Application.Current);
        }

        private void btnSiguiente_Click(object sender, RoutedEventArgs e)
        {
            InstruccionesDatafono w = new InstruccionesDatafono();
            this.Close();
            w.ShowDialog();
        }

        private void btnVolver_Click(object sender, RoutedEventArgs e)
        {
            ResumenCompra w = new ResumenCompra();
            this.Close();
            w.ShowDialog();
        }
    }
}
