using APIPortalKiosco.Data;
using APIPortalKiosco.Entities;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
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
        private readonly IOptions<MyConfig> config;
        private bool isThreadActive = true;
        public static string[] sillasSeleccionadasArray = new string[10]; // Arreglo para almacenar las sillas seleccionadas
        private int sillasSeleccionadas = 0;

        public LayoutAsientos(IOptions<MyConfig> config)
        {
            try
            {
                InitializeComponent();
                sillasSeleccionadasArray = new string[10];
                this.config = config;
                ContenedorSala.Children.Clear();
                GenerarSala();
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
            catch (Exception e) { }
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

        private async void btnSiguiente_Click(object sender, RoutedEventArgs e)
        {

            var pelicula = App.Peliculas.FirstOrDefault(x => x.Id == App.Pelicula.Id);
            Room(App.BolVentaRoom);
            if (lblTotal.Content == "TOTAL: $0")
            {
                MessageBox.Show("UPS! Aun no ha seleccionado ninguna ubicación", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

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
            catch (Exception ex) { MessageBox.Show("No se logro generar la sala de la funcion", "Error", MessageBoxButton.OK, MessageBoxImage.Error); }

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
                else if (sillasSeleccionadas < Convert.ToInt32(App.CantSillasBol))
                {
                    // Si la silla no está seleccionada y el límite de selección no ha sido alcanzado

                    // Encuentra el primer índice vacío en el arreglo de sillas seleccionadas
                    int index = Array.IndexOf(sillasSeleccionadasArray, null);
                    App.BolVentaRoom.SelUbicaciones = App.BolVentaRoom.SelUbicaciones + button.Name.ToString() + ";";
                    // Almacena el contenido del botón en el arreglo de sillas seleccionadas
                    sillasSeleccionadasArray[index] = silla;

                    // Cambia el color del botón a rojo
                    button.Background = Brushes.Red;

                    // Incrementa el contador de sillas seleccionadas
                    sillasSeleccionadas++;
                }
                else
                {
                    MessageBox.Show("Solo se pueden seleccionar hasta "+App.CantidadBoletas+" sillas.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                    labelCantidad.Content =  App.ValorTarifa.ToString("C0");
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
                lblTotal.Content = Convert.ToDecimal("0").ToString("C0");
            }
            else
            {
                lblTotal.Content =  (sillasSeleccionadas * App.ValorTarifa).ToString("C0");
                App.CantidadBoletas = Convert.ToDecimal(sillasSeleccionadas);
            }
        }

        private async void btnSalir_Click(object sender, RoutedEventArgs e)
        {
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
                var secuencia = App.Secuencia;
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
                    pr_bolvta.Censura = App.Censura;
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
                        MessageBox.Show("Solo se pueden seleccionar hasta " + App.CantSillasBol + " sillas por transacción.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

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
            }
        }
    }
}