using System.Windows;

namespace Portal.Kiosco.Properties.Views
{
    /// <summary>
    /// Lógica de interacción para Frame11.xaml
    /// </summary>
    public partial class Combodeluxe1 : Window
    {
        public Combodeluxe1()
        {
            InitializeComponent();
        }

        private void btnSiguiente_Click(object sender, RoutedEventArgs e)
        {
            ResumenCompra w = new ResumenCompra();
            this.Close();
            w.ShowDialog();
        }

        private void btnVolver_Click(object sender, RoutedEventArgs e)
        {
            AlgoParaComer w = new AlgoParaComer();
            this.Close();
            w.ShowDialog();
        }
    }
}