using APIPortalKiosco.Data;
using APIPortalKiosco.Entities;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Portal.Kiosco.Properties.Views
{
    /// <summary>
    /// Lógica de interacción para F8.xaml
    /// </summary>
    public partial class Gafas3D : Window
    {
        private const int PrecioUnitario = 3000;
        private int[] cantidadGafas = new int[10];
        private readonly IOptions<MyConfig> config;
        private int indiceArray = 0;
        private bool isThreadActive = true;

        public Gafas3D()
        {
            InitializeComponent();
            DataContext = ((App)Application.Current);
            DoubleAnimation fadeInAnimation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.5));
            gridPrincipal.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);
            lblPrecio.Content = "$ 3.000";
            Thread thread = new Thread(() =>
            {
                while (isThreadActive)
                {
                    ComprobarTiempo();
                }
            });
            thread.IsBackground = true;
            thread.Start();
        }
        private bool ComprobarTiempo()
        {
            bool isMainWindowOpen = false; // Variable local para indicar si la ventana principal está abierta

            if (App._tiempoRestanteGlobal == "00:00")
            {
                this.Dispatcher.Invoke(() =>
                {
                    Principal principal = Application.Current.Windows.OfType<Principal>().FirstOrDefault();
                    if (principal != null && principal.Visibility == Visibility.Visible)
                    {
                        // Enfocar la ventana principal si está abierta y visible
                        principal.Activate();
                        isMainWindowOpen = true; // Marcar que la ventana principal está abierta
                    }
                    else
                    {
                        if (!isMainWindowOpen)
                        {
                            if (principal == null)
                            {
                                principal = new Principal();
                                principal.Show();
                                isMainWindowOpen = true;
                            }
                            // Cerrar todas las demás ventanas excepto la ventana principal
                            foreach (Window window in Application.Current.Windows)
                            {
                                if (window != principal && window != this)
                                {
                                    window.Close();
                                }
                            }
                        }
                    }

                });
            }

            return isMainWindowOpen; // Devolver el valor booleano
        }

        private async void btnVolver_Click(object sender, RoutedEventArgs e)
        {
            App.RoomReverse();
            LayoutAsientos openWindows = new LayoutAsientos(config);
            this.Close();
            openWindows.Show();

        }

        private async void btnSiguiente_Click(object sender, RoutedEventArgs e)
        {
            isThreadActive = false;
            AlgoParaComer openWindows = new AlgoParaComer();
            this.Close();
            openWindows.Show();
        }

        private int gafasSeleccionadas = 0;
        private string[] gafasSeleccionadasArray = new string[10]; 
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

                    gafasSeleccionadasArray[index] = nombreBoton;

                    gafasSeleccionadas++;

                    ultimoIndex = index;

                    lblCantidad.Content = gafasSeleccionadas;

                    if (gafasSeleccionadas == 10)
                    {
                        MessageBox.Show("Solo se pueden seleccionar hasta 10 GAFAS.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }

                }
                else if (nombreBoton == "-" && gafasSeleccionadas > 0 && ultimoIndex >= 0)
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

        private void btnSalir_Click(object sender, RoutedEventArgs e)
        {
            App.RoomReverse();
            var openWindow = new Principal();

            openWindow.Show();
            this.Close();
        }

        private  void btnOmitirGafas_Click(object sender, RoutedEventArgs e)
        {
            App.CantidadGafas = 0;
            var openWindows = new ResumenCompra(config);
            openWindows.Show();
            this.Close();
        }
    }
}