using System.Windows;

namespace Portal.Kiosco.Properties.Views
{
    /// <summary>
    /// Lógica de interacción para Frame15.xaml
    /// </summary>
    public partial class InstruccionesDatafono : Window
    {
        public InstruccionesDatafono()
        {
            InitializeComponent();
        }

        private void btnSiguiente_Click(object sender, RoutedEventArgs e)
        {
            BoletasGafasAlimentos w = new BoletasGafasAlimentos();
            this.Close();
            w.ShowDialog();
        }

        private void btnVolver_Click(object sender, RoutedEventArgs e)
        {
            PagoCashback w = new PagoCashback();
            this.Close();
            w.ShowDialog();
        }
    }
}