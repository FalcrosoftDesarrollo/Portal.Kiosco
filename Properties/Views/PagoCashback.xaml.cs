using System.Windows;

namespace Portal.Kiosco.Properties.Views
{
    /// <summary>
    /// Lógica de interacción para Frame14.xaml
    /// </summary>
    public partial class PagoCashback : Window
    {
        public PagoCashback()
        {
            InitializeComponent();
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
