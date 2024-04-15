using System;
using System.Windows;
using System.Windows.Controls;

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

        private void btnSumar_Click(object sender, RoutedEventArgs e)
        {
            // Obtener el botón que se hizo clic
            Button btn = sender as Button;

            if (btn != null)
            {
                // Obtener el nombre del botón
                string nombreBoton = btn.Name;

                // Determinar si es para suma o resta basado en el nombre del botón
                if (nombreBoton == "+")
                {
                    lblCantidad.Content = (Convert.ToInt32(lblCantidad.Content) + 1).ToString();
                    lblTotal.Content = "$" + (Convert.ToInt32(lblPrecio.Content.ToString().Replace("$", "")) * Convert.ToInt32(lblCantidad.Content)).ToString();
                }
                else if (nombreBoton == "-")
                {
                    lblCantidad.Content = (Convert.ToInt32(lblCantidad.Content) - 0).ToString();
                    lblTotal.Content = "$" + (Convert.ToInt32(lblPrecio.Content.ToString().Replace("$", "")) * Convert.ToInt32(lblCantidad.Content)).ToString();

                }
            }
        }

    }
}