using APIPortalKiosco.Entities;
using Microsoft.Extensions.Options;
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
        private const int PrecioUnitario = 3000; // Precio unitario de las gafas
        private int[] cantidadGafas = new int[10]; // Array de tamaño fijo para almacenar la cantidad de gafas seleccionadas
        private readonly IOptions<MyConfig> config;
        private int indiceArray = 0; // Índice para controlar la posición en el array

        public Gafas3D()
        {
            InitializeComponent();
            DataContext = ((App)Application.Current);
            DoubleAnimation fadeInAnimation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.5));
            gridPrincipal.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);

        }

        private async void btnVolver_Click(object sender, RoutedEventArgs e)
        {
            var openWindow = new LayoutAsientos(config);
            openWindow.Show();
            this.Close();
          
        }

        private async void btnSiguiente_Click(object sender, RoutedEventArgs e)
        {
            var openWindow = new AlgoParaComer();
            openWindow.Background = Brushes.White;
            openWindow.Show();
            this.Close();
          
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