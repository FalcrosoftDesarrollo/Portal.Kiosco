using System.Windows;

namespace Portal.Kiosco.Properties.Views
{
    /// <summary>
    /// Lógica de interacción para Frame13.xaml
    /// </summary>
    public partial class ResumenCompra : Window
    {
        public ResumenCompra()
        {
            InitializeComponent();
        }

        private void btnVolver_Click(object sender, RoutedEventArgs e)
        {
            Combodeluxe1 w = new Combodeluxe1();
            this.Close();
            w.ShowDialog();
        }

        private void btnSiguiente_Click(object sender, RoutedEventArgs e)
        {
            PagoCashback w = new PagoCashback();
            this.Close();
            w.ShowDialog();
        }
    }
}
