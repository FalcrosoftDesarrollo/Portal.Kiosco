using APIPortalKiosco.Entities;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace Portal.Kiosco.Properties.Views
{
    /// <summary>
    /// Lógica de interacción para Frame9.xaml
    /// </summary>
    public partial class Combos : Window
    {

        private IConfiguration configuration;
        private int SelectProd = 1;

        private List<UIElement> elementosCombos = new List<UIElement>();
        private List<UIElement> elementosAlimentos = new List<UIElement>();
        private List<UIElement> elementosBebidas = new List<UIElement>();
        private List<UIElement> elementosSnack = new List<UIElement>();


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
            App.ProductosSeleccionados.Clear();
            CombosConsult();
            App.ProductosSeleccionados = new List<Producto>();
        }

        private Border clickedBorder;

        private void tapAlimentos_Click(object sender, RoutedEventArgs e)
        {
            RestoreButtonState();

            System.Windows.Controls.Button clickedButton = sender as System.Windows.Controls.Button;

            clickedBorder = GetButtonBorder(clickedButton);

            clickedButton.Foreground = new SolidColorBrush(Colors.White);
            clickedButton.Background = new SolidColorBrush(Colors.Red);
            clickedBorder.Background = new SolidColorBrush(Colors.Red);


        }



        private Border GetButtonBorder(System.Windows.Controls.Button button)
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
                    // Manejar un caso no previsto
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
                        if (pr_tbview == "" || pr_tbview == "tab-combos") { 
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
            catch (Exception lc_syserr)
            {

            }

        }

        public void CrearCombosYbebidas(List<Producto> productos)
        {

            imagenes.Children.Clear();

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

                    // Crear el contenedor del diseño proporcionado
                    Border border = new Border();
                    border.Width = 300;
                    border.Height = 550;
                    border.Margin = new Thickness(5);
                    border.BorderBrush = Brushes.Transparent;
                    border.BorderThickness = new Thickness(1);

                    // Agregar el Grid al Border
                    Grid grid = new Grid();
                    border.Child = grid;

                    // Definir las columnas del Grid
                    grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                    grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                    grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                    // Definir las filas del Grid
                    grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(462) });
                    grid.RowDefinitions.Add(new RowDefinition());

                    // Agregar la imagen al Grid
                    Image image = new Image();
                    image.Source = new BitmapImage(new Uri(lc_auximg));
                    Grid.SetRow(image, 0);
                    Grid.SetColumnSpan(image, 3);
                    grid.Children.Add(image);

                    // Agregar los elementos interiores al Grid
                    Border innerBorder = new Border();
                    innerBorder.HorizontalAlignment = HorizontalAlignment.Center;
                    innerBorder.Width = 250;
                    Grid.SetRow(innerBorder, 1);
                    Grid.SetColumnSpan(innerBorder, 3);
                    grid.Children.Add(innerBorder);

                    // Definir el contenido del interior del Border
                    Grid innerGrid = new Grid();
                    innerBorder.Child = innerGrid;

                    // Definir las columnas del Grid interior
                    innerGrid.ColumnDefinitions.Add(new ColumnDefinition());
                    innerGrid.ColumnDefinitions.Add(new ColumnDefinition());
                    innerGrid.ColumnDefinitions.Add(new ColumnDefinition());

                    // Agregar el botón "-" al Grid interior
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

                    // Agregar el Label al Grid interior
                    Label countLabel = new Label();
                    countLabel.FontFamily = new FontFamily("Myanmar Khyay");
                    countLabel.FontSize = 32;
                    countLabel.Content = "0";
                    countLabel.Name = string.Concat("lbl", lc_auxcod.Substring(0, lc_auxcod.Length - 2));
                    countLabel.VerticalAlignment = VerticalAlignment.Center;
                    countLabel.HorizontalAlignment = HorizontalAlignment.Center;
                    Grid.SetColumn(countLabel, 1);
                    innerGrid.Children.Add(countLabel);

                    // Agregar el botón "+" al Grid interior
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
                    // Manejar un caso no previsto
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

            string totalString = totalLabel.Content.ToString().Replace("$", "").Replace("€", "").Replace(".", "").Replace(",", "").Trim(); // Remueve el símbolo de la moneda y cualquier separador de miles
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
                else
                {
                    var openWindow = new ResumenCompra();
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
            else
            {
                MessageBox.Show("Ups, aun no haz seleccionado un producto.", "Notificación", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private async void btnVolver_Click(object sender, RoutedEventArgs e)
        {
            if (App.IsBoleteriaConfiteria == false)
            {
             
                App.IsCinefans = true;
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
            else
            {
              
                App.IsCinefans = true;
                var openWindow = new ComoCompra();
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
