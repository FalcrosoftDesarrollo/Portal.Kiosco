using APIPortalKiosco.Entities;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Xml;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace Portal.Kiosco.Properties.Views
{
    public partial class SeleccionarFuncion : Window
    {
        public List<UIElement> elementosConservados3D;
        public List<UIElement> elementosConservadosGeneral;
        private readonly IOptions<MyConfig> config;
        private bool isThreadActive = true;
        private bool errorgeneral = false;
        private HashSet<string> fechasProcesadas = new HashSet<string>();
        private HashSet<string> horasProcesadas = new HashSet<string>();
        public SeleccionarFuncion()
        {
            InitializeComponent();
            DataContext = ((App)Application.Current);
            CargarFechasDesdeXml();
            imgPelicula.Source = new BitmapImage(new Uri(App.Pelicula.Imagen));
            lblClasificacion.Content = "Clasificación: " + App.Pelicula.Censura;
            lblNombre.Content = App.Pelicula.TituloOriginal;
            lblnombre.IsEnabled = false;
            lblDuracion.Content = "Duración: " + App.Pelicula.Duracion + " min";
            lblGenero.Content = "Genero: " + App.Pelicula.Genero;
            //App.IsFecha = false;
            if (App.ob_diclst.Count > 0)
            {
                lblnombre.Content = "!HOLA " + App.ob_diclst["Nombre"].ToString() + " " + App.ob_diclst["Apellido"].ToString() + "!";
            }
            else
            {
                lblnombre.Content = "!HOLA INVITADO¡";
            }

            DoubleAnimation fadeInAnimation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(3));
            gridPrincipal.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);
            Thread thread = new Thread(() =>
            {
                while (isThreadActive)
                {
                    if (ComprobarTiempo())
                    {
                        break;
                    }
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

        public void CargarFechasDesdeSelect()
        {
            borSiguente.Visibility = Visibility.Hidden;
            HashSet<string> zonasProcesadas = new HashSet<string>();
            int isfecha = 0;


            LimpiarContenedores();
            InicializarBotonesFormato();

            var fechasel = App.Pelicula.FechaSel.Substring(3);

            var listpelicula = App.Peliculas.Where(x => x.TituloOriginal == App.Pelicula.TituloOriginal.ToString()).ToList();
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
            General ob_fncgrl = new General();
            APIPortalKiosco.Entities.Cartelera ob_carprg = new APIPortalKiosco.Entities.Cartelera();
            Dictionary<string, object> ob_diclst = new Dictionary<string, object>();
            Dictionary<string, object> ob_lsala = new Dictionary<string, object>();
            List<sala> ob_lisprg = new List<sala>();

            fechasProcesadas = new HashSet<string>();
            horasProcesadas = new HashSet<string>();
            foreach (var pelicula in listpelicula)
            {
                horasProcesadas.Clear();
                foreach (var fechas in pelicula.DiasDisponibles.Where(x => x.fecunv == fechasel).OrderByDescending(or => or.fecunv))
                {
                    ob_carprg.Teatro = App.idCine;
                    ob_carprg.tercero = App.ValorTercero;
                    ob_carprg.IdPelicula = pelicula.Id;
                    ob_carprg.FcPelicula = fechasel;
                    ob_carprg.TpPelicula = fechas.horafun.FirstOrDefault().tipSala; //   App.TipoSala == null ? "Normal" : App.TipoSala;
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

                    lc_result = ob_fncgrl.WebServices(string.Concat(App.ScoreServices, "scocar/"), lc_srvpar);
                    if (lc_result.StartsWith("0"))
                    {
                        if (lc_result.StartsWith("0"))
                        {
                            if (lc_result.StartsWith("0"))
                            {

                            }
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
                                    foreach (var item in ob_lisprg)
                                    {
                                        foreach (var itemZonas in Zonas)
                                        {

                                            if (item.hora != null && item.hora.Count > 0)
                                            {
                                                foreach (var item2 in item.hora)
                                                {
                                                    DateTime FechaHoraInicio = Convert.ToDateTime(DateTime.Now.ToString("dd/MM/yyyy") + " " + item2.militar.Substring(0, 2) + ":" + item2.militar.Substring(2, 2) + ":00");
                                                    DateTime FechaHoraTermino = DateTime.ParseExact(DateTime.Now.ToString("HH:mm"), "HH:mm", System.Globalization.CultureInfo.InvariantCulture);
                                                    foreach (var item3 in item2.TipoZonaOld)
                                                    {
                                                        if (DateTime.Now.ToString("yyyyMMdd") == App.Pelicula.FechaSel)
                                                        {
                                                            if (App.MinDifHora != "0")
                                                            {
                                                                //Diferencia de tiempo entre hora funcion y hora del dia 
                                                                TimeSpan diferencia = FechaHoraInicio - FechaHoraTermino;
                                                                var diferenciaenminutos = diferencia.TotalMinutes;

                                                                if (diferenciaenminutos > Convert.ToDouble(App.MinDifHora))
                                                                {
                                                                    if (pelicula.Formato == "Digital 3D")
                                                                    {
                                                                        if (item.tipoSala.Contains("GENERAL"))
                                                                        {
                                                                            if (!horasProcesadas.Contains(item2.horario))
                                                                                ContenedorHoras3D.Children.Add(CrearBotonHora(item2.horario, item2.militar, "GENERAL3D"));
                                                                        }
                                                                        if (item.tipoSala.Contains("Black Star"))
                                                                        {
                                                                            if (!horasProcesadas.Contains(item2.horario))
                                                                                ContenedorHorasBlackStar.Children.Add(CrearBotonHora(item2.horario, item2.militar, "BlackStar"));
                                                                        }
                                                                        if (item.tipoSala.Contains("Black Star 3D"))
                                                                        {
                                                                            if (!horasProcesadas.Contains(item2.horario))
                                                                                ContenedorHoras3DBlackStar.Children.Add(CrearBotonHora(item2.horario, item2.militar, "EDBlackStar"));
                                                                        }
                                                                        if (item.tipoSala.Contains("SUPERNOVA"))
                                                                        {
                                                                            if (!horasProcesadas.Contains(item2.horario))
                                                                                ContenedorHoras3DSuperNova.Children.Add(CrearBotonHora(item2.horario, item2.militar, "EDBlackStar"));
                                                                        }
                                                                    }

                                                                    if (pelicula.Formato == "Digital 2D")
                                                                    {
                                                                        if (item.tipoSala.Contains("GENERAL"))
                                                                        {
                                                                            if (!horasProcesadas.Contains(item2.horario))
                                                                                ContenedorHorasGeneral.Children.Add(CrearBotonHora(item2.horario, item2.militar, itemZonas.Key));
                                                                        }
                                                                        if (item.tipoSala.Contains("Black Star"))
                                                                        {
                                                                            if (!horasProcesadas.Contains(item2.horario))
                                                                                ContenedorHorasBlackStar.Children.Add(CrearBotonHora(item2.horario, item2.militar, "BlackStar"));
                                                                        }
                                                                        if (item.tipoSala.Contains("Black Star 3D"))
                                                                        {
                                                                            if (!horasProcesadas.Contains(item2.horario))
                                                                                ContenedorHoras3DBlackStar.Children.Add(CrearBotonHora(item2.horario, item2.militar, "EDBlackStar"));
                                                                        }
                                                                        if (item.tipoSala.Contains("SUPERNOVA"))
                                                                        {
                                                                            if (!horasProcesadas.Contains(item2.horario))
                                                                                ContenedorHorasSupernova.Children.Add(CrearBotonHora(item2.horario, item2.militar, "EDBlackStar"));
                                                                        }
                                                                    }
                                                                    horasProcesadas.Add(item2.horario);
                                                                    zonasProcesadas.Add(itemZonas.Key);
                                                                }
                                                            }
                                                            isfecha = 1;
                                                        }
                                                        else
                                                        {
                                                            if (isfecha == 0)
                                                            {
                                                                if (pelicula.Formato == "Digital 3D")
                                                                {
                                                                    if (item.tipoSala.Contains("GENERAL"))
                                                                    {
                                                                        if (!horasProcesadas.Contains(item2.horario))
                                                                            ContenedorHoras3D.Children.Add(CrearBotonHora(item2.horario, item2.militar, "GENERAL3D"));
                                                                    }
                                                                    if (item.tipoSala.Contains("Black Star"))
                                                                    {
                                                                        if (!horasProcesadas.Contains(item2.horario))
                                                                            ContenedorHorasBlackStar.Children.Add(CrearBotonHora(item2.horario, item2.militar, "BlackStar"));
                                                                    }
                                                                    if (item.tipoSala.Contains("Black Star 3D"))
                                                                    {
                                                                        if (!horasProcesadas.Contains(item2.horario))
                                                                            ContenedorHoras3DBlackStar.Children.Add(CrearBotonHora(item2.horario, item2.militar, "EDBlackStar"));
                                                                    }
                                                                    if (item.tipoSala.Contains("SUPERNOVA"))
                                                                    {
                                                                        if (!horasProcesadas.Contains(item2.horario))
                                                                            ContenedorHoras3DSuperNova.Children.Add(CrearBotonHora(item2.horario, item2.militar, "Supernova"));
                                                                    }
                                                                    if (item.tipoSala.Contains("4DX"))
                                                                    {
                                                                        if (!horasProcesadas.Contains(item2.horario))
                                                                            ContenedorHoras3D4DX.Children.Add(CrearBotonHora(item2.horario, item2.militar, "P4DX"));
                                                                    }
                                                                }

                                                                if (pelicula.Formato == "Digital 2D")
                                                                {
                                                                    if (item.tipoSala.Contains("GENERAL"))
                                                                    {
                                                                        if (!horasProcesadas.Contains(item2.horario))
                                                                            ContenedorHorasGeneral.Children.Add(CrearBotonHora(item2.horario, item2.militar, itemZonas.Key));
                                                                    }
                                                                    if (item.tipoSala.Contains("Black Star"))
                                                                    {
                                                                        if (!horasProcesadas.Contains(item2.horario))
                                                                            ContenedorHorasBlackStar.Children.Add(CrearBotonHora(item2.horario, item2.militar, "BlackStar"));
                                                                    }
                                                                    if (item.tipoSala.Contains("Black Star 3D"))
                                                                    {
                                                                        if (!horasProcesadas.Contains(item2.horario))
                                                                            ContenedorHoras3DBlackStar.Children.Add(CrearBotonHora(item2.horario, item2.militar, "EDBlackStar"));
                                                                    }
                                                                    if (item.tipoSala.Contains("SUPERNOVA"))
                                                                    {
                                                                        if (!horasProcesadas.Contains(item2.horario))
                                                                            ContenedorHorasSupernova.Children.Add(CrearBotonHora(item2.horario, item2.militar, "Supernova"));
                                                                    }
                                                                    if (item.tipoSala.Contains("4DX"))
                                                                    {
                                                                        if (!horasProcesadas.Contains(item2.horario))
                                                                            ContenedorHoras4DX.Children.Add(CrearBotonHora(item2.horario, item2.militar, "P4DX"));
                                                                    }
                                                                }
                                                                horasProcesadas.Add(item2.horario);
                                                                zonasProcesadas.Add(itemZonas.Key);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            //}
                        }
                    }
                }
            }
        }


        public void CalcularTarifa()
        {
            var tituloriginal = App.Pelicula.TituloOriginal;
            var fechaSel = App.Pelicula.FechaSel.Substring(3);
            var horaSel = App.Pelicula.HoraSel;
            var horaMilitar = App.Pelicula.HoraMilitar ?? App.Pelicula.HoraSel;
            var peliculaDias = App.Peliculas.Where(p => p.TituloOriginal == tituloriginal).ToList();

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
                                App.TipoSala = hora.tipSala;
                                App.TipoSilla = hora.tipSala;
                                break;
                            }
                        }
                    }
                }
            }

            var ob_carprg = new APIPortalKiosco.Entities.Cartelera
            {
                Teatro = App.idCine,
                tercero = App.ValorTercero,
                IdPelicula = App.Pelicula.Id,
                FcPelicula = fechaSel,
                TpPelicula = App.TipoSala ?? "Normal",
                FgPelicula = "2",
                CfPelicula = "No"
            };

            var ob_fncgrl = new General();
            var lc_srvpar = ob_fncgrl.JsonConverter(ob_carprg)
                .Replace("teatro", "Teatro")
                .Replace("idPelicula", "IdPelicula")
                .Replace("fcPelicula", "FcPelicula")
                .Replace("tpPelicula", "TpPelicula")
                .Replace("fgPelicula", "FgPelicula")
                .Replace("cfPelicula", "CfPelicula");

            lc_srvpar = ob_fncgrl.EncryptStringAES(lc_srvpar);
            var lc_result = ob_fncgrl.WebServices($"{App.ScoreServices}scocar/", lc_srvpar);

            if (lc_result.StartsWith("0"))
            {
                lc_result = lc_result.Replace("0-", "");
                var ob_diclst = JsonConvert.DeserializeObject<Dictionary<string, object>>(lc_result);
                if (ob_diclst != null && ob_diclst.TryGetValue("GetHora", out var getHoraObj))
                {
                    var ob_lsala = JsonConvert.DeserializeObject<Dictionary<string, object>>(getHoraObj.ToString());
                    if (ob_lsala != null && ob_lsala.TryGetValue("Lsala", out var lsalaObj) && ob_lsala.TryGetValue("Zonas", out var zonasObj))
                    {
                        var ob_lisprg = JsonConvert.DeserializeObject<List<sala>>(lsalaObj.ToString());
                        var Zonas = JsonConvert.DeserializeObject<Dictionary<string, string>>(zonasObj.ToString());

                        if (Zonas != null && Zonas.Count > 0 && ob_lisprg != null)
                        {
                            foreach (var item in ob_lisprg)
                            {
                                if (item.tipoSala.ToLower() == App.TipoSilla.ToLower())
                                {
                                    if (item.hora != null && item.hora.Count > 0)
                                    {
                                        foreach (var item2 in item.hora.Where(x => x.militar == horaMilitar))
                                        {
                                            //if (item2.militar == horaMilitar)
                                            //{
                                            foreach (var item3 in item2.TipoZonaOld)
                                            {
                                                if (Zonas.Values.Contains(item3.nombreZona))
                                                {
                                                    foreach (var item4 in item3.TipoSilla)
                                                    {
                                                        if (item4.Tarifa.Count > 0)
                                                        {
                                                            foreach (var item5 in item4.Tarifa)
                                                            {
                                                                App.ValorTarifa = Convert.ToDecimal(item5.valor);
                                                                App.KeyTarifa = Convert.ToDecimal(item5.codigoTarifa);
                                                                App.NombreTarifa = $"{item5.nombreTarifa};{item5.valor}";
                                                                App.TipoSilla = item4.nombreTipoSilla;
                                                                App.Pelicula.numeroSala = item.numeroSala;
                                                                borSiguente.Visibility = Visibility.Visible;
                                                                App.Pelicula.HoraMilitar = item2.militar;
                                                                errorgeneral = true;
                                                                break;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            // Manejo de error: la función no tiene una tarifa asignada
                                                            break;
                                                        }
                                                    }
                                                }
                                            }
                                            //}
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private async void CargarFechasDesdeXml()
        {
            LimpiarContenedores();
            InicializarBotonesFormato();

            fechasProcesadas = new HashSet<string>();
            horasProcesadas = new HashSet<string>();
            var totalfecha = 0;
            int contador = 0;
            try
            {
                foreach (var pelicula in App.Peliculas.Where(x => x.TituloOriginal == App.Pelicula.TituloOriginal))
                {
                    contador = 0;
                    string id = pelicula.Id;
                    string formato = pelicula.Formato;

                    foreach (var fecha in pelicula.DiasDisponibles.Distinct())
                    {
                        if (!fechasProcesadas.Contains(fecha.fecunv) && totalfecha < 7)
                        {
                            CrearBotonFecha($"{fecha.fecham}", "btn" + fecha.fecunv);
                            fechasProcesadas.Add(fecha.fecunv);
                            totalfecha++;
                        }

                        var ob_carprg = CrearCartelera(id, fecha.fecunv, formato);
                        //formato = ob_carprg.TpPelicula == "Normal" ? "GENERAL" : ob_carprg.TpPelicula;
                        var lc_srvpar = PrepararParametros(ob_carprg);


                        if (contador == 0)
                        {
                            var lc_result = await new General().WebServicesAsync(string.Concat(App.ScoreServices, "scocar/"), lc_srvpar);
                            if (lc_result.StartsWith("0"))
                            {
                                ProcesarResultado(lc_result, formato, fecha.fecunv);
                            }
                        }
                        contador++;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }
        }

        private void LimpiarContenedores()
        {
            ContenedorHorasGeneral.Children.Clear();
            ContenedorHoras3D.Children.Clear();
            ContenedorHorasBlackStar.Children.Clear();
            ContenedorHoras3DBlackStar.Children.Clear();
            ContenedorHorasSupernova.Children.Clear();
            ContenedorHoras3DSuperNova.Children.Clear();
            ContenedorHoras3D4DX.Children.Clear();
            ContenedorHoras4DX.Children.Clear();
        }

        private void InicializarBotonesFormato()
        {
            ContenedorHoras3D.Children.Add(CrearBotonFormato("3D"));
            ContenedorHorasSupernova.Children.Add(CrearBotonFormato("Supernova"));
            ContenedorHoras3DSuperNova.Children.Add(CrearBotonFormato("Supernova 3D"));
            ContenedorHorasGeneral.Children.Add(CrearBotonFormato("General"));
            ContenedorHorasBlackStar.Children.Add(CrearBotonFormato("Black Star"));
            ContenedorHoras3DBlackStar.Children.Add(CrearBotonFormato("Black Star 3D"));

            ContenedorHoras3D4DX.Children.Add(CrearBotonFormato("3D 4DX"));
            ContenedorHoras4DX.Children.Add(CrearBotonFormato("4DX"));
        }

        private APIPortalKiosco.Entities.Cartelera CrearCartelera(string idPelicula, string fcPelicula, string formato)
        {
            return new APIPortalKiosco.Entities.Cartelera
            {
                Teatro = App.idCine,
                tercero = App.ValorTercero,
                IdPelicula = idPelicula,
                FcPelicula = fcPelicula,
                TpPelicula = App.TipoSala ?? "Normal",
                FgPelicula = "2",
                CfPelicula = "No"
            };
        }

        private string PrepararParametros(APIPortalKiosco.Entities.Cartelera ob_carprg)
        {
            var ob_fncgrl = new General();
            var lc_srvpar = ob_fncgrl.JsonConverter(ob_carprg);
            lc_srvpar = lc_srvpar.Replace("teatro", "Teatro")
                                 .Replace("idPelicula", "IdPelicula")
                                 .Replace("fcPelicula", "FcPelicula")
                                 .Replace("tpPelicula", "TpPelicula")
                                 .Replace("fgPelicula", "FgPelicula")
                                 .Replace("cfPelicula", "CfPelicula");

            return ob_fncgrl.EncryptStringAES(lc_srvpar);
        }

        private void ProcesarResultado(string lc_result, string formato, string fechaUnv)
        {
            lc_result = lc_result.Replace("0-", "");
            var ob_diclst = JsonConvert.DeserializeObject<Dictionary<string, object>>(lc_result);
            var ob_lsala = JsonConvert.DeserializeObject<Dictionary<string, object>>(ob_diclst["GetHora"].ToString());
            var ob_lisprg = JsonConvert.DeserializeObject<List<sala>>(ob_lsala["Lsala"].ToString());
            var Zonas = JsonConvert.DeserializeObject<Dictionary<string, string>>(ob_lsala["Zonas"].ToString());

            if (Zonas != null && Zonas.Count > 0 && ob_lisprg != null)
            {
                foreach (var item in ob_lisprg)
                {
                    foreach (var itemZonas in Zonas)
                    {
                        ProcesarZonas(item, itemZonas, formato, fechaUnv);
                    }
                }
            }
        }

        private void ProcesarZonas(sala item, KeyValuePair<string, string> itemZonas, string formato, string fechaUnv)
        {
            if (item.hora != null && item.hora.Count > 0)
            {
                foreach (var item2 in item.hora.OrderBy(x => x.horario))
                {
                    DateTime FechaHoraInicio = DateTime.ParseExact(DateTime.Now.ToString("dd/MM/yyyy") + " " + item2.militar.Substring(0, 2) + ":" + item2.militar.Substring(2, 2) + ":00", "dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                    DateTime FechaHoraTermino = DateTime.ParseExact(DateTime.Now.ToString("HH:mm"), "HH:mm", System.Globalization.CultureInfo.InvariantCulture);

                    foreach (var item3 in item2.TipoZonaOld)
                    {
                        if (DateTime.Now.ToString("yyyyMMdd") == fechaUnv)
                        {
                            if (App.MinDifHora != "0")
                            {
                                TimeSpan diferencia = FechaHoraInicio - FechaHoraTermino;
                                if (diferencia.TotalMinutes > Convert.ToDouble(App.MinDifHora))
                                {
                                    AgregarBotonesPorFormato(item, item2.horario, item2.militar, formato, itemZonas.Key);
                                }
                            }
                        }
                        else
                        {
                            AgregarBotonesPorFormato(item, item2.horario, item2.militar, formato, itemZonas.Key);
                        }
                    }
                }
            }
        }

        private void AgregarBotonesPorFormato(dynamic item3, string horario, string militar, string formato, string zonaKey)
        {
            //if (!horasProcesadas.Contains(horario))
            //{
                if (formato == "Digital 3D")
                {
                    if (item3.tipoSala.Contains("GENERAL"))
                    {
                    //if (!horasProcesadas.Contains(horario))
                        ContenedorHoras3D.Children.Add(CrearBotonHora(horario, militar, "GENERAL3D"));
                    }
                    if (item3.tipoSala.Contains("Black Star"))
                    {
                    //if (!horasProcesadas.Contains(horario))
                        ContenedorHorasBlackStar.Children.Add(CrearBotonHora(horario, militar, "BlackStar"));
                    }
                    if (item3.tipoSala.Contains("Black Star 3D"))
                    {
                    //if (!horasProcesadas.Contains(horario))
                        ContenedorHoras3DBlackStar.Children.Add(CrearBotonHora(horario, militar, "EDBlackStar"));
                    }
                    if (item3.tipoSala.Contains("SUPERNOVA"))
                    {
                    //if (!horasProcesadas.Contains(horario))
                        ContenedorHoras3DSuperNova.Children.Add(CrearBotonHora(horario, militar, "SUPERNOVA"));
                    }
                    if (item3.tipoSala.Contains("4DX"))
                    {
                    //if (!horasProcesadas.Contains(horario))
                        ContenedorHoras3D4DX.Children.Add(CrearBotonHora(horario, militar, "P4DX"));
                    }
                }

                if (formato == "Digital 2D")
                {
                    if (item3.tipoSala.Contains("GENERAL"))
                    {
                    //if (!horasProcesadas.Contains(horario))
                        ContenedorHorasGeneral.Children.Add(CrearBotonHora(horario, militar, zonaKey));
                    }
                    if (item3.tipoSala.Contains("Black Star"))
                    {
                    //if (!horasProcesadas.Contains(horario))
                        ContenedorHorasBlackStar.Children.Add(CrearBotonHora(horario, militar, "BlackStar"));
                    }
                    if (item3.tipoSala.Contains("Black Star 3D"))
                    {
                    //if (!horasProcesadas.Contains(horario))
                        ContenedorHoras3DBlackStar.Children.Add(CrearBotonHora(horario, militar, "EDBlackStar"));
                    }
                    if (item3.tipoSala.Contains("SUPERNOVA"))
                    {
                    //if (!horasProcesadas.Contains(horario))
                        ContenedorHorasSupernova.Children.Add(CrearBotonHora(horario, militar, "Supernova"));
                    }
                    if (item3.tipoSala.Contains("4DX"))
                    {
                    //if (!horasProcesadas.Contains(horario))
                        ContenedorHoras4DX.Children.Add(CrearBotonHora(horario, militar, "P4DX"));
                    }
                }

                horasProcesadas.Add(horario);
            //}
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
            string[] partesFecha = fecha.Split(' ');
            string diaSemana = partesFecha[0];
            string diaMes = partesFecha[2];
            string fechaFormateada = $"{diaSemana} {diaMes}";

            Button btn = new Button();
            Border border = new Border();

            btn.Name = fecunv;
            btn.Content = fechaFormateada;
            btn.Background = Brushes.Transparent;
            btn.BorderThickness = new Thickness(0);
            btn.FontFamily = new FontFamily("Myanmar Khyay");
            btn.FontSize = 20;
            btn.Foreground = Brushes.Red;

            DropShadowEffect shadowEffect = new DropShadowEffect();
            shadowEffect.Color = Colors.Gray;
            shadowEffect.Direction = 270;
            shadowEffect.ShadowDepth = 3;
            shadowEffect.Opacity = 0.5;
            btn.Effect = shadowEffect;

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
                App.IsFecha = true;
            }

            btn.Click += btnSeleccionFecha_Click;

            ContenedorFechas.Children.Add(border);
        }

        public Border CrearBotonFormato(string ContenidoFormato)
        {
            Button nuevoBoton = new Button();
            nuevoBoton.Content = ContenidoFormato;
            nuevoBoton.Background = Brushes.Transparent;
            nuevoBoton.BorderThickness = new Thickness(0);
            nuevoBoton.Foreground = Brushes.Black;
            nuevoBoton.FontFamily = new FontFamily("Myanmar Khyay");
            nuevoBoton.FontSize = 14;
            nuevoBoton.HorizontalContentAlignment = HorizontalAlignment.Center;

            Border nuevoBorder = new Border();
            nuevoBorder.CornerRadius = new CornerRadius(10);
            nuevoBorder.Background = Brushes.White;
            nuevoBorder.Margin = new Thickness(0, 7, 6, 5);
            nuevoBorder.Width = 115;
            nuevoBorder.Height = 46;
            nuevoBorder.Child = nuevoBoton;
            nuevoBorder.Name = "border" + ContenidoFormato.Replace(" ", "");

            return nuevoBorder;
        }

        private Border CrearBotonHora(string hora, string idFuncion, string tipotarifa)
        {
            Button btn = new Button();
            btn.Name = "btn" + idFuncion;
            btn.Content = hora;
            btn.Width = 115;
            btn.Height = 46;
            btn.FontFamily = new FontFamily("Myanmar Khyay");
            btn.FontSize = 14;
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
            border.Name = tipotarifa.Replace(" ", "_");

            DropShadowEffect shadowEffect = new DropShadowEffect();
            shadowEffect.Color = Colors.Gray;
            shadowEffect.Direction = 270;
            shadowEffect.ShadowDepth = 3;
            shadowEffect.Opacity = 0.5;
            btn.Effect = shadowEffect;
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
            string buttonName = "";
            Button clickedButton = sender as Button;
            buttonName = clickedButton.Name.ToString();
            App.Pelicula.HoraSel = clickedButton.Name.ToString().Substring(3);
            App.Pelicula.HoraUsuario = clickedButton.Content.ToString();
            Border buttonBorder = FindParent<Border>(clickedButton);

            var wrapPanelName = "";
            var formato = "";
            var TituloOriginal = App.Pelicula.TituloOriginal;
            var DiasDisponibles = App.Peliculas.Where(p => p.TituloOriginal == TituloOriginal).ToList();
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
                if (control is Border)
                {
                    Border border = (Border)control;
                    if (border.Child is Button)
                    {
                        Button btn = (Button)border.Child;
                        if (btn.Content.ToString() != "General")
                        {
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

                            btn.Width = 115;
                            btn.Height = 46;
                            btn.FontFamily = new FontFamily("Myanmar Khyay");
                            btn.FontSize = 14;
                            btn.Foreground = Brushes.Red;
                            btn.BorderBrush = Brushes.Black;
                        }
                    }
                }
            }

            Panel contenedor3d = ContenedorHoras3D;

            foreach (var control3d in contenedor3d.Children)
            {
                if (control3d is Border)
                {
                    Border border = (Border)control3d;
                    if (border.Child is Button)
                    {
                        Button btn = (Button)border.Child;
                        if (!btn.Content.ToString().Contains("3D"))
                        {
                            if (!border.Name.Replace(" ", "").Contains("3D"))
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

                            btn.Width = 115;
                            btn.Height = 46;
                            btn.FontFamily = new FontFamily("Myanmar Khyay");
                            btn.FontSize = 14;
                            btn.Foreground = Brushes.Red;
                            btn.BorderBrush = Brushes.Black;
                        }
                    }
                }
            }

            Panel contenedorBlackStar = ContenedorHorasBlackStar;

            foreach (var controlBlackStar in contenedorBlackStar.Children)
            {
                if (controlBlackStar is Border)
                {
                    Border border = (Border)controlBlackStar;
                    if (border.Child is Button)
                    {
                        Button btn = (Button)border.Child;
                        if (!btn.Content.ToString().Contains("BlackStar"))
                        {
                            if (!border.Name.Replace(" ", "").Contains("BlackStar"))
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

                            btn.Width = 115;
                            btn.Height = 46;
                            btn.FontFamily = new FontFamily("Myanmar Khyay");
                            btn.FontSize = 14;
                            btn.Foreground = Brushes.Red;
                            btn.BorderBrush = Brushes.Black;
                        }
                    }
                }
            }


            Panel contenedor3DBlackStar = ContenedorHoras3DBlackStar;

            foreach (var control3DBlackStar in contenedor3DBlackStar.Children)
            {
                if (control3DBlackStar is Border)
                {
                    Border border = (Border)control3DBlackStar;
                    if (border.Child is Button)
                    {
                        Button btn = (Button)border.Child;
                        if (!btn.Content.ToString().Contains("BlackStar3D"))
                        {
                            if (!border.Name.Replace(" ", "").Contains("BlackStar3D"))
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

                            btn.Width = 115;
                            btn.Height = 46;
                            btn.FontFamily = new FontFamily("Myanmar Khyay");
                            btn.FontSize = 14;
                            btn.Foreground = Brushes.Red;
                            btn.BorderBrush = Brushes.Black;
                        }
                    }
                }
            }

            Panel contenedorHoras3DSuperNova = ContenedorHoras3DSuperNova;

            foreach (var control3DBlackStar in contenedorHoras3DSuperNova.Children)
            {
                if (control3DBlackStar is Border)
                {
                    Border border = (Border)control3DBlackStar;
                    if (border.Child is Button)
                    {
                        Button btn = (Button)border.Child;
                        if (!btn.Content.ToString().Contains("EDSupernova"))
                        {
                            if (!border.Name.Replace(" ", "").Contains("EDSupernova"))
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

                            btn.Width = 115;
                            btn.Height = 46;
                            btn.FontFamily = new FontFamily("Myanmar Khyay");
                            btn.FontSize = 14;
                            btn.Foreground = Brushes.Red;
                            btn.BorderBrush = Brushes.Black;
                        }
                    }
                }
            }

            Panel contenedorHorasSuperNova = ContenedorHorasSupernova;

            foreach (var control3DBlackStar in contenedorHorasSuperNova.Children)
            {
                if (control3DBlackStar is Border)
                {
                    Border border = (Border)control3DBlackStar;
                    if (border.Child is Button)
                    {
                        Button btn = (Button)border.Child;
                        if (!btn.Content.ToString().Contains("Supernova"))
                        {
                            if (!border.Name.Replace(" ", "").Contains("Supernova"))
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

                            btn.Width = 115;
                            btn.Height = 46;
                            btn.FontFamily = new FontFamily("Myanmar Khyay");
                            btn.FontSize = 14;
                            btn.Foreground = Brushes.Red;
                            btn.BorderBrush = Brushes.Black;
                        }
                    }
                }
            }

            Panel contenedorHoras3D4DX = ContenedorHoras3D4DX;

            foreach (var control3DBlackStar in contenedorHoras3D4DX.Children)
            {
                if (control3DBlackStar is Border)
                {
                    Border border = (Border)control3DBlackStar;
                    if (border.Child is Button)
                    {
                        Button btn = (Button)border.Child;
                        if (!btn.Content.ToString().Replace(" ", "").Contains("3D4DX"))
                        {
                            if (!border.Name.Replace(" ", "").Replace("border","").Contains("3D4DX"))
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

                            btn.Width = 115;
                            btn.Height = 46;
                            btn.FontFamily = new FontFamily("Myanmar Khyay");
                            btn.FontSize = 14;
                            btn.Foreground = Brushes.Red;
                            btn.BorderBrush = Brushes.Black;
                        }
                    }
                }
            }

          

            Panel contenedorHoras4DX = ContenedorHoras4DX;

            foreach (var control3DBlackStar in ContenedorHoras4DX.Children)
            {
                if (control3DBlackStar is Border)
                {
                    Border border = (Border)control3DBlackStar;
                    if (border.Child is Button)
                    {
                        Button btn = (Button)border.Child;
                        if (!btn.Content.ToString().Contains("4DX"))
                        {
                            if (!border.Name.Replace(" ", "").Contains("4DX"))
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

                            btn.Width = 115;
                            btn.Height = 46;
                            btn.FontFamily = new FontFamily("Myanmar Khyay");
                            btn.FontSize = 14;
                            btn.Foreground = Brushes.Red;
                            btn.BorderBrush = Brushes.Black;
                        }
                    }
                }
            }



            if (buttonBorder != null)
            {
                buttonBorder.BorderBrush = Brushes.Red;
                buttonBorder.Background = new SolidColorBrush(ColorConverter.ConvertFromString("#F30613") as Color? ?? Colors.Red);
                App.TipoSilla = buttonBorder.Name.ToString();
            }
            clickedButton.Foreground = Brushes.White;
            CalcularTarifa();

            if (errorgeneral == false)
            {
                MessageBox.Show("Tarifa no programada para la pelicula ");
            }
        }

        private void btnSeleccionFecha_Click(object sender, RoutedEventArgs e)
        {

            string buttonName = "";
            Button clickedButton = sender as Button;
            buttonName = clickedButton.Name;

            Border buttonBorder = FindParent<Border>(clickedButton);
            clickedButton.Foreground = Brushes.White;
            App.Pelicula.FechaSel = buttonName;
            ContenedorHoras3D.Children.Clear();
            ContenedorHorasGeneral.Children.Clear();
            ContenedorHoras3D.Children.Add(CrearBotonFormato("3D"));
            ContenedorHorasGeneral.Children.Add(CrearBotonFormato("General"));
            Panel contenedor = ContenedorFechas;

            foreach (var control in contenedor.Children)
            {
                if (control is Border)
                {
                    Border border = (Border)control;
                    if (border.Child is Button)
                    {

                        Button btn = (Button)border.Child;
                        if (btn.Content.ToString() != "General")
                        {
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
            clickedButton.Foreground = Brushes.White;
            clickedButton.Foreground = Brushes.White;
            CargarFechasDesdeSelect();
        }

        private async void btnSalir_Click(object sender, RoutedEventArgs e)
        {
            isThreadActive = false;
            App.IsBoleteriaConfiteria = false;
            Principal openWindows = new Principal();
            openWindows.Show();
            App.IsFecha = false;
            this.Close();
        }
    }
}