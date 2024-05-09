using APIPortalKiosco.Data;
using APIPortalKiosco.Entities;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;

namespace Portal.Kiosco.Properties.Views
{
    /// <summary>
    /// Lógica de interacción para Frame17.xaml
    /// </summary>
    public partial class CorreoTecladoFlotante : Window
    {
        private readonly IOptions<MyConfig> config;
        public CorreoTecladoFlotante(IOptions<MyConfig> config)
        {
            InitializeComponent();
            DataContext = ((App)Application.Current);
            this.config = config;
            DoubleAnimation fadeInAnimation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.5));
            gridPrincipal.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);
        }

        private async void btnObtenerDatos_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string email = TextCorreoElectronico.Text;

                if (string.IsNullOrWhiteSpace(email))
                {
                    MessageBox.Show("!Ups, aún falta ingresar el correo electrónico!", "Notificación", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                else if (!IsValidEmail(email))
                {
                    MessageBox.Show("El correo electrónico ingresado no es válido.", "Notificación", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                else if(IsValidEmail(email))
                {
                    EnvioCorreo(email);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void EnvioCorreo (string correo)
        {
            string lc_fectra = string.Empty;
            string lc_valtra = string.Empty;
            string lc_idsepy = string.Empty;
            string lc_status = string.Empty;
            string lc_coreli = correo;
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

            try
            {

                //Adicionar valores de envio de correo Score
                lc_urlcor = lc_urlcor.Replace("#xxx", lc_keytea.ToString());
                lc_urlcor = lc_urlcor.Replace("#yyy", App.PuntoVenta);
                lc_urlcor = lc_urlcor.Replace("#zzz", lc_secsec.ToString());
                lc_urlcor = lc_urlcor.Replace("#ccc", correo);

                //Estado Exitoso
                if (lc_status == "Aceptada" || lc_status == "Cashback")
                {
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

        private bool IsValidEmail(string email)
        {
            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            Regex regex = new Regex(pattern);
            return regex.IsMatch(email);
        }


        private void KeyButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button != null)
            {
                string character = button.Content.ToString();

                if (character == "Del")
                {
                    if (TextCorreoElectronico.Text.Length > 0)
                    {
                        TextCorreoElectronico.Text = TextCorreoElectronico.Text.Substring(0, TextCorreoElectronico.Text.Length - 1);
                    }
                }
                else
                {
                    TextCorreoElectronico.Text += character;
                }
            }
        }


        private async void btnVolver_Click(object sender, RoutedEventArgs e)
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
    }
}