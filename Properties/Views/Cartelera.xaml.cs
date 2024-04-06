using APIPortalWebMed.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace Portal.Kiosco.Properties.Views
{
    public partial class Cartelera : Window
    {

        private List<Pelicula> Peliculas = new List<Pelicula>();

        public Cartelera()
        {
            InitializeComponent();
            CargarPeliculasDesdeXml();
        }    

        public string ObtenerValorDeConfiguracion(string clave)
        { 
            string valor = ConfigurationManager.AppSettings[clave];
            return valor;
        }

        private void CargarPeliculasDesdeXml()
        {
            string teatro = ObtenerValorDeConfiguracion("TeaDefault");
            // Ruta del archivo XML
            string rutaXml = "https://scorecoorp.procinal.com/mobilecomjson//nuevaweb/variableCARTELERA.aspx?teatro=310";

            try
            { 
                XDocument xmlDocument = XDocument.Load(rutaXml);
                var peliculas = xmlDocument.Descendants("pelicula");
                int fila = 0;
                int columna = 0;
                foreach (var pelicula in peliculas)
                {
                    if (pelicula.Attribute("tipo")?.Value == "Normal")
                    {
                        Pelicula nuevaPelicula = new Pelicula
                        {
                            Id = pelicula.Attribute("id")?.Value,
                            Nombre = pelicula.Attribute("nombre")?.Value,
                            Tipo = pelicula.Attribute("tipo")?.Value,
                            Sinopsis = pelicula.Element("sinopsis")?.Value,
                            TituloOriginal = pelicula.Element("data")?.Attribute("TituloOriginal")?.Value,
                            Imagen = pelicula.Element("data")?.Attribute("Imagen")?.Value,
                            RutaCartelera = pelicula.Element("data")?.Attribute("RutaCartelera")?.Value,
                            Censura = pelicula.Element("data")?.Attribute("Censura")?.Value,
                            Idioma = pelicula.Element("data")?.Attribute("idioma")?.Value,
                            Genero = pelicula.Element("data")?.Attribute("genero")?.Value,
                            Pais = pelicula.Element("data")?.Attribute("pais")?.Value,
                            Duracion = pelicula.Element("data")?.Attribute("duracion")?.Value,
                            Medio = pelicula.Element("data")?.Attribute("medio")?.Value,
                            Formato = pelicula.Element("data")?.Attribute("formato")?.Value,
                            Version = pelicula.Element("data")?.Attribute("versión")?.Value,
                            Trailer1 = pelicula.Element("data")?.Attribute("trailer1")?.Value,
                            Trailer2 = pelicula.Element("data")?.Attribute("trailer2")?.Value,
                            //FechaEstreno = pelicula.Element("data")?.Attribute("fechaEstreno")?.Value,
                            Reparto = pelicula.Element("data")?.Attribute("reparto")?.Value,
                            Director = pelicula.Element("data")?.Attribute("director")?.Value,
                            Distribuidor = pelicula.Element("data")?.Attribute("distribuidor")?.Value,
                            Habilitado = Convert.ToBoolean(pelicula.Element("data")?.Attribute("Habilitado")?.Value),
                            //DiasDisponibles = pelicula.Element("DiasDisponiblesTodosCinemas")
                            //                ?.Elements("dia")
                            //                ?.Select(d => DateTime.ParseExact(d.Attribute("fecha")?.Value, "ddd, MMM dd yyyy", CultureInfo.InvariantCulture))
                            //                ?.ToList(),
                        };

                        Border nuevoBorde = new Border
                        {
                            Name = "P" + nuevaPelicula.Id.ToString(), // Asignar el Id como el Name del Border
                            Margin = new Thickness(55),
                            BorderThickness = new Thickness(0),
                            VerticalAlignment = VerticalAlignment.Center,
                            HorizontalAlignment = HorizontalAlignment.Right,
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
                            }
                        };

                        Image nuevaImagen = new Image
                        {
                            Source = new BitmapImage(new Uri(nuevaPelicula.Imagen)),
                            Stretch = Stretch.Fill,
                        };

                        Button imagenButton = new Button
                        {
                            Content = nuevaImagen, // Colocar la imagen dentro del contenido del botón
                            Background = Brushes.Transparent, // Fondo transparente para que solo se vea la imagen
                            BorderThickness = new Thickness(0), // Eliminar el borde del botón
                            Padding = new Thickness(0) // Eliminar el relleno del botón
                        };

                        // Agregar evento Click al botón
                        imagenButton.Click += async (sender, e) =>
                        {
                            // Llenar App.Pelicula.Id con el Id de la película al hacer clic
                            App.Pelicula.Id = nuevaPelicula.Id.ToString();

                            // Navegar a la siguiente pantalla (SeleccionFuncion)
                            var openWindow = new SeleccionarFuncion();
                            DoubleAnimation fadeOutAnimation = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.5));
                            this.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);
                            await Task.Delay(300);
                            this.Visibility = Visibility.Collapsed;
                            openWindow.Background = Brushes.White;
                            openWindow.Show();
                            DoubleAnimation fadeInAnimation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.5));
                            openWindow.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);
                        };

                        // Agregar el botón al contenedor de películas
                        nuevoBorde.Child = imagenButton;

                        nuevoBorde.Width = 360;
                        nuevoBorde.Height = 500;

                        Grid.SetRow(nuevoBorde, fila);
                        Grid.SetColumn(nuevoBorde, columna);
                        ContenedorPeliculas.Children.Add(nuevoBorde);


                        columna++;
                        if (columna >= 4) // cambiar 4 por el número de columnas deseado
                        {
                            columna = 0;
                            fila++;
                        }

                        Peliculas.Add(nuevaPelicula);
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
            var openWindow = new ComoCompra();
            DoubleAnimation fadeOutAnimation = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.5));
            this.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);
            await Task.Delay(300);
            this.Visibility = Visibility.Collapsed;
            openWindow.Background = Brushes.White;
            openWindow.Show();
            DoubleAnimation fadeInAnimation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.5));
            openWindow.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);
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

        private void btnVolver_Click(object sender, RoutedEventArgs e)
        {
            Scanear_documento w = new Scanear_documento();
            this.Close();
            w.ShowDialog();
        }

        private void btnSiguiente_Click(object sender, RoutedEventArgs e)
        {
            SeleccionarFuncion w = new SeleccionarFuncion();
            this.Close();
            w.ShowDialog();
        }
    }
}