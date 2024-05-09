using APIPortalKiosco.Data;
using APIPortalKiosco.Entities;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace Portal.Kiosco.Properties.Views
{
    public partial class ResumenCompra : Window
    {
        private readonly IOptions<MyConfig> config;

        public ResumenCompra(IOptions<MyConfig> config)
        {
            InitializeComponent();
            DataContext = ((App)Application.Current);
            this.config = config;
            if (App.ob_diclst.Count > 0)
            {
                lblnombre.Content = "!HOLA " + App.ob_diclst["Nombre"].ToString() + " " + App.ob_diclst["Apellido"].ToString();
            }
            else
            {
                lblnombre.Content = "!HOLA INVITADO";
            }

            GenerateResumen();

        }

        public void Responses(string ref_payco = "")
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
            string lc_urlcor = ""/*config.Value.UrlCorreo*/;
            string session = "";
            string status = "";
            decimal lc_secsec = Convert.ToDecimal(App.Secuencia);
            decimal lc_keytea = Convert.ToDecimal(App.idCine);
            decimal lc_puntea = Convert.ToDecimal(App.PuntoVenta);

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

                    string ClienteFrecuente = session/*.GetString("ClienteFrecuente")*/;
                    string CashBack_Acumulado = String.Format("{0:C0}", Convert.ToDecimal(session/*.GetString("CashBack_Acumulado")*/));
                }

                //Inicializar instancia web client para leer respuesta
                using (WebClient wc = new WebClient())
                {
                    //Obtener información de epayco
                    var ob_json = ""/* wc.DownloadString(config.Value.data_epayco_secure + ref_payco)*/;
                    var ob_response = JsonConvert.DeserializeObject<EpaycoApiGet>(ob_json);

                    //validar rta y Obtener y deserializar respuesta
                    if (!ref_payco.Contains("CashBack"))
                    {
                        //Obtener valores de rta Epayco y consultar registro en la bd
                        lc_secsec = Convert.ToDecimal(ob_response.data.x_extra1);
                        lc_keytea = Convert.ToDecimal(ob_response.data.x_extra2);
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
                        lc_secsec = Convert.ToDecimal(session/*.GetString("Secuencia")*/);
                        lc_keytea = Convert.ToDecimal(session/*.GetString("Teatro")*/);
                        lc_coreli = session/*.GetString("Usuario")*/;
                        lc_valtra = ref_payco.Substring(ref_payco.IndexOf(":") + 1);
                        lc_status = "Cashback";
                        lc_idsepy = "Cashback:SEC-" + session/*.GetString("Secuencia")*/;
                        lc_fectra = DateTime.Now.ToString();
                        lc_refepy = App.PuntoVenta + "-" + session;
                        lc_bankpy = "CashBack Procinal";
                    }

                    //Inicializar instancia de BD
                    using (var context = new DataDB(config))
                    {                    
                        //Consultar registro de venta en BD transacciones
                        var ob_repsl1 = context.TransactionSales.Where(x => x.Secuencia == lc_secsec).Where(x => x.PuntoVenta == lc_puntea).Where(x => x.Teatro == lc_keytea);
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
                            ob_repsle.NombreEli = session/*.GetString("Nombre")*/ + " " + session/*.GetString("Apellido")*/;
                            ob_repsle.TelefonoEli = session/*.GetString("Telefono")*/;
                            ob_repsle.DocumentoEli = session/*.GetString("Documento")*/;
                        }

                        //Actualizar estado de transacción
                        context.TransactionSales.Update(ob_repsle);
                        context.SaveChanges();

                        var Teatro = lc_keytea.ToString();
                        var PtoVenta = lc_puntea.ToString();
                        var Secuencia = lc_secsec.ToString();
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
                        var rs = context.RetailSales.Where(x => x.Secuencia == lc_secsec).Where(x => x.PuntoVenta == lc_puntea).Where(x => x.KeyTeatro == lc_keytea).Where(x => x.Tipo == "C").ToList();
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

                        rs = context.RetailSales.Where(x => x.Secuencia == lc_secsec).Where(x => x.PuntoVenta == lc_puntea).Where(x => x.KeyTeatro == lc_keytea).Where(x => x.Tipo != "C").ToList();

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

                       /* ViewBag.ListR = ob_ordite;*/ //ViewBag.ListCarritoR;
                    }

                    if (session/*.GetString("Secuencia")*/ != null)
                    {
                        try
                        {
                            //Envio de correo Score
                            var request = (HttpWebRequest)WebRequest.Create(lc_urlcor);
                            request.GetResponse();
                        }
                        catch (Exception)
                        {
                            string EnvioCorreo = "Fallo envío de correo compra APROBADA, por favor comunicarse con el teatro.";
                        }
                    }
                }

                //Estado Fallido
                //Validar venta
                if (lc_status == "Rechazada" || lc_status == "Fallida")
                {
                    if (session/*.GetString("Secuencia")*/ != null)
                    {
                        #region SERVICO SCORET
                        //Json de servicio RET
                        lc_objson = "{\"Punto\":" + Convert.ToInt32(App.PuntoVenta) + ",\"Pedido\":" + Convert.ToInt32(lc_secsec) + ",\"teatro\":\"" + Convert.ToInt32(lc_keytea) + "\",\"tercero\":\"" + App.ValorTercero + "\"}";

                        //Encriptar Json RET
                        lc_srvpar = ob_fncgrl.EncryptStringAES(lc_objson);

                        //Consumir servicio RET
                        lc_jsnrst = ob_fncgrl.WebServices(string.Concat(App.ScoreServices, "scoret/"), lc_srvpar);

                        //Generar Log
                        //LogSales logSales = new LogSales();
                        //LogAudit logAudit = new LogAudit(config);
                        //logSales.Id = Guid.NewGuid().ToString();
                        //logSales.Fecha = DateTime.Now;
                        //logSales.Programa = "Pages/Responses";
                        //logSales.Metodo = "SCORET";
                        //logSales.ExceptionMessage = lc_srvpar;
                        //logSales.InnerExceptionMessage = lc_jsnrst;

                        //Escribir Log
                        //logAudit.LogApp(logSales);

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
                                //ModelState.AddModelError("", ob_diclst["Validación"].ToString());
                                //return View();
                            }
                            else
                            {
                                //Validar respuesta llave 2
                                if (ob_diclst.ContainsKey("Respuesta"))
                                {
                                    if (ob_diclst["Respuesta"].ToString() != "Proceso exitoso")
                                    {
                                        //ModelState.AddModelError("", ob_diclst["Respuesta"].ToString());
                                        //return View();
                                    }
                                }
                            }

                            ob_diclst.Clear();
                        }
                        else
                        {
                            //ModelState.AddModelError("", "Reembolso no culminado con exito SCORET");
                            //return View();
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
                            string EnvioCorreo = "Fallo envío de correo compra RECHAZADA, por favor comunicarse con el teatro.";
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

                    if (session/*.GetString("Secuencia")*/ != null)
                    {
                        try
                        {
                            //Envio de correo Score
                            lc_urlcor = lc_urlcor.Replace("compra", "Pendiente");
                            var request = (HttpWebRequest)WebRequest.Create(lc_urlcor);
                            request.GetResponse();

                            //Generar Log
                            //LogSales logSales = new LogSales();
                            //LogAudit logAudit = new LogAudit(config);
                            //logSales.Id = Guid.NewGuid().ToString();
                            //logSales.Fecha = DateTime.Now;
                            //logSales.Programa = "Pages/Responses";
                            //logSales.Metodo = "EMAIL";
                            //logSales.ExceptionMessage = "Envío de correo compra PENDIENTE: Exitoso";
                            //logSales.InnerExceptionMessage = "null";

                            //Escribir Log
                            //logAudit.LogApp(logSales);
                        }
                        catch (Exception)
                        {
                            string EnvioCorreo = "Fallo envío de correo compra PENDIENTE, por favor comunicarse con el teatro.";

                            //Generar Log
                            //LogSales logSales = new LogSales();
                            //LogAudit logAudit = new LogAudit(config);
                            //logSales.Id = Guid.NewGuid().ToString();
                            //logSales.Fecha = DateTime.Now;
                            //logSales.Programa = "Pages/Responses";
                            //logSales.Metodo = "EMAIL";
                            //logSales.ExceptionMessage = "Fallo envío de correo compra PENDIENTE, por favor comunicarse con el teatro.";
                            //logSales.InnerExceptionMessage = "null";

                            ////Escribir Log
                            //logAudit.LogApp(logSales);
                        }
                    }
                }

                //Validar y remover sesion invitada
                if (session/*.GetString("FlagLogin")*/ == "INV")
                {

                }

                //Quitar secuencia
                //Session.Remove("Secuencia");
                //return View();
            }
            catch (Exception lc_syserr)
            {
                #region SERVICO SCORET
                //Json de servicio RET
                //lc_objson = "{\"Punto\":" + Convert.ToInt32(config.Value.PuntoVenta) + ",\"Pedido\":" + Convert.ToInt32(lc_secsec) + ",\"teatro\":\"" + Convert.ToInt32(lc_keytea) + "\",\"tercero\":\"" + config.Value.ValorTercero + "\"}";

                //Encriptar Json RET
                //lc_srvpar = ob_fncgrl.EncryptStringAES(lc_objson);

                //Consumir servicio RET
                //lc_jsnrst = ob_fncgrl.WebServices(string.Concat(config.Value.ScoreServices, "scoret/"), lc_srvpar);

                //Generar Log
                //LogSales logSales = new LogSales();
                //LogAudit logAudit = new LogAudit(config);
                //logSales.Id = Guid.NewGuid().ToString();
                //logSales.Fecha = DateTime.Now;
                //logSales.Programa = "Pages/Responses";
                //logSales.Metodo = "SCORET_CATCH";
                //logSales.ExceptionMessage = lc_srvpar;
                //logSales.InnerExceptionMessage = lc_jsnrst;

                //Escribir Log
                //logAudit.LogApp(logSales);
                #endregion

                //Generar Log
                //logSales.Id = Guid.NewGuid().ToString();
                //logSales.Fecha = DateTime.Now;
                //logSales.Programa = "Pages/Responses";
                //logSales.Metodo = "GET";
                //logSales.ExceptionMessage = lc_syserr.Message;
                //logSales.InnerExceptionMessage = logSales.ExceptionMessage.Contains("Inner") ? lc_syserr.InnerException.Message : "null";

                //Escribir Log
                //logAudit.LogApp(logSales);

                //Validar si esta la sesion activa y Devolver vista de error
                //if (Session.GetString("ClienteFrecuente") != null)
                //    return RedirectToAction("Error", "Pages", new { pr_message = config.Value.MessageException + logSales.Id, pr_flag = "RSPNS" });
                //else
                //    return RedirectToAction("Home", "Home");
            }
        }



        private async void btnVolver_Click(object sender, RoutedEventArgs e)
        {
            var openWindow = new Combodeluxe1();
            DoubleAnimation fadeOutAnimation = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.5));
            this.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);
            await Task.Delay(300);
            this.Visibility = Visibility.Collapsed;
            openWindow.Background = Brushes.White;
            openWindow.Show();
            this.Close();
            DoubleAnimation fadeInAnimation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.5));
            openWindow.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);
        }

        private void btnSiguiente_Click(object sender, RoutedEventArgs e)
        {
            //PagoCashback w = new PagoCashback();
            //this.Close();
            //w.ShowDialog();
        }

        private async void btnSalir_Click(object sender, RoutedEventArgs e)
        {
            var openWindow = new Principal();
            DoubleAnimation fadeOutAnimation = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.5));
            this.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);
            await Task.Delay(300);
            this.Visibility = Visibility.Collapsed;
            openWindow.Background = Brushes.White;
            openWindow.Show();
            this.Close();
            DoubleAnimation fadeInAnimation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.5));
            openWindow.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);

        }

        private async void btnPagoTarjeta_Click(object sender, RoutedEventArgs e)
        {

            var openWindow = new BoletasGafasAlimentos();
            DoubleAnimation fadeOutAnimation = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.5));
            this.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);
            await Task.Delay(300);
            this.Visibility = Visibility.Collapsed;
            openWindow.Background = Brushes.White;
            openWindow.Show();
            this.Close();
            DoubleAnimation fadeInAnimation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.5));
            openWindow.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);
        }

        private async void btnPagarCash_Click(object sender, RoutedEventArgs e)
        {
            var openWindow = new PagoCashback();
            DoubleAnimation fadeOutAnimation = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.5));
            this.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);
            await Task.Delay(300);
            this.Visibility = Visibility.Collapsed;
            openWindow.Background = Brushes.White;
            openWindow.Show();
            this.Close();
            DoubleAnimation fadeInAnimation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.5));
            openWindow.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);
        }

        public void GenerateResumen()
        {
            // Generar resumen de boletas
            decimal totalcombos = 0;
            GenerateResumenCategoria("Boletas", App.Pelicula.Nombre == null || App.Pelicula.Nombre == "" ? "Sin Pelicula" : App.Pelicula.Nombre, App.ValorTarifa, App.CantidadBoletas.ToString(), App.CantidadBoletas * App.ValorTarifa);
            totalcombos += (App.CantidadBoletas * App.ValorTarifa);
            // Generar resumen de gafas
            GenerateResumenCategoria("Gafas", "Gafas", App.PrecioUnitario, App.CantidadGafas.ToString(), (App.CantidadGafas * App.PrecioUnitario) );
            totalcombos += 0;
            // Generar resumen de combos
            var combos = App.ProductosSeleccionados;

            // Agrupar los combos por código
            var combosAgrupados = combos.GroupBy(c => c.Codigo);

            foreach (var grupoCombos in combosAgrupados)
            {
                decimal codigo = grupoCombos.Key;

                // Obtener el nombre, precio y cantidad para el grupo de combos
                string nombre = buscarNombre(combos, codigo);
                decimal precio = buscarprecio(combos, codigo);
                int cantidad = grupoCombos.Count();
                decimal total = (Convert.ToDecimal(precio) * cantidad);
                totalcombos += total;
                // Generar el resumen para el grupo de combos
                GenerateResumenCategoria("Combos", nombre, precio, cantidad.ToString(), total);
            }

            // Calcular y mostrar el total
            TotalResumen.Content = "TOTAL A PAGAR: " + totalcombos.ToString("C", new CultureInfo("es-CO"));
        }

        private void GenerateResumenCategoria(string categoria, string nombre, decimal valor, string cantidad, decimal total)
        {
            // Crear un nuevo Grid
            Grid grid = new Grid();

            // Crear definiciones de columna
            for (int i = 0; i < 9; i++)
            {
                ColumnDefinition columnDefinition = new ColumnDefinition();
                // Asignar el ancho de la columna según corresponda
                if (i == 0)
                    columnDefinition.Width = new GridLength(28);
                else if (i == 1)
                    columnDefinition.Width = new GridLength(870);
                else if (i == 2)
                    columnDefinition.Width = new GridLength(0);
                else if (i == 3)
                    columnDefinition.Width = new GridLength(125);
                else if (i == 4)
                    columnDefinition.Width = new GridLength(105);
                else if (i == 5)
                    columnDefinition.Width = new GridLength(125);
                else if (i == 6)
                    columnDefinition.Width = new GridLength(118);
                else if (i == 7)
                    columnDefinition.Width = new GridLength(125);
                // La última columna no tiene un ancho específico

                grid.ColumnDefinitions.Add(columnDefinition);
            }

            // Crear los bordes con etiquetas dentro
            for (int i = 1; i <= 4; i++)
            {
                Border border = new Border();
                Grid.SetColumn(border, i * 2 - 1); // Colocar el borde en columnas impares (1, 3, 5)
                Label label = new Label();
                label.FontFamily = new FontFamily("Myanmar Khyay");
                label.FontSize = 24;
                label.VerticalContentAlignment = VerticalAlignment.Center;
                border.BorderBrush = Brushes.Black; // Color de la línea



                // Configurar contenido y márgenes según corresponda
                if (i == 1)
                {
                    label.Content = nombre;
                    label.Margin = new Thickness(0, 0, -78, 0);
                }
                else if (i == 2)
                {
                    label.Content = valor.ToString("C", new CultureInfo("es-CO")); ;
                    label.Margin = new Thickness(0, 22, -44, 23);
                }
                else if (i == 3)
                {
                    label.Content = cantidad;
                }
                else if (i == 4)
                {
                    label.Content = total.ToString("C", new CultureInfo("es-CO"));
                    ;
                    total += Convert.ToDecimal(total);
                }

                border.Child = label;
                grid.Children.Add(border);
            }

            // Agregar el Grid al contenedor correspondiente
            switch (categoria)
            {
                case "Boletas":
                    ContenedorBoletas.Children.Add(grid);
                    break;
                case "Gafas":
                    ContenedorGafas.Children.Add(grid);
                    break;
                case "Combos":
                    ContenedorResumen.Children.Add(grid);
                    break;
                // Agrega más casos según las categorías que tengas
                default:
                    break;
            }
        }


        public string buscarNombre(List<Producto> productos, Decimal Codigo)
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
            var nombre = "";

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
                            nombre = itepro.Descripcion;
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
                                        // Hacer algo con precioFinalCombo
                                        if (Convert.ToBoolean(frecuenciaBotella) == true)
                                        {
                                            Precios += precioFinalBotella;
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
                                            Precios += precioFinalComida;
                                        }
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
                            nombre = itepro.Descripcion;
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



                    break;
                }
            }

            return nombre;

        }

        public decimal total = 0;
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
                                        // Hacer algo con precioFinalCombo
                                        if (Convert.ToBoolean(frecuenciaBotella) == true)
                                        {
                                            Precios += precioFinalBotella;
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
                                            Precios += precioFinalComida;
                                        }
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

                                foreach (var itempre in itepro.Precios)
                                {
                                    Precios += (itempre.General * itecat.Cantidad);
                                };

                                ob_datpro.LisProducto.Add(ob_itecat);
                            }

                            break;
                    }



                    break;
                }
            }

            return Precios;

        }
    }
}