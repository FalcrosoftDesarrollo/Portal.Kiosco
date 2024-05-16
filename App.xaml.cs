﻿using APIPortalKiosco.Data;
using APIPortalKiosco.Entities;
using APIPortalWebMed.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Windows;
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
        public static decimal PrecioUnitario { get; set; } = 3000;
        public static decimal CantidadGafas { get; set; } 
        public static string variables41 { get; set; }
        public static string _tiempoRestanteGlobal { get; set; }
        public static TimeSpan tiempoRestante { get; set; }
        private static IOptions<MyConfig> config { get; set; }
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
        public static CinefansINI Cashback = new CinefansINI();
        public static Dictionary<string, string> FacturaCompra = new Dictionary<string, string>();
        public static bool IsFecha = true;
        public static Dictionary<string, string> ob_diclst = new Dictionary<string, string>();
        public event PropertyChangedEventHandler PropertyChanged;
        public static bool IsPrimeraCarga = true;
        public static string NomEmpresa { get; set; }
        public static string DirEmpresa { get; set; }
        public static string CiuEmpresa { get; set; }
        public static string TelEmpresa { get; set; }
        public static string CorEmpresa { get; set; }
        public static string TeaDefault { get; set; }
        public static string NomDefault { get; set; }
        public static string CiuDefault { get; set; }
        public static string EmailEli { get; set; }
        public static string NombreEli { get; set; }
        public static string ApellidoEli { get; set; }
        public static string TelefonoEli { get; set; }
        public static string PortalWebDB { get; set; }
        public static string Credicor { get; set; }
        public static string TotalPagar { get; set; }
        public static string ResponseDatafono { get; set; }
        public static string UrlCorreo { get; set; }
        public static string ClienteFrecuente { get; set; }
        public static string Saldo { get; set; }
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


            //string appSettingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            var appSettingsSection = configuration.GetSection("MyConfig");

            NomEmpresa = appSettingsSection["NomEmpresa"].ToString();
            DirEmpresa = appSettingsSection["DirEmpresa"].ToString();
            CiuEmpresa = appSettingsSection["CiuEmpresa"].ToString();
            TelEmpresa = appSettingsSection["TelEmpresa"].ToString();
            CorEmpresa = appSettingsSection["CorEmpresa"].ToString();
            TeaDefault = appSettingsSection["TeaDefault"].ToString();
            NomDefault = appSettingsSection["NomDefault"].ToString();
            CiuDefault = appSettingsSection["CiuDefault"].ToString();


            Credicor = appSettingsSection["Credicor"].ToString();
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
            UrlCorreo = appSettingsSection["UrlCorreo"].ToString();
            carteleraXML = XDocument.Load(appSettingsSection["Variables41"].ToString());

            Peliculas = ObtenerPeliculas(carteleraXML, idCine);

        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event EventHandler<string> TiempoRestanteActualizado;

        private void IniciarContadorGlobal()
        {
            tiempoRestante = TimeSpan.FromSeconds(900);
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
            tiempoRestante = TimeSpan.FromSeconds(900);
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

        public static string RunProgramAndWait(string arguments)
        {
            try
            {
                using (Process process = new Process())
                {
                    process.StartInfo.FileName = App.Credicor;
                    process.StartInfo.Arguments = arguments;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.RedirectStandardInput = true; 
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.CreateNoWindow = true;
                    process.StartInfo.Verb = "runas"; 
        
                    process.Start();

                    string output = process.StandardOutput.ReadToEnd();

                    process.StandardInput.Close();

                    process.WaitForExit();

                    process.Close();

                    return output;
                }

            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }

        public static void Payment(Producto pr_datpro)
        {
            #region VARIABLES LOCALES
            int lc_swtcat = 0;
            int lc_idearr = 0;
            int lc_cntubi = 0;
            int lc_keytar = 0;
            int lc_barclf = 0;
            double lc_boltot = 0;
            string lc_despel = string.Empty;
            string lc_auxitm = string.Empty;
            string lc_ubiprg = string.Empty;
            string lc_result = string.Empty;
            string lc_srvpar = string.Empty;
            string lc_tipven = string.Empty;
            string lc_doceli = string.Empty;
            string lc_direli = string.Empty;
            string lc_teleli = string.Empty;
            string lc_ubilbl = "Ubicaciones: ";
            string lc_fecven = DateTime.Now.ToString("dd/MM/yyyy");

            string SwtVenta = string.Empty;
            string EmailEli = string.Empty;
            string NombreEli = string.Empty;
            string KeyTeatro = App.idCine;
            string DesTeatro = string.Empty;
            string TipoCompra = string.Empty;
            string ApellidoEli = string.Empty;
            string TelefonoEli = string.Empty;
            string DireccionEli = string.Empty;
            string DocumentoEli = string.Empty;
            string KeySecuencia = string.Empty;
            string[] ls_lstsel = new string[5];

            string lc_secsec = App.Secuencia;
            decimal lc_canrot = 0;
            decimal lc_prorot = 0;
            decimal lc_valpro = 0;

            decimal Base = 0;
            decimal Total = 0;
            decimal Impuesto_1 = 0;
            decimal Impuesto_2 = 0;
            decimal CashBack_Acumulado = 0;
            string session = "";
            bool alertS;
            string clientFrecnt = "";

            List<string> ls_lstubi = new List<string>();
            List<Producto> ob_retpro = new List<Producto>();
            List<Productos> ob_proven = new List<Productos>();
            List<OrderItem> ob_ordite = new List<OrderItem>();
            List<Ubicaciones> ob_ubiprg = new List<Ubicaciones>();
            Dictionary<string, object> ob_lstpro = new Dictionary<string, object>();
            Dictionary<string, string> ob_diclst = new Dictionary<string, string>();

            InternetSales ob_intvta = new InternetSales();
            Secuencia ob_scopre = new Secuencia();
            General ob_fncgrl = new General();
            #endregion

            try
            {
                //URLPortal(config);
                //ListCarrito();

                //Inicializar valores
                alertS = false;
                var PuntoVenta = App.PuntoVenta;

                EmailEli = session/*.GetString("Usuario")*/;
                NombreEli = session/*.GetString("Nombre")*/;
                KeyTeatro = App.idCine;
                TipoCompra = pr_datpro.TipoCompra;
                ApellidoEli = session/*.GetString("Apellido")*/;
                TelefonoEli = session/*.GetString("Telefono")*/;
                DireccionEli = session/*.GetString("Direccion")*/;
                DocumentoEli = session/*.GetString("Documento")*/;
                KeySecuencia = pr_datpro.KeySecuencia;

                if (session/*.GetString("NroTarjeta") */!= null)
                    lc_barclf = Convert.ToInt32(session/*.GetString("NroTarjeta")*/);
                else
                    lc_barclf = 0;

                lc_tipven = TipoCompra;
                lc_secsec = KeySecuencia;

                //Tipo de venta
                switch (lc_tipven)
                {
                    case "B": //VENTA BOLETAS
                        //Obtener boletas carrito de compra
                        using (var context = new DataDB(config))
                        {

                            var ReportSales = context.ReportSales.Where(x => x.Secuencia == lc_secsec.ToString()).Where(x => x.KeyPunto == PuntoVenta).Where(x => x.KeyTeatro == KeyTeatro).ToList();
                            foreach (var vr_itevta in ReportSales)
                            {
                                ob_intvta.Sala = Convert.ToInt32(vr_itevta.KeySala);
                                ob_intvta.Funcion = Convert.ToInt32(vr_itevta.HorProg.Substring(0, 2));
                                ob_intvta.Pelicula = Convert.ToInt32(vr_itevta.KeyPelicula);
                                ob_intvta.FechaFun = string.Concat(vr_itevta.FecProg.Substring(0, 4), "-", vr_itevta.FecProg.Substring(4, 2), "-", vr_itevta.FecProg.Substring(6, 2));
                                ob_intvta.InicioFun = Convert.ToInt32(vr_itevta.HorProg);

                                lc_boltot = vr_itevta.Precio;
                                lc_ubiprg = vr_itevta.SelUbicaciones;
                                lc_keytar = Convert.ToInt32(vr_itevta.KeyTarifa);
                            }

                            //Obtener ubicaciones de vista
                            char[] ar_charst = lc_ubiprg.ToCharArray();
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

                                lc_cntubi++;
                                lc_ubilbl += string.Concat("Fila: ", ls_lstsel[1], " Columna: ", ls_lstsel[2], ";");
                                ob_ubiprg.Add(new Ubicaciones() { Fila = ls_lstsel[3], Columna = Convert.ToInt32(ls_lstsel[4]), Tarifa = lc_keytar, FilRelativa = ls_lstsel[1], ColRelativa = Convert.ToInt32(ls_lstsel[2]), TipoSilla = "", TipoZona = "", EstadoSilla = "" });
                            }

                            //Adicionar a lista
                            ob_ordite.Add(new OrderItem
                            {
                                Precio = Convert.ToDecimal(lc_boltot),
                                Cantidad = lc_cntubi,
                                Descripcion = string.Concat(lc_despel, "-", lc_ubilbl),
                                KeyProducto = ob_intvta.Pelicula
                            });

                            //Asignar valores
                            ob_intvta.Productos = ob_proven;
                            ob_intvta.Ubicaciones = ob_ubiprg;
                            ob_intvta.Accion = pr_datpro.SwtVenta;
                            ob_intvta.TotalVenta = lc_boltot;

                            //Validar pago cashback
                            if (pr_datpro.SwitchCashback == "S")
                            {
                                ob_intvta.CodMedioPago = Convert.ToInt32(config.Value.CodMedioPagoCB);
                                ob_intvta.PagoInterno = Convert.ToDouble(pr_datpro.Valor);
                                ob_intvta.PagoCredito = 0;
                            }
                            else
                            {
                                ob_intvta.CodMedioPago = Convert.ToInt32(config.Value.CodMedioPago);
                                ob_intvta.PagoInterno = 0;
                                ob_intvta.PagoCredito = Convert.ToDouble(pr_datpro.Valor);
                            }
                        }

                        break;

                    case "P": //VENTA RETAIL
                        #region SERVICIO SCOPRE
                        //Asignar valores PRE
                        ob_scopre.Punto = Convert.ToInt32(App.PuntoVenta);
                        ob_scopre.Teatro = Convert.ToInt32(KeyTeatro);
                        ob_scopre.Tercero = App.ValorTercero;

                        //Generar y encriptar JSON para servicio PRE
                        lc_srvpar = ob_fncgrl.JsonConverter(ob_scopre);
                        lc_srvpar = lc_srvpar.Replace("Teatro", "teatro");
                        lc_srvpar = lc_srvpar.Replace("Tercero", "tercero");
                        lc_srvpar = lc_srvpar.Replace("punto", "Punto");

                        //Encriptar Json PRE
                        lc_srvpar = ob_fncgrl.EncryptStringAES(lc_srvpar);

                        //Consumir servicio PRE
                        if (clientFrecnt == "No")
                            lc_result = ob_fncgrl.WebServices(string.Concat(App.ScoreServices, "scopre/"), lc_srvpar);
                        else
                            lc_result = ob_fncgrl.WebServices(string.Concat(App.ScoreServices, "scopcf/"), lc_srvpar);

                        //Validar respuesta
                        if (lc_result.Substring(0, 1) == "0")
                        {
                            //Quitar switch
                            lc_result = lc_result.Replace("0-", "");
                            ob_lstpro = (Dictionary<string, object>)JsonConvert.DeserializeObject(lc_result, (typeof(Dictionary<string, object>)));
                            ob_retpro = (List<Producto>)JsonConvert.DeserializeObject(ob_lstpro["Lista_Productos"].ToString(), (typeof(List<Producto>)));

                            if (ob_lstpro.ContainsKey("Validación"))
                                MessageBox.Show(ob_lstpro["Validación"].ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        else
                        {
                            lc_result = lc_result.Replace("1-", "");
                            MessageBox.Show(lc_result, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        #endregion

                        //Obtener productos carrito de compra
                        using (var context = new DataDB(config))
                        {
                            //Select * From RetailSales Where Secuencia == ob_datpro.KeySecuencia

                            decimal IdTeatro = Convert.ToDecimal(KeyTeatro);
                            var RetailSales = context.RetailSales.Where(x => x.Secuencia == Convert.ToDecimal(lc_secsec)).Where(x => x.PuntoVenta == Convert.ToDecimal(PuntoVenta)).Where(x => x.KeyTeatro == IdTeatro).ToList();
                            foreach (var vr_itevta in RetailSales)
                            {
                                //Recorrer productos habilitados para internet y valodar carrito de compras
                                foreach (var vr_itepro in ob_retpro)
                                {
                                    //Valiar keypro vta VS keypro int
                                    if (vr_itevta.KeyProducto == vr_itepro.Codigo)
                                    {
                                        //Recorrer y asignar productos por cantidad seleccionada
                                        for (int j = 0; j < vr_itevta.Cantidad; j++)
                                        {
                                            //Asignar valores
                                            Productos ob_auxven = new Productos();
                                            List<Receta> ob_comrec = new List<Receta>();

                                            //Validar tipo
                                            if (vr_itevta.Tipo == "A")
                                            {
                                                ob_auxven.Codigo = vr_itevta.ProCategoria1;
                                                ob_auxven.Tipo = "P";
                                            }
                                            else
                                            {
                                                ob_auxven.Codigo = vr_itevta.KeyProducto;
                                                ob_auxven.Tipo = vr_itevta.Tipo;
                                            }

                                            //validar Cliente frecuente
                                            ob_auxven.Precio = 1;
                                            //if (Session.GetString("ClienteFrecuente") == "No")
                                            //    ob_auxven.Precio = 1;
                                            //else
                                            //    ob_auxven.Precio = 2;

                                            ob_auxven.Receta = null;
                                            ob_auxven.Descripcion = vr_itevta.Descripcion;

                                            //Validar receta del combo
                                            if (ob_auxven.Tipo == "C")
                                            {
                                                //Recorrer receta del combo y guardar
                                                lc_swtcat = 1;
                                                foreach (var vr_itecom in vr_itepro.Receta)
                                                {
                                                    List<Producto> ob_compro = new List<Producto>();


                                                    //Validar si es categoria
                                                    if (vr_itecom.Tipo == "A")
                                                    {
                                                        //Asignar valores
                                                        switch (lc_swtcat)
                                                        {
                                                            case 1:
                                                                lc_canrot = vr_itevta.CanCategoria1;
                                                                lc_prorot = vr_itevta.ProCategoria1;
                                                                break;
                                                            case 2:
                                                                lc_canrot = vr_itevta.CanCategoria2;
                                                                lc_prorot = vr_itevta.ProCategoria2;
                                                                break;
                                                            case 3:
                                                                lc_canrot = vr_itevta.CanCategoria3;
                                                                lc_prorot = vr_itevta.ProCategoria3;
                                                                break;
                                                            case 4:
                                                                lc_canrot = vr_itevta.CanCategoria4;
                                                                lc_prorot = vr_itevta.ProCategoria4;
                                                                break;
                                                            case 5:
                                                                lc_canrot = vr_itevta.CanCategoria5;
                                                                lc_prorot = vr_itevta.ProCategoria5;
                                                                break;
                                                        }




                                                        lc_swtcat++;

                                                        //Adicionar retail detcombo

                                                        decimal lc_secsec1 = Convert.ToDecimal(App.Secuencia);
                                                        using (var context2 = new DataDB(config))
                                                        {
                                                            var RetailDet = context.RetailDet.Where(x => x.IdRetailSales == vr_itevta.Id).Where(x => x.Secuencia == lc_secsec1).Where(x => x.ProCategoria == lc_prorot).ToList();
                                                            foreach (var retail in RetailDet)
                                                            {
                                                                //Adicionar producto seleccionado de la categoria la cantidad indicada

                                                                ob_compro.Add(new Producto
                                                                {
                                                                    Tipo = "P",
                                                                    Codigo = Convert.ToDecimal(retail.ProItem.Substring(0, retail.ProItem.IndexOf(","))),
                                                                    Cantidad = 1,
                                                                    Descripcion = retail.ProItem.Substring(retail.ProItem.IndexOf("-") + 1)
                                                                });

                                                            }
                                                        }

                                                        //Adicionar producto a receta del combo
                                                        ob_comrec.Add(new Receta
                                                        {
                                                            Tipo = vr_itecom.Tipo,
                                                            Codigo = vr_itecom.Codigo,
                                                            Cantidad = vr_itecom.Cantidad,
                                                            Descripcion = vr_itecom.Descripcion,
                                                            RecetaCategoria = ob_compro
                                                        });
                                                    }
                                                    else
                                                    {
                                                        //Adicionar producto a receta del combo
                                                        ob_comrec.Add(vr_itecom);
                                                    }
                                                }
                                            }

                                            //Cargar producto
                                            ob_auxven.Receta = ob_comrec;
                                            ob_proven.Add(ob_auxven);
                                        }

                                        break;
                                    }
                                }

                                //Asignar valor total de productos
                                lc_valpro += vr_itevta.Precio * vr_itevta.Cantidad;
                            }

                            //Asignar valores
                            ob_intvta.Productos = ob_proven;
                            ob_intvta.Ubicaciones = ob_ubiprg;

                            //Asignar valores
                            ob_intvta.Sala = 0;
                            ob_intvta.Funcion = 0;
                            ob_intvta.Pelicula = 0;
                            ob_intvta.FechaFun = string.Concat(lc_fecven.Substring(6, 4), "-", lc_fecven.Substring(3, 2), "-", lc_fecven.Substring(0, 2));
                            ob_intvta.InicioFun = 0;
                            ob_intvta.Accion = "C";
                            ob_intvta.TotalVenta = Convert.ToDouble(lc_valpro);

                            //Validar pago cashback
                            if (pr_datpro.SwitchCashback == "S")
                            {
                                ob_intvta.CodMedioPago = Convert.ToInt32(config.Value.CodMedioPagoCB);
                                ob_intvta.PagoInterno = Convert.ToDouble(lc_valpro);
                                ob_intvta.PagoCredito = 0;
                            }
                            else
                            {
                                ob_intvta.CodMedioPago = Convert.ToInt32(config.Value.CodMedioPago);
                                ob_intvta.PagoInterno = 0;
                                ob_intvta.PagoCredito = Convert.ToDouble(lc_valpro);
                            }
                        }

                        break;

                    case "M": //VENTA MIXTA (BOLETAS Y RETAIL)
                        #region SERVICIO SCOPRE
                        //Asignar valores PRE
                        ob_scopre.Punto = Convert.ToInt32(App.PuntoVenta);
                        ob_scopre.Teatro = Convert.ToInt32(KeyTeatro);
                        ob_scopre.Tercero = App.ValorTercero;

                        //Generar y encriptar JSON para servicio PRE
                        lc_srvpar = ob_fncgrl.JsonConverter(ob_scopre);
                        lc_srvpar = lc_srvpar.Replace("Teatro", "teatro");
                        lc_srvpar = lc_srvpar.Replace("Tercero", "tercero");
                        lc_srvpar = lc_srvpar.Replace("punto", "Punto");

                        //Encriptar Json PRE
                        lc_srvpar = ob_fncgrl.EncryptStringAES(lc_srvpar);

                        //Consumir servicio PRE
                        if (clientFrecnt == "No")
                            lc_result = ob_fncgrl.WebServices(string.Concat(App.ScoreServices, "scopre/"), lc_srvpar);
                        else
                            lc_result = ob_fncgrl.WebServices(string.Concat(App.ScoreServices, "scopcf/"), lc_srvpar);

                        //Validar respuesta
                        if (lc_result.Substring(0, 1) == "0")
                        {
                            //Quitar switch
                            lc_result = lc_result.Replace("0-", "");
                            ob_lstpro = (Dictionary<string, object>)JsonConvert.DeserializeObject(lc_result, (typeof(Dictionary<string, object>)));
                            ob_retpro = (List<Producto>)JsonConvert.DeserializeObject(ob_lstpro["Lista_Productos"].ToString(), (typeof(List<Producto>)));

                            if (ob_lstpro.ContainsKey("Validación"))
                                MessageBox.Show(ob_diclst["Validación"].ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        else
                        {
                            lc_result = lc_result.Replace("1-", "");
                            MessageBox.Show(lc_result, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        #endregion

                        //Obtener productos carrito de compra
                        using (var context = new DataDB(config))
                        {
                            //Select * From RetailSales Where Secuencia == ob_datpro.KeySecuencia

                            decimal IdTeatro = Convert.ToDecimal(KeyTeatro);
                            var RetailSales = context.RetailSales.Where(x => x.Secuencia == Convert.ToDecimal(lc_secsec)).Where(x => x.PuntoVenta == Convert.ToDecimal(PuntoVenta)).Where(x => x.KeyTeatro == IdTeatro).ToList();
                            foreach (var vr_itevta in RetailSales)
                            {
                                //Recorrer productos habilitados para internet y valodar carrito de compras
                                foreach (var vr_itepro in ob_retpro)
                                {
                                    //Valiar keypro vta VS keypro int
                                    if (vr_itevta.KeyProducto == vr_itepro.Codigo)
                                    {
                                        //Recorrer y asignar productos por cantidad seleccionada
                                        for (int j = 0; j < vr_itevta.Cantidad; j++)
                                        {
                                            //Asignar valores
                                            Productos ob_auxven = new Productos();
                                            List<Receta> ob_comrec = new List<Receta>();

                                            //Validar tipo
                                            if (vr_itevta.Tipo == "A")
                                            {
                                                ob_auxven.Codigo = vr_itevta.ProCategoria1;
                                                ob_auxven.Tipo = "P";
                                            }
                                            else
                                            {
                                                ob_auxven.Codigo = vr_itevta.KeyProducto;
                                                ob_auxven.Tipo = vr_itevta.Tipo;
                                            }

                                            //validar Cliente frecuente
                                            ob_auxven.Precio = 1;
                                            //if (Session.GetString("ClienteFrecuente") == "No")
                                            //    ob_auxven.Precio = 1;
                                            //else
                                            //    ob_auxven.Precio = 2;

                                            ob_auxven.Receta = null;
                                            ob_auxven.Descripcion = vr_itevta.Descripcion;

                                            //Validar receta del combo
                                            if (ob_auxven.Tipo == "C")
                                            {
                                                //Recorrer receta del combo y guardar
                                                lc_swtcat = 1;
                                                foreach (var vr_itecom in vr_itepro.Receta)
                                                {
                                                    List<Producto> ob_compro = new List<Producto>();
                                                    //Validar si es categoria
                                                    if (vr_itecom.Tipo == "A")
                                                    {
                                                        //Asignar valores
                                                        switch (lc_swtcat)
                                                        {
                                                            case 1:
                                                                lc_canrot = vr_itevta.CanCategoria1;
                                                                lc_prorot = vr_itevta.ProCategoria1;
                                                                break;
                                                            case 2:
                                                                lc_canrot = vr_itevta.CanCategoria2;
                                                                lc_prorot = vr_itevta.ProCategoria2;
                                                                break;
                                                            case 3:
                                                                lc_canrot = vr_itevta.CanCategoria3;
                                                                lc_prorot = vr_itevta.ProCategoria3;
                                                                break;
                                                            case 4:
                                                                lc_canrot = vr_itevta.CanCategoria4;
                                                                lc_prorot = vr_itevta.ProCategoria4;
                                                                break;
                                                            case 5:
                                                                lc_canrot = vr_itevta.CanCategoria5;
                                                                lc_prorot = vr_itevta.ProCategoria5;
                                                                break;
                                                        }



                                                        lc_swtcat++;

                                                        //Adicionar retail detcombo

                                                        decimal lc_secsec1 = Convert.ToDecimal(App.Secuencia);
                                                        using (var context2 = new DataDB(config))
                                                        {
                                                            var RetailDet = context.RetailDet.Where(x => x.IdRetailSales == vr_itevta.Id).Where(x => x.Secuencia == lc_secsec1).Where(x => x.ProCategoria == lc_prorot).ToList();
                                                            foreach (var retail in RetailDet)
                                                            {
                                                                //Adicionar producto seleccionado de la categoria la cantidad indicada

                                                                ob_compro.Add(new Producto
                                                                {
                                                                    Tipo = "P",
                                                                    Codigo = Convert.ToDecimal(retail.ProItem.Substring(0, retail.ProItem.IndexOf(","))),
                                                                    Cantidad = 1,
                                                                    Descripcion = retail.ProItem.Substring(retail.ProItem.IndexOf("-") + 1)
                                                                });

                                                            }
                                                        }

                                                        //Adicionar producto a receta del combo
                                                        ob_comrec.Add(new Receta
                                                        {
                                                            Tipo = vr_itecom.Tipo,
                                                            Codigo = vr_itecom.Codigo,
                                                            Cantidad = vr_itecom.Cantidad,
                                                            Descripcion = vr_itecom.Descripcion,
                                                            RecetaCategoria = ob_compro
                                                        });
                                                    }
                                                    else
                                                    {
                                                        //Adicionar producto a receta del combo
                                                        ob_comrec.Add(vr_itecom);
                                                    }
                                                }
                                            }

                                            //Cargar producto
                                            ob_auxven.Receta = ob_comrec;
                                            ob_proven.Add(ob_auxven);
                                        }

                                        break;
                                    }
                                }

                                //Asignar valor total de productos
                                lc_valpro += vr_itevta.Precio * vr_itevta.Cantidad;
                            }


                            //Obtener boletas carrito de compra

                            //Select * From ReportSales Where Secuencia == ob_datpro.KeySecuencia

                            var ReportSales = context.ReportSales.Where(x => x.Secuencia == lc_secsec.ToString()).Where(x => x.KeyPunto == PuntoVenta).Where(x => x.KeyTeatro == KeyTeatro).ToList();
                            foreach (var vr_itevta in ReportSales)
                            {
                                ob_intvta.Sala = Convert.ToInt32(vr_itevta.KeySala);
                                ob_intvta.Funcion = Convert.ToInt32(vr_itevta.HorProg.Substring(0, 2));
                                ob_intvta.Pelicula = Convert.ToInt32(vr_itevta.KeyPelicula);
                                ob_intvta.FechaFun = string.Concat(vr_itevta.FecProg.Substring(0, 4), "-", vr_itevta.FecProg.Substring(4, 2), "-", vr_itevta.FecProg.Substring(6, 2));
                                ob_intvta.InicioFun = Convert.ToInt32(vr_itevta.HorProg);

                                lc_boltot = vr_itevta.Precio;
                                lc_ubiprg = vr_itevta.SelUbicaciones;
                                lc_keytar = Convert.ToInt32(vr_itevta.KeyTarifa);
                            }

                            //Obtener ubicaciones de vista
                            char[] ar_charst = lc_ubiprg.ToCharArray();
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

                                lc_cntubi++;
                                lc_ubilbl += string.Concat("Fila: ", ls_lstsel[1], " Columna: ", ls_lstsel[2], ";");
                                ob_ubiprg.Add(new Ubicaciones() { Fila = ls_lstsel[3], Columna = Convert.ToInt32(ls_lstsel[4]), Tarifa = lc_keytar, FilRelativa = ls_lstsel[1], ColRelativa = Convert.ToInt32(ls_lstsel[2]), TipoSilla = "", TipoZona = "", EstadoSilla = "" });
                            }

                            //Adicionar a lista
                            ob_ordite.Add(new OrderItem
                            {
                                Precio = Convert.ToDecimal(lc_boltot),
                                Cantidad = lc_cntubi,
                                Descripcion = string.Concat(lc_despel, "-", lc_ubilbl),
                                KeyProducto = ob_intvta.Pelicula
                            });
                        }

                        //Venta de boletas y confites
                        lc_boltot += Convert.ToDouble(lc_valpro);

                        //Asignar valores
                        ob_intvta.Productos = ob_proven;
                        ob_intvta.Ubicaciones = ob_ubiprg;
                        ob_intvta.Accion = pr_datpro.SwtVenta;
                        ob_intvta.TotalVenta = lc_boltot;

                        //Validar pago cashback
                        if (pr_datpro.SwitchCashback == "S")
                        {
                            ob_intvta.CodMedioPago = Convert.ToInt32(config.Value.CodMedioPagoCB);
                            ob_intvta.PagoInterno = Convert.ToDouble(lc_boltot);
                            ob_intvta.PagoCredito = 0;
                        }
                        else
                        {
                            ob_intvta.CodMedioPago = Convert.ToInt32(config.Value.CodMedioPago);
                            ob_intvta.PagoInterno = 0;
                            ob_intvta.PagoCredito = Convert.ToDouble(lc_boltot);
                        }

                        break;
                }

                #region SERVICIO SCOINT
                //Asignar valores
                ob_intvta.Nombre = NombreEli;
                ob_intvta.Factura = Convert.ToInt32(KeySecuencia);
                ob_intvta.Apellido = ApellidoEli;
                ob_intvta.Telefono = Convert.ToInt64(TelefonoEli);
                ob_intvta.Direccion = DireccionEli;
                ob_intvta.PuntoVenta = Convert.ToInt32(App.PuntoVenta);
                ob_intvta.DocIdentidad = Convert.ToInt64(DocumentoEli);
                ob_intvta.CorreoCliente = EmailEli;

                ob_intvta.Obs1 = "";
                ob_intvta.Obs2 = "";
                ob_intvta.Obs3 = "";
                ob_intvta.Obs4 = "";
                ob_intvta.Placa = "0";
                ob_intvta.Teatro = Convert.ToInt32(KeyTeatro);
                ob_intvta.Tercero = Convert.ToInt32(App.ValorTercero);
                ob_intvta.AudiPrev = 0;
                ob_intvta.TipoBono = 0;
                ob_intvta.Cortesia = "";
                ob_intvta.TipoEntrega = "T";

                ob_intvta.PagoEfectivo = 0;
                ob_intvta.ClienteFrecuente = lc_barclf;

                //Generar y encriptar JSON para servicio
                lc_srvpar = ob_fncgrl.JsonConverter(ob_intvta);
                lc_srvpar = lc_srvpar.Replace("puntoVenta", "PuntoVenta");
                lc_srvpar = lc_srvpar.Replace("factura", "Factura");
                lc_srvpar = lc_srvpar.Replace("correoCliente", "CorreoCliente");
                lc_srvpar = lc_srvpar.Replace("docIdentidad", "DocIdentidad");
                lc_srvpar = lc_srvpar.Replace("nombre", "Nombre");
                lc_srvpar = lc_srvpar.Replace("apellido", "Apellido");
                lc_srvpar = lc_srvpar.Replace("telefono", "Telefono");
                lc_srvpar = lc_srvpar.Replace("direccion", "Direccion");
                lc_srvpar = lc_srvpar.Replace("sala", "Sala");
                lc_srvpar = lc_srvpar.Replace("fechaFun", "FechaFun");
                lc_srvpar = lc_srvpar.Replace("funcion", "Funcion");
                lc_srvpar = lc_srvpar.Replace("inicioFun", "InicioFun");
                lc_srvpar = lc_srvpar.Replace("ubicaciones", "Ubicaciones");
                lc_srvpar = lc_srvpar.Replace("fila", "Fila");
                lc_srvpar = lc_srvpar.Replace("columna", "Columna");
                lc_srvpar = lc_srvpar.Replace("tarifa", "Tarifa");
                lc_srvpar = lc_srvpar.Replace("filRelativa", "FilRelativa");
                lc_srvpar = lc_srvpar.Replace("colRelativa", "ColRelativa");
                lc_srvpar = lc_srvpar.Replace("pelicula", "Pelicula");
                lc_srvpar = lc_srvpar.Replace("productos", "Productos");

                lc_srvpar = lc_srvpar.Replace("receta", "Receta");
                lc_srvpar = lc_srvpar.Replace("RecetaCategoria", "Receta");
                lc_srvpar = lc_srvpar.Replace("codigo", "Codigo");
                lc_srvpar = lc_srvpar.Replace("tipo", "Tipo");
                lc_srvpar = lc_srvpar.Replace("descripcion", "Descripcion");
                lc_srvpar = lc_srvpar.Replace("precio", "Precio");
                lc_srvpar = lc_srvpar.Replace("precios", "Precio");
                lc_srvpar = lc_srvpar.Replace("Precios", "Precio");
                lc_srvpar = lc_srvpar.Replace("cantidad", "Cantidad");

                lc_srvpar = lc_srvpar.Replace("placa", "Placa");
                lc_srvpar = lc_srvpar.Replace("audiPrev", "AudiPrev");
                lc_srvpar = lc_srvpar.Replace("tipoEntrega", "TipoEntrega");
                lc_srvpar = lc_srvpar.Replace("cortesia", "Cortesia");
                lc_srvpar = lc_srvpar.Replace("tipoBono", "TipoBono");
                lc_srvpar = lc_srvpar.Replace("clienteFrecuente", "ClienteFrecuente");
                lc_srvpar = lc_srvpar.Replace("totalVenta", "TotalVenta");
                lc_srvpar = lc_srvpar.Replace("pagoInterno", "PagoInterno");
                lc_srvpar = lc_srvpar.Replace("pagoCredito", "PagoCredito");
                lc_srvpar = lc_srvpar.Replace("pagoEfectivo", "PagoEfectivo");
                lc_srvpar = lc_srvpar.Replace("codMedioPago", "CodMedioPago");
                lc_srvpar = lc_srvpar.Replace("obs1", "Obs1");
                lc_srvpar = lc_srvpar.Replace("obs2", "Obs2");
                lc_srvpar = lc_srvpar.Replace("obs3", "Obs3");
                lc_srvpar = lc_srvpar.Replace("obs4", "Obs4");
                lc_srvpar = lc_srvpar.Replace("accion", "Accion");

                //Encriptar Json
                lc_srvpar = ob_fncgrl.EncryptStringAES(lc_srvpar);

                //Consumir servicio
                lc_result = ob_fncgrl.WebServices(string.Concat(App.ScoreServices, "scoint/"), lc_srvpar);

                if (lc_result.Substring(0, 1) == "0")
                {
                    //Quitar switch
                    lc_result = lc_result.Replace("0-", "");
                    lc_result = lc_result.Replace("[", "");
                    lc_result = lc_result.Replace("]", "");

                    //Deserializar Json y validar respuesta
                    ob_diclst = (Dictionary<string, string>)JsonConvert.DeserializeObject(lc_result, (typeof(Dictionary<string, string>)));

                    //Validar respuesta llave 1
                    if (ob_diclst.ContainsKey("Validación"))
                    {
                        MessageBox.Show(ob_diclst["Validación"].ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        //Validar respuesta llave 2
                        if (ob_diclst.ContainsKey("Respuesta"))
                        {
                            if (ob_diclst["Respuesta"].ToString() != "Proceso exitoso.")
                            {
                                //Crear transacción del registro
                                using (var transaction = new DataDB(config))
                                {
                                    var transactionSales = new TransactionSales
                                    {
                                        Secuencia = Convert.ToDecimal(KeySecuencia),
                                        PuntoVenta = Convert.ToDecimal(App.PuntoVenta),
                                        Teatro = Convert.ToDecimal(KeyTeatro),
                                        EmailEli = EmailEli,
                                        NombreEli = NombreEli + " " + ApellidoEli,
                                        DocumentoEli = DocumentoEli,
                                        TelefonoEli = TelefonoEli,
                                        EstadoTx = "ERR",
                                        FechaTx = DateTime.Now,
                                        ValorTx = 0,
                                        BaseTx = 0,
                                        IvaTx = 0,
                                        IcoTx = 0,
                                        AutorizacionTx = "-",
                                        ReferenciaTx = "-",
                                        ReferenciaEx = "VENTA RECHAZADA SOLO SCORE ",
                                        BancoTx = "-",
                                        Observaciones = ob_diclst["Respuesta"].ToString(),
                                        FechaCreado = DateTime.Now,
                                        FechaModificado = DateTime.Now
                                    };

                                    //Adicionar y guardar registro a tabla
                                    transaction.TransactionSales.Add(transactionSales);
                                    transaction.SaveChanges();
                                }

                                MessageBox.Show(ob_diclst["Respuesta"].ToString() + " SECUENCIA: " + App.Secuencia + "-PUNTOVTA: " + App.PuntoVenta, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                            else
                            {
                                if (lc_tipven == "B")
                                {
                                    Base = Math.Round(Convert.ToDecimal(ob_diclst["Total"].ToString()), 2);
                                    Total = Math.Round(Convert.ToDecimal(ob_diclst["Total"].ToString()), 2);
                                }
                                else if (lc_tipven == "M")
                                {
                                    Base = Math.Round(Convert.ToDecimal(ob_diclst["Boletas+Base"].ToString()), 2);
                                    Total = Math.Round(Convert.ToDecimal(ob_diclst["Total"].ToString()), 2);
                                }
                                else
                                {
                                    Base = Math.Round(Convert.ToDecimal(ob_diclst["Base"].ToString()), 2);
                                    Total = Math.Round(Convert.ToDecimal(ob_diclst["Total"].ToString()), 2);
                                }

                                if (ob_diclst["Impuesto_1"].ToString() != "0")
                                {
                                    //Validar impoconsumo 1
                                    if (ob_diclst["TipoImpuesto_1"].ToString().Contains("8%"))
                                        Impuesto_1 = Math.Round(Convert.ToDecimal(ob_diclst["Impuesto_1"].ToString()), 2);

                                    //Validar iva 1
                                    if (ob_diclst["TipoImpuesto_1"].ToString().Contains("19%"))
                                        Impuesto_2 = Math.Round(Convert.ToDecimal(ob_diclst["Impuesto_1"].ToString()), 2);
                                }

                                if (ob_diclst["Impuesto_2"].ToString() != "0")
                                {
                                    //Validar impoconsumo 1
                                    if (ob_diclst["TipoImpuesto_2"].ToString().Contains("8%"))
                                        Impuesto_1 = Math.Round(Convert.ToDecimal(ob_diclst["Impuesto_2"].ToString()), 2);

                                    //Validar iva 1
                                    if (ob_diclst["TipoImpuesto_2"].ToString().Contains("19%"))
                                        Impuesto_2 = Math.Round(Convert.ToDecimal(ob_diclst["Impuesto_2"].ToString()), 2);
                                }

                                CashBack_Acumulado = Math.Round(Convert.ToDecimal(ob_diclst["CashBack_Acumulado"].ToString()), 2);



                                //Crear transacción del registro
                                using (var transaction = new DataDB(config))
                                {
                                    var transactionSales = new TransactionSales
                                    {
                                        Secuencia = Convert.ToDecimal(KeySecuencia),
                                        PuntoVenta = Convert.ToDecimal(App.PuntoVenta),
                                        Teatro = Convert.ToDecimal(KeyTeatro),
                                        EmailEli = EmailEli,
                                        NombreEli = NombreEli + " " + ApellidoEli,
                                        DocumentoEli = DocumentoEli,
                                        TelefonoEli = TelefonoEli,
                                        EstadoTx = "SCO",
                                        FechaTx = DateTime.Now,
                                        ValorTx = Total,
                                        BaseTx = Base,
                                        IvaTx = Impuesto_2,
                                        IcoTx = Impuesto_1,
                                        AutorizacionTx = "-",
                                        ReferenciaTx = "-",
                                        ReferenciaEx = "-",
                                        BancoTx = "-",
                                        Observaciones = "VENTA SOLO SCORE",
                                        FechaCreado = DateTime.Now,
                                        FechaModificado = DateTime.Now
                                    };

                                    //Adicionar y guardar registro a tabla
                                    transaction.TransactionSales.Add(transactionSales);
                                    transaction.SaveChanges();

                                    try
                                    {
                                        MessageBox.Show(ob_diclst["Respuesta"].ToString() + " SECUENCIA: " + App.Secuencia + "-PUNTOVTA: " + App.PuntoVenta, "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show(ob_diclst["Respuesta"].ToString() + " SECUENCIA: " + App.Secuencia + "-PUNTOVTA: " + App.PuntoVenta, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show(ob_diclst["Respuesta"].ToString() + " SECUENCIA: " + App.Secuencia + "-PUNTOVTA: " + App.PuntoVenta, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                #endregion


                //Validar redireccion si hay pago con cashback
                if (pr_datpro.SwitchCashback == "S")
                {
                    string ref_payco = "CashBack:" + Total.ToString();

                    App.Responses(ref_payco);
                }
                else
                {
                    //var secretKey = config.Value.secretKey;
                    //var queryString = $"{pr_datpro.KeySecuencia}{pr_datpro.SwtVenta}{Total}{Impuesto_1}{Impuesto_2}{Base}{CashBack_Acumulado}";
                    //var computedSignature = ComputeSignature(queryString, secretKey);
                    //return RedirectToAction("Finish", "Pages", new { pr_secpro = pr_datpro.KeySecuencia, pr_swtven = pr_datpro.SwtVenta, pr_valven = Total, pr_valimp = Impuesto_1, pr_valiva = Impuesto_2, pr_valbas = Base, pr_casbck = CashBack_Acumulado, signature = computedSignature });
                }
            }
            catch (Exception lc_syserr)
            {
                //Crear transacción del registro
                using (var transaction = new DataDB(config))
                {
                    var transactionSales = new TransactionSales
                    {
                        Secuencia = Convert.ToDecimal(KeySecuencia),
                        PuntoVenta = Convert.ToDecimal(App.PuntoVenta),
                        Teatro = Convert.ToDecimal(App.idCine),
                        EmailEli = EmailEli,
                        NombreEli = NombreEli + " " + ApellidoEli,
                        DocumentoEli = DocumentoEli,
                        TelefonoEli = TelefonoEli,
                        EstadoTx = "ERR",
                        FechaTx = DateTime.Now,
                        ValorTx = 0,
                        BaseTx = 0,
                        IvaTx = 0,
                        IcoTx = 0,
                        AutorizacionTx = "-",
                        ReferenciaTx = "-",
                        ReferenciaEx = "VENTA RECHAZADA SOLO SCORE ",
                        BancoTx = "-",
                        Observaciones = lc_syserr.Message,
                        FechaCreado = DateTime.Now,
                        FechaModificado = DateTime.Now
                    };

                    //Adicionar y guardar registro a tabla
                    transaction.TransactionSales.Add(transactionSales);
                    transaction.SaveChanges();
                }

                MessageBox.Show((lc_syserr.Message).Contains("Inner") ? lc_syserr.InnerException.Message : "null", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static void Responses(string ref_payco = "")
        {
            #region VARIABLES LOCALES
            string lc_fectra = string.Empty;
            string lc_valtra = string.Empty;
            string lc_idsepy = string.Empty;
            string lc_status = string.Empty;
            string lc_coreli = string.Empty;
            string lc_jsnrst = string.Empty;
            string lc_objson = string.Empty;
            string lc_srvpar = string.Empty;
            string lc_refepy = string.Empty;
            string lc_bankpy = string.Empty;
            string lc_urlcor = config.Value.UrlCorreo;

            string lc_secsec = App.Secuencia;
            string lc_keytea = App.idCine;
            decimal lc_puntea = Convert.ToDecimal(App.PuntoVenta);

            string session = "";
            string status;

            List<OrderItem> ob_ordite = new List<OrderItem>();
            Dictionary<string, string> ob_diclst = new Dictionary<string, string>();

            TransactionSales ob_repsle = new TransactionSales();
            General ob_fncgrl = new General();
            #endregion

            try
            {
                //Validar si esta la sesion activa
                if (session/*.GetString("ClienteFrecuente")*/ != null)
                {
                    //URLPortal(config);
                    //ListCarrito();

                    //ViewBag.ClienteFrecuente = Session.GetString("ClienteFrecuente");
                    //ViewBag.CashBack_Acumulado = String.Format("{0:C0}", Convert.ToDecimal(Session.GetString("CashBack_Acumulado")));
                }

                //Inicializar instancia web client para leer respuesta
                using (WebClient wc = new WebClient())
                {
                    //Obtener información de epayco
                    var ob_json = wc.DownloadString(config.Value.data_epayco_secure + ref_payco);
                    var ob_response = JsonConvert.DeserializeObject<EpaycoApiGet>(ob_json);

                    //validar rta y Obtener y deserializar respuesta
                    if (!ref_payco.Contains("CashBack"))
                    {
                        //Obtener valores de rta Epayco y consultar registro en la bd
                        lc_secsec = App.Secuencia;
                        lc_keytea = App.idCine;
                        lc_coreli = ob_response.data.x_customer_email.ToString();
                        lc_valtra = ob_response.data.x_amount.ToString();
                        lc_status = ob_response.data.x_response.ToString();
                        lc_idsepy = ob_response.data.x_transaction_id.ToString();
                        lc_fectra = ob_response.data.x_fecha_transaccion.ToString();
                        lc_refepy = ob_response.data.x_ref_payco.ToString();
                        lc_bankpy = ob_response.data.x_bank_name.ToString();
                    }
                    else
                    {
                        //Obtener valores de rta cashback y consultar registro en la bd
                        lc_secsec = App.Secuencia;
                        lc_keytea = App.idCine;
                        lc_coreli = session/*.GetString("Usuario")*/;
                        lc_valtra = ref_payco.Substring(ref_payco.IndexOf(":") + 1);
                        lc_status = "Cashback";
                        lc_idsepy = "Cashback:SEC-" + session/*.GetString("Secuencia")*/;
                        lc_fectra = DateTime.Now.ToString();
                        lc_refepy = App.PuntoVenta + "-" + session/*.GetString("Secuencia")*/;
                        lc_bankpy = "CashBack Procinal";
                    }

                    //Inicializar instancia de BD
                    using (var context = new DataDB(config))
                    {
                        //Consultar registro de venta en BD transacciones
                        var ob_repsl1 = context.TransactionSales.Where(x => x.Secuencia == Convert.ToDecimal(lc_secsec)).Where(x => x.PuntoVenta == lc_puntea).Where(x => x.Teatro == Convert.ToDecimal(lc_keytea));
                        foreach (var TransactionSales in ob_repsl1)
                            ob_repsle = context.TransactionSales.Find(TransactionSales.Id);

                        //Inicializar valores
                        switch (lc_status)
                        {
                            case "Cashback":
                                ob_repsle.EstadoTx = "CBK";
                                ob_repsle.Observaciones = "VENTA SCORE/PROCINAL";

                                status = "success";
                                break;

                            case "Aceptada":
                                ob_repsle.EstadoTx = "EPY";
                                ob_repsle.Observaciones = "VENTA SCORE/EPAYCO";

                                status = "success";
                                break;

                            case "Rechazada":
                            case "Fallida":
                                ob_repsle.EstadoTx = "REX";
                                ob_repsle.Observaciones = "VENTA RECHAZADA EPAYCO";

                                status = "failure";
                                break;

                            case "Pendiente":
                                ob_repsle.EstadoTx = "EPX";
                                ob_repsle.Observaciones = "VENTA PENDIENTE EPAYCO";

                                status = "pending";
                                break;
                        }

                        ob_repsle.FechaTx = Convert.ToDateTime(lc_fectra);
                        ob_repsle.AutorizacionTx = string.Concat(lc_idsepy, ",", lc_status);
                        ob_repsle.ReferenciaTx = lc_refepy;
                        ob_repsle.ReferenciaEx = ref_payco;
                        ob_repsle.BancoTx = lc_bankpy;
                        ob_repsle.FechaModificado = DateTime.Now;

                        //Validar si la sesion esta activa
                        if (session/*.GetString("Usuario")*/ != null)
                        {
                            ob_repsle.EmailEli = session/*.GetString("Usuario")*/;
                            ob_repsle.NombreEli = session/*.GetString("Nombre") */+ " " + session/*.GetString("Apellido")*/;
                            ob_repsle.TelefonoEli = session/*.GetString("Telefono")*/;
                            ob_repsle.DocumentoEli = session/*.GetString("Documento")*/;
                        }

                        //Actualizar estado de transacción
                        context.TransactionSales.Update(ob_repsle);
                        context.SaveChanges();


                    }
                }

                //ViewBag.CarteleraWP = config.Value.CarteleraWP;

                //Adicionar valores de envio de correo Score
                lc_urlcor = lc_urlcor.Replace("#xxx", lc_keytea.ToString());
                lc_urlcor = lc_urlcor.Replace("#yyy", App.PuntoVenta);
                lc_urlcor = lc_urlcor.Replace("#zzz", lc_secsec.ToString());
                lc_urlcor = lc_urlcor.Replace("#ccc", lc_coreli);

                //Estado Exitoso
                if (lc_status == "Aceptada" || lc_status == "Cashback")
                {
                    //Obtener resumen de compra
                    //ViewBag.ListB = ViewBag.ListCarritoB;

                    using (var context = new DataDB(config))
                    {
                        var rs = context.RetailSales.Where(x => x.Secuencia == Convert.ToDecimal(lc_secsec)).Where(x => x.PuntoVenta == lc_puntea).Where(x => x.KeyTeatro == Convert.ToDecimal(lc_keytea)).Where(x => x.Tipo == "C").ToList();
                        List<RetailSales> retailsales = rs
                            .GroupBy(l => l.KeyProducto)
                            .Select(cl => new RetailSales
                            {
                                Descripcion = cl.First().Descripcion,
                                Cantidad = cl.Sum(c => c.Cantidad),
                                Precio = cl.Sum(c => c.Precio)
                            }).ToList();

                        foreach (var vr_itevta in retailsales)
                        {
                            //Adicionar a lista
                            ob_ordite.Add(new OrderItem
                            {
                                Precio = vr_itevta.Precio,
                                Cantidad = Convert.ToInt32(vr_itevta.Cantidad),
                                Descripcion = vr_itevta.Descripcion
                            });
                        }

                        rs = context.RetailSales.Where(x => x.Secuencia == Convert.ToDecimal(lc_secsec)).Where(x => x.PuntoVenta == lc_puntea).Where(x => x.KeyTeatro == Convert.ToDecimal(lc_keytea)).Where(x => x.Tipo != "C").ToList();

                        foreach (var vr_itevta in rs)
                        {
                            //Adicionar a lista
                            ob_ordite.Add(new OrderItem
                            {
                                Precio = vr_itevta.Precio * Convert.ToInt32(vr_itevta.Cantidad),
                                Cantidad = Convert.ToInt32(vr_itevta.Cantidad),
                                Descripcion = vr_itevta.Descripcion
                            });
                        }

                        //ViewBag.ListR = ob_ordite; //ViewBag.ListCarritoR;
                    }

                    //ViewBag.ListCarritoB = null;
                    //ViewBag.ListCarritoR = null;

                    if (App.Secuencia != null)
                    {
                        try
                        {
                            //Envio de correo Score
                            var request = (HttpWebRequest)WebRequest.Create(lc_urlcor);
                            request.GetResponse();

                        }
                        catch (Exception)
                        {
                            MessageBox.Show("Fallo envío de correo compra APROBADA, por favor comunicarse con el teatro.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }

                //Estado Fallido
                //Validar venta
                if (lc_status == "Rechazada" || lc_status == "Fallida")
                {
                    if (App.Secuencia != null)
                    {
                        #region SERVICO SCORET
                        //Json de servicio RET
                        lc_objson = "{\"Punto\":" + Convert.ToInt32(App.PuntoVenta) + ",\"Pedido\":" + Convert.ToInt32(lc_secsec) + ",\"teatro\":\"" + Convert.ToInt32(lc_keytea) + "\",\"tercero\":\"" + config.Value.ValorTercero + "\"}";

                        //Encriptar Json RET
                        lc_srvpar = ob_fncgrl.EncryptStringAES(lc_objson);

                        //Consumir servicio RET
                        lc_jsnrst = ob_fncgrl.WebServices(string.Concat(App.ScoreServices, "scoret/"), lc_srvpar);

                        //Validar respuesta
                        if (lc_jsnrst.Substring(0, 1) == "0")
                        {
                            //Quitar switch
                            lc_jsnrst = lc_jsnrst.Replace("0-", "");
                            lc_jsnrst = lc_jsnrst.Replace("[", "");
                            lc_jsnrst = lc_jsnrst.Replace("]", "");

                            //Deserializar Json y validar respuesta SEC
                            ob_diclst = (Dictionary<string, string>)JsonConvert.DeserializeObject(lc_jsnrst, (typeof(Dictionary<string, string>)));

                            //Validar respuesta llave 1
                            if (ob_diclst.ContainsKey("Validación"))
                            {
                                MessageBox.Show(ob_diclst["Validación"].ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                            else
                            {
                                //Validar respuesta llave 2
                                if (ob_diclst.ContainsKey("Respuesta"))
                                {
                                    if (ob_diclst["Respuesta"].ToString() != "Proceso exitoso")
                                    {
                                        MessageBox.Show(ob_diclst["Respuesta"].ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                    }
                                }
                            }

                            ob_diclst.Clear();
                        }
                        else
                        {
                            MessageBox.Show("Reembolso no culminado con exito SCORET", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        #endregion

                        try
                        {
                            //Envio de correo Score
                            lc_urlcor = lc_urlcor.Replace("compra", "Fallida");
                            var request = (HttpWebRequest)WebRequest.Create(lc_urlcor);
                            request.GetResponse();

                        }
                        catch (Exception)
                        {
                            MessageBox.Show("Fallo envío de correo compra RECHAZADA, por favor comunicarse con el teatro.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }

                    //ViewBag.ListB = null;
                    //ViewBag.ListR = null;
                }

                //Estado Pendiente
                //Validar venta
                if (lc_status == "Pendiente")
                {
                    //ViewBag.ListB = null;
                    //ViewBag.ListR = null;

                    if (App.Secuencia != null)
                    {
                        try
                        {
                            //Envio de correo Score
                            lc_urlcor = lc_urlcor.Replace("compra", "Pendiente");
                            var request = (HttpWebRequest)WebRequest.Create(lc_urlcor);
                            request.GetResponse();
                        }
                        catch (Exception)
                        {
                            MessageBox.Show("Fallo envío de correo compra PENDIENTE, por favor comunicarse con el teatro.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }

                //Validar y remover sesion invitada
                //if (Session.GetString("FlagLogin") == "INV")
                //{
                //    Session.Remove("Nombre");
                //    Session.Remove("Passwrd");
                //    Session.Remove("Usuario");
                //    Session.Remove("Apellido");
                //    Session.Remove("Telefono");
                //    Session.Remove("Direccion");
                //    Session.Remove("Documento");
                //    Session.Remove("ClienteFrecuente");
                //    Session.Remove("FlagLogin");
                //    ViewBag.ListCarritoR = null;
                //    ViewBag.ListCarritoB = null;
                //}

                ////Quitar secuencia
                //Session.Remove("Secuencia");
                //return View();
            }
            catch (Exception lc_syserr)
            {
                #region SERVICO SCORET
                //Json de servicio RET
                //lc_objson = "{\"Punto\":" + Convert.ToInt32(App.PuntoVenta) + ",\"Pedido\":" + Convert.ToInt32(lc_secsec) + ",\"teatro\":\"" + Convert.ToInt32(lc_keytea) + "\",\"tercero\":\"" + config.Value.ValorTercero + "\"}";

                //Encriptar Json RET
                //lc_srvpar = ob_fncgrl.EncryptStringAES(lc_objson);

                //Consumir servicio RET
                //lc_jsnrst = ob_fncgrl.WebServices(string.Concat(App.ScoreServices, "scoret/"), lc_srvpar);
                #endregion
                MessageBox.Show((lc_syserr.Message).Contains("Inner") ? lc_syserr.InnerException.Message : "null", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                //Escribir Log
                //logAudit.LogApp(logSales);

                //Validar si esta la sesion activa y Devolver vista de error
                //if (Session.GetString("ClienteFrecuente") != null)
                //    return RedirectToAction("Error", "Pages", new { pr_message = config.Value.MessageException + logSales.Id, pr_flag = "RSPNS" });
                //else
                //    return RedirectToAction("Home", "Home");
            }
        }

        public static void agregarProducto(Producto pr_datpro)
        {
            #region VARIABLES LOCALES
            string lc_result = string.Empty;
            string lc_srvpar = string.Empty;
            string lc_auxite = string.Empty;
            string lc_secsec = string.Empty;

            List<Producto> ob_return = new List<Producto>();
            Dictionary<string, object> ob_diclst = new Dictionary<string, object>();
            Dictionary<string, string> ob_seclst = new Dictionary<string, string>();

            Secuencia ob_secsec = new Secuencia();
            Secuencia ob_scopre = new Secuencia();
            Producto ob_datpro = new Producto();
            General ob_fncgrl = new General();
            #endregion

            try
            {

                #region SERVICIO SCOSEC
                lc_secsec = pr_datpro.KeySecuencia;
                if (lc_secsec == "0")
                {
                    //Asignar valores SEC
                    ob_secsec.Punto = Convert.ToInt32(App.PuntoVenta);
                    ob_secsec.Teatro = Convert.ToInt32(App.idCine);
                    ob_secsec.Tercero = App.ValorTercero;

                    //Generar y encriptar JSON para servicio SEC
                    lc_srvpar = ob_fncgrl.JsonConverter(ob_secsec);
                    lc_srvpar = lc_srvpar.Replace("Teatro", "teatro");
                    lc_srvpar = lc_srvpar.Replace("Tercero", "tercero");
                    lc_srvpar = lc_srvpar.Replace("punto", "Punto");

                    //Encriptar Json SEC
                    lc_srvpar = ob_fncgrl.EncryptStringAES(lc_srvpar);

                    //Consumir servicio SEC
                    lc_result = ob_fncgrl.WebServices(string.Concat(App.ScoreServices, "scosec/"), lc_srvpar);

                    //Validar respuesta
                    if (lc_result.Substring(0, 1) == "0")
                    {
                        //Quitar switch
                        lc_result = lc_result.Replace("0-", "");
                        lc_result = lc_result.Replace("[", "");
                        lc_result = lc_result.Replace("]", "");

                        //Deserializar Json y validar respuesta SEC
                        ob_seclst = (Dictionary<string, string>)JsonConvert.DeserializeObject(lc_result, (typeof(Dictionary<string, string>)));

                        //Validar respuesta del servicio
                        if (ob_seclst.ContainsKey("Validación"))
                            MessageBox.Show("", ob_seclst["Validación"].ToString());
                        else
                        {
                            lc_secsec = ob_seclst["Secuencia"].ToString().Substring(0, ob_seclst["Secuencia"].ToString().IndexOf("."));
                            //Session.SetString("Secuencia", lc_secsec);
                            App.Secuencia = lc_secsec;
                        }


                        //Limpiar objeto
                        ob_seclst.Clear();
                    }
                    else
                    {
                       
                    }
                }
                #endregion

                //Validar Categoria 
                if (pr_datpro.Tipo == "A")
                {
                    if (pr_datpro.Check != null)
                    {
                        pr_datpro.ProCategoria_1 = Convert.ToDecimal(pr_datpro.Check);
                        pr_datpro.Check1 = "0";
                        pr_datpro.Check2 = "0";
                        pr_datpro.Check3 = "0";
                        pr_datpro.Check4 = "0";
                        pr_datpro.Check5 = "0";

                        pr_datpro.CanCategoria_1 = pr_datpro.Cantidad;
                        pr_datpro.Cantidad1 = 0;
                        pr_datpro.Cantidad2 = 0;
                        pr_datpro.Cantidad3 = 0;
                        pr_datpro.Cantidad4 = 0;
                        pr_datpro.Cantidad5 = 0;
                    }
                    else
                    {
                        //Obtener detalle del producto seleccionado
                        ob_datpro = GetDetails(pr_datpro);
                        if (ob_datpro.Codigo == -1)
                        {
                            //Cargar mensaje de error
                            lc_auxite = ob_datpro.Descripcion;
                            ///*ModelState.AddModelError*/("", lc_auxite);

                            //Devolver a vista
                            ob_datpro.Codigo = pr_datpro.Codigo;
                            ob_datpro.Descripcion = pr_datpro.Descripcion;
                            //return View(ob_datpro);
                        }
                        else
                        {
                            //Devolver a vista
                            MessageBox.Show("", "Debe seleccionar un ítem de la categoría para continuar");

                            //return View(ob_datpro);
                        }
                    }
                }

                //Inicializar instancia de BD
                using (var context = new DataDB(config))
                {
                    //Agregar valores a tabla ReportSales
                    var retailSales = new RetailSales
                    {
                        Tipo = pr_datpro.Tipo,
                        Precio = Convert.ToDecimal(pr_datpro.Valor),
                        Cantidad = pr_datpro.Cantidad,
                        Secuencia = Convert.ToDecimal(lc_secsec),
                        PuntoVenta = Convert.ToDecimal(App.PuntoVenta),
                        KeyProducto = pr_datpro.Codigo,
                        Descripcion = pr_datpro.Descripcion,
                        ProProducto1 = pr_datpro.ProProducto_1,
                        ProProducto2 = pr_datpro.ProProducto_2,
                        ProProducto3 = pr_datpro.ProProducto_3,
                        ProProducto4 = pr_datpro.ProProducto_4,
                        ProProducto5 = pr_datpro.ProProducto_5,
                        CanProducto1 = pr_datpro.ProCantidad_1,
                        CanProducto2 = pr_datpro.ProCantidad_2,
                        CanProducto3 = pr_datpro.ProCantidad_3,
                        CanProducto4 = pr_datpro.ProCantidad_4,
                        CanProducto5 = pr_datpro.ProCantidad_5,
                        ProCategoria1 = pr_datpro.ProCategoria_1,
                        ProCategoria2 = pr_datpro.ProCategoria_2,
                        ProCategoria3 = pr_datpro.ProCategoria_3,
                        ProCategoria4 = pr_datpro.ProCategoria_4,
                        ProCategoria5 = pr_datpro.ProCategoria_5,
                        CanCategoria1 = pr_datpro.CanCategoria_1,
                        CanCategoria2 = pr_datpro.CanCategoria_2,
                        CanCategoria3 = pr_datpro.CanCategoria_3,
                        CanCategoria4 = pr_datpro.CanCategoria_4,
                        CanCategoria5 = pr_datpro.CanCategoria_5,
                        FechaRegistro = DateTime.Now,
                        KeyTeatro = Convert.ToDecimal(App.Secuencia),
                        SwitchAdd = pr_datpro.SwitchAdd
                    };

                    //Adicionar y guardar registro a tabla
                    context.RetailSales.Add(retailSales);
                    context.SaveChanges();
                }

                //Validar Combo
                if (pr_datpro.Tipo == "C")
                {
                    //Obtener id de prodcutos de combo
                    int IdRetail = 0;
                    using (var context = new DataDB(config))
                        IdRetail = context.RetailSales.Max(u => u.Id);

                    //Recorrer la cantidad maxima de categorias por combo
                    for (int lc_variii = 0; lc_variii < 4; lc_variii++)
                    {
                        switch (lc_variii)
                        {
                            case 0:
                                RetailDet(Convert.ToDecimal(lc_secsec), IdRetail, pr_datpro.ProCategoria_1, pr_datpro.Check1, pr_datpro.Check11, pr_datpro.Check111, pr_datpro.Check1111);
                                break;
                            case 1:
                                RetailDet(Convert.ToDecimal(lc_secsec), IdRetail, pr_datpro.ProCategoria_2, pr_datpro.Check2, pr_datpro.Check22, pr_datpro.Check222, pr_datpro.Check2222);
                                break;
                            case 2:
                                RetailDet(Convert.ToDecimal(lc_secsec), IdRetail, pr_datpro.ProCategoria_3, pr_datpro.Check3, pr_datpro.Check33, pr_datpro.Check333, pr_datpro.Check3333);
                                break;
                            case 3:
                                RetailDet(Convert.ToDecimal(lc_secsec), IdRetail, pr_datpro.ProCategoria_4, pr_datpro.Check4, pr_datpro.Check44, pr_datpro.Check444, pr_datpro.Check4444);
                                break;
                            case 4:
                                RetailDet(Convert.ToDecimal(lc_secsec), IdRetail, pr_datpro.ProCategoria_5, pr_datpro.Check5, pr_datpro.Check44, pr_datpro.Check555, pr_datpro.Check5555);
                                break;
                        }
                    }
                }



            }
            catch (Exception lc_syserr)
            {

            }
        }

        private static Producto GetDetails(Producto pr_datpro)
        {
            #region VARIABLES LOCALES
            string lc_result = string.Empty;
            string lc_srvpar = string.Empty;
            string lc_auxitem = string.Empty;

            List<Producto> ob_return = new List<Producto>();
            Dictionary<string, object> ob_diclst = new Dictionary<string, object>();

            Secuencia ob_scopre = new Secuencia();
            Producto ob_datpro = new Producto();
            General ob_fncgrl = new General();
            #endregion

            try
            {
                ob_datpro.Codigo = pr_datpro.Codigo;
                ob_datpro.SwtVenta = ob_fncgrl.DecryptStringAES(pr_datpro.SwtVenta);
                ob_datpro.EmailEli = ""; //Session.GetString("Usuario");
                ob_datpro.NombreEli = ""; // Session.GetString("Nombre");
                ob_datpro.KeyTeatro = App.Secuencia;
                ob_datpro.DesTeatro = ""; //Session.GetString("TeatroNombre");
                ob_datpro.TipoCompra = ob_fncgrl.DecryptStringAES(pr_datpro.TipoCompra);
                ob_datpro.ApellidoEli = ""; //Session.GetString("Apellido");
                ob_datpro.TelefonoEli = ""; //Session.GetString("Telefono");
                ob_datpro.KeySecuencia = ob_fncgrl.DecryptStringAES(pr_datpro.KeySecuencia);

                #region SERVICIO SCOPRE
                //Asignar valores PRE
                ob_scopre.Punto = Convert.ToInt32(App.PuntoVenta);
                ob_scopre.Teatro = Convert.ToInt32(App.Secuencia);
                ob_scopre.Tercero = config.Value.ValorTercero;

                //Generar y encriptar JSON para servicio PRE
                lc_srvpar = ob_fncgrl.JsonConverter(ob_scopre);
                lc_srvpar = lc_srvpar.Replace("Teatro", "teatro");
                lc_srvpar = lc_srvpar.Replace("Tercero", "tercero");
                lc_srvpar = lc_srvpar.Replace("punto", "Punto");

                //Encriptar Json PRE
                lc_srvpar = ob_fncgrl.EncryptStringAES(lc_srvpar);

                //Consumir servicio PRE
                if (App.ClienteFrecuente == "No")
                    lc_result = ob_fncgrl.WebServices(string.Concat(App.ScoreServices, "scopre/"), lc_srvpar);
                else
                    lc_result = ob_fncgrl.WebServices(string.Concat(App.ScoreServices, "scopcf/"), lc_srvpar);



                //Validar respuesta
                if (lc_result.Substring(0, 1) == "0")
                {
                    //Quitar switch
                    lc_result = lc_result.Replace("0-", "");
                    ob_diclst = (Dictionary<string, object>)JsonConvert.DeserializeObject(lc_result, (typeof(Dictionary<string, object>)));
                    ob_return = (List<Producto>)JsonConvert.DeserializeObject(ob_diclst["Lista_Productos"].ToString(), (typeof(List<Producto>)));

                    //if (ob_diclst.ContainsKey("Validación"))
                    //    ModelState.AddModelError("", ob_diclst["Validación"].ToString());
                    //else
                    //ViewBag.ListaM = ob_return;
                }
                else
                {
                    lc_result = lc_result.Replace("1-", "");
                    //ModelState.AddModelError("", lc_result);
                }
                #endregion

                //Recorrido por productos para obtener el seleccionado y sus valores
                foreach (var itepro in ob_return)
                {
                    if (itepro.Codigo == ob_datpro.Codigo)
                    {
                        switch (itepro.Tipo)
                        {
                            case "P": //PRODUCTOS
                                ob_datpro.Codigo = itepro.Codigo;
                                ob_datpro.Descripcion = itepro.Descripcion;
                                ob_datpro.Tipo = itepro.Tipo;
                                ob_datpro.Precios = itepro.Precios;
                                break;

                            case "C": //COMBOS
                                ob_datpro.Codigo = itepro.Codigo;
                                ob_datpro.Descripcion = itepro.Descripcion;
                                ob_datpro.Tipo = itepro.Tipo;
                                ob_datpro.Receta = itepro.Receta;
                                break;

                            case "A": //CATEGORIAS
                                ob_datpro.Tipo = itepro.Tipo;
                                ob_datpro.Check = string.Empty;
                                ob_datpro.Codigo = itepro.Codigo;
                                ob_datpro.Descripcion = itepro.Descripcion;

                                List<Receta> ob_recpro = new List<Receta>();
                                List<Precios> ob_prepro = new List<Precios>();
                                List<Producto> ob_lispro = new List<Producto>();
                                List<Pantallas> ob_panpro = new List<Pantallas>();

                                ob_datpro.Receta = ob_recpro;
                                ob_datpro.Precios = ob_prepro;
                                ob_datpro.Pantallas = ob_panpro;
                                ob_datpro.LisProducto = ob_lispro;

                                foreach (var itecat in itepro.Receta)
                                {
                                    Producto ob_itecat = new Producto();

                                    ob_itecat.Tipo = itecat.Tipo;
                                    ob_itecat.Check = string.Empty;
                                    ob_itecat.Codigo = itecat.Codigo;
                                    ob_itecat.Precios = itecat.Precios;
                                    ob_itecat.Cantidad = itecat.Cantidad;
                                    ob_itecat.Descripcion = itecat.Descripcion;

                                    ob_datpro.LisProducto.Add(ob_itecat);
                                }

                                break;
                        }

                        //Romper el ciclo
                        break;
                    }
                }

                //Asignar valores encriptados
                ob_datpro.SwtVenta = pr_datpro.SwtVenta;
                ob_datpro.EmailEli = ob_fncgrl.EncryptStringAES(""); //Session.GetString("Usuario"));
                ob_datpro.NombreEli = ob_fncgrl.EncryptStringAES(""); //(Session.GetString("Nombre"));
                ob_datpro.KeyTeatro = ob_fncgrl.EncryptStringAES(App.Secuencia);
                ob_datpro.DesTeatro = ob_fncgrl.EncryptStringAES(""); //(Session.GetString("TeatroNombre"));
                ob_datpro.TipoCompra = pr_datpro.TipoCompra;
                ob_datpro.ApellidoEli = ob_fncgrl.EncryptStringAES(""); //(Session.GetString("Apellido"));
                ob_datpro.TelefonoEli = ob_fncgrl.EncryptStringAES(""); //(Session.GetString("Telefono"));
                ob_datpro.KeySecuencia = pr_datpro.KeySecuencia;

                return ob_datpro;
            }
            catch (Exception lc_syserr)
            {

                //Devolver vista de error
                ob_datpro.Codigo = -1;
                ob_datpro.Descripcion = lc_syserr.Message;

                return ob_datpro;
            }
        }

        private static void RetailDet(decimal secuencia, int idRetail, decimal categoria, string valor1, string valor2, string valor3, string valor4)
        {
            //Recorrido para guardar valores en base de datos
            for (int lc_variii = 0; lc_variii < 3; lc_variii++)
            {
                using (var context = new DataDB(config))
                {
                    switch (lc_variii)
                    {
                        case 0:
                            if (valor1 != string.Empty && valor1 != null)
                            {
                                //Inicializar instancia de BD


                                //Agregar valores a tabla RetailDet
                                var retailDet = new RetailDet
                                {
                                    Secuencia = secuencia,
                                    IdRetailSales = idRetail,
                                    ProCategoria = categoria,
                                    ProItem = valor1
                                };

                                //Adicionar y guardar registro a tabla
                                context.RetailDet.Add(retailDet);
                                context.SaveChanges();

                            }
                            break;
                        case 1:
                            if (valor2 != string.Empty && valor2 != null)
                            {
                                //Inicializar instancia de BD

                                //Agregar valores a tabla RetailDet
                                var retailDet = new RetailDet
                                {
                                    Secuencia = secuencia,
                                    IdRetailSales = idRetail,
                                    ProCategoria = categoria,
                                    ProItem = valor2
                                };

                                //Adicionar y guardar registro a tabla
                                context.RetailDet.Add(retailDet);
                                context.SaveChanges();

                            }
                            break;
                        case 2:
                            if (valor3 != string.Empty && valor3 != null)
                            {
                                //Inicializar instancia de BD
                                //Agregar valores a tabla RetailDet
                                var retailDet = new RetailDet
                                {
                                    Secuencia = secuencia,
                                    IdRetailSales = idRetail,
                                    ProCategoria = categoria,
                                    ProItem = valor3
                                };

                                //Adicionar y guardar registro a tabla
                                context.RetailDet.Add(retailDet);
                                context.SaveChanges();

                            }
                            break;
                        case 3:
                            if (valor4 != string.Empty && valor4 != null)
                            {
                                //Inicializar instancia de BD

                                //Agregar valores a tabla RetailDet
                                var retailDet = new RetailDet
                                {
                                    Secuencia = secuencia,
                                    IdRetailSales = idRetail,
                                    ProCategoria = categoria,
                                    ProItem = valor4
                                };

                                //Adicionar y guardar registro a tabla
                                context.RetailDet.Add(retailDet);
                                context.SaveChanges();

                            }
                            break;

                    }
                }
            }
        }

        public static void CineFans()
        {
            #region VARIABLES LOCALES
            string lc_result = string.Empty;
            string lc_srvpar = string.Empty;

            List<CinefansDET> ob_cfsdet = new List<CinefansDET>();
            Dictionary<string, string> ob_diclst = new Dictionary<string, string>();

            Login pr_datlog = new Login();
            General ob_fncgrl = new General();
            Cinefans ob_rtacnfs = new Cinefans();
            CinefansSRV ob_servicio = new CinefansSRV();
            CinefansINI ob_cfinicio = new CinefansINI();
            #endregion

            try
            {

                //Asignar Valores
                ob_servicio.Clave = ""; // Session.GetString("Passwrd");
                ob_servicio.Correo = ""; // Session.GetString("Usuario");
                ob_servicio.Fecha1 = Convert.ToString(DateTime.Now.Year - 1) + "0101";
                ob_servicio.Fecha2 = Convert.ToString(DateTime.Now.Year + 1) + "1231";
                ob_servicio.tercero = App.ValorTercero;


                // Generar y encriptar JSON para servicio
                lc_srvpar = ob_fncgrl.JsonConverter(ob_servicio);
                lc_srvpar = lc_srvpar.Replace("correo", "Correo");
                lc_srvpar = lc_srvpar.Replace("clave", "Clave");
                lc_srvpar = lc_srvpar.Replace("fecha1", "Fecha1");
                lc_srvpar = lc_srvpar.Replace("fecha2", "Fecha2");

                //Encriptar Json
                lc_srvpar = ob_fncgrl.EncryptStringAES(lc_srvpar);

                #region SCOMOV
                //Consumir servicio
                lc_result = ob_fncgrl.WebServices(string.Concat(config.Value.ScoreServices, "scomov/"), lc_srvpar);

                //Validar respuesta
                if (lc_result.Substring(0, 1) == "0")
                {
                    //Quitar switch
                    lc_result = lc_result.Replace("0-", "");
                    lc_result = lc_result.Replace("[", "");
                    lc_result = lc_result.Replace("]", "");

                    //Deserializar Json y validar respuesta
                    ob_diclst = (Dictionary<string, string>)JsonConvert.DeserializeObject(lc_result, (typeof(Dictionary<string, string>)));
                    if (ob_diclst.ContainsKey("puntos_acumulados"))
                    {
                        ob_rtacnfs.puntos_vencidos = Convert.ToDecimal(ob_diclst["puntos_vencidos"]);
                        ob_rtacnfs.puntos_redimidos = Convert.ToDecimal(ob_diclst["puntos_redimidos"]);
                        ob_rtacnfs.puntos_acumulados = Convert.ToDecimal(ob_diclst["puntos_acumulados"]);
                        ob_rtacnfs.puntos_disponibles = Convert.ToDecimal(ob_diclst["puntos_disponibles"]);
                    }
                    else
                    {
                        ob_rtacnfs.puntos_vencidos = 0;
                        ob_rtacnfs.puntos_redimidos = 0;
                        ob_rtacnfs.puntos_acumulados = 0;
                        ob_rtacnfs.puntos_disponibles = 0;
                    }
                }
                else
                {
                    //Devolver a vista
                    lc_result = lc_result.Replace("1-", "");
                    MessageBox.Show(lc_result, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                #endregion

                #region SCODES
                //Consumir servicio
                lc_result = ob_fncgrl.WebServices(string.Concat(App.ScoreServices, "scodes/"), lc_srvpar);

                //Validar respuesta
                if (lc_result.Substring(0, 1) == "0")
                {
                    //Quitar switch
                    lc_result = lc_result.Replace("0-[", "[");
                    ob_cfsdet = (List<CinefansDET>)JsonConvert.DeserializeObject(lc_result, (typeof(List<CinefansDET>))); //Deserializar Json y validar respuesta
                }
                else
                {
                    //Devolver a vista
                    lc_result = lc_result.Replace("1-", "");
                    MessageBox.Show(lc_result, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                #endregion

                #region SCOHIS
                //Consumir servicio
                lc_result = ob_fncgrl.WebServices(string.Concat(config.Value.ScoreServices, "scohis/"), lc_srvpar);

                //Validar respuesta
                if (lc_result.Substring(0, 1) == "0")
                {
                    if (!lc_result.Contains("Validación"))
                    {
                        //Quitar switch
                        lc_result = lc_result.Replace("0-{", "{");
                        ob_cfinicio = (CinefansINI)JsonConvert.DeserializeObject(lc_result, (typeof(CinefansINI))); //Deserializar Json y validar respuesta

                        foreach (var item in ob_cfinicio.Saldo)
                            App.Saldo = String.Format("{0:C0}", Convert.ToInt32(item.Saldo));

                    }
                }
                else
                {
                    //Devolver a vista
                    lc_result = lc_result.Replace("1-", "");
                    MessageBox.Show(lc_result, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                #endregion

                #region SCOLOG
                //Asignar valores
                pr_datlog.Correo = ""; // Session.GetString("Usuario");
                pr_datlog.Password = ""; //Session.GetString("Passwrd2");
                pr_datlog.Tercero = App.ValorTercero;

                //Generar y encriptar JSON para servicio
                lc_srvpar = ob_fncgrl.JsonConverter(pr_datlog);
                lc_srvpar = lc_srvpar.Replace("correo", "Correo");
                lc_srvpar = lc_srvpar.Replace("password", "Clave");

                //Encriptar Json
                lc_srvpar = ob_fncgrl.EncryptStringAES(lc_srvpar);

                //Consumir servicio
                lc_result = ob_fncgrl.WebServices(string.Concat(App.ScoreServices, "scolog/"), lc_srvpar);

                //Validar respuesta
                if (lc_result.Substring(0, 1) == "0")
                {
                    //Quitar switch
                    lc_result = lc_result.Replace("0-", "");
                    lc_result = lc_result.Replace("[", "");
                    lc_result = lc_result.Replace("]", "");

                    //Deserializar Json y validar respuesta
                    ob_diclst = (Dictionary<string, string>)JsonConvert.DeserializeObject(lc_result, (typeof(Dictionary<string, string>)));
                    if (ob_diclst.ContainsKey("Validación"))
                    {
                        //Devolver a vista
                        MessageBox.Show(ob_diclst["Validación"].ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    //Devolver a vista
                    lc_result = lc_result.Replace("1-", "");
                    MessageBox.Show(lc_result, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                #endregion
            }
            catch (Exception lc_syserr)
            {
                MessageBox.Show(lc_syserr.Message.Contains("Inner") ? lc_syserr.InnerException.Message : "null", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            }

        }
    }
}