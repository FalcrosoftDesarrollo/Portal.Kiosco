using System.Threading.Tasks;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media;
using APIPortalKiosco.Data;
using APIPortalKiosco.Entities;
using Newtonsoft.Json;
using static Org.BouncyCastle.Math.EC.ECCurve;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;
using System.Linq;
using Microsoft.Extensions.Configuration;
using System.Configuration;
using System.IO;
using Image = System.Windows.Controls.Image;
using System.Windows.Media.Imaging;

namespace Portal.Kiosco.Properties.Views
{
    /// <summary>
    /// Lógica de interacción para Frame9.xaml
    /// </summary>
    public partial class Combos : Window
    {

        private IConfiguration configuration;

        public Combos()
        {
            InitializeComponent();
            

            // Cargar la configuración desde appsettings.json
            
            DataContext = ((App)Application.Current);
            if (App.ob_diclst.Count > 0)
            {
                lblnombre.Content = "!HOLA " + App.ob_diclst["Nombre"].ToString() + " " + App.ob_diclst["Apellido"].ToString();
            }
            else
            {
                lblnombre.Content = "!HOLA INVITADO";
            }
            CombosConsult();
        }
        //private void MinusButton_Click(object sender, RoutedEventArgs e)
        //{
        //    int currentValue = int.Parse(countLabel.Content.ToString());
        //    if (currentValue > 0)
        //    {
        //        countLabel.Content = (currentValue - 1).ToString();
        //    }
        //}

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Restaurar todos los botones a su estado original
            combosBorder.Background = new SolidColorBrush(Colors.LightGray);
            alimentosBorder.Background = new SolidColorBrush(Colors.LightGray);
            bebidasBorder.Background = new SolidColorBrush(Colors.LightGray);
            snacksBorder.Background = new SolidColorBrush(Colors.LightGray);
            combosButton.Foreground = new SolidColorBrush(Colors.Black);
            alimentosButton.Foreground = new SolidColorBrush(Colors.Black);
            bebidasButton.Foreground = new SolidColorBrush(Colors.Black);
            snacksButton.Foreground = new SolidColorBrush(Colors.Black);

            // Obtener el botón que se hizo clic
            System.Windows.Controls.Button clickedButton = sender as System.Windows.Controls.Button;

            // Cambiar el color del botón clicado a rojo con texto blanco
            clickedButton.Foreground = new SolidColorBrush(Colors.White);
            clickedButton.Background = new SolidColorBrush(Colors.Red);
        }
        //private void PlusButton_Click(object sender, RoutedEventArgs e)
        //{
        //    int currentValue = int.Parse(countLabel.Content.ToString());
        //    countLabel.Content = (currentValue + 1).ToString();
        //}
        public void CombosConsult(string pr_secpro = "", string pr_swtven = "", string pr_tiplog = "", string pr_tbview = "", string Teatro = "0", string Ciudad = "0") {

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
                //Validar redireccion externa
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
                    //Asignar valores PRE
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
                                        int lc_cntcom = CombosWeb.Count;

                                        CombosWeb.Add(item);
                                        CombosWeb[lc_cntcom].OrdenView = pantallas.Orden;
                                        CombosWeb[lc_cntcom].Descripcion_Web = pantallas.Descripcion_Web;
                                        CombosWeb[lc_cntcom].Flag = pantallas.Flag;
                                        break;

                                    case "ALIMENTOS WEB":
                                        int lc_cntali = AlimentosWeb.Count;

                                        AlimentosWeb.Add(item);
                                        AlimentosWeb[lc_cntali].OrdenView = pantallas.Orden;
                                        AlimentosWeb[lc_cntali].Descripcion_Web = pantallas.Descripcion_Web;
                                        AlimentosWeb[lc_cntali].Flag = pantallas.Flag;
                                        break;

                                    case "BEBIDAS WEB":
                                        int lc_cntbeb = BebidasWeb.Count;

                                        BebidasWeb.Add(item);
                                        BebidasWeb[lc_cntbeb].OrdenView = pantallas.Orden;
                                        BebidasWeb[lc_cntbeb].Descripcion_Web = pantallas.Descripcion_Web;
                                        BebidasWeb[lc_cntbeb].Flag = pantallas.Flag;
                                        break;

                                    case "SNACKS WEB":
                                        int lc_cntsnk = SnacksWeb.Count;

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

                        //Validar productos a mostrar alimentos
                        if (pr_tbview == "tab-alimentos")
                            CrearCombosYbebidas(AlimentosWeb.OrderBy(o => o.OrdenView).ToList());
                        //Validar productos a mostrar bebidas
                        if (pr_tbview == "tab-bebidas")
                            CrearCombosYbebidas(BebidasWeb.OrderBy(o => o.OrdenView).ToList());

                        //Validar productos a mostrar snacks
                        if (pr_tbview == "tab-snacks")
                            CrearCombosYbebidas(SnacksWeb.OrderBy(o => o.OrdenView).ToList());
                    }
                }
                else
                {
                    lc_result = lc_result.Replace("1-", "");
                }
                #endregion

            }
            catch (Exception lc_syserr)
            {

            }

        }








        public void CrearCombosYbebidas(List<Producto> productos) {
            
            string appSettingsPath = "C:\\FALCROSOFT\\PROCINAL\\Portal.Kiosco\\appsettings.json";

            // Cargar la configuración desde el archivo appsettings.json
            var builder = new ConfigurationBuilder()
                .AddJsonFile(appSettingsPath, optional: true, reloadOnChange: true);

            configuration = builder.Build();

            var myConfigSection = configuration.GetSection("MyConfig");

            // Obtener la URL de las imágenes desde la configuración
            string urlRetailImg = myConfigSection["UrlRetailImg"];

            if (productos != null)
            {
                foreach (var item in productos)
                {
                   
                    string lc_auxcod = item.Codigo.ToString();
                    string lc_auximg = string.Concat(urlRetailImg, lc_auxcod.Substring(0, lc_auxcod.Length - 2), ".jpg");


                    Image imagen = new Image();
                    imagen.Source = new BitmapImage(new Uri(lc_auximg));
                    imagenes.Children.Add(imagen);

                }

            }

        }

        private void btnSiguiente_Click(object sender, RoutedEventArgs e)
        {
            Combodeluxe1 w = new Combodeluxe1();
            this.Close();
            w.ShowDialog();
        }

        private void btnVolver_Click(object sender, RoutedEventArgs e)
        {
            AlgoParaComer w = new AlgoParaComer();
            this.Close();
            w.ShowDialog();
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
