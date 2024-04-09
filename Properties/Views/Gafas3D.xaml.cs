using System.Windows;

namespace Portal.Kiosco.Properties.Views
{
    /// <summary>
    /// Lógica de interacción para F8.xaml
    /// </summary>
    public partial class Gafas3D : Window
    {
        public Gafas3D()
        {
            InitializeComponent();
            DataContext = ((App)Application.Current);
        }

        private void btnVolver_Click(object sender, RoutedEventArgs e)
        {
            LayoutAsientos w = new LayoutAsientos();
            this.Close();
            w.ShowDialog();
        }

        private void btnSiguiente_Click(object sender, RoutedEventArgs e)
        {
            AlgoParaComer w = new AlgoParaComer();
            this.Close();
            w.ShowDialog();
        }
    }
}