using System.Collections.Generic;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Effects;
using System.Windows.Media;
using System.Xml.Linq;

namespace Portal.Kiosco.Properties.Views
{
    /// <summary>
    /// Lógica de interacción para Frame6.xaml
    /// </summary>
    public partial class SeleccionarFuncion : Window
    {
        public SeleccionarFuncion()
        {
            InitializeComponent();
            CargarFechasDesdeXml();
            CargarHorariosDesdeXml();
        }

        private void btnVolver_Click(object sender, RoutedEventArgs e)
        {
            Cartelera w = new Cartelera();
            this.Close();
            w.ShowDialog();
        }

        private void btnSiguiente_Click(object sender, RoutedEventArgs e)
        {
            LayoutAsientos w = new LayoutAsientos();
            this.Close();
            w.ShowDialog();
        }

        private void CargarFechasDesdeXml()
        {           
            XDocument doc = XDocument.Load("https://scorecoorp.procinal.com/mobilecomjson//nuevaweb/variable41CARTELERA_TEA_PEL310_1944.xml");
            bool esPrimerBoton = true;

            foreach (XElement pelicula in doc.Descendants("pelicula"))
            {
                foreach (XElement cinema in pelicula.Descendants("cinema"))
                {
                    foreach (XElement dia in cinema.Element("DiasDisponiblesCinema").Elements("dia"))
                    {
                        string fechaCompleta = dia.Attribute("fecha")?.Value;

                        // Obtener el día de la semana y el día del mes correctamente
                        string[] partesFecha = fechaCompleta.Split(',');
                        string[] partesDia = partesFecha[1].Trim().Split(' ');
                        string diaSemana = partesFecha[0]; // Obtener las tres primeras letras para el día de la semana
                        string diaMes = partesDia[1]; // Obtener el día del mes

                        string fechaFormateada = $"{diaSemana} {diaMes}";

                        // Agregar la fecha única al conjunto
                        HashSet<string> fechas = new HashSet<string>();
                        fechas.Add(fechaFormateada);
                        Button btn = new Button();
                        Border border = new Border();
                        btn.MouseEnter += (sender, e) =>
                        {
                            btn.Foreground = Brushes.Red; // Cambiar el color al hacer hover
                            
                        };

                        // Suscribirse al evento MouseLeave para volver al color original al salir del hover
                        btn.MouseLeave += (sender, e) =>
                        {
                            btn.Foreground = new SolidColorBrush(ColorConverter.ConvertFromString("#F30613") as Color? ?? Colors.Red);
                        };
                        foreach (string fecha in fechas)
                        {
                            

                            // Crear un nuevo botón
                           
                            btn.Content = fecha;
                            btn.Background = Brushes.Transparent;
                            btn.BorderThickness = new Thickness(0);
                            btn.Foreground = Brushes.White;
                            btn.FontFamily = new FontFamily("Myanmar Khyay");
                            btn.FontSize = 20;
                            btn.Style = FindResource("MyButton") as Style; // Puedes cambiar "MyButton" por el nombre correcto del estilo

                            // Agregar efecto de sombra al botón
                            DropShadowEffect shadowEffect = new DropShadowEffect();
                            shadowEffect.Color = Colors.Gray;
                            shadowEffect.Direction = 270;
                            shadowEffect.ShadowDepth = 3;
                            shadowEffect.Opacity = 0.5;
                            btn.Effect = shadowEffect;

                            // Crear un contenedor (Border) para el botón
                            
                            border.Width = 115;
                            border.Height = 60;
                            border.Background = Brushes.White;
                            border.BorderBrush = Brushes.Red; // Cambiar por el color de borde deseado
                            border.BorderThickness = new Thickness(1);
                            border.CornerRadius = new CornerRadius(10);
                            border.Margin = new Thickness(0, 0, 6, 0); // Ajustar márgenes según necesidad
                            border.Child = btn;

                            if (esPrimerBoton)
                            {
                                btn.Foreground = Brushes.White;
                                border.Background = new SolidColorBrush(ColorConverter.ConvertFromString("#F30613") as Color? ?? Colors.Red);

                                esPrimerBoton = false;
                            }
                            else
                            {
                                btn.Foreground = new SolidColorBrush(ColorConverter.ConvertFromString("#F30613") as Color? ?? Colors.Red);
                                border.Background = Brushes.White;
                                border.BorderBrush = new SolidColorBrush(ColorConverter.ConvertFromString("#F30613") as Color? ?? Colors.Red);
                                border.BorderThickness = new Thickness(1);
                            }

                            // Agregar el contenedor al WrapPanel
                            ContenedorFechas.Children.Add(border); // Suponiendo que el WrapPanel se llama "ContenedorPeliculas"

                            
                        }

                        
                    }
                }
            }
        }

        private void CargarHorariosDesdeXml()
        {
            XDocument doc = XDocument.Load("https://scorecoorp.procinal.com/mobilecomjson//nuevaweb/variable41CARTELERA_TEA_PEL_FEC310_6318.xml");
         
            foreach (var pelicula in doc.Descendants("Peliculas"))
            {
                
                string ultimaActualizacion = pelicula.Attribute("UltimaActualizacion")?.Value;
                string[] partes = ultimaActualizacion.Split(' ');

                // Asignar la fecha a una variable
                string fecha = partes[0];

                // Separar la hora y los minutos, ignorando los segundos
                string[] tiempoPartes = partes[1].Split(':');
                string hora = tiempoPartes[0];
                string minuto = tiempoPartes[1];
                string amPm = partes[2]; // AM o PM

                string horaFormateada = $"{hora}:{minuto} {amPm}";

                // Agregar la fecha única al conjunto
                HashSet<string> horas = new HashSet<string>();
                horas.Add(horaFormateada);

                foreach(string horasBtn in horas)
                {

                    Button btn = new Button();
                    btn.Content = horasBtn;
                    btn.Width = 115;
                    btn.Height = 46;
                    btn.FontFamily = new FontFamily("Myanmar Khyay");
                    btn.FontSize = 14;
                    btn.Style = FindResource("MyButton") as Style; // Puedes cambiar "MyButton" por el nombre correcto del estilo
                    btn.Foreground = new SolidColorBrush(ColorConverter.ConvertFromString("#F30613") as Color? ?? Colors.Red);

                    Border border = new Border();
                    border.Width = 115;
                    border.Height = 46;
                    border.Background = Brushes.White;
                    border.CornerRadius = new CornerRadius(5);
                    border.Margin = new Thickness(0, 0, 6, 7);
                    border.BorderThickness = new Thickness(1);
                    border.BorderBrush = Brushes.Black;
                    border.Child = btn;

                    // Agregamos el botón con borde al WrapPanel
                    ButtonWrapPanel.Children.Add(border);
                }

               
            }
        }
           
        }
    }

