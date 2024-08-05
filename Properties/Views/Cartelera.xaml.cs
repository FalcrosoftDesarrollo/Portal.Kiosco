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
                lblnombre.Content = "¡HOLA " + App.ob_diclst["Nombre"].ToString() + " " + App.ob_diclst["Apellido"].ToString() + "!";
            }
            else
            {
                lblnombre.Content = "¡HOLA INVITADO!";
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

        public async Task LoadDataAsync()
        {
            await Task.Run(() =>
            {
                System.Threading.Thread.Sleep(3000);
            });
        }

        private bool ComprobarTiempo()
        {
            bool isMainWindowOpen = false;

            if (App._tiempoRestanteGlobal == "00:00")
            {
                this.Dispatcher.Invoke(() =>
                {
                    Principal principal = Application.Current.Windows.OfType<Principal>().FirstOrDefault();

                    if (principal == null)
                    {
                        principal = new Principal();
                        principal.Show();
                        isMainWindowOpen = true;
                    }
                    else if (principal.Visibility == Visibility.Visible)
                    {
                        principal.Activate();
                        isMainWindowOpen = true;
                    }
                    else
                    {
                        principal.Show();
                        isMainWindowOpen = true;
                    }

                    this.Close();

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
                            Margin = new Thickness(30), // Margen de 30 en todos los lados
                            BorderThickness = new Thickness(0),
                            VerticalAlignment = VerticalAlignment.Center,
                            HorizontalAlignment = HorizontalAlignment.Right,
                            CornerRadius = new CornerRadius(10), // Esquinas redondeadas con un radio de 10
                            OpacityMask = new VisualBrush
                            {
                                Stretch = Stretch.Fill,
                                Visual = new Rectangle
                                { 
                                    Width = 1,
                                    Height = 1,
                                    Fill = Brushes.Black
                                }
                            }
                        };

                        var stackPanel = new StackPanel
                        {
                            VerticalAlignment = VerticalAlignment.Center,
                            HorizontalAlignment = HorizontalAlignment.Center
                        };

                        // Añadir el título de la película encima de la imagen
                        TextBlock tituloPelicula = new TextBlock
                        {
                            Text = pelicula.TituloOriginal, // Usar la propiedad Description para el título
                            Foreground = Brushes.Black,
                            Background = Brushes.White,
                            FontSize = 12,
                            FontWeight = FontWeights.Bold, // Make the text bold
                            HorizontalAlignment = HorizontalAlignment.Center,
                            TextAlignment = TextAlignment.Center,
                            Margin = new Thickness(0, 0, 0, 10) // Margen inferior para separación
                        };
                        stackPanel.Children.Add(tituloPelicula);

                      
                        var imagenBorde = new Border
                        {
                            Width = 260,
                            Height = 344,
                            CornerRadius = new CornerRadius(10), // Esquinas redondeadas para la imagen
                            ClipToBounds = true,
                            OpacityMask = new VisualBrush
                            {
                                Stretch = Stretch.Fill,
                                Visual = new Rectangle
                                {
                                    RadiusX = 0.1,
                                    RadiusY = 0.1,
                                    Width = 1,
                                    Height = 1,
                                    Fill = Brushes.Black
                                }
                            },
                            Child = new Image
                            {
                                Width = 260,
                                Height = 344,
                                Stretch = Stretch.Fill,
                                Source = new BitmapImage(new Uri(pelicula.Imagen))
                            }
                        };

                        stackPanel.Children.Add(imagenBorde);

                        if (pelicula.Tipo == "Preventa")
                        {
                            Border preventaBorder = new Border
                            {
                                Height = 56,
                                VerticalAlignment = VerticalAlignment.Top,
                                Background = Brushes.Red,
                                CornerRadius = new CornerRadius(10) // Esquinas redondeadas para el borde de preventa
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
                            stackPanel.Children.Add(preventaBorder);
                        }

                        nuevoBorde.Child = stackPanel;

                        nuevoBorde.MouseLeftButtonUp += async (sender, e) =>
                        {
                            App.Pelicula.Id = pelicula.Id.ToString();
                            App.Pelicula.TituloOriginal = pelicula.TituloOriginal.ToString();
                            App.Pelicula.Imagen = pelicula.Imagen;
                            App.Pelicula.Nombre = pelicula.Nombre;
                            App.Pelicula.Duracion = pelicula.Duracion;
                            App.Pelicula.Genero = pelicula.Genero;
                            App.Pelicula.Censura = pelicula.Censura;

                            if (this.ActualWidth > this.ActualHeight) // Pantalla en horizontal
                            {
                             var transicion = new transicion();
                                transicion.Show();
                                this.Close();
                                var openWindows = new SeleccionarFuncionH();
                                await openWindows.LoadDataAsync();
                                openWindows.Show();
                                transicion.Close();
                            }
                            else
                            {
                           
                                var transicion = new transicion();
                                transicion.Show();
                                this.Close();
                                var openWindows = new SeleccionarFuncion();
                                await openWindows.LoadDataAsync();
                                openWindows.Show();
                                transicion.Close();
                            }

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
        


            var transicion = new transicion();
            transicion.Show();
            this.Close();
            var openWindows = new Scanear_documento();
            await openWindows.LoadDataAsync();
            openWindows.Show();
            transicion.Close();

        }

        private async void btnSiguiente_Click(object sender, RoutedEventArgs e)
        {
            isThreadActive = false;

            if (this.ActualWidth > this.ActualHeight) // Pantalla en horizontal
            { 
                var transicion = new transicion();
                transicion.Show();
                this.Close();
                var openWindows = new SeleccionarFuncionH();
                await openWindows.LoadDataAsync();
                openWindows.Show();
                transicion.Close();

            }
            else  
            {
                var transicion = new transicion();
                transicion.Show();
                this.Close();
                var openWindows = new SeleccionarFuncion();
                await openWindows.LoadDataAsync();
                openWindows.Show();
                transicion.Close();
            }

            this.Close();
        }

    }
}