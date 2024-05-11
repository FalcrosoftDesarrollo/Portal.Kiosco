using APIPortalKiosco.Entities;
using APIPortalWebMed.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using System.Xml.Linq;
using static Org.BouncyCastle.Math.EC.ECCurve;

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
        public static string Secuencia { get; set; }
        public static string ValorTercero { get; set; }
        public static string ScoreServices { get; set; }
        public static string CantSillasBol { get; set; }
        public static string CantProductos { get; set; }
        public static string CodigoProducto { get; set; }
        public static decimal CantidadBoletas { get; set; }
        public static decimal ValorTarifa { get; set; }
        public static decimal KeyTarifa { get; set; }
        public static string MinDifHora { get; set; }
        public static string NombreTarifa { get; set; }
        public static string NombreFec { get; set; }
        public static string Censura { get; set; }
        public static string Imagen { get; set; }
        public static string TipoSilla { get; set; }


        public static String TipoSala { get; set; }
        public static decimal PrecioUnitario { get; set; } = 3000; // Precio unitario de las gafas
        public static decimal CantidadGafas { get; set; } // Lista para almacenar la cantidad de gafas seleccionadas
        public static string variables41 { get; set; }
        public static string _tiempoRestanteGlobal { get; set; }
        public static TimeSpan tiempoRestante { get; set; }


        private List<UIElement> elementosCombos = new List<UIElement>();
        private List<UIElement> elementosAlimentos = new List<UIElement>();
        private List<UIElement> elementosBebidas = new List<UIElement>();
        private List<UIElement> elementosSnack = new List<UIElement>();

        public static List<Producto> CombosWeb = new List<Producto>();
        public static List<Producto> AlimentosWeb = new List<Producto>();
        public static List<Producto> BebidasWeb = new List<Producto>();
        public static List<Producto> SnacksWeb = new List<Producto>();
        public static List<Producto> ProductosSeleccionados = new List<Producto>();
        public static BolVenta BolVentaRoom = new BolVenta();

        public static Dictionary<string, string> FacturaCompra = new Dictionary<string, string>();
        public static bool IsFecha = true;

        public static Dictionary<string, string> ob_diclst = new Dictionary<string, string>();
        public event PropertyChangedEventHandler PropertyChanged;
        public static bool IsPrimeraCarga = true;
    

        public static string EmailEli { get; set; }
        public static string NombreEli { get; set; }
        public static string ApellidoEli { get; set; }
        public static string TelefonoEli { get; set; }
        public static string PortalWebDB { get; set; }
        

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
            

            string appSettingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");

            if (File.Exists(appSettingsPath))
            {
                string jsonContent = File.ReadAllText(appSettingsPath);
                var appSettings = JObject.Parse(jsonContent);
                var appSettingsSection = appSettings["MyConfig"];

                MinDifHora = appSettingsSection["MinDifHora"].ToString();
                idCine = appSettingsSection["TeaDefault"].ToString();
                PuntoVenta = appSettingsSection["PuntoVenta"].ToString();
                ValorTercero = appSettingsSection["ValorTercero"].ToString();
                CantProductos = appSettingsSection["CantProductos"].ToString();
                ScoreServices = appSettingsSection["ScoreServices"].ToString();
                CantSillasBol = appSettingsSection["CantSillasBol"].ToString();
                EmailEli = appSettingsSection["Email"].ToString();
                NombreEli = appSettingsSection["Nombre"].ToString();
                ApellidoEli = appSettingsSection["Apellido"].ToString();
                PortalWebDB = appSettingsSection["PortalWebDB"].ToString();
                TelefonoEli = appSettingsSection["Telefono"].ToString();
                carteleraXML = XDocument.Load(appSettingsSection["Variables41"].ToString());
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
            tiempoRestante = TimeSpan.FromSeconds(10);
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
                }
            };
            contadorGlobalTimer.Interval = TimeSpan.FromSeconds(1);
            contadorGlobalTimer.Start();
        }

        public void ResetearTimer()
        {
            tiempoRestante = TimeSpan.FromSeconds(10);
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

        public static void RoomReverse()
        {
            #region VARIABLES LOCALES
            int lc_idearr = 0;

            string pr_fecprg = App.Pelicula.FechaSel;
            string pr_horprg = App.Pelicula.HoraSel.ToString();
            string pr_salprg = App.Pelicula.numeroSala.ToString();

            string pr_selubi = "";
            string lc_result = string.Empty;
            string lc_srvpar = string.Empty;
            string lc_auxitm = string.Empty;
            string[] ls_lstsel = new string[5];

            General ob_fncgrl = new General();
            List<string> ls_lstubi = new List<string>();
            Dictionary<string, string> ob_diclst = new Dictionary<string, string>();
            #endregion

            try
            {
                //Obtener ubicaciones de vista
                char[] ar_charst = App.BolVentaRoom.SelUbicaciones.ToCharArray();
                for (int lc_iditem = 0; lc_iditem < ar_charst.Length; lc_iditem++)
                {
                    //Concatenar caracteres
                    lc_auxitm += ar_charst[lc_iditem].ToString();

                    //Obtener parámetro
                    if (ar_charst[lc_iditem].ToString() == ";")
                    {
                        ls_lstubi.Add(lc_auxitm.Substring(0, lc_auxitm.Length - 1));
                        lc_auxitm = string.Empty;
                    }
                }

                //Cargar ubicaciones al modelo JSON
                lc_auxitm = string.Empty;
                foreach (var item in ls_lstubi)
                {
                    lc_idearr = 0;
                    char[] ar_chars2 = item.ToCharArray();
                    for (int lc_iditem = 0; lc_iditem < ar_chars2.Length; lc_iditem++)
                    {
                        //Concatenar caracteres
                        lc_auxitm += ar_chars2[lc_iditem].ToString();

                        //Obtener parámetro
                        if (ar_chars2[lc_iditem].ToString() == "_")
                        {
                            ls_lstsel[lc_idearr] = lc_auxitm.Substring(0, lc_auxitm.Length - 1);

                            lc_idearr++;
                            lc_auxitm = string.Empty;
                        }
                    }

                    #region SCOSIL
                    LiberaSilla ob_libsrv = new LiberaSilla();
                    ob_libsrv.Fila = ls_lstsel[3];
                    ob_libsrv.Sala = Convert.ToInt32(pr_salprg.Substring(0, pr_salprg.IndexOf(";")));
                    ob_libsrv.teatro = Convert.ToInt32(App.idCine);
                    ob_libsrv.Funcion = Convert.ToInt32(pr_horprg.Length == 4 ? pr_horprg.Substring(0, 2) : pr_horprg.Substring(0, 1));
                    ob_libsrv.Columna = Convert.ToInt32(ls_lstsel[4]);
                    ob_libsrv.Usuario = 777;
                    ob_libsrv.tercero = App.ValorTercero;
                    ob_libsrv.FechaFuncion = pr_fecprg;

                    //Generar y encriptar JSON para servicio
                    lc_srvpar = ob_fncgrl.JsonConverter(ob_libsrv);

                    lc_srvpar = lc_srvpar.Replace("fechaFuncion", "FechaFuncion");
                    lc_srvpar = lc_srvpar.Replace("sala", "Sala");
                    lc_srvpar = lc_srvpar.Replace("funcion", "Funcion");
                    lc_srvpar = lc_srvpar.Replace("fila", "Fila");
                    lc_srvpar = lc_srvpar.Replace("columna", "Columna");
                    lc_srvpar = lc_srvpar.Replace("usuario", "Usuario");

                    //Encriptar Json LIB
                    lc_srvpar = ob_fncgrl.EncryptStringAES(lc_srvpar);

                    //Consumir servicio LIB
                    lc_result = ob_fncgrl.WebServices(string.Concat(App.ScoreServices, "scosil/"), lc_srvpar);


                    //Validar secuencia
                    if (lc_result.Substring(0, 1) == "0")
                    {
                        //Quitar switch
                        lc_result = lc_result.Replace("0-", "");
                        lc_result = lc_result.Replace("[", "");
                        lc_result = lc_result.Replace("]", "");

                        //Deserializar Json y validar respuesta
                        ob_diclst = (Dictionary<string, string>)JsonConvert.DeserializeObject(lc_result, (typeof(Dictionary<string, string>)));


                    }

                    #endregion
                }

                //Validar acción
            }
            catch (Exception lc_syserr)
            {

            }
        }
    }
}