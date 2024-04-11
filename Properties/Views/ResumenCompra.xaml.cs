using System.Windows;

namespace Portal.Kiosco.Properties.Views
{
    public partial class ResumenCompra : Window
    {
        public ResumenCompra()
        {
            InitializeComponent();
            DataContext = ((App)Application.Current);
        }

        private void btnVolver_Click(object sender, RoutedEventArgs e)
        {
            Combodeluxe1 w = new Combodeluxe1();
            this.Close();
            w.ShowDialog();
        }

        private void btnSiguiente_Click(object sender, RoutedEventArgs e)
        {
            //PagoCashback w = new PagoCashback();
            //this.Close();
            //w.ShowDialog();
        }

        private async void btnSalir_Click(object sender, RoutedEventArgs e)
        {
            await App.OpenWindow("Principal");
        }

        private async void btnPagoTarjeta_Click(object sender, RoutedEventArgs e)
        {
            await App.OpenWindow("InstruccionesDatafono");
        }

        private async void btnPagarCash_Click(object sender, RoutedEventArgs e)
        {
            await App.OpenWindow("PagoCashback");
        }
    }
}
