using APIPortalWebMed.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Linq;
using static Org.BouncyCastle.Bcpg.Attr.ImageAttrib;

namespace Portal.Kiosco.Properties.Views
{
    /// <summary>
    /// Lógica de interacción para Cartelera.xaml
    /// </summary>
    public partial class Cartelera : Window
    {

        private List<Pelicula> Peliculas = new List<Pelicula>();

        public Cartelera()
        {
            InitializeComponent();
            CargarPeliculasDesdeXml();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        public string ObtenerValorDeConfiguracion(string clave)
        {
            // Leer el valor de configuración usando la clave proporcionada
            string valor = ConfigurationManager.AppSettings[clave];

            // Retornar el valor encontrado
            return valor;
        }

        private void CargarPeliculasDesdeXml()
        {
            string teatro = ObtenerValorDeConfiguracion("TeaDefault");
            // Ruta del archivo XML
            string rutaXml = "https://scorecoorp.procinal.com/mobilecomjson//nuevaweb/variable41CARTELERA310_7163.xml";

            try
            {
                // Cargar el XML desde el archivo
                XDocument xmlDocument = XDocument.Load(rutaXml);

                // Obtener los elementos de película
                var peliculas = xmlDocument.Descendants("pelicula");

                int fila = 0;
                int columna = 0;

                // Iterar sobre cada elemento de película y crear objetos Pelicula
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


                        // Crear un nuevo borde para la imagen de la película
                        Border nuevoBorde = new Border
                        {
                            CornerRadius = new CornerRadius(20),
                            Margin = new Thickness(30),
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

                        // Crear una nueva imagen para la película
                        Image nuevaImagen = new Image
                        {
                            Source = new BitmapImage(new Uri(nuevaPelicula.Imagen)),
                            Stretch = Stretch.Fill,
                        };

                        // Agregar la imagen al borde
                        nuevoBorde.Child = nuevaImagen;

                        nuevoBorde.Width = 380;
                        nuevoBorde.Height = 380;
                       
                        // Agregar el borde al contenedor de películas (Grid)
                        Grid.SetRow(nuevoBorde, fila);
                        Grid.SetColumn(nuevoBorde, columna);
                        ContenedorPeliculas.Children.Add(nuevoBorde);

                        // Incrementar la columna y fila
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


        private void btnSalir_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnSoyCineFans_Click(object sender, RoutedEventArgs e)
        {
            if (App.IsUserAuthenticated)
            {
                var datosMembresiaWindow = new Datos_Membresía_CineFans();
                datosMembresiaWindow.Owner = this;
                datosMembresiaWindow.Closed += (s, args) =>
                {
                    this.Visibility = Visibility.Visible;
                };
                datosMembresiaWindow.ShowDialog();
            }
            else
            {
                var scanerWindow = new IngresoDeDocumento();
                scanerWindow.Owner = this;
                scanerWindow.Closed += (s, args) =>
                {
                    this.Visibility = Visibility.Visible;
                };
                scanerWindow.ShowDialog();
            }
        }

    }
}
