using APIPortalKiosco.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

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
            DoubleAnimation fadeInAnimation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.5));
            gridPrincipal.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);

        }

        private async void btnVolver_Click(object sender, RoutedEventArgs e)
        {
            var openWindow = new LayoutAsientos();
            DoubleAnimation fadeOutAnimation = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.5));
            this.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);
            await Task.Delay(300);
            this.Visibility = Visibility.Collapsed;
            openWindow.Background = Brushes.White;
            openWindow.Show();
            this.Close();
            DoubleAnimation fadeInAnimation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.5));
            openWindow.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);
        }

        private async void btnSiguiente_Click(object sender, RoutedEventArgs e)
        {
            var openWindow = new AlgoParaComer();
            DoubleAnimation fadeOutAnimation = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.5));
            this.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);
            await Task.Delay(300);
            this.Visibility = Visibility.Collapsed;
            openWindow.Background = Brushes.White;
            openWindow.Show();
            this.Close();
            DoubleAnimation fadeInAnimation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.5));
            openWindow.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);
        }

        private int gafasSeleccionadas = 0;
        private string[] gafasSeleccionadasArray = new string[10]; // Arreglo para almacenar las sillas seleccionadas
        private int ultimoIndex = -1;
        private void btnSumar_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;

            if (btn != null)
            {
                string nombreBoton = btn.Content.ToString();

                if (nombreBoton == "+" && gafasSeleccionadas < 10)
                {
                  
                    int index = Array.IndexOf(gafasSeleccionadasArray, null);

                    // Almacena el contenido del botón en el arreglo de sillas seleccionadas
                    gafasSeleccionadasArray[index] = nombreBoton;

                    // Incrementa el contador de sillas seleccionadas
                    gafasSeleccionadas++;

                    ultimoIndex = index;

                    lblCantidad.Content = gafasSeleccionadas;

                    if(gafasSeleccionadas == 10)
                    {
                        MessageBox.Show("Solo se pueden seleccionar hasta 10 GAFAS.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                   
                }
                else if(nombreBoton == "-" && gafasSeleccionadas > 0 && ultimoIndex >= 0)
                {
                    gafasSeleccionadasArray[ultimoIndex] = null;

                    // Decrementa el contador de gafas seleccionadas
                    gafasSeleccionadas--;

                    // Busca el nuevo índice de la última gafa seleccionada
                    ultimoIndex = Array.LastIndexOf(gafasSeleccionadasArray, "+");

                    lblCantidad.Content = gafasSeleccionadas;
                }


            }

            if (gafasSeleccionadas == 0)
            {
                lblTotal.Content = "$0";
            }
            else
            {
                lblTotal.Content = "$" + (gafasSeleccionadas * App.PrecioUnitario).ToString();
                App.CantidadGafas = Convert.ToDecimal(gafasSeleccionadas);
            }
        }

    }
}