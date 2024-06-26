using APIPortalKiosco.Data;
using APIPortalKiosco.Entities;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Xml;

namespace Portal.Kiosco.Properties.Views
{
    public partial class LayoutAsientos : Window
    {
        private readonly IOptions<MyConfig> config;
        private bool isThreadActive = true;
        public static string[] sillasSeleccionadasArray = new string[10];
        private int sillasSeleccionadas = 0;
        private bool isError;
        public LayoutAsientos(IOptions<MyConfig> config)
        {
            try
            {

                var fecha = App.Pelicula.FechaSel;
                InitializeComponent();
                sillasSeleccionadasArray = new string[10];

                this.config = config;
                ContenedorSala.Children.Clear();
                sillasSeleccionadas = 0;
                GenerarSala();
                DataContext = ((App)Application.Current);
                btnSiguiente.Visibility = Visibility.Hidden;
                App.IsFecha = false;
                if (App.ob_diclst.Count > 0)
                {
                    lblnombre.Content = "¡HOLA " + App.ob_diclst["Nombre"].ToString() + " " + App.ob_diclst["Apellido"].ToString() + "!";
                }
                else
                {
                    lblnombre.Content = "¡HOLA INVITADO!";
                }



                lblSala.Content = App.Pelicula.numeroSala;
                lblNombrePelicula.Content = App.Pelicula.TituloOriginal;
                FechaImpresion(App.Pelicula.FechaSel);
                lblFecha.Content = FormatearFecha(App.Pelicula.FechaSel.Substring(3));
                lblHora.Content = App.Pelicula.HoraUsuario;

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
            catch (Exception e) { }
        }



        public static string FormatearFecha(string fecha)
        {
            DateTime fechaDateTime = DateTime.ParseExact(fecha, "yyyyMMdd", CultureInfo.InvariantCulture);

            string fechaFormateada = fechaDateTime.ToString("MMM dd", CultureInfo.InvariantCulture);

            return fechaFormateada;
        }


        public static void FechaImpresion(string fechaImpresion)
        {

            string fechaString = fechaImpresion;

            string fechaStr = fechaString.Substring(3);

            if (DateTime.TryParseExact(fechaStr, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out DateTime fecha))
            {
                string fechaFinal = fecha.ToString("dd/MM/yyyy");
                App.FechaSeleccionada = fechaFinal;
            }
        }
        private bool ComprobarTiempo()
        {
            bool isMainWindowOpen = false;

            if (App._tiempoRestanteGlobal == "00:00")
            {
                this.Dispatcher.Invoke(() =>
                {
                    Principal principal = Application.Current.Windows.OfType<Principal>().FirstOrDefault();

                    if (principal == null)
                    {
                        principal = new Principal();
                        principal.Show();
                        isMainWindowOpen = true;
                    }
                    else if (principal.Visibility == Visibility.Visible)
                    {
                        principal.Activate();
                        isMainWindowOpen = true;
                    }
                    else
                    {
                        principal.Show();
                        isMainWindowOpen = true;
                    }

                    this.Close();

                });
            }

            return isMainWindowOpen;
        }


        private async void btnVolver_Click(object sender, RoutedEventArgs e)
        {
            App.RoomReverse();
            isThreadActive = false;
            Cartelera openWindows = new Cartelera();
            openWindows.Show();
            this.Close();
        }

        private async void btnSiguiente_Click(object sender, RoutedEventArgs e)
        {
            var pelicula = App.Peliculas.FirstOrDefault(x => x.Id == App.Pelicula.Id);


            if (isError == false)
            {
                if (lblTotal.Content == "TOTAL: $0")
                {
                    MessageBox.Show("UPS! Aun no ha seleccionado ninguna ubicación", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                    return;
                }

                Room(App.BolVentaRoom);

                if (pelicula != null && pelicula.Formato != null && pelicula.Formato.Contains("3D"))
                {
                    isThreadActive = false;
                    Gafas3D openWindows = new Gafas3D();
                    openWindows.Show();
                    this.Close();
                }
                else
                {
                    isThreadActive = false;
                    AlgoParaComer openWindows = new AlgoParaComer();
                    openWindows.Show();
                    this.Close();
                }
            }
        }

        public void GenerarSala()
        {
            try
            {
                int lc_maxcol = 0;
                int lc_maxfil = 0;
                int lc_idxrow = 0;
                string lc_auxval = string.Empty;
                string lc_auxtar = string.Empty;
                string lc_auxhor = string.Empty;
                string lc_srvpar = string.Empty;
                string lc_result = string.Empty;

                DataTable ob_datubi = new DataTable();

                XmlDocument ob_xmldoc = new XmlDocument();

                Dictionary<string, object> ob_estsil = new Dictionary<string, object>();
                Dictionary<string, object> ob_diclst = new Dictionary<string, object>();
                Dictionary<string, object>[] ob_diclst2 = null;
                List<BolVenta> ob_lisprg = new List<BolVenta>();

                MapaSala ob_datsal = new MapaSala();
                BolVenta ob_datprg = new BolVenta();
                General ob_fncgrl = new General();

                string keypelicula = App.Pelicula.Id;

                //var keysal = App.Peliculas.FirstOrDefault(x => x.Id == keypelicula);
                var pelicula = App.Pelicula;
                ob_datsal.Sala = Convert.ToInt32(App.Pelicula.numeroSala);
                ob_datsal.Teatro = Convert.ToInt32(App.idCine);
                ob_datsal.Tercero = 2.ToString();
                ob_datsal.Correo = "";
                ob_datsal.FechaFuncion = "";

                //Generar y encriptar JSON para servicio MAP
                lc_srvpar = ob_fncgrl.JsonConverter(ob_datsal);
                lc_srvpar = lc_srvpar.Replace("sala", "Sala");

                //Encriptar Json MAP
                lc_srvpar = ob_fncgrl.EncryptStringAES(lc_srvpar);

                //Consumir servicio MAP
                lc_result = ob_fncgrl.WebServices(string.Concat(App.ScoreServices, "scomap/"), lc_srvpar);

                if (lc_result.Substring(0, 1) == "0")
                {
                    lc_result = lc_result.Replace("0-", "");
                    ob_diclst = (Dictionary<string, object>)JsonConvert.DeserializeObject(lc_result, (typeof(Dictionary<string, object>)));
                }
                else
                {
                    lc_result = lc_result.Replace("1-", "");
                }

                //Asignar valores EST
                ob_datsal.Sala = Convert.ToInt32(App.Pelicula.numeroSala);
                ob_datsal.Teatro = Convert.ToInt32(App.idCine);
                ob_datsal.Tercero = "2";

                ob_datsal.Correo = App.EmailEli;
                ob_datsal.FechaFuncion = App.Pelicula.FechaSel.Substring(3);

                string idFuncion = App.Pelicula.HoraSel;

                ob_datsal.Funcion = Convert.ToInt32(App.Pelicula.HoraMilitar.Substring(0, 2));

                //Generar y encriptar JSON para servicio EST
                lc_srvpar = ob_fncgrl.JsonConverter(ob_datsal);
                lc_srvpar = lc_srvpar.Replace("sala", "Sala");
                lc_srvpar = lc_srvpar.Replace("correo", "Correo");
                lc_srvpar = lc_srvpar.Replace("funcion", "Funcion");
                lc_srvpar = lc_srvpar.Replace("fechaFuncion", "FechaFuncion");

                //Encriptar Json EST
                lc_srvpar = ob_fncgrl.EncryptStringAES(lc_srvpar);

                //Consumir servicio EST
                lc_result = ob_fncgrl.WebServices(string.Concat(App.ScoreServices, "scoest/"), lc_srvpar);

                if (lc_result.Substring(0, 1) == "0")
                {
                    //Quitar switch
                    lc_result = lc_result.Replace("0-", "");

                    //Deserializar Json y validar respuesta EST
                    ob_diclst2 = (Dictionary<string, object>[])JsonConvert.DeserializeObject(lc_result, (typeof(Dictionary<string, object>[])));
                }

                foreach (var item in ob_diclst2)
                {
                    lc_maxcol = Convert.ToInt32(item["maxCol"]);
                    lc_maxfil = ob_diclst2.Length;
                    ob_estsil.Add(item["filRel"].ToString(), item["DescripcionSilla"]);
                }

                double[] ColumnaTotal = (double[])JsonConvert.DeserializeObject(ob_diclst["ColumnaTotal"].ToString(), (typeof(double[])));
                double[] ColumnaRelativa = (double[])JsonConvert.DeserializeObject(ob_diclst["ColumnaRelativa"].ToString(), (typeof(double[])));
                string[] FilaTotal = (string[])JsonConvert.DeserializeObject(ob_diclst["FilaTotal"].ToString(), (typeof(string[])));
                string[] FilaRelativa = (string[])JsonConvert.DeserializeObject(ob_diclst["FilaRelativa"].ToString(), (typeof(string[])));
                string[] TipoSilla = (string[])JsonConvert.DeserializeObject(ob_diclst["TipoSilla"].ToString(), (typeof(string[])));
                string[] TipoZona = (string[])JsonConvert.DeserializeObject(ob_diclst["TipoZona"].ToString(), (typeof(string[])));

                Ubicaciones[,] mt_datsal = new Ubicaciones[lc_maxfil, lc_maxcol];
                for (int lc_idxiii = 0; lc_idxiii < lc_maxfil; lc_idxiii++)
                {
                    //Recorrer y cargar matriz de sala (columnas)
                    for (int lc_idxjjj = 0; lc_idxjjj < lc_maxcol; lc_idxjjj++)
                    {
                        //Inicializar objeto de ubicaciones 
                        Ubicaciones ob_ubisal = new Ubicaciones();

                        //Cargar valores numericos de los arreglos al objeto
                        ob_ubisal.Columna = Convert.ToInt32(ColumnaTotal[lc_idxrow]);
                        ob_ubisal.ColRelativa = Convert.ToInt32(ColumnaRelativa[lc_idxrow]);

                        //Cargar valores string de los arreglos al objeto
                        ob_ubisal.Fila = FilaTotal[lc_idxrow];
                        ob_ubisal.FilRelativa = FilaRelativa[lc_idxrow];
                        ob_ubisal.TipoSilla = TipoSilla[lc_idxrow];
                        ob_ubisal.TipoZona = TipoZona[lc_idxrow];

                        //Recorrer y buscar fila en ciclo de matriz
                        List<EstadoDeSilla> ls_estsil = new List<EstadoDeSilla>((List<EstadoDeSilla>)JsonConvert.DeserializeObject(ob_estsil[FilaRelativa[lc_idxrow]].ToString(), (typeof(List<EstadoDeSilla>))));
                        foreach (var item in ls_estsil)
                        {
                            //Validar columna en ciclo de matriz
                            if (Convert.ToInt32(item.Columna) == ColumnaRelativa[lc_idxrow])
                            {
                                //Asignar valor y romper ciclo
                                ob_ubisal.EstadoSilla = item.EstadoSilla;
                                break;
                            }
                        }

                        //Cargar objeto ubicaciones a la matriz
                        mt_datsal[lc_idxiii, lc_idxjjj] = ob_ubisal;
                        lc_idxrow++;
                    }
                }

                //Asignar Sala a Objeto
                ob_datprg.FilSala = lc_maxfil;
                ob_datprg.ColSala = lc_maxcol;
                ob_datprg.MapaSala = mt_datsal;
                App.BolVentaRoom = ob_datprg;

                AgregarUbicacionAlWrapPanel(ob_datprg);
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se logro generar la sala de la funcion", "Error", MessageBoxButton.OK, MessageBoxImage.Error); isError = true;
                var openWindows = new SeleccionarFuncion();
                openWindows.Show();
                this.Close();
            }

        }

        public void CalcularTarifa(String ZonaSeleccionda)
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
                var horaMilitar = App.Pelicula.HoraMilitar;
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

                lc_result = ob_fncgrl.WebServices(string.Concat(App.ScoreServices, "scocar/"), lc_srvpar);
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
                                if (ZonaSeleccionda == itemZonas.Key)
                                {
                                    foreach (var item in ob_lisprg)
                                    {
                                        if (item.hora != null && item.hora.Count > 0)
                                        {
                                            foreach (var item2 in item.hora)
                                            {
                                                if (item2.militar == horaMilitar)
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
                                                                        //borSiguente.Visibility = Visibility.Visible;
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
                        }
                        else
                        {
                            // Código relacionado con la falta de datos en ViewBag
                        }
                    }
                }
            }
            catch (Exception e) { }
        }

        private void AgregarUbicacionAlWrapPanel(BolVenta bolVenta)
        {
            // Crear la plantilla de control personalizada
            Ubicaciones[,] ubicaciones = bolVenta.MapaSala;

            for (int i = 0; i < ubicaciones.GetLength(0); i++)
            {
                for (int j = 0; j < ubicaciones.GetLength(1); j++)
                {
                    Ubicaciones ubicacion = ubicaciones[i, j];
                    Button button = new Button();
                    string lc_valmos = string.Concat(ubicacion.FilRelativa, ubicacion.ColRelativa);
                    string lc_values = string.Concat(ubicacion.EstadoSilla, "_", ubicacion.FilRelativa, "_", ubicacion.ColRelativa, "_", ubicacion.Fila, "_", ubicacion.Columna, "_");

                    App.TipoSilla.Replace("_", " ");

                    button.Content = lc_valmos;
                    button.Name = lc_values;
                    button.Click += Button_Click;
                    button.Style = (Style)FindResource("AvailableSeat");

                    Border border = new Border();
                    border.CornerRadius = new CornerRadius(5);
                    border.Margin = new Thickness(0, 0, 1, 1);
                    border.BorderThickness = new Thickness(1);

                    Label label = new Label();

                    var tipozona = App.TipoSilla.Replace("_", " ");


                    if (ubicacion.TipoSilla == "pasillo")
                    {
                        border.Visibility = Visibility.Hidden;
                    }
                    else
                    {
                        if (ubicacion.TipoSilla.ToLower() == tipozona.ToLower())
                        {

                            switch (ubicacion.EstadoSilla)
                            {
                                case "S":
                                    if (ubicacion.TipoSilla == "Discapacitado")
                                    {
                                        border.Background = Brushes.Blue;
                                        label.Content = lc_valmos;
                                        label.Background = Brushes.Blue;
                                        label.Foreground = Brushes.White;

                                        label.FontSize = 14;
                                        label.FontWeight = FontWeights.Bold;

                                        label.HorizontalAlignment = HorizontalAlignment.Center;
                                        label.VerticalAlignment = VerticalAlignment.Center;
                                        border.Child = label;

                                    }
                                    else
                                    {
                                        button.Background = new SolidColorBrush(ColorConverter.ConvertFromString("#D9D9D9") as Color? ?? Colors.LightGray);
                                        button.BorderBrush = new SolidColorBrush(ColorConverter.ConvertFromString("#D9D9D9") as Color? ?? Colors.LightGray);
                                        button.Foreground = Brushes.Black;
                                        border.Child = button;
                                    }


                                    break;
                                case "B":
                                    border.Background = Brushes.Red;
                                    label.Content = lc_valmos;
                                    label.Background = Brushes.Red;
                                    label.Foreground = Brushes.White;

                                    label.FontSize = 14;
                                    label.FontWeight = FontWeights.Bold;

                                    label.HorizontalAlignment = HorizontalAlignment.Center;
                                    label.VerticalAlignment = VerticalAlignment.Center;
                                    border.Child = label;
                                    break;
                                case "R":
                                    border.Visibility = Visibility.Hidden;
                                    break;
                                case "O":
                                    border.Background = Brushes.Yellow;
                                    label.Content = lc_valmos;
                                    label.Background = Brushes.Yellow;
                                    label.Foreground = Brushes.Black;

                                    label.FontSize = 14;
                                    label.FontWeight = FontWeights.Bold;

                                    label.HorizontalAlignment = HorizontalAlignment.Center;
                                    label.VerticalAlignment = VerticalAlignment.Center;
                                    border.Child = label;
                                    break;
                                case "X":
                                    button.Background = Brushes.Green;
                                    button.BorderBrush = Brushes.Green;
                                    button.Foreground = Brushes.Black;
                                    border.Child = button;
                                    break;
                                default:
                                    border.Background = Brushes.Red;
                                    label.Content = lc_valmos;
                                    label.Background = Brushes.Red;
                                    label.Foreground = Brushes.White;

                                    label.FontSize = 14;
                                    label.FontWeight = FontWeights.Bold;

                                    label.HorizontalAlignment = HorizontalAlignment.Center;
                                    label.VerticalAlignment = VerticalAlignment.Center;
                                    border.Child = label;
                                    break;
                            }
                        }
                        else
                        {
                            border.Background = Brushes.Red;
                            label.Content = lc_valmos;
                            label.Background = Brushes.Red;
                            label.Foreground = Brushes.White;

                            label.FontSize = 14;
                            label.FontWeight = FontWeights.Bold;

                            label.HorizontalAlignment = HorizontalAlignment.Center;
                            label.VerticalAlignment = VerticalAlignment.Center;
                            border.Child = label;
                        }

                    }

                    ContenedorSala.Rows = bolVenta.FilSala;
                    ContenedorSala.Columns = bolVenta.ColSala;
                    ContenedorSala.Children.Add(border);
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            if (button != null)
            {
                string silla = button.Content.ToString();

                if (sillasSeleccionadasArray.Contains(silla))
                {
                    int index = Array.IndexOf(sillasSeleccionadasArray, silla);

                    sillasSeleccionadasArray[index] = null;

                    button.Background = new SolidColorBrush(ColorConverter.ConvertFromString("#D9D9D9") as Color? ?? Colors.LightGray);

                    App.BolVentaRoom.SelUbicaciones = App.BolVentaRoom.SelUbicaciones.Replace(button.Name.ToString() + ";", "");

                    sillasSeleccionadas--;
                    isError = false;
                }
                else if (sillasSeleccionadas < Convert.ToInt32(App.CantSillasBol))
                {

                    int index = Array.IndexOf(sillasSeleccionadasArray, null);

                    App.BolVentaRoom.SelUbicaciones = App.BolVentaRoom.SelUbicaciones + button.Name.ToString() + ";";

                    sillasSeleccionadasArray[index] = silla;

                    button.Background = Brushes.Red;
                    isError = false;
                    sillasSeleccionadas++;
                }
                else
                {
                    MessageBox.Show("Solo se pueden seleccionar hasta " + App.CantidadBoletas + " sillas.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    isError = false;
                }

                ActualizarInterfazSillasSeleccionadas();
            }
        }

        private void ActualizarInterfazSillasSeleccionadas()
        {
            ContenedorBoletas.Children.Clear();

            for (int i = 0; i < sillasSeleccionadasArray.Length; i++)
            {
                string silla = sillasSeleccionadasArray[i];

                if (silla != null)
                {
                    Label labelSilla = new Label();
                    labelSilla.Content = silla;
                    labelSilla.FontFamily = new FontFamily("Myanmar Khyay");
                    labelSilla.FontSize = 16;
                    labelSilla.VerticalAlignment = VerticalAlignment.Center;
                    labelSilla.HorizontalAlignment = HorizontalAlignment.Center;

                    Label labelCantidad = new Label();
                    labelCantidad.Content = App.ValorTarifa.ToString("C0");
                    labelCantidad.FontFamily = new FontFamily("Myanmar Khyay");
                    labelCantidad.FontSize = 16;
                    labelCantidad.VerticalAlignment = VerticalAlignment.Center;
                    labelCantidad.HorizontalAlignment = HorizontalAlignment.Center;

                    ContenedorBoletas.Children.Add(labelSilla);
                    ContenedorBoletas.Children.Add(labelCantidad);
                }
            }

            if (sillasSeleccionadas == 0)
            {
                btnSiguiente.Visibility = Visibility.Visible;
                lblTotal.Content = Convert.ToDecimal("0").ToString("C0");
            }
            else
            {
                btnSiguiente.Visibility = Visibility.Visible;
                lblTotal.Content = (sillasSeleccionadas * App.ValorTarifa).ToString("C0");
                App.CantidadBoletas = Convert.ToDecimal(sillasSeleccionadas);
            }
        }

        private async void btnSalir_Click(object sender, RoutedEventArgs e)
        {
            App.RoomReverse();
            isThreadActive = false;
            Principal openWindows = new Principal();
            openWindows.Show();
            this.Close();
        }

        public void Room(BolVenta pr_bolvta)
        {
            #region VARIABLES LOCALES
            int lc_idearr = 0;
            double lc_valtar = 0;
            string lc_auxitm = string.Empty;
            string lc_auxtel = string.Empty;
            string lc_auxsec = string.Empty;
            string lc_srvpar = string.Empty;
            string lc_result = string.Empty;
            string[] ls_lstsel = new string[5];

            string TelefonoEli = string.Empty;

            List<string> ls_lstubi = new List<string>();
            List<Ubicaciones> ob_ubiprg = new List<Ubicaciones>();
            Dictionary<string, string> ob_diclst = new Dictionary<string, string>();

            Secuencia ob_secsec = new Secuencia();
            General ob_fncgrl = new General();
            #endregion

            try
            {
                var secuencia = App.Secuencia == "0" ? null : App.Secuencia;
                if (secuencia != null)
                {
                    lc_auxsec = App.Secuencia.ToString();
                }
                else
                {
                    #region SERVICIO SCOSEC
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
                        ob_diclst = (Dictionary<string, string>)JsonConvert.DeserializeObject(lc_result, (typeof(Dictionary<string, string>)));

                        if (ob_diclst.ContainsKey("Validación"))
                        {
                            MessageBox.Show(ob_diclst["Validación"].ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            isError = true;

                        }
                        else
                        {
                            lc_auxsec = ob_diclst["Secuencia"].ToString().Substring(0, ob_diclst["Secuencia"].ToString().IndexOf("."));
                            App.Secuencia = lc_auxsec;
                        }

                        ob_diclst.Clear();
                    }
                    else
                    {
                        lc_result = lc_result.Replace("1-", "");

                    }
                    #endregion
                }

                #region SERVICIO SCOGRU
                //Validar secuencia
                if (lc_auxsec != "0")
                {
                    //Asignar valores
                    pr_bolvta.Funcion = "";
                    pr_bolvta.Horario = "";
                    pr_bolvta.message = "";
                    pr_bolvta.FechaPrg = "";
                    pr_bolvta.FechaDia = "";
                    pr_bolvta.ValorTarifa = "";
                    pr_bolvta.IdTarifa = "";
                    pr_bolvta.MapaSala = new Ubicaciones[1, 1];

                    pr_bolvta.EmailEli = App.EmailEli.ToString();
                    pr_bolvta.NombreEli = App.NombreEli.ToString();
                    pr_bolvta.Nombre = App.NombreEli.ToString();
                    pr_bolvta.ApellidoEli = App.ApellidoEli.ToString(); ;
                    pr_bolvta.TelefonoEli = App.TelefonoEli.ToString();

                    pr_bolvta.Tipo = "B";
                    pr_bolvta.NombreTarifa = App.NombreTarifa;
                    pr_bolvta.NombrePelicula = App.Pelicula.Nombre;
                    pr_bolvta.SwtVenta = "V";
                    pr_bolvta.Sala = Convert.ToInt32(App.Pelicula.numeroSala);
                    pr_bolvta.KeySala = App.Pelicula.numeroSala;
                    var telefonodefault = 0;
                    pr_bolvta.Telefono = Convert.ToInt64(telefonodefault);

                    pr_bolvta.NombrePel = App.Pelicula.Nombre;
                    pr_bolvta.NombreFec = App.Pelicula.FechaUsuario;

                    pr_bolvta.NombreHor = App.Pelicula.HoraUsuario;
                    pr_bolvta.Telefono = Convert.ToUInt32(App.TelefonoEli);
                    pr_bolvta.NombreTar = App.NombreTarifa;

                    pr_bolvta.Apellido = App.ApellidoEli;

                    pr_bolvta.FecProg = App.Pelicula.FechaSel.ToString().Substring(3);
                    pr_bolvta.HorProg = App.Pelicula.HoraMilitar;
                    pr_bolvta.KeyTarifa = App.KeyTarifa.ToString();
                    pr_bolvta.KeyTeatro = App.idCine;
                    pr_bolvta.KeyPelicula = App.Pelicula.Id;
                    pr_bolvta.KeySecuencia = lc_auxsec;
                    pr_bolvta.NombreFec = App.NombreFec;
                    pr_bolvta.Censura = App.Pelicula.Censura;
                    pr_bolvta.Tercero = App.ValorTercero;
                    pr_bolvta.Imagen = App.Imagen;
                    pr_bolvta.Secuencia = Convert.ToInt32(lc_auxsec);
                    pr_bolvta.PuntoVenta = Convert.ToInt32(App.PuntoVenta);
                    pr_bolvta.IdFuncion = pr_bolvta.HorProg.ToString().Length == 4 ? Convert.ToInt32(pr_bolvta.HorProg.ToString().Substring(0, 2)) : Convert.ToInt32(pr_bolvta.HorProg.ToString().Substring(0, 1));
                    pr_bolvta.TipoSilla = App.TipoSilla;

                    //Obtener ubicaciones de vista
                    char[] ar_charst = pr_bolvta.SelUbicaciones.ToCharArray();
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

                        ob_ubiprg.Add(new Ubicaciones() { Fila = ls_lstsel[3], Columna = Convert.ToInt32(ls_lstsel[4]), Tarifa = Convert.ToInt32(pr_bolvta.KeyTarifa), FilRelativa = ls_lstsel[1], ColRelativa = Convert.ToInt32(ls_lstsel[2]), TipoSilla = "", TipoZona = "", EstadoSilla = "" });
                    }

                    pr_bolvta.Ubicaciones = ob_ubiprg;

                    pr_bolvta.FilSala = 0;
                    pr_bolvta.ColSala = 0;
                    if (pr_bolvta.Imagen == null)
                        pr_bolvta.Imagen = "NA";

                    //Validar cantidad de sillas
                    if (pr_bolvta.Ubicaciones.Count > Convert.ToInt32(App.CantSillasBol))
                    {
                        MessageBox.Show("Solo se pueden seleccionar hasta " + App.CantSillasBol + " sillas por transacción.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        isError = true;
                    }
                    //Generar y encriptar JSON para servicio
                    lc_srvpar = ob_fncgrl.JsonConverter(pr_bolvta);

                    lc_srvpar = lc_srvpar.Replace("fila", "Fila");
                    lc_srvpar = lc_srvpar.Replace("sala", "Sala");
                    lc_srvpar = lc_srvpar.Replace("nombre", "Nombre");
                    lc_srvpar = lc_srvpar.Replace("tarifa", "Tarifa");
                    lc_srvpar = lc_srvpar.Replace("columna", "Columna");
                    lc_srvpar = lc_srvpar.Replace("Tercero", "tercero");
                    lc_srvpar = lc_srvpar.Replace("keyTeatro", "teatro");
                    lc_srvpar = lc_srvpar.Replace("apellido", "Apellido");
                    lc_srvpar = lc_srvpar.Replace("telefono", "Telefono");
                    lc_srvpar = lc_srvpar.Replace("keyPelicula", "Pelicula");
                    lc_srvpar = lc_srvpar.Replace("secuencia", "Secuencia");
                    lc_srvpar = lc_srvpar.Replace("fecProg", "FechaFuncion");
                    lc_srvpar = lc_srvpar.Replace("\"id\"", "\"IdMessage\"");
                    lc_srvpar = lc_srvpar.Replace("horProg", "InicioFuncion");
                    lc_srvpar = lc_srvpar.Replace("idFuncion", "HoraFuncion");
                    lc_srvpar = lc_srvpar.Replace("puntoVenta", "PuntoVenta");
                    lc_srvpar = lc_srvpar.Replace("ubicaciones", "Ubicaciones");
                    lc_srvpar = lc_srvpar.Replace("NombrePelicula", "Descripcion");

                    //Encriptar Json GRU
                    lc_srvpar = ob_fncgrl.EncryptStringAES(lc_srvpar);

                    //Consumir servicio GRU
                    lc_result = ob_fncgrl.WebServices(string.Concat(App.ScoreServices, "scogru/"), lc_srvpar);

                    //Validar respuesta GRU
                    if (lc_result.Substring(0, 1) == "0")
                    {
                        //Quitar switch
                        lc_result = lc_result.Replace("0-", "");

                        //Deserializar Json y validar respuesta
                        Dictionary<string, string>[] ob_result = (Dictionary<string, string>[])JsonConvert.DeserializeObject(lc_result, (typeof(Dictionary<string, string>[])));

                        //Validar respuesta llave 1
                        for (int lc_idxiii = 0; lc_idxiii < ob_result.Length; lc_idxiii++)
                        {
                            Dictionary<string, string> ob_auxrta = ob_result[lc_idxiii];

                            if (ob_auxrta.ContainsKey("Validación"))
                            {
                                //MessageBox.Show("");
                                MessageBox.Show("Error en la seleccion de la hubicación", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                isError = true;
                            }
                            else
                            {
                                //Validar respuesta llave 2
                                if (ob_auxrta.ContainsKey("Respuesta"))
                                {
                                    if (ob_auxrta["Respuesta"].ToString() == "Proceso exitoso")
                                    {
                                        lc_valtar = Convert.ToDouble(pr_bolvta.NombreTar.Substring(pr_bolvta.NombreTar.IndexOf(";") + 1, pr_bolvta.NombreTar.Length - (pr_bolvta.NombreTar.IndexOf(";") + 1)));

                                        //Inicializar instancia de BD
                                        using (var context = new DataDB(config))
                                        {
                                            //Agregar valores a tabla ReportSales
                                            TelefonoEli = "";//string.Concat(Session.GetString("Telefono"), ";", Session.GetString("Documento"), "*", Session.GetString("Direccion"));
                                            var reportSales = new ReportSales
                                            {
                                                Secuencia = pr_bolvta.KeySecuencia,
                                                KeySala = pr_bolvta.KeySala,
                                                KeyTeatro = App.idCine,
                                                KeyPelicula = pr_bolvta.KeyPelicula,
                                                SelUbicaciones = pr_bolvta.SelUbicaciones,
                                                Precio = (lc_valtar * pr_bolvta.Ubicaciones.Count),
                                                HorProg = pr_bolvta.HorProg,
                                                FecProg = pr_bolvta.FecProg,
                                                EmailEli = "", //Session.GetString("Usuario"),
                                                NombreEli = "", //Session.GetString("Nombre"),
                                                ApellidoEli = "", //Session.GetString("Apellido"),
                                                TelefonoEli = TelefonoEli,
                                                NombrePel = pr_bolvta.NombrePel,
                                                NombreFec = pr_bolvta.NombreFec,
                                                NombreHor = pr_bolvta.NombreHor,
                                                NombreTar = pr_bolvta.NombreTar,
                                                KeyTarifa = pr_bolvta.KeyTarifa,
                                                Transaccion = pr_bolvta.Censura,
                                                Referencia = pr_bolvta.Imagen,
                                                FechaCreado = DateTime.Now,
                                                FechaModificado = DateTime.Now,
                                                KeyPunto = App.PuntoVenta
                                            };

                                            context.ReportSales.Add(reportSales);
                                            context.SaveChanges();
                                        }
                                    }

                                    else
                                    {
                                        MessageBox.Show(ob_auxrta["Respuesta"].ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                        isError = true;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        lc_result = lc_result.Replace("1-", "");
                    }
                }
                else
                {
                }
                #endregion

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                isError = true;


            }
        }
    }
}