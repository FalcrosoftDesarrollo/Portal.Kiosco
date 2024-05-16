using APIPortalKiosco.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace Portal.Kiosco.Properties.Views
{
    public partial class Combos : Window
    {
        private IConfiguration configuration;
        private int SelectProd = 1;
        private readonly IOptions<MyConfig> config;
        private List<UIElement> elementosCombos = new List<UIElement>();
        private List<UIElement> elementosAlimentos = new List<UIElement>();
        private List<UIElement> elementosBebidas = new List<UIElement>();
        private List<UIElement> elementosSnack = new List<UIElement>();
        private bool isThreadActive = true;

        public Combos()
        {
            InitializeComponent();
            DataContext = ((App)Application.Current);

            if (App.ob_diclst.Count > 0)
            {
                lblnombre.Content = "!HOLA " + App.ob_diclst["Nombre"].ToString() + " " + App.ob_diclst["Apellido"].ToString();
            }
            else
            {
                lblnombre.Content = "!HOLA INVITADO";
            }

            try
            {
                App.ProductosSeleccionados.Clear();
                CombosConsult();
                App.ProductosSeleccionados = new List<Producto>();

                DoubleAnimation fadeInAnimation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.5));
                gridPrincipal.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);
            }
            catch (Exception ex) 
            {
                MessageBox.Show(ex.Message.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

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


        private Border clickedBorder;

        public void ProductList(string pr_secpro, string pr_swtven, string pr_tiplog, string pr_tbview = "", string Teatro = "0", string Ciudad = "0")
        {
            #region VARIABLES LOCALES
            string lc_result = string.Empty;
            string lc_srvpar = string.Empty;
            string lc_auxitem = string.Empty;
            string session = "";
            string secuencia;

            List<Producto> ob_return = new List<Producto>();
            List<Producto> ob_result = new List<Producto>();
            Dictionary<string, object> ob_diclst = new Dictionary<string, object>();

            Secuencia ob_scopre = new Secuencia();
            General ob_fncgrl = new General();
            #endregion

            try
            {
                //Validar redireccion externa
                if (pr_secpro == null)
                    pr_secpro = "0";
                if (pr_swtven == null)
                    pr_swtven = "V";
                if (pr_tiplog == null)
                    pr_tiplog = "P";
                if (pr_tbview == null)
                    pr_tbview = "";

                //Inicializar valores de entrada
                pr_secpro = pr_secpro;
                pr_swtven = pr_swtven;
                pr_tiplog = pr_tiplog;
                pr_tbview = pr_tbview;

                //Session carrito de compras
                //session.Remove("pr_tbview");
                //session.SetString("pr_tbview", pr_tbview);
                //session.Remove("pr_secpro");
                //session.SetString("pr_secpro", pr_secpro);
                //session.Remove("pr_swtven");
                //session.SetString("pr_swtven", pr_swtven);
                //session.Remove("pr_tiplog");
                //session.SetString("pr_tiplog", pr_tiplog);

                //URLPortal(config);

                //Validar ciudad y teatro desde web externa
                //if (Teatro != "0")
                //    Selteatros(Teatro);

                ////Validar seleccion de teatro
                //if (Session.GetString("Teatro") == null)
                //{
                //    //Devolver vista de error
                //    return RedirectToAction("Error", "Pages", new { pr_message = "Debe seleccionar un teatro para continuar", pr_flag = "P" });
                //}


                //ListCarrito();

                //Inicializar variables
                string listaM = null;
                bool alertS = false;
                string cantidadProductos = App.CantProductos;
                //ViewBag.UrlRetailImg = config.Value.UrlRetailImg;
                //ViewBag.ClientFrecnt = Session.GetString("ClienteFrecuente");
                string tipo = pr_tiplog;

                Teatro = session/*.GetString("TeatroNombre")*/;
                string correo = session/*.GetString("Usuario")*/;
                string nombre = session/*.GetString("Nombre")*/;
                string apellido = session/*.GetString("Apellido")*/;
                string telefono = session/*.GetString("Telefoho")*/;
                string KeyTeatro = session/*.GetString("Teatro")*/;

                App.CombosWeb = null;
                App.AlimentosWeb = null;
                App.BebidasWeb = null;
                App.SnacksWeb = null;

                if (session/*.GetString("Secuencia")*/ != null)
                    secuencia = session/*.GetString("Secuencia")*/;
                else
                    secuencia = pr_secpro;

                string switchVenta = pr_swtven;

                #region SERVICIO SCOPRE
                //Asignar valores PRE
                ob_scopre.Punto = Convert.ToInt32(config.Value.PuntoVenta);
                ob_scopre.Teatro = session/*.GetString("Teatro")*/ != "0" ? Convert.ToInt32(session/*.GetString("Teatro")*/) : Convert.ToInt32(session/*.GetString("Teatro")*/);
                ob_scopre.Tercero = config.Value.ValorTercero;

                //Generar y encriptar JSON para servicio PRE
                lc_srvpar = ob_fncgrl.JsonConverter(ob_scopre);
                lc_srvpar = lc_srvpar.Replace("Teatro", "teatro");
                lc_srvpar = lc_srvpar.Replace("Tercero", "tercero");
                lc_srvpar = lc_srvpar.Replace("punto", "Punto");

                //Encriptar Json PRE
                lc_srvpar = ob_fncgrl.EncryptStringAES(lc_srvpar);

                //Consumir servicio PRE
                //if (ViewBag.ClientFrecnt == "No")
                //    lc_result = ob_fncgrl.WebServices(string.Concat(App.ScoreServices, "scopre/"), lc_srvpar);
                //else
                //    lc_result = ob_fncgrl.WebServices(string.Concat(App.ScoreServices, "scopcf/"), lc_srvpar);

                //Generar Log
                //LogSales logSales = new LogSales();
                //LogAudit logAudit = new LogAudit(config);
                //logSales.Id = Guid.NewGuid().ToString();
                //logSales.Fecha = DateTime.Now;
                //logSales.Programa = "SalesCon/ProductList";
                //logSales.Metodo = "GET";
                //logSales.ExceptionMessage = lc_srvpar;
                //logSales.InnerExceptionMessage = lc_result;

                //Escribir Log
                //logAudit.LogApp(logSales);

                //Validar respuesta
                if (lc_result.Substring(0, 1) == "0")
                {
                    //Quitar switch
                    lc_result = lc_result.Replace("0-", "");
                    ob_diclst = (Dictionary<string, object>)JsonConvert.DeserializeObject(lc_result, (typeof(Dictionary<string, object>)));
                    ob_return = (List<Producto>)JsonConvert.DeserializeObject(ob_diclst["Lista_Productos"].ToString(), (typeof(List<Producto>)));

                    if (ob_diclst.ContainsKey("Validación"))
                    {
                        /*ModelState.AddModelError("", ob_diclst["Validación"].ToString())*/
                        ;
                    }
                    else
                    {
                        //Recorrido por objeto para obtener descripcion de receta combos
                        foreach (Producto item in ob_return)
                        {
                            if (item.Tipo == "C")
                            {
                                string fr_axurec = string.Empty;
                                foreach (var receta in item.Receta)
                                    fr_axurec += string.Concat(receta.Cantidad.ToString().Substring(0, receta.Cantidad.ToString().IndexOf(",")), " ", receta.Descripcion, ", ");

                                item.RecetaResumen = fr_axurec.Substring(0, fr_axurec.Length - 2);
                                ob_result.Add(item);
                            }
                            else
                            {
                                ob_result.Add(item);
                            }
                        }

                        //Recorrido por objeto para obtener el orden de pantallas y mostrar en vista
                        List<Producto> CombosWeb = new List<Producto>();
                        List<Producto> AlimentosWeb = new List<Producto>();
                        List<Producto> BebidasWeb = new List<Producto>();
                        List<Producto> SnacksWeb = new List<Producto>();
                        foreach (Producto item in ob_return)
                        {
                            //Recorrido por pantallas
                            foreach (var pantallas in item.Pantallas)
                            {
                                switch (pantallas.Descripcion)
                                {
                                    case "COMBOS WEB":
                                        int lc_cntcom = CombosWeb.Count();

                                        CombosWeb.Add(item);
                                        CombosWeb[lc_cntcom].OrdenView = pantallas.Orden;
                                        CombosWeb[lc_cntcom].Descripcion_Web = pantallas.Descripcion_Web;
                                        CombosWeb[lc_cntcom].Flag = pantallas.Flag;
                                        break;

                                    case "ALIMENTOS WEB":
                                        int lc_cntali = AlimentosWeb.Count();

                                        AlimentosWeb.Add(item);
                                        AlimentosWeb[lc_cntali].OrdenView = pantallas.Orden;
                                        AlimentosWeb[lc_cntali].Descripcion_Web = pantallas.Descripcion_Web;
                                        AlimentosWeb[lc_cntali].Flag = pantallas.Flag;
                                        break;

                                    case "BEBIDAS WEB":
                                        int lc_cntbeb = BebidasWeb.Count();

                                        BebidasWeb.Add(item);
                                        BebidasWeb[lc_cntbeb].OrdenView = pantallas.Orden;
                                        BebidasWeb[lc_cntbeb].Descripcion_Web = pantallas.Descripcion_Web;
                                        BebidasWeb[lc_cntbeb].Flag = pantallas.Flag;
                                        break;

                                    case "SNACKS WEB":
                                        int lc_cntsnk = SnacksWeb.Count();

                                        SnacksWeb.Add(item);
                                        SnacksWeb[lc_cntsnk].OrdenView = pantallas.Orden;
                                        SnacksWeb[lc_cntsnk].Descripcion_Web = pantallas.Descripcion_Web;
                                        SnacksWeb[lc_cntsnk].Flag = pantallas.Flag;
                                        break;
                                }
                            }
                        }

                        //Validar productos a mostrar combos
                        if (pr_tbview == "" || pr_tbview == "tab-combos")
                            CrearCombosYbebidas(CombosWeb.OrderBy(o => o.OrdenView).ToList());

                        if (pr_tbview == "tab-alimentos")
                            CrearCombosYbebidas(App.AlimentosWeb.OrderBy(o => o.OrdenView).ToList());
                        //Validar productos a mostrar bebidas
                        if (pr_tbview == "tab-bebidas")
                            CrearCombosYbebidas(App.BebidasWeb.OrderBy(o => o.OrdenView).ToList());

                        //Validar productos a mostrar snacks
                        if (pr_tbview == "tab-snacks")
                            CrearCombosYbebidas(App.SnacksWeb.OrderBy(o => o.OrdenView).ToList());
                    }
                }
                else
                {
                    lc_result = lc_result.Replace("1-", "");
                    /* ModelState.AddModelError("", lc_result)*/
                    ;
                }
                #endregion

                //Devolver a vista
                //return View();
            }
            catch (Exception lc_syserr)
            {
                ////Generar Log
                //LogSales logSales = new LogSales();
                //LogAudit logAudit = new LogAudit(config);
                //logSales.Id = Guid.NewGuid().ToString();
                //logSales.Fecha = DateTime.Now;
                //logSales.Programa = "SalesCon/ProductList";
                //logSales.Metodo = "GET";
                //logSales.ExceptionMessage = lc_syserr.Message;
                //logSales.InnerExceptionMessage = logSales.ExceptionMessage.Contains("Inner") ? lc_syserr.InnerException.Message : "null";

                ////Escribir Log
                //logAudit.LogApp(logSales);

                ////Devolver vista de error
                //return RedirectToAction("Error", "Pages", new { pr_message = config.Value.MessageException + logSales.Id });
            }
        }

        public void AddProduct(string pr_secpro, string pr_swtven, string pr_tiplog, string pr_tbview = "")
        {
            #region VARIABLES LOCALES
            string lc_result = string.Empty;
            string lc_srvpar = string.Empty;
            string lc_auxitem = string.Empty;
            string session = "";
            string secuencia;

            List<Producto> ob_return = new List<Producto>();
            List<Producto> ob_result = new List<Producto>();
            Dictionary<string, object> ob_diclst = new Dictionary<string, object>();

            Secuencia ob_scopre = new Secuencia();
            General ob_fncgrl = new General();
            #endregion

            try
            {
                //Inicializar valores de entrada
                pr_secpro = pr_secpro;
                pr_swtven = pr_swtven;
                pr_tiplog = pr_tiplog;
                pr_tbview = pr_tbview;

                //URLPortal(config);
                //ListCarrito();

                //Inicializar variables
                //ViewBag.ListaM = null;
                bool alertS = false;
                string cantidadProductos = App.CantProductos;
                //ViewBag.UrlRetailImg = config.Value.UrlRetailImg;
                string clientFrecnt = session/*.GetString("ClienteFrecuente")*/;
                string tipo = pr_tiplog;

                string teatro = session/*.GetString("TeatroNombre")*/;
                string Correo = session/*.GetString("Usuario")*/;
                string nombre = session/*.GetString("Nombre")*/;
                string apellido = session/*.GetString("Apellido")*/;
                string telefono = session/*.GetString("Telefoho")*/;
                string keyTeatro = session/*.GetString("Teatro")*/;

                App.CombosWeb = null;
                App.AlimentosWeb = null;
                App.BebidasWeb = null;
                App.SnacksWeb = null;

                //ViewBag.ClientFrecnt = "No";

                if (session/*.GetString("Secuencia")*/ != null)
                    secuencia = session/*.GetString("Secuencia")*/;
                else
                    secuencia = pr_secpro;

                string switchVenta = pr_swtven;

                #region SERVICIO SCOPRE
                //Asignar valores PRE
                ob_scopre.Punto = Convert.ToInt32(config.Value.PuntoVenta);
                ob_scopre.Teatro = session/*.GetString("Teatro")*/ != "0" ? Convert.ToInt32(session/*.GetString("Teatro")*/) : Convert.ToInt32(session/*.GetString("Teatro")*/);
                ob_scopre.Tercero = config.Value.ValorTercero;

                //Generar y encriptar JSON para servicio PRE
                lc_srvpar = ob_fncgrl.JsonConverter(ob_scopre);
                lc_srvpar = lc_srvpar.Replace("Teatro", "teatro");
                lc_srvpar = lc_srvpar.Replace("Tercero", "tercero");
                lc_srvpar = lc_srvpar.Replace("punto", "Punto");

                //Encriptar Json PRE
                lc_srvpar = ob_fncgrl.EncryptStringAES(lc_srvpar);

                //Consumir servicio PRE
                //if (ViewBag.ClientFrecnt == "No")
                //    lc_result = ob_fncgrl.WebServices(string.Concat(config.Value.ScoreServices, "scopre/"), lc_srvpar);
                //else
                //    lc_result = ob_fncgrl.WebServices(string.Concat(config.Value.ScoreServices, "scopcf/"), lc_srvpar);

                ////Generar Log
                //LogSales logSales = new LogSales();
                //LogAudit logAudit = new LogAudit(config);
                //logSales.Id = Guid.NewGuid().ToString();
                //logSales.Fecha = DateTime.Now;
                //logSales.Programa = "SalesCon/Addproduct";
                //logSales.Metodo = "SCOPRE";
                //logSales.ExceptionMessage = lc_srvpar;
                //logSales.InnerExceptionMessage = lc_result;

                ////Escribir Log
                //logAudit.LogApp(logSales);

                //Validar respuesta
                if (lc_result.Substring(0, 1) == "0")
                {
                    //Quitar switch
                    lc_result = lc_result.Replace("0-", "");
                    ob_diclst = (Dictionary<string, object>)JsonConvert.DeserializeObject(lc_result, (typeof(Dictionary<string, object>)));
                    ob_return = (List<Producto>)JsonConvert.DeserializeObject(ob_diclst["Lista_Productos"].ToString(), (typeof(List<Producto>)));

                    if (ob_diclst.ContainsKey("Validación"))
                    {
                        //ModelState.AddModelError("", ob_diclst["Validación"].ToString());
                    }
                    else
                    {
                        //Recorrido por objeto para obtener descripcion de receta combos
                        foreach (Producto item in ob_return)
                        {
                            if (item.Tipo == "C")
                            {
                                string fr_axurec = string.Empty;
                                foreach (var receta in item.Receta)
                                    fr_axurec += string.Concat(receta.Cantidad.ToString().Substring(0, receta.Cantidad.ToString().IndexOf(",")), " ", receta.Descripcion, ", ");

                                item.RecetaResumen = fr_axurec.Substring(0, fr_axurec.Length - 2);
                                ob_result.Add(item);
                            }
                            else
                            {
                                ob_result.Add(item);
                            }
                        }

                        //Recorrido por objeto para obtener el orden de pantallas y mostrar en vista
                        List<Producto> CombosWeb = new List<Producto>();
                        List<Producto> AlimentosWeb = new List<Producto>();
                        List<Producto> BebidasWeb = new List<Producto>();
                        List<Producto> SnacksWeb = new List<Producto>();
                        List<Producto> AdicionesWeb = new List<Producto>();

                        foreach (Producto item in ob_return)
                        {
                            //Recorrido por pantallas
                            foreach (var pantallas in item.Pantallas)
                            {
                                switch (pantallas.Descripcion)
                                {
                                    case "COMBOS WEB":
                                        int lc_cntcom = CombosWeb.Count();

                                        CombosWeb.Add(item);
                                        CombosWeb[lc_cntcom].OrdenView = pantallas.Orden;
                                        break;

                                    case "ALIMENTOS WEB":
                                        int lc_cntali = AlimentosWeb.Count();

                                        AlimentosWeb.Add(item);
                                        AlimentosWeb[lc_cntali].OrdenView = pantallas.Orden;
                                        break;

                                    case "BEBIDAS WEB":
                                        int lc_cntbeb = BebidasWeb.Count();

                                        BebidasWeb.Add(item);
                                        BebidasWeb[lc_cntbeb].OrdenView = pantallas.Orden;
                                        break;

                                    case "SNACKS WEB":
                                        int lc_cntsnk = SnacksWeb.Count();

                                        SnacksWeb.Add(item);
                                        SnacksWeb[lc_cntsnk].OrdenView = pantallas.Orden;
                                        break;

                                    case "ADICIONES WEB":
                                        int lc_cntadd = AdicionesWeb.Count();

                                        AdicionesWeb.Add(item);
                                        AdicionesWeb[lc_cntadd].OrdenView = pantallas.Orden;
                                        break;
                                }
                            }
                        }

                        //Validar productos a mostrar combos
                        if (pr_tbview == "" || pr_tbview == "tab-combos")
                            CrearCombosYbebidas(CombosWeb.OrderBy(o => o.OrdenView).ToList());

                        if (pr_tbview == "tab-alimentos")
                            CrearCombosYbebidas(App.AlimentosWeb.OrderBy(o => o.OrdenView).ToList());
                        //Validar productos a mostrar bebidas
                        if (pr_tbview == "tab-bebidas")
                            CrearCombosYbebidas(App.BebidasWeb.OrderBy(o => o.OrdenView).ToList());

                        //Validar productos a mostrar snacks
                        if (pr_tbview == "tab-snacks")
                            CrearCombosYbebidas(App.SnacksWeb.OrderBy(o => o.OrdenView).ToList());

                        //Asignar lista de adiciones
                        //ViewBag.ListaA = AdicionesWeb.OrderBy(o => o.OrdenView).ToList();
                    }
                }
                else
                {
                    lc_result = lc_result.Replace("1-", "");
                    //ModelState.AddModelError("", lc_result);
                }
                #endregion

                //Devolver a vista
                //return View();
            }
            catch (Exception lc_syserr)
            {
                //Generar Log
                //LogSales logSales = new LogSales();
                //LogAudit logAudit = new LogAudit(config);
                //logSales.Id = Guid.NewGuid().ToString();
                //logSales.Fecha = DateTime.Now;
                //logSales.Programa = "SalesCon/Addproduct";
                //logSales.Metodo = "GET";
                //logSales.ExceptionMessage = lc_syserr.Message;
                //logSales.InnerExceptionMessage = logSales.ExceptionMessage.Contains("Inner") ? lc_syserr.InnerException.Message : "null";

                ////Escribir Log
                //logAudit.LogApp(logSales);

                ////Devolver vista de error
                //return RedirectToAction("Error", "Pages", new { pr_message = config.Value.MessageException + logSales.Id });
            }
        }

        public void Details(string pr_keypro, string pr_secpro, string pr_swtven, string pr_tiplog, string pr_swtadd)
        {
            #region VARIABLES LOCALES
            string lc_result = string.Empty;
            string lc_srvpar = string.Empty;
            string lc_auxitem = string.Empty;
            int CodigoBebidas = 1244;
            int CodigoBebidas2 = 2444;
            int CodigoComidas = 246;
            string session = "";
            List<(decimal CodigoBotella, string NombreFinalBotella, decimal PrecioFinalBotella, string frecuenciaBotella, decimal categoria)> datosFinalesBotella = new List<(decimal, string, decimal, string, decimal)>();
            List<(decimal CodigoComida, string NombreFinalComida, decimal PrecioFinalComida, string frecuenciaComida, decimal categoria)> datosFinalesComida = new List<(decimal, string, decimal, string, decimal)>();

            List<Producto> ob_return = new List<Producto>();
            Dictionary<string, object> ob_diclst = new Dictionary<string, object>();

            Secuencia ob_scopre = new Secuencia();
            Producto ob_datpro = new Producto();
            General ob_fncgrl = new General();
            #endregion

            //Inicializar variables
            //ViewBag.ListaM = null;
            bool alertS = false;
            //ViewBag.UrlRetailImg = config.Value.UrlRetailImg;
            string CantidadProductos = App.CantProductos;
            string secuencia = pr_secpro;
            string switchVenta = pr_swtven;
            string tipo = pr_tiplog;
            string switchAdd = pr_swtadd;
            //ViewBag.ListaB = null;
            //ViewBag.ListaC = null;

            //Session carrito de compras
            //Session.Remove("pr_keypro");
            //Session.SetString("pr_keypro", pr_keypro);
            //Session.Remove("pr_secpro");
            //Session.SetString("pr_secpro", pr_secpro);
            //Session.Remove("pr_swtven");
            //Session.SetString("pr_swtven", pr_swtven);
            //Session.Remove("pr_tiplog");
            //Session.SetString("pr_tiplog", pr_tiplog);

            try
            {
                //URLPortal(config);
                //ListCarrito();

                //Validar inicio de sesión
                if (session/*.GetString("Usuario")*/ == null)
                    //return RedirectToAction("Error", "Pages", new { pr_message = "Se debe iniciar Sesión para Continuar", pr_flag = "PX" });

                    //ViewBag.ClientFrecnt = Session.GetString("ClienteFrecuente"); //"No;"

                    ob_datpro.Codigo = Convert.ToDecimal(pr_keypro);
                ob_datpro.SwtVenta = pr_swtven;
                ob_datpro.EmailEli = session/*.GetString("Usuario")*/;
                ob_datpro.NombreEli = session/*.GetString("Nombre")*/;
                ob_datpro.KeyTeatro = session/*.GetString("Teatro")*/;
                ob_datpro.DesTeatro = session/*.GetString("TeatroNombre")*/;
                ob_datpro.ApellidoEli = session/*.GetString("Apellido")*/;
                ob_datpro.TelefonoEli = session/*.GetString("Telefono")*/;
                ob_datpro.KeySecuencia = pr_secpro;

                #region SERVICIO SCOPRE
                //Asignar valores PRE
                ob_scopre.Punto = Convert.ToInt32(config.Value.PuntoVenta);
                ob_scopre.Teatro = Convert.ToInt32(ob_datpro.KeyTeatro);
                ob_scopre.Tercero = config.Value.ValorTercero;

                //Generar y encriptar JSON para servicio PRE
                lc_srvpar = ob_fncgrl.JsonConverter(ob_scopre);
                lc_srvpar = lc_srvpar.Replace("Teatro", "teatro");
                lc_srvpar = lc_srvpar.Replace("Tercero", "tercero");
                lc_srvpar = lc_srvpar.Replace("punto", "Punto");

                //Encriptar Json PRE
                lc_srvpar = ob_fncgrl.EncryptStringAES(lc_srvpar);

                //Consumir servicio PRE
                //if (ViewBag.ClientFrecnt == "No")
                //    lc_result = ob_fncgrl.WebServices(string.Concat(config.Value.ScoreServices, "scopre/"), lc_srvpar);
                //else
                //    lc_result = ob_fncgrl.WebServices(string.Concat(config.Value.ScoreServices, "scopcf/"), lc_srvpar);

                ////Generar Log
                //LogSales logSales = new LogSales();
                //LogAudit logAudit = new LogAudit(config);
                //logSales.Id = Guid.NewGuid().ToString();
                //logSales.Fecha = DateTime.Now;
                //logSales.Programa = "SalesCon/Details";
                //logSales.Metodo = "SCOPRE";
                //logSales.ExceptionMessage = lc_srvpar;
                //logSales.InnerExceptionMessage = lc_result;

                //Escribir Log
                //logAudit.LogApp(logSales);

                //Validar respuesta
                if (lc_result.Substring(0, 1) == "0")
                {
                    //Quitar switch
                    lc_result = lc_result.Replace("0-", "");
                    ob_diclst = (Dictionary<string, object>)JsonConvert.DeserializeObject(lc_result, (typeof(Dictionary<string, object>)));
                    ob_return = (List<Producto>)JsonConvert.DeserializeObject(ob_diclst["Lista_Productos"].ToString(), (typeof(List<Producto>)));

                    if (ob_diclst.ContainsKey("Validación"))
                    {
                        //ModelState.AddModelError("", ob_diclst["Validación"].ToString());
                    }
                    else
                    {
                        //Recorrido por objeto para obtener el orden de pantallas y mostrar en vista
                        List<Producto> CombosWeb = new List<Producto>();
                        List<Producto> AlimentosWeb = new List<Producto>();
                        List<Producto> BebidasWeb = new List<Producto>();
                        List<Producto> SnacksWeb = new List<Producto>();
                        foreach (Producto item in ob_return)
                        {
                            //Recorrido por pantallas
                            foreach (var pantallas in item.Pantallas)
                            {
                                switch (pantallas.Descripcion)
                                {
                                    case "COMBOS WEB":
                                        int lc_cntcom = App.CombosWeb.Count;

                                        App.CombosWeb.Add(item);
                                        App.CombosWeb[lc_cntcom].OrdenView = pantallas.Orden;
                                        break;

                                    case "ALIMENTOS WEB":
                                        int lc_cntali = App.AlimentosWeb.Count;

                                        App.AlimentosWeb.Add(item);
                                        App.AlimentosWeb[lc_cntali].OrdenView = pantallas.Orden;
                                        break;

                                    case "BEBIDAS WEB":
                                        int lc_cntbeb = App.BebidasWeb.Count;

                                        App.BebidasWeb.Add(item);
                                        App.BebidasWeb[lc_cntbeb].OrdenView = pantallas.Orden;
                                        break;

                                    case "SNACKS WEB":
                                        int lc_cntsnk = App.SnacksWeb.Count;

                                        App.SnacksWeb.Add(item);
                                        App.SnacksWeb[lc_cntsnk].OrdenView = pantallas.Orden;
                                        break;
                                }
                            }
                        }

                        //ViewBag.ListaM = CombosWeb.OrderBy(o => o.OrdenView).ToList();
                    }
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
                                ob_datpro.Precios = itepro.Precios;
                                List<Precios> precio_Combo = new List<Precios>();
                                ob_datpro.Precios = precio_Combo;

                                bool condicionCumplida = false;

                                foreach (var itecat in itepro.Receta)
                                {
                                    var precio_Combo_Receta = itecat.Codigo;
                                    if (precio_Combo_Receta == CodigoBebidas || precio_Combo_Receta == CodigoBebidas2)
                                    {
                                        foreach (var i in itecat.RecetaCategoria)
                                        {
                                            var CodioBotella = i.Codigo;
                                            var NombreFinalBotella = i.Descripcion.ToString();
                                            var precioFinalBotella = i.Precios.Sum(precio => precio.General);
                                            var frecuenciaBotella = i.Frecuente.ToString();
                                            // Hacer algo con precioFinalCombo
                                            datosFinalesBotella.Add((CodioBotella, NombreFinalBotella, precioFinalBotella, frecuenciaBotella, itecat.Codigo));
                                        }
                                    }
                                    else if (precio_Combo_Receta == CodigoComidas)
                                    {
                                        foreach (var i in itecat.RecetaCategoria)
                                        {
                                            var CodioComida = i.Codigo;
                                            var NombreFinalComida = i.Descripcion.ToString();
                                            var precioFinalComida = i.Precios.Sum(precio => precio.General);
                                            var frecuenciaComida = i.Frecuente.ToString();

                                            // Hacer algo con precioFinalCombo
                                            datosFinalesComida.Add((CodioComida, NombreFinalComida, precioFinalComida, frecuenciaComida, itecat.Codigo));
                                        }
                                    }
                                    //Valido que las listas se hayan llenado
                                    if (datosFinalesBotella.Count > 0 && datosFinalesComida.Count > 0)
                                    {
                                        // Establecer el indicador para salir del bucle
                                        condicionCumplida = true;
                                    }
                                    // Si se cumplió la condición, salir del bucle foreach
                                    if (condicionCumplida)
                                    {
                                        break;
                                    }
                                }

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
                //ViewBag.ListaB = datosFinalesBotella.Distinct().ToList();
                //ViewBag.ListaC = datosFinalesComida.Distinct().ToList();
                //Asignar valores encriptados
                ob_datpro.SwtVenta = pr_swtven;
                ob_datpro.EmailEli = session/*.GetString("Usuario")*/;
                ob_datpro.NombreEli = session/*.GetString("Nombre")*/;
                ob_datpro.KeyTeatro = session/*.GetString("Teatro")*/;
                ob_datpro.DesTeatro = session/*.GetString("TeatroNombre")*/;
                ob_datpro.TipoCompra = pr_tiplog;
                ob_datpro.ApellidoEli = session/*.GetString("Apellido")*/;
                ob_datpro.TelefonoEli = session/*.GetString("Telefono")*/;
                ob_datpro.KeySecuencia = pr_secpro;

                //return View(ob_datpro);
            }
            catch (Exception lc_syserr)
            {
                //Generar Log
                //LogSales logSales = new LogSales();
                //LogAudit logAudit = new LogAudit(config);
                //logSales.Id = Guid.NewGuid().ToString();
                //logSales.Fecha = DateTime.Now;
                //logSales.Programa = "SalesCon/Details";
                //logSales.Metodo = "GET";
                //logSales.ExceptionMessage = lc_syserr.Message;
                //logSales.InnerExceptionMessage = logSales.ExceptionMessage.Contains("Inner") ? lc_syserr.InnerException.Message : "null";

                ////Escribir Log
                //logAudit.LogApp(logSales);

                ////Devolver vista de error
                //return RedirectToAction("Error", "Pages", new { pr_message = config.Value.MessageException + logSales.Id });
            }
        }

        //public ActionResult Details(Producto pr_datpro)
        //{
        //    #region VARIABLES LOCALES
        //    string lc_result = string.Empty;
        //    string lc_srvpar = string.Empty;
        //    string lc_auxite = string.Empty;
        //    string lc_secsec = string.Empty;

        //    List<Producto> ob_return = new List<Producto>();
        //    Dictionary<string, object> ob_diclst = new Dictionary<string, object>();
        //    Dictionary<string, string> ob_seclst = new Dictionary<string, string>();

        //    Secuencia ob_secsec = new Secuencia();
        //    Secuencia ob_scopre = new Secuencia();
        //    Producto ob_datpro = new Producto();
        //    General ob_fncgrl = new General();
        //    #endregion

        //    try
        //    {
        //        URLPortal(config);
        //        ListCarrito();

        //        ViewBag.ClientFrecnt = Session.GetString("ClienteFrecuente"); //"No";

        //        #region SERVICIO SCOSEC
        //        lc_secsec = pr_datpro.KeySecuencia;
        //        if (lc_secsec == "0")
        //        {
        //            //Asignar valores SEC
        //            ob_secsec.Punto = Convert.ToInt32(config.Value.PuntoVenta);
        //            ob_secsec.Teatro = Convert.ToInt32(Session.GetString("Teatro"));
        //            ob_secsec.Tercero = config.Value.ValorTercero;

        //            //Generar y encriptar JSON para servicio SEC
        //            lc_srvpar = ob_fncgrl.JsonConverter(ob_secsec);
        //            lc_srvpar = lc_srvpar.Replace("Teatro", "teatro");
        //            lc_srvpar = lc_srvpar.Replace("Tercero", "tercero");
        //            lc_srvpar = lc_srvpar.Replace("punto", "Punto");

        //            //Encriptar Json SEC
        //            lc_srvpar = ob_fncgrl.EncryptStringAES(lc_srvpar);

        //            //Consumir servicio SEC
        //            lc_result = ob_fncgrl.WebServices(string.Concat(config.Value.ScoreServices, "scosec/"), lc_srvpar);

        //            //Generar Log
        //            LogSales logSales = new LogSales();
        //            LogAudit logAudit = new LogAudit(config);
        //            logSales.Id = Guid.NewGuid().ToString();
        //            logSales.Fecha = DateTime.Now;
        //            logSales.Programa = "SalesCon/Details";
        //            logSales.Metodo = "SCOSEC";
        //            logSales.ExceptionMessage = lc_srvpar;
        //            logSales.InnerExceptionMessage = lc_result;

        //            //Escribir Log
        //            //logAudit.LogApp(logSales);

        //            //Validar respuesta
        //            if (lc_result.Substring(0, 1) == "0")
        //            {
        //                //Quitar switch
        //                lc_result = lc_result.Replace("0-", "");
        //                lc_result = lc_result.Replace("[", "");
        //                lc_result = lc_result.Replace("]", "");

        //                //Deserializar Json y validar respuesta SEC
        //                ob_seclst = (Dictionary<string, string>)JsonConvert.DeserializeObject(lc_result, (typeof(Dictionary<string, string>)));

        //                //Validar respuesta del servicio
        //                if (ob_seclst.ContainsKey("Validación"))
        //                    ModelState.AddModelError("", ob_seclst["Validación"].ToString());
        //                else
        //                {
        //                    lc_secsec = ob_seclst["Secuencia"].ToString().Substring(0, ob_seclst["Secuencia"].ToString().IndexOf("."));
        //                    Session.SetString("Secuencia", lc_secsec);
        //                }


        //                //Limpiar objeto
        //                ob_seclst.Clear();
        //            }
        //            else
        //            {
        //                //Devolver a vista
        //                lc_result = lc_result.Replace("1-", "");
        //                ModelState.AddModelError("", lc_result);
        //                return View(new { pr_keypro = pr_datpro.Codigo.ToString(), pr_secpro = pr_datpro.KeySecuencia, pr_keytea = pr_datpro.KeyTeatro, pr_destea = pr_datpro.DesTeatro, pr_swtven = pr_datpro.SwtVenta, pr_tiplog = pr_datpro.TipoCompra, pr_coreli = pr_datpro.EmailEli, pr_nomeli = pr_datpro.NombreEli, pr_apeeli = pr_datpro.ApellidoEli, pr_teleli = pr_datpro.TelefonoEli });
        //            }
        //        }
        //        #endregion

        //        //Validar Categoria 
        //        if (pr_datpro.Tipo == "A")
        //        {
        //            if (pr_datpro.Check != null)
        //            {
        //                pr_datpro.ProCategoria_1 = Convert.ToDecimal(pr_datpro.Check);
        //                pr_datpro.Check1 = "0";
        //                pr_datpro.Check2 = "0";
        //                pr_datpro.Check3 = "0";
        //                pr_datpro.Check4 = "0";
        //                pr_datpro.Check5 = "0";

        //                pr_datpro.CanCategoria_1 = pr_datpro.Cantidad;
        //                pr_datpro.Cantidad1 = 0;
        //                pr_datpro.Cantidad2 = 0;
        //                pr_datpro.Cantidad3 = 0;
        //                pr_datpro.Cantidad4 = 0;
        //                pr_datpro.Cantidad5 = 0;
        //            }
        //            else
        //            {
        //                //Obtener detalle del producto seleccionado
        //                ob_datpro = GetDetails(pr_datpro);
        //                if (ob_datpro.Codigo == -1)
        //                {
        //                    //Cargar mensaje de error
        //                    lc_auxite = ob_datpro.Descripcion;
        //                    ModelState.AddModelError("", lc_auxite);

        //                    //Devolver a vista
        //                    ob_datpro.Codigo = pr_datpro.Codigo;
        //                    ob_datpro.Descripcion = pr_datpro.Descripcion;
        //                    return View(ob_datpro);
        //                }
        //                else
        //                {
        //                    //Devolver a vista
        //                    ModelState.AddModelError("", "Debe seleccionar un ítem de la categoría para continuar");
        //                    return View(ob_datpro);
        //                }
        //            }
        //        }

        //        //Inicializar instancia de BD
        //        using (var context = new DataDB(config))
        //        {
        //            //Agregar valores a tabla ReportSales
        //            var retailSales = new RetailSales
        //            {
        //                Tipo = pr_datpro.Tipo,
        //                Precio = Convert.ToDecimal(pr_datpro.Valor),
        //                Cantidad = pr_datpro.Cantidad,
        //                Secuencia = Convert.ToDecimal(lc_secsec),
        //                PuntoVenta = Convert.ToDecimal(config.Value.PuntoVenta),
        //                KeyProducto = pr_datpro.Codigo,
        //                Descripcion = pr_datpro.Descripcion,
        //                ProProducto1 = pr_datpro.ProProducto_1,
        //                ProProducto2 = pr_datpro.ProProducto_2,
        //                ProProducto3 = pr_datpro.ProProducto_3,
        //                ProProducto4 = pr_datpro.ProProducto_4,
        //                ProProducto5 = pr_datpro.ProProducto_5,
        //                CanProducto1 = pr_datpro.ProCantidad_1,
        //                CanProducto2 = pr_datpro.ProCantidad_2,
        //                CanProducto3 = pr_datpro.ProCantidad_3,
        //                CanProducto4 = pr_datpro.ProCantidad_4,
        //                CanProducto5 = pr_datpro.ProCantidad_5,
        //                ProCategoria1 = pr_datpro.ProCategoria_1,
        //                ProCategoria2 = pr_datpro.ProCategoria_2,
        //                ProCategoria3 = pr_datpro.ProCategoria_3,
        //                ProCategoria4 = pr_datpro.ProCategoria_4,
        //                ProCategoria5 = pr_datpro.ProCategoria_5,
        //                CanCategoria1 = pr_datpro.CanCategoria_1,
        //                CanCategoria2 = pr_datpro.CanCategoria_2,
        //                CanCategoria3 = pr_datpro.CanCategoria_3,
        //                CanCategoria4 = pr_datpro.CanCategoria_4,
        //                CanCategoria5 = pr_datpro.CanCategoria_5,
        //                FechaRegistro = DateTime.Now,
        //                KeyTeatro = Convert.ToDecimal(Session.GetString("Teatro")),
        //                SwitchAdd = pr_datpro.SwitchAdd
        //            };

        //            //Adicionar y guardar registro a tabla
        //            context.RetailSales.Add(retailSales);
        //            context.SaveChanges();
        //        }

        //        //Validar Combo
        //        if (pr_datpro.Tipo == "C")
        //        {
        //            //Obtener id de prodcutos de combo
        //            int IdRetail = 0;
        //            using (var context = new DataDB(config))
        //                IdRetail = context.RetailSales.Max(u => u.Id);

        //            //Recorrer la cantidad maxima de categorias por combo
        //            for (int lc_variii = 0; lc_variii < 4; lc_variii++)
        //            {
        //                switch (lc_variii)
        //                {
        //                    case 0:
        //                        RetailDet(Convert.ToDecimal(lc_secsec), IdRetail, pr_datpro.ProCategoria_1, pr_datpro.Check1, pr_datpro.Check11, pr_datpro.Check111, pr_datpro.Check1111);
        //                        break;
        //                    case 1:
        //                        RetailDet(Convert.ToDecimal(lc_secsec), IdRetail, pr_datpro.ProCategoria_2, pr_datpro.Check2, pr_datpro.Check22, pr_datpro.Check222, pr_datpro.Check2222);
        //                        break;
        //                    case 2:
        //                        RetailDet(Convert.ToDecimal(lc_secsec), IdRetail, pr_datpro.ProCategoria_3, pr_datpro.Check3, pr_datpro.Check33, pr_datpro.Check333, pr_datpro.Check3333);
        //                        break;
        //                    case 3:
        //                        RetailDet(Convert.ToDecimal(lc_secsec), IdRetail, pr_datpro.ProCategoria_4, pr_datpro.Check4, pr_datpro.Check44, pr_datpro.Check444, pr_datpro.Check4444);
        //                        break;
        //                    case 4:
        //                        RetailDet(Convert.ToDecimal(lc_secsec), IdRetail, pr_datpro.ProCategoria_5, pr_datpro.Check5, pr_datpro.Check44, pr_datpro.Check555, pr_datpro.Check5555);
        //                        break;
        //                }
        //            }
        //        }

        //        //Devolver a vista
        //        return RedirectToAction("ProductList", "SalesCon", new { pr_secpro = lc_secsec.ToString(), pr_swtven = "V", pr_tiplog = pr_datpro.TipoCompra });

        //    }
        //    catch (Exception lc_syserr)
        //    {
        //        //Generar Log
        //        LogSales logSales = new LogSales();
        //        LogAudit logAudit = new LogAudit(config);
        //        logSales.Id = Guid.NewGuid().ToString();
        //        logSales.Fecha = DateTime.Now;
        //        logSales.Programa = "SalesCon/Details";
        //        logSales.Metodo = "POST";
        //        logSales.ExceptionMessage = lc_syserr.Message;
        //        logSales.InnerExceptionMessage = logSales.ExceptionMessage.Contains("Inner") ? lc_syserr.InnerException.Message : "null";

        //        //Escribir Log
        //        logAudit.LogApp(logSales);

        //        //Devolver vista de error
        //        return RedirectToAction("Error", "Pages", new { pr_message = config.Value.MessageException + logSales.Id });
        //    }
        //}

        private void tapAlimentos_Click(object sender, RoutedEventArgs e)
        {
            RestoreButtonState();

            Button clickedButton = sender as Button;

            clickedBorder = GetButtonBorder(clickedButton);

            clickedButton.Foreground = new SolidColorBrush(Colors.White);
            clickedButton.Background = new SolidColorBrush(Colors.Red);
            clickedBorder.Background = new SolidColorBrush(Colors.Red);
        }

        private Border GetButtonBorder(Button button)
        {
            DependencyObject parent = VisualTreeHelper.GetParent(button);

            while (parent != null && !(parent is Border))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }

            switch (button.Name)
            {
                case "tabcombos":
                    if (elementosCombos.Count == 0)
                    {
                        CrearCombosYbebidas(App.CombosWeb.OrderBy(o => o.OrdenView).ToList());
                        elementosCombos.Clear();

                        foreach (UIElement elemento in imagenes.Children)
                        {
                            elementosCombos.Add(elemento);
                        }
                    }
                    else
                    {
                        imagenes.Children.Clear();

                        foreach (UIElement elemento in elementosCombos)
                        {
                            imagenes.Children.Add(elemento);
                        }
                    }
                    SelectProd = 1;
                    break;

                case "tabalimentos":
                    if (elementosAlimentos.Count == 0)
                    {
                        CrearCombosYbebidas(App.AlimentosWeb.OrderBy(o => o.OrdenView).ToList());
                        elementosAlimentos.Clear();

                        foreach (UIElement elemento in imagenes.Children)
                        {
                            elementosAlimentos.Add(elemento);
                        }
                    }
                    else
                    {
                        imagenes.Children.Clear();

                        foreach (UIElement elemento in elementosAlimentos)
                        {
                            imagenes.Children.Add(elemento);
                        }
                    }

                    SelectProd = 2;
                    break;

                case "tabbebidas":

                    if (elementosBebidas.Count == 0)
                    {
                        CrearCombosYbebidas(App.BebidasWeb.OrderBy(o => o.OrdenView).ToList());
                        elementosBebidas.Clear();

                        foreach (UIElement elemento in imagenes.Children)
                        {
                            elementosBebidas.Add(elemento);
                        }
                    }
                    else
                    {
                        imagenes.Children.Clear();

                        foreach (UIElement elemento in elementosBebidas)
                        {
                            imagenes.Children.Add(elemento);
                        }
                    }

                    SelectProd = 3;
                    break;

                case "tabsnacks":

                    if (elementosSnack.Count == 0)
                    {
                        CrearCombosYbebidas(App.SnacksWeb.OrderBy(o => o.OrdenView).ToList());
                        elementosSnack.Clear();

                        foreach (UIElement elemento in imagenes.Children)
                        {
                            elementosSnack.Add(elemento);
                        }
                    }
                    else
                    {
                        imagenes.Children.Clear();

                        foreach (UIElement elemento in elementosSnack)
                        {
                            imagenes.Children.Add(elemento);
                        }
                    }
                    SelectProd = 4;
                    break;

                default:
                    break;
            }

            if (parent is Border)
            {
                return parent as Border;
            }
            else
            {
                return null;
            }
        }

        private void RestoreButtonState()
        {
            combosBorder.Background = new SolidColorBrush(Colors.LightGray);
            alimentosBorder.Background = new SolidColorBrush(Colors.LightGray);
            bebidasBorder.Background = new SolidColorBrush(Colors.LightGray);
            snacksBorder.Background = new SolidColorBrush(Colors.LightGray);
            tabcombos.Foreground = new SolidColorBrush(Colors.Black);
            tabalimentos.Foreground = new SolidColorBrush(Colors.Black);
            tabbebidas.Foreground = new SolidColorBrush(Colors.Black);
            tabsnacks.Foreground = new SolidColorBrush(Colors.Black);

            if (clickedBorder != null)
            {
                clickedBorder.Background = new SolidColorBrush(Colors.LightGray);
            }
        }

        public void CombosConsult(string pr_secpro = "", string pr_swtven = "", string pr_tiplog = "", string pr_tbview = "", string Teatro = "0", string Ciudad = "0")
        {

            #region VARIABLES LOCALES
            string lc_result = string.Empty;
            string lc_srvpar = string.Empty;
            string lc_auxitem = string.Empty;

            List<Producto> ob_return = new List<Producto>();
            List<Producto> ob_result = new List<Producto>();
            Dictionary<string, object> ob_diclst = new Dictionary<string, object>();

            Secuencia ob_scopre = new Secuencia();
            General ob_fncgrl = new General();
            #endregion

            try
            {
                if (pr_secpro == null)
                    pr_secpro = "0";
                if (pr_swtven == null)
                    pr_swtven = "V";
                if (pr_tiplog == null)
                    pr_tiplog = "P";
                if (pr_tbview == null)
                    pr_tbview = "";

                if (Teatro != "0")

                    #region SERVICIO SCOPRE 
                    ob_scopre.Punto = Convert.ToInt32(App.PuntoVenta);
                ob_scopre.Teatro = Convert.ToInt32(App.idCine);
                ob_scopre.Tercero = App.ValorTercero;

                //Generar y encriptar JSON para servicio PRE
                lc_srvpar = ob_fncgrl.JsonConverter(ob_scopre);
                lc_srvpar = lc_srvpar.Replace("Teatro", "teatro");
                lc_srvpar = lc_srvpar.Replace("Tercero", "tercero");
                lc_srvpar = lc_srvpar.Replace("punto", "Punto");

                //Encriptar Json PRE
                lc_srvpar = ob_fncgrl.EncryptStringAES(lc_srvpar);

                //Consumir servicio PRE
                if (App.ob_diclst.Count > 0)
                    lc_result = ob_fncgrl.WebServices(string.Concat(App.ScoreServices, "scopre/"), lc_srvpar);
                else
                    lc_result = ob_fncgrl.WebServices(string.Concat(App.ScoreServices, "scopcf/"), lc_srvpar);

                ;

                //Validar respuesta
                if (lc_result.Substring(0, 1) == "0")
                {
                    //Quitar switch
                    lc_result = lc_result.Replace("0-", "");
                    ob_diclst = (Dictionary<string, object>)JsonConvert.DeserializeObject(lc_result, (typeof(Dictionary<string, object>)));
                    ob_return = (List<Producto>)JsonConvert.DeserializeObject(ob_diclst["Lista_Productos"].ToString(), (typeof(List<Producto>)));

                    if (ob_diclst.ContainsKey("Validación"))
                    {
                        //ModelState.AddModelError("", ob_diclst["Validación"].ToString());
                    }
                    else
                    {
                        //Recorrido por objeto para obtener descripcion de receta combos
                        foreach (Producto item in ob_return)
                        {
                            if (item.Tipo == "C")
                            {
                                string fr_axurec = string.Empty;
                                foreach (var receta in item.Receta)
                                    fr_axurec += string.Concat(receta.Cantidad.ToString().Substring(0, receta.Cantidad.ToString().IndexOf(",")), " ", receta.Descripcion, ", ");

                                item.RecetaResumen = fr_axurec.Substring(0, fr_axurec.Length - 2);
                                ob_result.Add(item);
                            }
                            else
                            {
                                ob_result.Add(item);
                            }
                        }

                        //Recorrido por objeto para obtener el orden de pantallas y mostrar en vista
                        App.CombosWeb = new List<Producto>();
                        App.AlimentosWeb = new List<Producto>();
                        App.BebidasWeb = new List<Producto>();
                        App.SnacksWeb = new List<Producto>();
                        foreach (Producto item in ob_return)
                        {
                            //Recorrido por pantallas
                            foreach (var pantallas in item.Pantallas)
                            {
                                switch (pantallas.Descripcion)
                                {
                                    case "COMBOS WEB":
                                        int lc_cntcom = App.CombosWeb.Count;

                                        App.CombosWeb.Add(item);
                                        App.CombosWeb[lc_cntcom].OrdenView = pantallas.Orden;
                                        App.CombosWeb[lc_cntcom].Descripcion_Web = pantallas.Descripcion_Web;
                                        App.CombosWeb[lc_cntcom].Flag = pantallas.Flag;
                                        break;

                                    case "ALIMENTOS WEB":
                                        int lc_cntali = App.AlimentosWeb.Count;

                                        App.AlimentosWeb.Add(item);
                                        App.AlimentosWeb[lc_cntali].OrdenView = pantallas.Orden;
                                        App.AlimentosWeb[lc_cntali].Descripcion_Web = pantallas.Descripcion_Web;
                                        App.AlimentosWeb[lc_cntali].Flag = pantallas.Flag;
                                        break;

                                    case "BEBIDAS WEB":
                                        int lc_cntbeb = App.BebidasWeb.Count;

                                        App.BebidasWeb.Add(item);
                                        App.BebidasWeb[lc_cntbeb].OrdenView = pantallas.Orden;
                                        App.BebidasWeb[lc_cntbeb].Descripcion_Web = pantallas.Descripcion_Web;
                                        App.BebidasWeb[lc_cntbeb].Flag = pantallas.Flag;
                                        break;

                                    case "SNACKS WEB":
                                        int lc_cntsnk = App.SnacksWeb.Count;

                                        App.SnacksWeb.Add(item);
                                        App.SnacksWeb[lc_cntsnk].OrdenView = pantallas.Orden;
                                        App.SnacksWeb[lc_cntsnk].Descripcion_Web = pantallas.Descripcion_Web;
                                        App.SnacksWeb[lc_cntsnk].Flag = pantallas.Flag;
                                        break;
                                }
                            }
                        }

                        //Validar productos a mostrar combos
                        if (pr_tbview == "" || pr_tbview == "tab-combos")
                        {
                            CrearCombosYbebidas(App.CombosWeb.OrderBy(o => o.OrdenView).ToList());
                            elementosCombos.Clear();

                            foreach (UIElement elemento in imagenes.Children)
                            {
                                elementosCombos.Add(elemento);
                            }
                        }
                        //Validar productos a mostrar alimentos
                        if (pr_tbview == "tab-alimentos")
                            CrearCombosYbebidas(App.AlimentosWeb.OrderBy(o => o.OrdenView).ToList());
                        //Validar productos a mostrar bebidas
                        if (pr_tbview == "tab-bebidas")
                            CrearCombosYbebidas(App.BebidasWeb.OrderBy(o => o.OrdenView).ToList());

                        //Validar productos a mostrar snacks
                        if (pr_tbview == "tab-snacks")
                            CrearCombosYbebidas(App.SnacksWeb.OrderBy(o => o.OrdenView).ToList());
                    }
                }
                else
                {
                    lc_result = lc_result.Replace("1-", "");
                }
                #endregion

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        public void CrearCombosYbebidas(List<Producto> productos)
        {

            imagenes.Children.Clear();

            string appSettingsPath = "C:\\FALCROSOFT\\PROCINAL\\Portal.Kiosco\\appsettings.json";

            var builder = new ConfigurationBuilder()
                .AddJsonFile(appSettingsPath, optional: true, reloadOnChange: true);

            configuration = builder.Build();
            var myConfigSection = configuration.GetSection("MyConfig");
            string urlRetailImg = myConfigSection["UrlRetailImg"];

            if (productos != null)
            {
                foreach (var item in productos)
                {
                    string lc_auxcod = item.Codigo.ToString();
                    string lc_auximg = string.Concat(urlRetailImg, lc_auxcod.Substring(0, lc_auxcod.Length - 2), ".jpg");

                    Border border = new Border();
                    border.Width = 300;
                    border.Height = 550;
                    border.Margin = new Thickness(5);
                    border.BorderBrush = Brushes.Transparent;
                    border.BorderThickness = new Thickness(1);

                    Grid grid = new Grid();
                    border.Child = grid;

                    grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                    grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                    grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                    grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(462) });
                    grid.RowDefinitions.Add(new RowDefinition());

                    Image image = new Image();
                    image.Source = new BitmapImage(new Uri(lc_auximg));
                    Grid.SetRow(image, 0);
                    Grid.SetColumnSpan(image, 3);
                    grid.Children.Add(image);

                    Border innerBorder = new Border();
                    innerBorder.HorizontalAlignment = HorizontalAlignment.Center;
                    innerBorder.Width = 250;
                    Grid.SetRow(innerBorder, 1);
                    Grid.SetColumnSpan(innerBorder, 3);
                    grid.Children.Add(innerBorder);

                    Grid innerGrid = new Grid();
                    innerBorder.Child = innerGrid;

                    innerGrid.ColumnDefinitions.Add(new ColumnDefinition());
                    innerGrid.ColumnDefinitions.Add(new ColumnDefinition());
                    innerGrid.ColumnDefinitions.Add(new ColumnDefinition());

                    Border minusButtonBorder = new Border();
                    minusButtonBorder.CornerRadius = new CornerRadius(100);
                    minusButtonBorder.Background = new SolidColorBrush(Color.FromRgb(243, 6, 19));
                    minusButtonBorder.HorizontalAlignment = HorizontalAlignment.Left;
                    minusButtonBorder.Width = 61;
                    minusButtonBorder.Height = 61;
                    Grid.SetColumn(minusButtonBorder, 0);
                    innerGrid.Children.Add(minusButtonBorder);

                    Button minusButton = new Button();
                    minusButton.Background = Brushes.Transparent;
                    minusButton.BorderThickness = new Thickness(0);
                    minusButton.Content = "-";
                    minusButton.FontFamily = new FontFamily("Myanmar Khyay");
                    minusButton.FontSize = 50;
                    minusButton.Foreground = Brushes.White;
                    minusButton.Click += MinusButton_Click;
                    minusButtonBorder.Child = minusButton;

                    Label countLabel = new Label();
                    countLabel.FontFamily = new FontFamily("Myanmar Khyay");
                    countLabel.FontSize = 32;
                    countLabel.Content = "0";
                    countLabel.Name = string.Concat("lbl", lc_auxcod.Substring(0, lc_auxcod.Length - 2));
                    countLabel.VerticalAlignment = VerticalAlignment.Center;
                    countLabel.HorizontalAlignment = HorizontalAlignment.Center;
                    Grid.SetColumn(countLabel, 1);
                    innerGrid.Children.Add(countLabel);

                    Border plusButtonBorder = new Border();
                    plusButtonBorder.CornerRadius = new CornerRadius(100);
                    plusButtonBorder.Background = new SolidColorBrush(Color.FromRgb(243, 6, 19));
                    plusButtonBorder.HorizontalAlignment = HorizontalAlignment.Right;
                    plusButtonBorder.Width = 61;
                    plusButtonBorder.Height = 61;
                    Grid.SetColumn(plusButtonBorder, 2);
                    innerGrid.Children.Add(plusButtonBorder);

                    Button plusButton = new Button();
                    plusButton.Background = Brushes.Transparent;
                    plusButton.BorderThickness = new Thickness(0);
                    plusButton.Content = "+";
                    plusButton.FontFamily = new FontFamily("Myanmar Khyay");
                    plusButton.FontSize = 50;
                    plusButton.Foreground = Brushes.White;
                    plusButton.Click += PlusButton_Click;
                    plusButtonBorder.Child = plusButton;

                    imagenes.Children.Add(border);
                }

                Producto ob_datpro = new Producto();
                List<(decimal CodigoBotella, string NombreFinalBotella, decimal PrecioFinalBotella, string frecuenciaBotella, decimal categoria)> datosFinalesBotella = new List<(decimal, string, decimal, string, decimal)>();
                List<(decimal CodigoComida, string NombreFinalComida, decimal PrecioFinalComida, string frecuenciaComida, decimal categoria)> datosFinalesComida = new List<(decimal, string, decimal, string, decimal)>();
                int CodigoBebidas = 1244;
                int CodigoBebidas2 = 2444;
                int CodigoComidas = 246;
            }
        }

        public decimal SelPrecio(int SelectProd, decimal Codigo)
        {

            var producto = new List<Producto>();
            decimal Precios = 0;

            switch (SelectProd)
            {
                case 1:

                    Precios = buscarprecio(App.CombosWeb, Codigo);
                    elementosCombos.Clear();

                    foreach (UIElement elemento in imagenes.Children)
                    {
                        elementosCombos.Add(elemento);
                    }
                    SelectProd = 1;
                    break;

                case 2:
                    Precios = buscarprecio(App.AlimentosWeb, Codigo);
                    elementosAlimentos.Clear();

                    foreach (UIElement elemento in imagenes.Children)
                    {
                        elementosAlimentos.Add(elemento);
                    }
                    SelectProd = 2;
                    break;

                case 3:
                    Precios = buscarprecio(App.BebidasWeb, Codigo);
                    elementosBebidas.Clear();

                    foreach (UIElement elemento in imagenes.Children)
                    {
                        elementosBebidas.Add(elemento);
                    }
                    SelectProd = 3;
                    break;

                case 4:
                    Precios = buscarprecio(App.SnacksWeb, Codigo);
                    elementosSnack.Clear();

                    foreach (UIElement elemento in imagenes.Children)
                    {
                        elementosSnack.Add(elemento);
                    }
                    SelectProd = 4;
                    break;

                default:
                    break;
            }

            return Precios;
        }

        public decimal buscarprecio(List<Producto> productos, Decimal Codigo)
        {
            List<(decimal CodigoBotella, string NombreFinalBotella, decimal PrecioFinalBotella, string frecuenciaBotella, decimal categoria)> datosFinalesBotella = new List<(decimal, string, decimal, string, decimal)>();
            List<(decimal CodigoComida, string NombreFinalComida, decimal PrecioFinalComida, string frecuenciaComida, decimal categoria)> datosFinalesComida = new List<(decimal, string, decimal, string, decimal)>();

            List<Producto> ob_return = new List<Producto>();
            Dictionary<string, object> ob_diclst = new Dictionary<string, object>();

            Secuencia ob_scopre = new Secuencia();
            Producto ob_datpro = new Producto();
            General ob_fncgrl = new General();
            int CodigoBebidas = 1244;
            int CodigoBebidas2 = 2444;
            int CodigoComidas = 246;

            decimal Precios = 0;

            foreach (var itepro in productos)
            {
                if (itepro.Codigo == Codigo)
                {
                    switch (itepro.Tipo)
                    {

                        case "P": //PRODUCTOS
                            ob_datpro.Codigo = itepro.Codigo;
                            ob_datpro.Descripcion = itepro.Descripcion;
                            ob_datpro.Tipo = itepro.Tipo;
                            ob_datpro.Precios = itepro.Precios;

                            foreach (var itempre in itepro.Precios)
                            {
                                Precios += (itempre.General * 1);
                            };

                            break;

                        case "C": //COMBOS
                            ob_datpro.Codigo = itepro.Codigo;
                            ob_datpro.Descripcion = itepro.Descripcion;
                            ob_datpro.Tipo = itepro.Tipo;
                            ob_datpro.Receta = itepro.Receta;
                            ob_datpro.Precios = itepro.Precios;

                            List<Precios> precio_Combo = new List<Precios>();

                            foreach (var itemprer in itepro.Receta)
                            {
                                if (itemprer.Precios != null)
                                {


                                    foreach (var itempre in itemprer.Precios)
                                    {
                                        var preitem = new Precios()
                                        {
                                            General = itempre.General,
                                            OtroPago = itempre.OtroPago,
                                            HorasDias = itempre.HorasDias,
                                            PagoInterno = itempre.PagoInterno,
                                            PrecioAtencion = itempre.PrecioAtencion

                                        };
                                        precio_Combo.Add(preitem);
                                        datosFinalesComida.Add((itemprer.Codigo, itemprer.Descripcion, itempre.General, "true", itemprer.Codigo));
                                        Precios += (itempre.General * itemprer.Cantidad);
                                    };
                                }

                            }

                            ob_datpro.Precios = precio_Combo;

                            bool condicionCumplida = false;

                            foreach (var itecat in itepro.Receta)
                            {
                                var precio_Combo_Receta = itecat.Codigo;
                                if (precio_Combo_Receta == CodigoBebidas || precio_Combo_Receta == CodigoBebidas2)
                                {
                                    foreach (var i in itecat.RecetaCategoria)
                                    {
                                        var CodioBotella = i.Codigo;
                                        var NombreFinalBotella = i.Descripcion.ToString();
                                        var precioFinalBotella = i.Precios.Sum(precio => precio.General);
                                        var frecuenciaBotella = i.Frecuente.ToString();

                                        if (Convert.ToBoolean(frecuenciaBotella) == true)
                                        {
                                            Precios += precioFinalBotella * itecat.Cantidad;
                                        }
                                        datosFinalesBotella.Add((CodioBotella, NombreFinalBotella, precioFinalBotella, frecuenciaBotella, itecat.Codigo));
                                    }
                                }
                                else if (precio_Combo_Receta == CodigoComidas)
                                {
                                    foreach (var i in itecat.RecetaCategoria)
                                    {
                                        var CodioComida = i.Codigo;
                                        var NombreFinalComida = i.Descripcion.ToString();
                                        var precioFinalComida = i.Precios.Sum(precio => precio.General);
                                        var frecuenciaComida = i.Frecuente.ToString();
                                        if (Convert.ToBoolean(frecuenciaComida) == true)
                                        {
                                            Precios += precioFinalComida * itecat.Cantidad;
                                        }

                                        datosFinalesComida.Add((CodioComida, NombreFinalComida, precioFinalComida, frecuenciaComida, itecat.Codigo));
                                    }
                                }

                                if (datosFinalesBotella.Count > 0 && datosFinalesComida.Count > 0)
                                {
                                    condicionCumplida = true;
                                }

                                if (condicionCumplida)
                                {
                                    break;
                                }
                            }

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

                                foreach (var itempre in itepro.Precios)
                                {
                                    Precios += (itempre.General * itecat.Cantidad);
                                };

                                ob_datpro.LisProducto.Add(ob_itecat);
                            }

                            break;
                    }

                    App.ProductosSeleccionados.Add(itepro);

                    break;
                }
            }

            return Precios;
        }

        private void PlusButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            Border parentBorder = (Border)button.Parent;
            Grid innerGrid = (Grid)parentBorder.Parent;
            Label countLabel = (Label)innerGrid.Children[1];

            int currentValue = int.Parse(countLabel.Content.ToString());
            currentValue++;
            countLabel.Content = currentValue.ToString();
            var precio = SelPrecio(SelectProd, Convert.ToDecimal(countLabel.Name.Substring(3)));

            string totalString = totalLabel.Content.ToString().Replace("$", "").Replace("€", "").Replace(".", "").Replace(",", "").Trim(); 
            decimal totalAnterior = decimal.Parse(totalString);
            decimal nuevoTotal = totalAnterior + precio;
            totalLabel.Content = nuevoTotal.ToString("C0");


            string codigo = countLabel.Name.Substring(3);
            var productoARemover = App.ProductosSeleccionados.FirstOrDefault(prod => prod.Codigo.ToString() == codigo);
            if (productoARemover != null)
            {
                App.ProductosSeleccionados.Remove(productoARemover);
            }
        }

        private void MinusButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            Border parentBorder = (Border)button.Parent;
            Grid innerGrid = (Grid)parentBorder.Parent;
            Label countLabel = (Label)innerGrid.Children[1];

            int currentValue = int.Parse(countLabel.Content.ToString());
            if (currentValue > 0)
            {
                currentValue--;
                countLabel.Content = currentValue.ToString();

                var precio = SelPrecio(SelectProd, Convert.ToDecimal(countLabel.Name.Substring(3)));

                var productoAEliminar = App.ProductosSeleccionados.FirstOrDefault(producto => producto.Codigo == Convert.ToDecimal(countLabel.Name.Substring(3)));

                if (productoAEliminar != null)
                {
                    string totalString = totalLabel.Content.ToString().Replace("$", "").Replace("€", "").Replace(".", "").Replace(",", "").Trim();
                    decimal totalAnterior = decimal.Parse(totalString);
                    decimal nuevoTotal = totalAnterior - precio;
                    totalLabel.Content = nuevoTotal.ToString("C0");

                    if (currentValue == 0)
                    {
                        App.ProductosSeleccionados.Remove(productoAEliminar);
                    }
                }
            }
        }

        private async void btnSiguiente_Click(object sender, RoutedEventArgs e)
        {
            if (totalLabel.Content.ToString() != "0")
            {
                var ProductosSeleccionados = App.ProductosSeleccionados.FirstOrDefault(tip => tip.Tipo == "C");
                if (ProductosSeleccionados != null)
                {
                    isThreadActive = false;
                    Combodeluxe1 openWindows = new Combodeluxe1();
                    openWindows.Show();
                    this.Close();

                }
                else
                {
                    isThreadActive = false;
                    ResumenCompra openWindows = new ResumenCompra(config);
                    openWindows.Show();
                    this.Close();

                }
            }
            else
            {
                MessageBox.Show("Ups, aun no haz seleccionado un producto.", "Notificación", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void btnVolver_Click(object sender, RoutedEventArgs e)
        {
            if (App.IsBoleteriaConfiteria == false)
            {
                isThreadActive = false;
                App.IsCinefans = true;
                AlgoParaComer openWindows = new AlgoParaComer();
                openWindows.Show();
                this.Close();
            }
            else
            {
                isThreadActive = false;
                App.IsCinefans = true;
                ComoCompra openWindows = new ComoCompra();
                openWindows.Show();
                this.Close();
            }
        }

        private async void btnSalir_Click(object sender, RoutedEventArgs e)
        {
            isThreadActive = false;
            Principal openWindows = new Principal();
            openWindows.Show();
            this.Close();
        }
    }
}