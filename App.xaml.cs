using APIPortalKiosco.Entities;
using APIPortalWebMed.Entities;
using Newtonsoft.Json.Linq;
using Portal.Kiosco.Properties.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using System.Xml.Linq;

namespace Portal.Kiosco
{
    public partial class App : Application, INotifyPropertyChanged
    {
        public static bool IsUserAuthenticated { get; set; }
        public static CineFansData DatosCineFans { get; set; }
        public static bool IsBoleteriaConfiteria { get; set; } = false;
        public static bool IsCinefans { get; set; } = false;
        public static List<Pelicula> Peliculas = new List<Pelicula>();
        public static Pelicula Pelicula = new Pelicula();
        public static XDocument carteleraXML { get; set; }
        public static string idCine { get; set; }
        public static string PuntoVenta { get; set; }
        public static string ValorTercero { get; set; }
        public static string ScoreServices { get; set; }
        public static string CantProductos { get; set; }
        public static string CodigoProducto { get; set; }
        public static decimal CantidadBoletas { get; set; }
        public static decimal ValorTarifa { get; set; }
        public static String TipoSala { get; set; }
        public static decimal PrecioUnitario { get; set; } = 3000; // Precio unitario de las gafas
        public static decimal CantidadGafas { get; set; } // Lista para almacenar la cantidad de gafas seleccionadas

        private   List<UIElement> elementosCombos = new List<UIElement>();
        private   List<UIElement> elementosAlimentos = new List<UIElement>();
        private   List<UIElement> elementosBebidas = new List<UIElement>();
        private   List<UIElement> elementosSnack = new List<UIElement>();

        public static List<Producto> CombosWeb = new List<Producto>();
        public static List<Producto> AlimentosWeb = new List<Producto>();
        public static List<Producto> BebidasWeb = new List<Producto>();
        public static List<Producto> SnacksWeb = new List<Producto>();
        public static List<Producto> ProductosSeleccionados = new List<Producto>();
        public static Dictionary<string, string> FacturaCompra = new Dictionary<string, string>();
        public static bool IsFecha = true;

        public static Dictionary<string, string> ob_diclst = new Dictionary<string, string>();
        public event PropertyChangedEventHandler PropertyChanged;
        public static bool IsPrimeraCarga = true;
        private string _tiempoRestanteGlobal;
        private TimeSpan tiempoRestante;

        

        public string TiempoRestanteGlobal
        {
            get { return _tiempoRestanteGlobal; }
            set
            {
                _tiempoRestanteGlobal = value;
                OnPropertyChanged(nameof(TiempoRestanteGlobal));
            }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            IniciarContadorGlobal();
            DatosCineFans = new CineFansData();
            Peliculas = new List<Pelicula>();
            Pelicula = new Pelicula();
            carteleraXML = XDocument.Load("https://scorecoorp.procinal.com/MobileComJson/variable41.xml");

            string appSettingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");

            if (File.Exists(appSettingsPath))
            {
                string jsonContent = File.ReadAllText(appSettingsPath);
                var appSettings = JObject.Parse(jsonContent);
                var appSettingsSection = appSettings["MyConfig"];

                idCine = appSettingsSection["TeaDefault"].ToString();
                PuntoVenta = appSettingsSection["PuntoVenta"].ToString();
                ValorTercero = appSettingsSection["ValorTercero"].ToString();
                CantProductos = appSettingsSection["CantProductos"].ToString();
                ScoreServices = appSettingsSection["ScoreServices"].ToString();
                Peliculas = ObtenerPeliculas(carteleraXML, idCine);
            }
        }
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public event EventHandler<string> TiempoRestanteActualizado;

