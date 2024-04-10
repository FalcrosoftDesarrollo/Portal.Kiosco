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
        }

        private async void btnVolver_Click(object sender, RoutedEventArgs e)
        {
            var openWindow = new SeleccionarFuncion();
            DoubleAnimation fadeOutAnimation = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.5));
            this.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);
            await Task.Delay(300);
            this.Visibility = Visibility.Collapsed;
            openWindow.Background = Brushes.White;
            openWindow.Show();
            DoubleAnimation fadeInAnimation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.5));
            openWindow.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);
        }

        private async void btnSiguiente_Click(object sender, RoutedEventArgs e)
        {
            var openWindow = new Gafas3D();
            DoubleAnimation fadeOutAnimation = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.5));
            this.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);
            await Task.Delay(300);
            this.Visibility = Visibility.Collapsed;
            openWindow.Background = Brushes.White;
            openWindow.Show();
            DoubleAnimation fadeInAnimation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.5));
            openWindow.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);
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
            ob_datsal.FechaFuncion = App.Pelicula.FechaSel;

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

        private void AgregarUbicacionAlWrapPanel(BolVenta bolVenta)
        {
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

                    switch (ubicacion.TipoSilla.ToLower())
                    {
                        case "pasillo":
                            // Establecer estilo para pasillo
                            button.Background = System.Windows.Media.Brushes.White;
                            button.BorderBrush = System.Windows.Media.Brushes.White;
                            button.Visibility = Visibility.Hidden;
                            break;
                        case "discapacitado":
                            // Establecer estilo para silla de discapacitado
                            button.Background = System.Windows.Media.Brushes.LightSkyBlue;
                            button.BorderBrush = System.Windows.Media.Brushes.LightSkyBlue;
                            button.Foreground = System.Windows.Media.Brushes.White;
                            break;
                        default:
                            // Establecer estilos para otros tipos de sillas
                            switch (ubicacion.EstadoSilla)
                            {
                                case "S":
                                    button.Background = System.Windows.Media.Brushes.White;
                                    button.BorderBrush = System.Windows.Media.Brushes.Gray;
                                    break;
                                case "B":
                                    button.Background = System.Windows.Media.Brushes.Gray;
                                    button.BorderBrush = System.Windows.Media.Brushes.Black;
                                    break;
                                case "R":
                                    button.Background = System.Windows.Media.Brushes.Yellow;
                                    button.BorderBrush = System.Windows.Media.Brushes.Gray;
                                    break;
                                case "O":
                                    button.Background = System.Windows.Media.Brushes.Black;
                                    button.BorderBrush = System.Windows.Media.Brushes.White;
                                    break;
                                case "X":
                                    button.Background = System.Windows.Media.Brushes.Green;
                                    button.BorderBrush = System.Windows.Media.Brushes.White;
                                    break;
                                default:
                                    button.Background = System.Windows.Media.Brushes.Red;
                                    button.Foreground = System.Windows.Media.Brushes.White;
                                    break;
                            }
                            break;
                    }

                    // Definir un nuevo estilo
                    button.Style = (Style)FindResource("AvailableSeat");


                    Border border = new Border();
                    //border.Background = Brushes.White;
                    border.CornerRadius = new CornerRadius(5);
                    border.Margin = new Thickness(0, 0, 1, 1);
                    border.BorderThickness = new Thickness(1);
                    //border.BorderBrush = Brushes.LightGray;
                    border.Child = button;

                    ContenedorSala.Rows = bolVenta.FilSala;

                    ContenedorSala.Columns = bolVenta.ColSala;
                    ContenedorSala.Children.Add(border);
                }
            }
        }

    }
}
