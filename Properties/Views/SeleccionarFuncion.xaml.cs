using System.Collections.Generic;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Effects;
using System.Windows.Media;
using System.Xml.Linq;
using APIPortalKiosco.Entities;
using System.Windows.Media.Imaging;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using System.Linq;
using System.Globalization;
using System.Windows.Markup;
using System.Windows.Automation.Peers;
using APIPortalWebMed.Entities;
using Microsoft.EntityFrameworkCore.Internal;

namespace Portal.Kiosco.Properties.Views
{
    /// <summary>
    /// Lógica de interacción para Frame6.xaml
    /// </summary>
    public partial class SeleccionarFuncion : Window
    {
        //private Dictionary<string, List<string>> horasPorFecha = new Dictionary<string, List<string>>();
        public List<UIElement> elementosConservados3D;
        public List<UIElement> elementosConservadosGeneral;
        public SeleccionarFuncion()
        {
            InitializeComponent();
            DataContext = ((App)Application.Current);
            CargarFechasDesdeXml();
            imgPelicula.Source = new BitmapImage(new Uri(App.Pelicula.Imagen));
            lblClasificacion.Content = "Clasificación: " + App.Pelicula.Censura;
            lblNombre.Content = App.Pelicula.Nombre;
            lblDuracion.Content = "Duración: " + App.Pelicula.Duracion + " min";
            lblGenero.Content = "Genero: " + App.Pelicula.Genero;

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
            HashSet<string> fechasProcesadas = new HashSet<string>();
            HashSet<string> horasProcesadas = new HashSet<string>();

            foreach (var pelicula in App.Peliculas)
            {
                if (pelicula.TituloOriginal == App.Pelicula.TituloOriginal)
                {
                    foreach (var fechas in pelicula.DiasDisponibles)
                    {
                        if (!fechasProcesadas.Contains(fechas.fecunv))
                        {
                            CrearBotonFecha($"{fechas.fecham}", "btn" + fechas.fecunv);
                            fechasProcesadas.Add(fechas.fecunv);
                            App.IsFecha = true;
                        }

                        foreach (var hora in fechas.horafun.OrderBy(h => DateTime.ParseExact(h.horario, "h:mm tt", CultureInfo.InvariantCulture)))
                        {
                            if (hora.idFuncion.Contains(fechas.fecunv))
                            {
                                if (!horasProcesadas.Contains(hora.horario))
                                {
                                    horasProcesadas.Add(hora.horario);

                                    if (pelicula.Formato.Contains("2D"))
                                    {
                                        ContenedorHorasGeneral.Children.Add(CrearBotonHora(hora.horario, hora.idFuncion));
                                    }
                                    if (pelicula.Formato.Contains("3D"))
                                    {
                                        ContenedorHoras3D.Children.Add(CrearBotonHora(hora.horario, hora.idFuncion));
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }


        private void CargarFechasDesdeSelect(string fecha)
        {
            HashSet<string> fechasProcesadas = new HashSet<string>();
            HashSet<string> horasProcesadas = new HashSet<string>();

            foreach (var pelicula in App.Peliculas)
            {
                if (pelicula.TituloOriginal == App.Pelicula.TituloOriginal)
                {
                    foreach (var fechas in pelicula.DiasDisponibles.Where(x => x.fecunv == fecha))
                    {
                        if (!fechasProcesadas.Contains(fechas.fecunv))
                        {
                            //CrearBotonFecha($"{fechas.fecham}", "btn" + fechas.fecunv);
                            //fechasProcesadas.Add(fechas.fecunv);
                            App.IsFecha = true;
                        }

                        foreach (var hora in fechas.horafun.OrderBy(h => DateTime.ParseExact(h.horario, "h:mm tt", CultureInfo.InvariantCulture)))
                        {
                            if (hora.idFuncion.Contains(fechas.fecunv))
                            {
                                if (!horasProcesadas.Contains(hora.horario))
                                {
                                    horasProcesadas.Add(hora.horario);

                                    if (hora.tipSala.Contains("GENERAL"))
                                    {
                                        ContenedorHorasGeneral.Children.Add(CrearBotonHora(hora.horario, hora.idFuncion));
                                    }
                                    if (hora.tipSala.Contains("3D"))
                                    {
                                        ContenedorHoras3D.Children.Add(CrearBotonHora(hora.horario, hora.idFuncion));
                                    }
                                    if (hora.tipSala.Contains("SUPERNOVA"))
                                    {
                                        ContenedorHorasSupernova.Children.Add(CrearBotonHora(hora.horario, hora.idFuncion));
                                    }
                                    if (hora.tipSala.Contains("4DX"))
                                    {
                                        ContenedorHoras4DX.Children.Add(CrearBotonHora(hora.horario, hora.idFuncion));
                                    }
                                    if (hora.tipSala.Contains("BLACK STAR"))
                                    {
                                        ContenedorHorasBlackStar.Children.Add(CrearBotonHora(hora.horario, hora.idFuncion));
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }



        private void CrearBotonFecha(string fecha, string fecunv)
        {
            string[] partesFecha = fecha.Split(' ');
            string diaSemana = partesFecha[0];
            string diaMes = partesFecha[2];
            string fechaFormateada = $"{diaSemana} {diaMes}";

            Button btn = new Button();
            Border border = new Border();
            // Crear un nuevo botón
            btn.Name = fecunv;

            btn.Content = fechaFormateada;
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

            if (App.IsFecha == true)
            {
                btn.Foreground = new SolidColorBrush(ColorConverter.ConvertFromString("#F30613") as Color? ?? Colors.Red);
                border.Background = Brushes.White;
                border.BorderBrush = new SolidColorBrush(ColorConverter.ConvertFromString("#F30613") as Color? ?? Colors.Red);
                border.BorderThickness = new Thickness(1);
                App.Pelicula.FechaSel = btn.Name.Substring(3);
            }
            else
            {
                btn.Foreground = Brushes.White;
                border.Background = new SolidColorBrush(ColorConverter.ConvertFromString("#F30613") as Color? ?? Colors.Red);
            }

            btn.Click += (sender, e) =>
            {
                if (sender is Button clickedButton)
                {
                    string buttonName = clickedButton.Name;
                    App.Pelicula.FechaSel = buttonName;
                }


                ContenedorHoras3D.Children.Clear();
                ContenedorHorasGeneral.Children.Clear();

                // Crear y agregar los botones al contenedor de horas


                ContenedorHoras3D.Children.Add(CrearBotonFormato("3D"));
                ContenedorHorasGeneral.Children.Add(CrearBotonFormato("General"));


                // Iterar sobre los elementos hijos del contenedor de horas
                foreach (var child in ContenedorFechas.Children.OfType<Border>().Select(b => b.Child as Button))
                {
                    if (child != null)
                    {
                        // Aplicar estilo al botón
                        child.Width = 115;
                        child.Height = 60;
                        child.Background = Brushes.White;
                        child.BorderBrush = Brushes.Red; // Cambiar por el color de borde deseado
                        //child.BorderThickness = new Thickness(1);
                        child.Margin = new Thickness(0, 0, 0, 0); // Ajustar márgenes según necesidad
                        child.BorderThickness = new Thickness(1);

                        // Verificar si el botón es el que recibió el clic
                        if (child == sender)
                        {
                            child.Foreground = Brushes.White;
                            child.Background = new SolidColorBrush(ColorConverter.ConvertFromString("#F30613") as Color? ?? Colors.Red);
                        }
                        else
                        {
                            child.Foreground = Brushes.Black; // Establece el color de texto deseado
                            child.Background = Brushes.White; // Restaura el color de fondo por defecto
                        }
                    }
                }

                // Llamar a la función para cargar las fechas
                CargarFechasDesdeSelect(fecunv.Substring(3));
            };


            ContenedorFechas.Children.Add(border);
        }

        public Border CrearBotonFormato(string ContenidoFormato)
        {
            // Crear el nuevo botón
            Button nuevoBoton = new Button();
            nuevoBoton.Content = ContenidoFormato; // Texto deseado con salto de línea
            nuevoBoton.Background = Brushes.Transparent;
            nuevoBoton.BorderThickness = new Thickness(0);
            nuevoBoton.Foreground = Brushes.Black;
            nuevoBoton.FontFamily = new FontFamily("Myanmar Khyay");
            nuevoBoton.FontSize = 14;

            // Aplicar el mismo estilo que el botón estático
            nuevoBoton.Style = FindResource("MyButton") as Style;

            nuevoBoton.HorizontalContentAlignment = HorizontalAlignment.Center;

            // Crear el contenedor (Border) para el nuevo botón
            Border nuevoBorder = new Border();
            nuevoBorder.CornerRadius = new CornerRadius(10);
            nuevoBorder.Background = Brushes.White; // Color de fondo deseado
            nuevoBorder.Margin = new Thickness(0, 7, 6, 5); // Ajusta los márgenes según sea necesario
            nuevoBorder.Width = 115;
            nuevoBorder.Height = 46;
            nuevoBorder.Child = nuevoBoton;

            // Retornar el nuevo botón y su contenedor
            return nuevoBorder;
        }

        private Border CrearBotonHora(string hora, string idFuncion)
        {

            Button btn = new Button();
            btn.Name = "btn" + idFuncion;
            btn.Content = hora;
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


            // Agregar efecto de sombra al botón
            DropShadowEffect shadowEffect = new DropShadowEffect();
            shadowEffect.Color = Colors.Gray;
            shadowEffect.Direction = 270;
            shadowEffect.ShadowDepth = 3;
            shadowEffect.Opacity = 0.5;
            btn.Effect = shadowEffect;

            // Suscribirse al evento Click
            btn.Click += async (sender, e) =>
            {
                var wrapPanelName = "";
                var formato = "";


                if (sender is Button clickedButton)
                {
                    string buttonName = clickedButton.Name;
                    App.Pelicula.HoraSel = buttonName.Substring(3);
                }


                var TituloOriginal = App.Pelicula.TituloOriginal;
                var DiasDisponibles = App.Pelicula.DiasDisponibles;
                var FechaSel = App.Pelicula.FechaSel;
                var HoraSel = App.Pelicula.HoraSel;

                var peliculas = App.Peliculas.Where(pelicula =>
                                     pelicula.TituloOriginal == App.Pelicula.TituloOriginal &&
                                     pelicula.DiasDisponibles.Any(diasDisponible =>
                                         diasDisponible.horafun.Any(hora =>
                                             hora.idFuncion == idFuncion
                                         )
                                     )
                                 );

                var numSalas = App.Peliculas
                    .Where(pelicula =>
                        pelicula.TituloOriginal == App.Pelicula.TituloOriginal &&
                        pelicula.DiasDisponibles.Any(diasDisponible =>
                            diasDisponible.horafun.Any(hora =>
                                hora.idFuncion == idFuncion
                            )
                        )
                    )
                    .SelectMany(pelicula =>
                        pelicula.DiasDisponibles.SelectMany(diasDisponible =>
                            diasDisponible.horafun
                                .Where(hora => hora.idFuncion == idFuncion)
                                .Select(hora => hora.numSala)
                        )
                    )
                    .Distinct()
                    .ToList();

                App.Pelicula.numeroSala = numSalas.FirstOrDefault();

                var peliculaSeleccionada = peliculas.FirstOrDefault();
                if (peliculaSeleccionada != null)
                {
                    //App.Pelicula.numeroSala = peliculaSeleccionada.DiasDisponibles.Where(x => x.fecunv == FechaSel);
                    App.Pelicula.Id = peliculaSeleccionada.Id;
                }


                var openWindow = new LayoutAsientos();
                DoubleAnimation fadeOutAnimation = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.5));
                this.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);
                await Task.Delay(300);
                this.Visibility = Visibility.Collapsed;
                openWindow.Background = Brushes.White;
                openWindow.Show();
                DoubleAnimation fadeInAnimation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.5));
                openWindow.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);
            };

            return border;

        }

        private async void btnSalir_Click(object sender, RoutedEventArgs e)
        {
            App.IsBoleteriaConfiteria = false;
            var comoComprarWindow = new Principal();
            DoubleAnimation fadeOutAnimation = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.5));
            this.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);
            await Task.Delay(300);
            this.Visibility = Visibility.Collapsed;
            comoComprarWindow.Background = Brushes.White;
            comoComprarWindow.Show();
            DoubleAnimation fadeInAnimation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.5));
            comoComprarWindow.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);
        }
    }
}

