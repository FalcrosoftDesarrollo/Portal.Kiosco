using APIPortalWebMed.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Portal.Kiosco.Properties.Views
{
    public partial class Cartelera : Window
    {
        private List<Pelicula> Peliculas = new List<Pelicula>();
        private bool isThreadActive = true;

        public Cartelera()
        {
            InitializeComponent();
            CargarPeliculasDesdeXml();
            DataContext = ((App)Application.Current);
            App.IsFecha = false;
            if (App.ob_diclst.Count > 0)
            {
                lblnombre.Content = "!HOLA " + App.ob_diclst["Nombre"].ToString() + " " + App.ob_diclst["Apellido"].ToString();
            }
            else
            {
                lblnombre.Content = "!HOLA INVITADO";
            }
            DoubleAnimation fadeInAnimation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(3));
            gridPrincipal.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);
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





        public string ObtenerValorDeConfiguracion(string clave)
        {
            string valor = ConfigurationManager.AppSettings[clave];
            return valor;
        }

        private void CargarPeliculasDesdeXml()
        {
            try
            {
                int fila = 0;
                int columna = 0;
                Dictionary<string, bool> titulosProcesados = new Dictionary<string, bool>();

                foreach (var pelicula in App.Peliculas.OrderByDescending(p => p.Tipo))
                {
                    // Verificar si el título original ya ha sido procesado
                    if (!titulosProcesados.ContainsKey(pelicula.TituloOriginal))
                    {
                        titulosProcesados[pelicula.TituloOriginal] = true;

                        // Crear el contenedor para la película
                        var nuevoBorde = new System.Windows.Controls.Border
                        {
                            Name = "P" + pelicula.Id.ToString(),
                            Margin = new Thickness(40),
                            BorderThickness = new Thickness(0),
                            VerticalAlignment = VerticalAlignment.Center,
                            HorizontalAlignment = HorizontalAlignment.Right,
                            OpacityMask = new VisualBrush
                            {
                                Stretch = Stretch.Fill,
                                Visual = new Rectangle
                                {
                                    RadiusX = 0,
                                    RadiusY = 0,
                                    Width = 1,
                                    Height = 1,
                                    Fill = Brushes.Black
                                }
                            }
                        };

                        // Crear un grid para contener la imagen y el borde superior rojo
                        var grid = new System.Windows.Controls.Grid();

                        // Agregar la imagen de la película al grid
                        System.Windows.Controls.Image nuevaImagen = new System.Windows.Controls.Image
                        {
                            Width = 360,
                            Height = 444, // Altura ajustada para dar espacio al borde superior rojo
                            Stretch = Stretch.Fill,
                            Source = new BitmapImage(new Uri(pelicula.Imagen))
                        };
                        grid.Children.Add(nuevaImagen);

                        // Verificar si la película es de tipo "Preventa"
                        if (pelicula.Tipo == "Preventa")
                        {
                            // Crear el borde superior rojo con el texto "PREVENTA"
                            System.Windows.Controls.Border preventaBorder = new System.Windows.Controls.Border
                            {
                                Height = 56,
                                VerticalAlignment = VerticalAlignment.Top,
                                Background = Brushes.Red
                            };

                            Label preventaLabel = new Label
                            {
                                Foreground = Brushes.White,
                                FontFamily = new FontFamily("Myanmar Khyay"),
                                FontSize = 32,
                                Content = "PREVENTA",
                                HorizontalContentAlignment = HorizontalAlignment.Center,
                                VerticalContentAlignment = VerticalAlignment.Center
                            };

                            preventaBorder.Child = preventaLabel;
                            grid.Children.Add(preventaBorder);
                        }

                        // Agregar el grid como contenido del borde
                        nuevoBorde.Child = grid;

                        // Configurar el evento Click para abrir la pantalla SeleccionarFuncion
                        nuevoBorde.MouseLeftButtonUp += async (sender, e) =>
                        {
                            // Llenar App.Pelicula.Id con el Id de la película al hacer clic
                            App.Pelicula.Id = pelicula.Id.ToString();
                            App.Pelicula.TituloOriginal = pelicula.TituloOriginal.ToString();
                            App.Pelicula.Imagen = pelicula.Imagen;
                            App.Pelicula.Nombre = pelicula.Nombre;
                            App.Pelicula.Duracion = pelicula.Duracion;
                            App.Pelicula.Genero = pelicula.Genero;
                            App.Pelicula.Censura = pelicula.Censura;


                            // Navegar a la siguiente pantalla (SeleccionFuncion)
                            var openWindow = new SeleccionarFuncion();

                            openWindow.Show();
                            this.Close();

                        };

                        // Configurar la posición en la cuadrícula
                        System.Windows.Controls.Grid.SetRow(nuevoBorde, fila);
                        System.Windows.Controls.Grid.SetColumn(nuevoBorde, columna);

                        // Agregar el contenedor a la cuadrícula
                        ContenedorPeliculas.Children.Add(nuevoBorde);

                        // Incrementar las columnas y filas
                        columna++;
                        if (columna >= 4) // cambiar 4 por el número de columnas deseado
                        {
                            columna = 0;
                            fila++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al cargar películas desde el archivo XML: " + ex.Message);
            }
        }

        private async void btnSalir_Click(object sender, RoutedEventArgs e)
        {
            isThreadActive = false;
            Principal openWindows = new Principal();
            openWindows.Show();
            this.Close();

        }

        private bool isDragging = false;

        private void ScrollViewer_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            isDragging = true;
        }

        private void ScrollViewer_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            isDragging = false;
        }

        private void ScrollViewer_PreviewTouchDown(object sender, TouchEventArgs e)
        {
            isDragging = true;
        }

        private void ScrollViewer_PreviewTouchUp(object sender, TouchEventArgs e)
        {
            isDragging = false;
        }

        private void ScrollViewer_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                System.Windows.Controls.ScrollViewer scrollViewer = sender as System.Windows.Controls.ScrollViewer;
                Point currentPoint = e.GetPosition(scrollViewer);

                if (currentPoint.Y < 0)
                    scrollViewer.LineUp();
                else if (currentPoint.Y > scrollViewer.ActualHeight)
                    scrollViewer.LineDown();
            }
        }

        private async void btnVolver_Click(object sender, RoutedEventArgs e)
        {
            isThreadActive = false;
            Scanear_documento openWindows = new Scanear_documento();
            openWindows.Show();
            this.Close();
        }

        private async void btnSiguiente_Click(object sender, RoutedEventArgs e)
        {
            isThreadActive = false;
            SeleccionarFuncion openWindows = new SeleccionarFuncion();
            openWindows.Show();
            this.Close();
        }
    }
}