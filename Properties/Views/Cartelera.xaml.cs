using APIPortalWebMed.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
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
            bool isMainWindowOpen = false;

            if (App._tiempoRestanteGlobal == "00:00")
            {
                this.Dispatcher.Invoke(() =>
                {
                    Principal principal = Application.Current.Windows.OfType<Principal>().FirstOrDefault();
                    if (principal != null && principal.Visibility == Visibility.Visible)
                    {
                        principal.Activate();
                        isMainWindowOpen = true;
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

            return isMainWindowOpen;
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
                    if (!titulosProcesados.ContainsKey(pelicula.TituloOriginal))
                    {
                        titulosProcesados[pelicula.TituloOriginal] = true;

                        var nuevoBorde = new Border
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

                        var grid = new Grid();

                        Image nuevaImagen = new Image
                        {
                            Width = 360,
                            Height = 444,
                            Stretch = Stretch.Fill,
                            Source = new BitmapImage(new Uri(pelicula.Imagen))
                        };
                        grid.Children.Add(nuevaImagen);

                        if (pelicula.Tipo == "Preventa")
                        {
                            Border preventaBorder = new Border
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

                        nuevoBorde.Child = grid;

                        nuevoBorde.MouseLeftButtonUp += async (sender, e) =>
                        {
                            App.Pelicula.Id = pelicula.Id.ToString();
                            App.Pelicula.TituloOriginal = pelicula.TituloOriginal.ToString();
                            App.Pelicula.Imagen = pelicula.Imagen;
                            App.Pelicula.Nombre = pelicula.Nombre;
                            App.Pelicula.Duracion = pelicula.Duracion;
                            App.Pelicula.Genero = pelicula.Genero;
                            App.Pelicula.Censura = pelicula.Censura;

                            var openWindow = new SeleccionarFuncion();
                            openWindow.Show();
                            this.Close();
                        };

                        Grid.SetRow(nuevoBorde, fila);
                        Grid.SetColumn(nuevoBorde, columna);

                        ContenedorPeliculas.Children.Add(nuevoBorde);

                        columna++;

                        if (columna >= 4)
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
                ScrollViewer scrollViewer = sender as ScrollViewer;
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