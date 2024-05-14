using APIPortalKiosco.Entities;
using APIPortalWebMed.Entities;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Xml;
//using Windows.UI.Xaml.Controls;

namespace Portal.Kiosco.Properties.Views
{
    public partial class SeleccionarFuncion : Window
    {
        //private Dictionary<string, List<string>> horasPorFecha = new Dictionary<string, List<string>>();
        public List<UIElement> elementosConservados3D;
        public List<UIElement> elementosConservadosGeneral;
        private readonly IOptions<MyConfig> config;
        private bool isThreadActive = true;

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
            if (App.ob_diclst.Count > 0)
            {
                lblnombre.Content = "!HOLA " + App.ob_diclst["Nombre"].ToString() + " " + App.ob_diclst["Apellido"].ToString();
            }
            else
            {
                lblnombre.Content = "!HOLA INVITADO";
            }

            DoubleAnimation fadeInAnimation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(3));
            gridPrincipal.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);
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
            bool isMainWindowOpen = false; // Variable local para indicar si la ventana principal está abierta

            if (App._tiempoRestanteGlobal == "00:00")
            {
                this.Dispatcher.Invoke(() =>
                {
                    Principal principal = Application.Current.Windows.OfType<Principal>().FirstOrDefault();
                    if (principal != null && principal.Visibility == Visibility.Visible)
                    {
                        // Enfocar la ventana principal si está abierta y visible
                        principal.Activate();
                        isMainWindowOpen = true; // Marcar que la ventana principal está abierta
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
                            // Cerrar todas las demás ventanas excepto la ventana principal
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

            return isMainWindowOpen; // Devolver el valor booleano
        }


        private async void btnVolver_Click(object sender, RoutedEventArgs e)
        {
            isThreadActive = false;
            Cartelera openWindows = new Cartelera();
            openWindows.Show();
            this.Close();
        }

        private void btnSiguiente_Click(object sender, RoutedEventArgs e)
        {

            isThreadActive = false;
            LayoutAsientos openWindows = new LayoutAsientos(config);
            openWindows.Show();
            this.Close();
        }

        private async void CargarFechasDesdeXml()
        {
            HashSet<string> fechasProcesadas = new HashSet<string>();
            HashSet<string> horasProcesadas = new HashSet<string>();
            int isfecha = 0;
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

                        }

                        foreach (var hora in fechas.horafun.OrderBy(h => DateTime.ParseExact(h.horario, "h:mm tt", CultureInfo.InvariantCulture)))
                        {
                            if (hora.idFuncion.Contains(fechas.fecunv))
                            {
                                DateTime FechaHoraInicio = Convert.ToDateTime(DateTime.Now.ToString("dd/MM/yyyy") + " " + hora.horunv.Substring(0, 2) + ":" + hora.horunv.Substring(2, 2) + ":00");
                                DateTime FechaHoraTermino = DateTime.ParseExact(DateTime.Now.ToString("HH:mm"), "HH:mm", System.Globalization.CultureInfo.InvariantCulture);

                                if (DateTime.Now.ToString("yyyyMMdd") == fechas.fecunv)
                                {
                                    if (App.MinDifHora != "0")
                                    {
                                        //Diferencia de tiempo entre hora funcion y hora del dia 
                                        TimeSpan diferencia = FechaHoraInicio - FechaHoraTermino;
                                        var diferenciaenminutos = diferencia.TotalMinutes;

                                        if (diferenciaenminutos > Convert.ToDouble(App.MinDifHora))
                                        {
                                            if (!horasProcesadas.Contains(hora.horario))
                                            {
                                                horasProcesadas.Add(hora.horario);

                                                if (pelicula.Formato.Contains("2D"))
                                                {
                                                    ContenedorHorasGeneral.Children.Add(CrearBotonHora(hora.horario, hora.horunv));
                                                }
                                                if (pelicula.Formato.Contains("3D"))
                                                {
                                                    ContenedorHoras3D.Children.Add(CrearBotonHora(hora.horario, hora.horunv));
                                                }
                                            }
                                        }
                                    }

                                    isfecha = 1;

                                }
                                //else
                                //{
                                //    if (isfecha == 0)
                                //    {
                                //        if (!horasProcesadas.Contains(hora.horario))
                                //        {
                                //            horasProcesadas.Add(hora.horario);

                                //            if (pelicula.Formato.Contains("2D"))
                                //            {
                                //                ContenedorHorasGeneral.Children.Add(CrearBotonHora(hora.horario, hora.idFuncion));
                                //            }
                                //            if (pelicula.Formato.Contains("3D"))
                                //            {
                                //                ContenedorHoras3D.Children.Add(CrearBotonHora(hora.horario, hora.idFuncion));
                                //            }
                                //        }
                                //    }

                                //}

                                App.TipoSala = hora.tipSala;
                            }
                        }
                    }
                }
            }
        }

        private async void CargarFechasDesdeSelect()
        {
            HashSet<string> fechasProcesadas = new HashSet<string>();
            HashSet<string> horasProcesadas = new HashSet<string>();
            int isfecha = 0;
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

                        }

                        foreach (var hora in fechas.horafun.OrderBy(h => DateTime.ParseExact(h.horario, "h:mm tt", CultureInfo.InvariantCulture)))
                        {
                            if (hora.idFuncion.Contains(App.Pelicula.FechaSel.Substring(3)))
                            {
                                DateTime FechaHoraInicio = Convert.ToDateTime(DateTime.Now.ToString("dd/MM/yyyy") + " " + hora.horunv.Substring(0, 2) + ":" + hora.horunv.Substring(2, 2) + ":00");
                                DateTime FechaHoraTermino = DateTime.ParseExact(DateTime.Now.ToString("HH:mm"), "HH:mm", System.Globalization.CultureInfo.InvariantCulture);

                                if (DateTime.Now.ToString("yyyyMMdd") == fechas.fecunv)
                                {
                                    if (App.MinDifHora != "0")
                                    {
                                        //Diferencia de tiempo entre hora funcion y hora del dia 
                                        TimeSpan diferencia = FechaHoraInicio - FechaHoraTermino;
                                        var diferenciaenminutos = diferencia.TotalMinutes;

                                        if (diferenciaenminutos > Convert.ToDouble(App.MinDifHora))
                                        {
                                            if (!horasProcesadas.Contains(hora.horario))
                                            {
                                                horasProcesadas.Add(hora.horario);

                                                if (pelicula.Formato.Contains("2D"))
                                                {
                                                    ContenedorHorasGeneral.Children.Add(CrearBotonHora(hora.horario, hora.horunv));
                                                }
                                                if (pelicula.Formato.Contains("3D"))
                                                {
                                                    ContenedorHoras3D.Children.Add(CrearBotonHora(hora.horario, hora.horunv));
                                                }
                                            }
                                        }
                                    }

                                    isfecha = 1;

                                }
                                else
                                {
                                    if (isfecha == 0)
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

                                App.TipoSala = hora.tipSala;
                            }
                        }
                    }
                }
            }

        }

        //public async void CargarFechasDesdeSelect()
        //{
        //    ContenedorHorasGeneral.Children.Clear();
        //    ContenedorHoras3D.Children.Clear();
        //    ContenedorHoras3D.Children.Add(CrearBotonFormato("3D"));
        //    ContenedorHorasGeneral.Children.Add(CrearBotonFormato("General"));

        //    var listpelicula = App.Peliculas.Where(x => x.TituloOriginal == lblnombre.Content.ToString());
        //    int lc_keypel = 0;
        //    int lc_auxpel = 0;
        //    int lc_keytea = 0;
        //    int lc_auxtea = 0;
        //    int lc_swtflg = 0;
        //    string Variables41TPF = string.Empty;
        //    string lc_auxitem = string.Empty;
        //    string lc_fecitem = string.Empty;
        //    string lc_flgpre = "S";
        //    string pr_tippel = "";

        //    string lc_result = string.Empty;
        //    string lc_srvpar = string.Empty;

        //    DateTime dt_fecpro;

        //    List<DateCartelera> ob_fechas = new List<DateCartelera>();

        //    XmlDocument ob_xmldoc = new XmlDocument();
        //    //Billboard ob_bilmov = new Billboard();
        //    General ob_fncgrl = new General();

        //    APIPortalKiosco.Entities.Cartelera ob_carprg = new APIPortalKiosco.Entities.Cartelera();
        //    Dictionary<string, object> ob_diclst = new Dictionary<string, object>();
        //    Dictionary<string, object> ob_lsala = new Dictionary<string, object>();
        //    List<sala> ob_lisprg = new List<sala>();


        //    //Obtener información de la web

        //    ob_carprg.Teatro = App.idCine;
        //    ob_carprg.tercero = App.ValorTercero;
        //    ob_carprg.IdPelicula = App.Pelicula.Id;
        //    ob_carprg.FcPelicula = App.Pelicula.FechaSel.Substring(3);//pr_tippel == "Preventa" ? pr_fecprg : ViewBag.Cartelera[0].FecSt;
        //    ob_carprg.TpPelicula = App.TipoSala == null ? "Normal" : App.TipoSala;
        //    ob_carprg.FgPelicula = "2";
        //    ob_carprg.CfPelicula = "No";

        //    //Generar y encriptar JSON para servicio PRE
        //    lc_srvpar = ob_fncgrl.JsonConverter(ob_carprg);
        //    lc_srvpar = lc_srvpar.Replace("teatro", "Teatro");
        //    lc_srvpar = lc_srvpar.Replace("idPelicula", "IdPelicula");
        //    lc_srvpar = lc_srvpar.Replace("fcPelicula", "FcPelicula");
        //    lc_srvpar = lc_srvpar.Replace("tpPelicula", "TpPelicula");
        //    lc_srvpar = lc_srvpar.Replace("fgPelicula", "FgPelicula");
        //    lc_srvpar = lc_srvpar.Replace("cfPelicula", "CfPelicula");

        //    //Encriptar Json
        //    lc_srvpar = ob_fncgrl.EncryptStringAES(lc_srvpar);

        //    //Consumir servicio

        //    lc_result = await ob_fncgrl.WebServicesAsync(string.Concat(App.ScoreServices, "scocar/"), lc_srvpar);
        //    if (lc_result.StartsWith("0"))
        //    {

        //        HashSet<string> fechasProcesadas = new HashSet<string>();
        //        HashSet<string> horasProcesadas = new HashSet<string>();
        //        //Validar respuesta
        //        if (lc_result.Substring(0, 1) == "0")
        //        {
        //            //Quitar switch
        //            lc_result = lc_result.Replace("0-", "");
        //            ob_diclst = (Dictionary<string, object>)JsonConvert.DeserializeObject(lc_result, (typeof(Dictionary<string, object>)));
        //            //ob_bilmov = (Billboard)JsonConvert.DeserializeObject(ob_diclst["Billboard"].ToString(), (typeof(Billboard)));
        //            ob_lsala = (Dictionary<string, object>)JsonConvert.DeserializeObject(ob_diclst["GetHora"].ToString(), (typeof(Dictionary<string, object>)));
        //            ob_lisprg = (List<sala>)JsonConvert.DeserializeObject(ob_lsala["Lsala"].ToString(), (typeof(List<sala>)));
        //            var Zonas = (Dictionary<string, string>)JsonConvert.DeserializeObject(ob_lsala["Zonas"].ToString(), (typeof(Dictionary<string, string>)));


        //            )

        //            foreach (var listProgramacion in listpelicula)
        //            {
        //                foreach (var listfecha in listProgramacion.DiasDisponibles)
        //                {
        //                    if (listfecha.fecunv != null)
        //                    {
        //                        foreach (var hora in listProgramacion.hora)
        //                        {
        //                            if (!horasProcesadas.Contains(hora.horario))
        //                            {
        //                                horasProcesadas.Add(hora.horario);

        //                                if (listProgramacion.tipoSala.Contains("GENERAL"))
        //                                {
        //                                    ContenedorHorasGeneral.Children.Add(CrearBotonHora(hora.horario, hora.militar));
        //                                }
        //                                if (listProgramacion.tipoSala.Contains("3D"))
        //                                {
        //                                    ContenedorHoras3D.Children.Add(CrearBotonHora(hora.horario, hora.militar));
        //                                }
        //                                if (listProgramacion.tipoSala.Contains("SUPERNOVA"))
        //                                {
        //                                    ContenedorHorasSupernova.Children.Add(CrearBotonHora(hora.horario, hora.militar));
        //                                }
        //                                if (listProgramacion.tipoSala.Contains("4DX"))
        //                                {
        //                                    ContenedorHoras4DX.Children.Add(CrearBotonHora(hora.horario, hora.militar));
        //                                }
        //                                if (listProgramacion.tipoSala.Contains("BLACK STAR"))
        //                                {
        //                                    ContenedorHorasBlackStar.Children.Add(CrearBotonHora(hora.horario, hora.militar));
        //                                }
        //                            }
        //                        }
        //                    }
        //                }

        //            }


        //            if (Zonas != null && Zonas.Count > 0 && ob_lisprg != null)
        //            {
        //                foreach (var itemZonas in Zonas)
        //                {
        //                    foreach (var item in ob_lisprg)
        //                    {
        //                        if (item.hora != null && item.hora.Count > 0)
        //                        {
        //                            foreach (var item2 in item.hora)
        //                            {
        //                                foreach (var item3 in item2.TipoZonaOld)
        //                                {
        //                                    if (itemZonas.Value == item3.nombreZona)
        //                                    {
        //                                        foreach (var item4 in item3.TipoSilla)
        //                                        {
        //                                            if (item4.Tarifa.Count > 0)
        //                                            {
        //                                                foreach (var item5 in item4.Tarifa)
        //                                                {
        //                                                    App.ValorTarifa = Convert.ToDecimal(item5.valor);
        //                                                    App.KeyTarifa = Convert.ToDecimal(item5.codigoTarifa);
        //                                                    App.NombreTarifa = item5.nombreTarifa + ";" + item5.valor.ToString();
        //                                                    App.TipoSilla = item4.nombreTipoSilla;
        //                                                }
        //                                            }
        //                                            else
        //                                            {
        //                                                if (item4.nombreTipoSilla != "Discapacitado")
        //                                                {
        //                                                    // Código relacionado con la ausencia de tarifas
        //                                                }
        //                                            }
        //                                        }
        //                                    }
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                // Código relacionado con la falta de datos en ViewBag
        //            }
        //        }
        //    }
        //}

        public async void CalcularTarifa()
        {
            try
            {
            
            int lc_keypel = 0;
            int lc_auxpel = 0;
            int lc_keytea = 0;
            int lc_auxtea = 0;
            int lc_swtflg = 0;
            string Variables41TPF = string.Empty;
            string lc_auxitem = string.Empty;
            string lc_fecitem = string.Empty;
            string lc_flgpre = "S";
            string pr_tippel = "";

            string lc_result = string.Empty;
            string lc_srvpar = string.Empty;

            DateTime dt_fecpro;

            List<DateCartelera> ob_fechas = new List<DateCartelera>();

            XmlDocument ob_xmldoc = new XmlDocument();
            //Billboard ob_bilmov = new Billboard();
            General ob_fncgrl = new General();

            APIPortalKiosco.Entities.Cartelera ob_carprg = new APIPortalKiosco.Entities.Cartelera();
            Dictionary<string, object> ob_diclst = new Dictionary<string, object>();
            Dictionary<string, object> ob_lsala = new Dictionary<string, object>();
            List<sala> ob_lisprg = new List<sala>();

            //Obtener información de la web
            var peliculaDias = App.Peliculas.Where(p => p.TituloOriginal == App.Pelicula.TituloOriginal);
            var fechaSel = App.Pelicula.FechaSel.Substring(3);
            var horaSel = App.Pelicula.HoraSel;
            if (peliculaDias != null)
            {
                foreach (var dias in peliculaDias)
                {
                    foreach (var fecha in dias.DiasDisponibles)
                    {
                        if (fecha.fecunv == fechaSel)
                        {
                            foreach (var hora in fecha.horafun)
                            {
                                if (horaSel == hora.horunv)
                                {
                                    App.Pelicula.Id = dias.Id;
                                    App.TipoSala = dias.tipoSala;
                                }
                            }
                        }
                    }
                }
            }

            ob_carprg.Teatro = App.idCine;
            ob_carprg.tercero = App.ValorTercero;
            ob_carprg.IdPelicula = App.Pelicula.Id;
            ob_carprg.FcPelicula = App.Pelicula.FechaSel.Substring(3);//pr_tippel == "Preventa" ? pr_fecprg : ViewBag.Cartelera[0].FecSt;
            ob_carprg.TpPelicula = App.TipoSala == null ? "Normal" : App.TipoSala;
            ob_carprg.FgPelicula = "2";
            ob_carprg.CfPelicula = "No";

            //Generar y encriptar JSON para servicio PRE
            lc_srvpar = ob_fncgrl.JsonConverter(ob_carprg);
            lc_srvpar = lc_srvpar.Replace("teatro", "Teatro");
            lc_srvpar = lc_srvpar.Replace("idPelicula", "IdPelicula");
            lc_srvpar = lc_srvpar.Replace("fcPelicula", "FcPelicula");
            lc_srvpar = lc_srvpar.Replace("tpPelicula", "TpPelicula");
            lc_srvpar = lc_srvpar.Replace("fgPelicula", "FgPelicula");
            lc_srvpar = lc_srvpar.Replace("cfPelicula", "CfPelicula");

            //Encriptar Json
            lc_srvpar = ob_fncgrl.EncryptStringAES(lc_srvpar);

            //Consumir servicio

            lc_result = await ob_fncgrl.WebServicesAsync(string.Concat(App.ScoreServices, "scocar/"), lc_srvpar);
            if (lc_result.StartsWith("0"))
            {

                HashSet<string> fechasProcesadas = new HashSet<string>();
                HashSet<string> horasProcesadas = new HashSet<string>();
                //Validar respuesta
                if (lc_result.Substring(0, 1) == "0")
                {
                    //Quitar switch
                    lc_result = lc_result.Replace("0-", "");
                    ob_diclst = (Dictionary<string, object>)JsonConvert.DeserializeObject(lc_result, (typeof(Dictionary<string, object>)));
                    //ob_bilmov = (Billboard)JsonConvert.DeserializeObject(ob_diclst["Billboard"].ToString(), (typeof(Billboard)));
                    ob_lsala = (Dictionary<string, object>)JsonConvert.DeserializeObject(ob_diclst["GetHora"].ToString(), (typeof(Dictionary<string, object>)));
                    ob_lisprg = (List<sala>)JsonConvert.DeserializeObject(ob_lsala["Lsala"].ToString(), (typeof(List<sala>)));
                    var Zonas = (Dictionary<string, string>)JsonConvert.DeserializeObject(ob_lsala["Zonas"].ToString(), (typeof(Dictionary<string, string>)));

                    if (Zonas != null && Zonas.Count > 0 && ob_lisprg != null)
                    {
                        foreach (var itemZonas in Zonas)
                        {
                            foreach (var item in ob_lisprg)
                            {
                                if (item.hora != null && item.hora.Count > 0)
                                {
                                    foreach (var item2 in item.hora)
                                    {
                                        if (item2.militar == App.Pelicula.HoraSel)
                                        {
                                            foreach (var item3 in item2.TipoZonaOld)
                                            {
                                                if (itemZonas.Value == item3.nombreZona)
                                                {
                                                    foreach (var item4 in item3.TipoSilla)
                                                    {
                                                        if (item4.Tarifa.Count > 0)
                                                        {
                                                            foreach (var item5 in item4.Tarifa)
                                                            {
                                                                App.ValorTarifa = Convert.ToDecimal(item5.valor);
                                                                App.KeyTarifa = Convert.ToDecimal(item5.codigoTarifa);
                                                                App.NombreTarifa = item5.nombreTarifa + ";" + item5.valor.ToString();
                                                                App.TipoSilla = item4.nombreTipoSilla;
                                                                App.Pelicula.numeroSala = item.numeroSala;
                                                                borSiguente.Visibility = Visibility.Visible;
                                                                App.Pelicula.HoraMilitar = item2.militar;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (item4.nombreTipoSilla != "Discapacitado")
                                                            {
                                                                // Código relacionado con la ausencia de tarifas
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        // Código relacionado con la falta de datos en ViewBag
                    }
                }
            }
            }
            catch (Exception e) {   }
        }


        private async void btnSelectFecha_Click(object sender, RoutedEventArgs e)
        {
            isThreadActive = false;
            Cartelera openWindows = new Cartelera();
            openWindows.Show();
            this.Close();
        }

        private void CrearBotonFecha(string fecha, string fecunv)
        {
            // Separar la fecha en sus partes
            string[] partesFecha = fecha.Split(' ');
            string diaSemana = partesFecha[0];
            string diaMes = partesFecha[2];
            string fechaFormateada = $"{diaSemana} {diaMes}";

            // Crear un nuevo botón y un nuevo borde
            Button btn = new Button();
            Border border = new Border();

            // Establecer propiedades del botón
            btn.Name = fecunv;
            btn.Content = fechaFormateada;
            btn.Background = Brushes.Transparent;
            btn.BorderThickness = new Thickness(0);
            btn.FontFamily = new FontFamily("Myanmar Khyay");
            btn.FontSize = 20;
            btn.Foreground = Brushes.Red;

            // Agregar efecto de sombra al botón
            DropShadowEffect shadowEffect = new DropShadowEffect();
            shadowEffect.Color = Colors.Gray;
            shadowEffect.Direction = 270;
            shadowEffect.ShadowDepth = 3;
            shadowEffect.Opacity = 0.5;
            btn.Effect = shadowEffect;

            // Establecer propiedades del borde
            border.Width = 115;
            border.Height = 60;
            border.Background = Brushes.White;
            border.BorderBrush = Brushes.Red;
            border.BorderThickness = new Thickness(1);
            border.CornerRadius = new CornerRadius(10);
            border.Margin = new Thickness(0, 0, 6, 0);
            border.Child = btn;

            if (App.IsFecha == false)
            {
                btn.Foreground = Brushes.White;
                border.Background = new SolidColorBrush(ColorConverter.ConvertFromString("#F30613") as Color? ?? Colors.Red);
                App.Pelicula.FechaSel = fecunv;
                //App.Pelicula.FechaUsuario = fechaFormateada;
                App.IsFecha = true;
            }

            // Manejador de eventos Click para el botón
            btn.Click += btnSeleccionFecha_Click;

            // Agregar el botón con su borde al contenedor de fechas
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
            //nuevoBoton.Style = FindResource("MyButton") as Style;

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
            //btn.Style = FindResource("MyButton") as Style; // Puedes cambiar "MyButton" por el nombre correcto del estilo
            btn.Foreground = Brushes.Red;


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
            btn.Click += btnSeleccionHora_Click;

            return border;

        }

        public T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parent = VisualTreeHelper.GetParent(child);

            while (parent != null)
            {
                if (parent is T)
                {
                    return (T)parent;
                }
                parent = VisualTreeHelper.GetParent(parent);
            }
            return null;
        }

        private void btnSeleccionHora_Click(object sender, RoutedEventArgs e)
        {

            CalcularTarifa();
            //ResetarStylos();
            string buttonName = "";
            Button clickedButton = sender as Button;
            buttonName = clickedButton.Name.ToString();
            App.Pelicula.HoraSel = clickedButton.Name.ToString().Substring(3);
            App.Pelicula.HoraUsuario = clickedButton.Content.ToString();
            // Buscar el Border padre del botón
            Border buttonBorder = FindParent<Border>(clickedButton);

            // Aquí tienes el Border al que pertenece el botón


            var wrapPanelName = "";
            var formato = "";


            var TituloOriginal = App.Pelicula.TituloOriginal;
            var DiasDisponibles = App.Pelicula.DiasDisponibles;
            var FechaSel = App.Pelicula.FechaSel;
            var HoraSel = App.Pelicula.HoraSel;
            var apppelicula = App.Pelicula;

            foreach (var pel in App.Peliculas)
            {
                if (pel.TituloOriginal == App.Pelicula.TituloOriginal)
                {
                    foreach (var diaSel in pel.DiasDisponibles)
                    {
                        if (App.Pelicula.FechaSel.Substring(3) == diaSel.fecunv)
                        {
                            foreach (var sala in diaSel.horafun)
                            {
                                if (App.Pelicula.Id == pel.Id)
                                {
                                    if (clickedButton.Content.ToString() == sala.horario)
                                    {
                                        App.NombreFec = diaSel.fecham;
                                        App.Imagen = pel.Imagen;
                                        App.Censura = pel.Censura;
                                        App.Pelicula.tipoSala = sala.tipSala;
                                        App.TipoSala = sala.tipSala;
                                        App.Pelicula.numeroSala = sala.numSala;
                                        App.Pelicula.HoraMilitar = sala.horunv;
                                        break;

                                    }
                                }
                            }

                        }
                    }
                }
            }
            Panel contenedor = ContenedorHorasGeneral;
            foreach (var control in contenedor.Children)
            {
                if (control is Border) // Si el control es un borde
                {

                    Border border = (Border)control;
                    // Verificar si el contenido del borde es un botón
                    if (border.Child is Button)
                    {

                        Button btn = (Button)border.Child;
                        // Almacenar el contenido y el nombre del botón
                        if (btn.Content.ToString() != "General")
                        {
                            // Aplicar estilos al borde
                            if (border.Name != "General")
                            {
                                border.Width = 115;
                                border.Height = 46;
                                border.Background = Brushes.White;
                                border.CornerRadius = new CornerRadius(5);
                                border.Margin = new Thickness(0, 0, 6, 7);
                                border.BorderThickness = new Thickness(1);
                                border.BorderBrush = Brushes.Black;
                            }

                            object contenido = btn.Content;
                            string nombre = btn.Name;

                            // Aplicar estilos al botón
                            btn.Width = 115;
                            btn.Height = 46;
                            btn.FontFamily = new FontFamily("Myanmar Khyay");
                            btn.FontSize = 14;
                            btn.Foreground = Brushes.Red; // Establecer el color del texto como rojo
                            btn.BorderBrush = Brushes.Black;
                        }
                    }
                }
            }

            Panel contenedor3d = ContenedorHoras3D;
            foreach (var control3d in contenedor3d.Children)
            {
                if (control3d is Border) // Si el control es un borde
                {

                    Border border = (Border)control3d;
                    // Verificar si el contenido del borde es un botón
                    if (border.Child is Button)
                    {

                        Button btn = (Button)border.Child;
                        // Almacenar el contenido y el nombre del botón
                        if (btn.Content.ToString().Contains("3D"))
                        {
                            // Aplicar estilos al borde
                            if (border.Name.Contains("3D"))
                            {
                                border.Width = 115;
                                border.Height = 46;
                                border.Background = Brushes.White;
                                border.CornerRadius = new CornerRadius(5);
                                border.Margin = new Thickness(0, 0, 6, 7);
                                border.BorderThickness = new Thickness(1);
                                border.BorderBrush = Brushes.Black;
                            }

                            object contenido = btn.Content;
                            string nombre = btn.Name;

                            // Aplicar estilos al botón
                            btn.Width = 115;
                            btn.Height = 46;
                            btn.FontFamily = new FontFamily("Myanmar Khyay");
                            btn.FontSize = 14;
                            btn.Foreground = Brushes.Red; // Establecer el color del texto como rojo
                            btn.BorderBrush = Brushes.Black;
                        }
                    }
                }
            }



            if (buttonBorder != null)
            {
                buttonBorder.BorderBrush = Brushes.Red;
                buttonBorder.Background = new SolidColorBrush(ColorConverter.ConvertFromString("#F30613") as Color? ?? Colors.Red);
            }
            //App.Pelicula.HoraUsuario = clickedButton.Content.ToString();

            clickedButton.Foreground = Brushes.White;
            CalcularTarifa();
        }


        private void btnSeleccionFecha_Click(object sender, RoutedEventArgs e)
        {

            string buttonName = "";
            Button clickedButton = sender as Button;
            buttonName = clickedButton.Name;

            // Buscar el Border padre del botón
            Border buttonBorder = FindParent<Border>(clickedButton);
            clickedButton.Foreground = Brushes.White;
            App.Pelicula.FechaSel = buttonName;
            // Limpiar contenedores y cargar nuevas fechas
            ContenedorHoras3D.Children.Clear();
            ContenedorHorasGeneral.Children.Clear();
            ContenedorHoras3D.Children.Add(CrearBotonFormato("3D"));
            ContenedorHorasGeneral.Children.Add(CrearBotonFormato("General"));
            Panel contenedor = ContenedorFechas;

            foreach (var control in contenedor.Children)
            {
                if (control is Border) // Si el control es un borde
                {
                    Border border = (Border)control;
                    // Verificar si el contenido del borde es un botón
                    if (border.Child is Button)
                    {

                        Button btn = (Button)border.Child;
                        // Almacenar el contenido y el nombre del botón
                        if (btn.Content.ToString() != "General")
                        {
                            // Aplicar estilos al borde
                            if (border.Name != "General")
                            {
                                border.Width = 115;
                                border.Height = 60;
                                border.Background = Brushes.White;
                                border.BorderBrush = Brushes.Red;
                                border.BorderThickness = new Thickness(1);
                                border.CornerRadius = new CornerRadius(10);
                                border.Margin = new Thickness(0, 0, 6, 0);
                                border.BorderBrush = Brushes.Black;
                            }

                            object contenido = btn.Content;
                            string nombre = btn.Name;
                            btn.Background = Brushes.Transparent;
                            btn.BorderThickness = new Thickness(0);
                            btn.FontFamily = new FontFamily("Myanmar Khyay");
                            btn.FontSize = 20;
                            btn.Foreground = Brushes.Red;
                        }
                    }
                }
            }
            if (buttonBorder != null)
            {
                buttonBorder.BorderBrush = Brushes.Red;
                buttonBorder.Background = new SolidColorBrush(ColorConverter.ConvertFromString("#F30613") as Color? ?? Colors.Red);
            }
            //App.Pelicula.FechaUsuario = clickedButton.Content.ToString();
            clickedButton.Foreground = Brushes.White;
            CargarFechasDesdeSelect();
        }

        private async void btnSalir_Click(object sender, RoutedEventArgs e)
        {
            isThreadActive = false;
            App.IsBoleteriaConfiteria = false;
            Principal openWindows = new Principal();
            openWindows.Show();
            this.Close();
        }
    }
}