        private void IniciarContadorGlobal()
        {
            tiempoRestante = TimeSpan.FromMinutes(15);
            TiempoRestanteGlobal = tiempoRestante.ToString(@"mm\:ss");

            DispatcherTimer contadorGlobalTimer = new DispatcherTimer();
            contadorGlobalTimer.Tick += (sender, e) =>
            {
                if (tiempoRestante > TimeSpan.Zero)
                {
                    tiempoRestante = tiempoRestante.Subtract(TimeSpan.FromSeconds(1));
                    TiempoRestanteGlobal = tiempoRestante.ToString(@"mm\:ss");
                }
                else
                {
                    ResetearTimer();

                    // Aquí puedes añadir el código para cambiar de ventana si es necesario
                    var openWindow = new Principal();
                    DoubleAnimation fadeOutAnimation = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.5));
                    Application.Current.MainWindow.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);
                    Application.Current.MainWindow.Visibility = Visibility.Collapsed;
                    openWindow.Background = Brushes.White;
                    openWindow.Show();
                    DoubleAnimation fadeInAnimation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.5));
                    openWindow.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);
                }
            };
            contadorGlobalTimer.Interval = TimeSpan.FromSeconds(1);
            contadorGlobalTimer.Start();
        }

        public void ResetearTimer()
        {
            tiempoRestante = TimeSpan.FromMinutes(15);
            TiempoRestanteGlobal = tiempoRestante.ToString(@"mm\:ss");
        }
        private string DiaMes(string pr_daynum, string pr_flag)
        {
            #region VARIABLES LOCALES
            string lc_daystr = string.Empty;
            #endregion

            if (pr_flag == "D")
            {
                //Selección de día.
                switch (pr_daynum)
                {
                    case "Sunday":
                        lc_daystr = "DOM";
                        break;
                    case "Monday":
                        lc_daystr = "LUN";
                        break;
                    case "Tuesday":
                        lc_daystr = "MAR";
                        break;
                    case "Wednesday":
                        lc_daystr = "MIE";
                        break;
                    case "Thursday":
                        lc_daystr = "JUE";
                        break;
                    case "Friday":
                        lc_daystr = "VIE";
                        break;
                    case "Saturday":
                        lc_daystr = "SAB";
                        break;
                }
            }
            else
            {
                //Selección de día.
                switch (pr_daynum)
                {
                    case "01":
                        lc_daystr = "ENERO";
                        break;
                    case "02":
                        lc_daystr = "FEBRERO";
                        break;
                    case "03":
                        lc_daystr = "MARZO";
                        break;
                    case "04":
                        lc_daystr = "ABRIL";
                        break;
                    case "05":
                        lc_daystr = "MAYO";
                        break;
                    case "06":
                        lc_daystr = "JUNIO";
                        break;
                    case "07":
                        lc_daystr = "JULIO";
                        break;
                    case "08":
                        lc_daystr = "AGOSTO";
                        break;
                    case "09":
                        lc_daystr = "SEPTIEMBRE";
                        break;
                    case "10":
                        lc_daystr = "OCTUBRE";
                        break;
                    case "11":
                        lc_daystr = "NOVIEMBRE";
                        break;
                    case "12":
                        lc_daystr = "DICIEMBRE";
                        break;
                }
            }

            //Devovler Valores
            return lc_daystr;
        }

        public List<Pelicula> ObtenerPeliculas(XDocument carteleraXML, string keyteatro)
        {
            var peliculas = new List<Pelicula>();

            var peliculaNodes = carteleraXML.Descendants("pelicula");

            foreach (var peliculaNode in peliculaNodes)
            {
                var cinemasNodes = peliculaNode.Elements("cinemas");
                foreach (var cinemasNode in cinemasNodes)
                {
                    var cinemaNodes = cinemasNode.Elements("cinema");
                    foreach (var cinemaNode in cinemaNodes)
                    {
                        string cinemaId = (string)cinemaNode.Attribute("id");

                        if (cinemaId == keyteatro)
                        {
                            var pelicula = new Pelicula
                            {
                                Id = (string)peliculaNode.Attribute("id"),
                                Nombre = (string)peliculaNode.Attribute("nombre"),
                                Tipo = (string)peliculaNode.Attribute("tipo"),
                                Sinopsis = (string)peliculaNode.Attribute("sinopsis"),
                                // Acceso a TituloOriginal como atributo del elemento data
                                TituloOriginal = (string)peliculaNode.Element("data").Attribute("TituloOriginal"),
                                Imagen = (string)peliculaNode.Element("data").Attribute("Imagen"),
                                RutaCartelera = (string)peliculaNode.Element("data").Attribute("RutaCartelera"),
                                Censura = (string)peliculaNode.Element("data").Attribute("Censura"),
                                Idioma = (string)peliculaNode.Element("data").Attribute("idioma"),
                                Genero = (string)peliculaNode.Element("data").Attribute("genero"),
                                Pais = (string)peliculaNode.Element("data").Attribute("pais"),
                                Duracion = (string)peliculaNode.Element("data").Attribute("duracion"),
                                Medio = (string)peliculaNode.Element("data").Attribute("medio"),
                                Formato = (string)peliculaNode.Element("data").Attribute("formato"),
                                Version = (string)peliculaNode.Element("data").Attribute("version"),
                                Trailer1 = (string)peliculaNode.Element("data").Attribute("trailer1"),
                                Trailer2 = (string)peliculaNode.Element("data").Attribute("trailer2"),
                                FechaEstreno = (string)peliculaNode.Element("data").Attribute("fechaEstreno"),
                                Reparto = (string)peliculaNode.Element("data").Attribute("reparto"),
                                Director = (string)peliculaNode.Element("data").Attribute("director"),
                                Distribuidor = (string)peliculaNode.Element("data").Attribute("distribuidor"),
                                Habilitado = (bool)peliculaNode.Element("data").Attribute("Habilitado"),
                                DiasDisponibles = new List<Fechas>(),
                                CinesDisponibles = new List<string>()
                            };


                            var diasDisponiblesNodes = cinemaNode.Elements("DiasDisponiblesCinema").Elements("dia");
                            foreach (var diasDisponiblesNode in diasDisponiblesNodes)
                            {
                                var fechas = new Fechas
                                {
                                    fecham = (string)diasDisponiblesNode.Attribute("fecha"),
                                    fecunv = (string)diasDisponiblesNode.Attribute("univ"),

                                    horafun = new List<Hora>()
                                };

                                var salasNodes = cinemaNode.Descendants("salas");
                                foreach (var salasNode in salasNodes)
                                {
                                    var salaNodes = salasNode.Descendants("sala");
                                    foreach (var salaNode in salaNodes)
                                    {


                                        var fechaNodes = salaNode.Elements("Fecha");
                                        foreach (var fechaNode in fechaNodes)
                                        {


                                            var horaNodes = fechaNode.Elements("hora");
                                            foreach (var horaNode in horaNodes)
                                            {
                                                var hora = new Hora
                                                {
                                                    idFuncion = (string)horaNode.Attribute("idFuncion"),
                                                    horario = (string)horaNode.Attribute("horario"),
                                                    horunv = (string)horaNode.Attribute("militar"),
                                                    fecvin = (string)diasDisponiblesNode.Attribute("univ"),
                                                    numSala = (string)salaNode.Attribute("numeroSala"),
                                                    tipSala = (string)salaNode.Attribute("tipoSala")

                                                };
                                                fechas.horafun.Add(hora);
                                            }
                                        }
                                    }
                                }


                                pelicula.DiasDisponibles.Add(fechas);
                            }

                            pelicula.CinesDisponibles.Add(cinemaId);

                            peliculas.Add(pelicula);
                        }
                    }
                }
            }

            return peliculas;
        }

        

    }
}

