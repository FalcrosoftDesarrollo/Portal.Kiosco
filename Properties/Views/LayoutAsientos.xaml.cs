using APIPortalKiosco.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Xml;
 



namespace Portal.Kiosco.Properties.Views
{
    /// <summary>
    /// Lógica de interacción para Frame7DEF.xaml
    /// </summary>
    public partial class LayoutAsientos : Window
    {
        public LayoutAsientos()
        {

            InitializeComponent();
            GenerarSala();
            ConsultarZona();
            DataContext = ((App)Application.Current);
            if (App.ob_diclst.Count > 0)
            {
                lblnombre.Content = "!HOLA " + App.ob_diclst["Nombre"].ToString() + " " + App.ob_diclst["Apellido"].ToString();
            }
            else
            {
                lblnombre.Content = "!HOLA INVITADO";
            }

            //lblFecha.Content = App.Pelicula.FechaUsuario;
            //lblHora.Content = App.Pelicula.HoraUsuario;
            lblSala.Content = App.Pelicula.numeroSala;
            lblNombrePelicula.Content = App.Pelicula.Nombre;

            DoubleAnimation fadeInAnimation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.5));
            gridPrincipal.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);

        }


        public void ConsultarZona()
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

            ob_carprg.Teatro = App.idCine;
            ob_carprg.tercero = App.ValorTercero;
            ob_carprg.IdPelicula = App.Pelicula.Id;
            ob_carprg.FcPelicula = App.Pelicula.FechaSel.Substring(3);//pr_tippel == "Preventa" ? pr_fecprg : ViewBag.Cartelera[0].FecSt;
            ob_carprg.TpPelicula = App.TipoSala;
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
                else
                {
                    // Código relacionado con la falta de datos en ViewBag
                }
            }

        }

        private async void btnVolver_Click(object sender, RoutedEventArgs e)
        {
            var openWindow = new Cartelera();
            openWindow.Show();
            this.Close();
        }

        private async void btnSiguiente_Click(object sender, RoutedEventArgs e)
        {
            var pelicula = App.Peliculas.FirstOrDefault(x => x.Id == App.Pelicula.Id);

            if (lblTotal.Content == "TOTAL: $0")
            {
                MessageBox.Show("UPS! Aun no ha seleccionado ninguna ubicación");
                return;
            }

            if (pelicula != null && pelicula.Formato != null /*&& /*pelicula.Formato.Contains("3D")*/)
            {
                var openWindow = new Gafas3D();
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
            else
            {
                var openWindow = new AlgoParaComer();
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

        public void GenerarSala()
        {

            ContenedorSala.Children.Clear();

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
                //Quitar switch
                lc_result = lc_result.Replace("0-", "");

                //Deserializar Json y validar respuesta MAP
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

            ob_datsal.Correo = "pol@scoreprojects.net";
            ob_datsal.FechaFuncion = App.Pelicula.FechaSel.Substring(3);

            string idFuncion = App.Pelicula.HoraSel;
            string miliar = App.Peliculas.SelectMany(p => p.DiasDisponibles)
                                          .SelectMany(f => f.horafun)
                                          .FirstOrDefault(h => h.idFuncion == idFuncion)?.horunv;

            ob_datsal.Funcion = Convert.ToInt32(miliar.Substring(0, 2));

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
            AgregarUbicacionAlWrapPanel(ob_datprg);

        }

        private int sillasSeleccionadas = 0;
        private string[] sillasSeleccionadasArray = new string[10]; // Arreglo para almacenar las sillas seleccionadas

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

                    button.Content = lc_valmos;
                    button.Name = lc_values;



                    // Suscribe el evento Click al botón
                    button.Click += Button_Click;

                    // Definir un nuevo estilo
                    button.Style = (Style)FindResource("AvailableSeat");

                    Border border = new Border();
                    border.CornerRadius = new CornerRadius(5);
                    border.Margin = new Thickness(0, 0, 1, 1);
                    border.BorderThickness = new Thickness(1);



                    switch (ubicacion.TipoSilla.ToLower())
                    {
                        case "pasillo":
                            button.Background = System.Windows.Media.Brushes.White;
                            button.BorderBrush = System.Windows.Media.Brushes.White;
                            button.Visibility = Visibility.Hidden;
                            break;
                        case "discapacitado":
                            button.Background = System.Windows.Media.Brushes.LightSkyBlue;
                            button.BorderBrush = System.Windows.Media.Brushes.LightSkyBlue;
                            button.Foreground = System.Windows.Media.Brushes.Black;
                            break;
                        default:
                            switch (ubicacion.EstadoSilla)
                            {
                                case "S":
                                    button.Background = new SolidColorBrush(ColorConverter.ConvertFromString("#D9D9D9") as Color? ?? Colors.LightGray);
                                    button.BorderBrush = new SolidColorBrush(ColorConverter.ConvertFromString("#D9D9D9") as Color? ?? Colors.LightGray);
                                    button.Foreground = System.Windows.Media.Brushes.Black;
                                    break;
                                case "B":
                                case "R":
                                case "O":
                                    button.Background = System.Windows.Media.Brushes.Yellow;
                                    button.BorderBrush = System.Windows.Media.Brushes.Yellow;
                                    button.Foreground = System.Windows.Media.Brushes.Black;
                                    break;
                                case "X":
                                    button.Background = System.Windows.Media.Brushes.Green;
                                    button.BorderBrush = System.Windows.Media.Brushes.Green;
                                    button.Foreground = System.Windows.Media.Brushes.Black;
                                    break;
                                default:
                                    button.Background = System.Windows.Media.Brushes.Red;
                                    button.BorderBrush = System.Windows.Media.Brushes.Red;
                                    button.Foreground = System.Windows.Media.Brushes.Black;
                                    break;
                            }
                            break;
                    }

                    border.Child = button;

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

                // Verifica si la silla ya está seleccionada
                if (sillasSeleccionadasArray.Contains(silla))
                {
                    // Si la silla ya está seleccionada, la elimina de la selección

                    // Encuentra el índice de la silla seleccionada en el arreglo
                    int index = Array.IndexOf(sillasSeleccionadasArray, silla);

                    // Elimina la silla seleccionada del arreglo
                    sillasSeleccionadasArray[index] = null;

                    // Cambia el color del botón a su estado original
                    button.Background = new SolidColorBrush(ColorConverter.ConvertFromString("#D9D9D9") as Color? ?? Colors.LightGray);

                    // Decrementa el contador de sillas seleccionadas
                    sillasSeleccionadas--;
                }
                else if (sillasSeleccionadas < 10)
                {
                    // Si la silla no está seleccionada y el límite de selección no ha sido alcanzado

                    // Encuentra el primer índice vacío en el arreglo de sillas seleccionadas
                    int index = Array.IndexOf(sillasSeleccionadasArray, null);

                    // Almacena el contenido del botón en el arreglo de sillas seleccionadas
                    sillasSeleccionadasArray[index] = silla;

                    // Cambia el color del botón a rojo
                    button.Background = Brushes.Red;

                    // Incrementa el contador de sillas seleccionadas
                    sillasSeleccionadas++;
                }
                else
                {
                    MessageBox.Show("Solo se pueden seleccionar hasta 10 sillas.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                // Actualiza el contenido del UniformGrid con las sillas seleccionadas
                ActualizarInterfazSillasSeleccionadas();
            }
        }

        private void ActualizarInterfazSillasSeleccionadas()
        {
            // Borra todos los elementos existentes en el UniformGrid
            ContenedorBoletas.Children.Clear();

            // Recorre el arreglo de sillas seleccionadas y agrega cada una al UniformGrid
            for (int i = 0; i < sillasSeleccionadasArray.Length; i++)
            {
                string silla = sillasSeleccionadasArray[i];

                if (silla != null)
                {
                    // Crea un nuevo Label para la silla
                    Label labelSilla = new Label();
                    labelSilla.Content = silla;
                    labelSilla.FontFamily = new FontFamily("Myanmar Khyay");
                    labelSilla.FontSize = 16;
                    labelSilla.VerticalAlignment = VerticalAlignment.Center;
                    labelSilla.HorizontalAlignment = HorizontalAlignment.Center;

                    // Crea un nuevo Label para la cantidad (siempre será 1)
                    Label labelCantidad = new Label();
                    labelCantidad.Content = App.ValorTarifa.ToString();
                    labelCantidad.FontFamily = new FontFamily("Myanmar Khyay");
                    labelCantidad.FontSize = 16;
                    labelCantidad.VerticalAlignment = VerticalAlignment.Center;
                    labelCantidad.HorizontalAlignment = HorizontalAlignment.Center;

                    // Agrega los Labels al UniformGrid
                    ContenedorBoletas.Children.Add(labelSilla);
                    ContenedorBoletas.Children.Add(labelCantidad);
                }
            }

            // Muestra el total actualizado
            if (sillasSeleccionadas == 0)
            {
                lblTotal.Content = "TOTAL: $0";
            }
            else
            {
                lblTotal.Content = "TOTAL: $" + (sillasSeleccionadas * App.ValorTarifa).ToString();
                App.CantidadBoletas = Convert.ToDecimal(sillasSeleccionadas);
            }
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
    }
}