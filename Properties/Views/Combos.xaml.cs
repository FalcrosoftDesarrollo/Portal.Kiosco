using APIPortalKiosco.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Portal.Kiosco.Properties.Views
{
    public partial class Combos : Window
    {
        private IConfiguration configuration;
        private int SelectProd = 1;
        private readonly IOptions<MyConfig> config;
        private List<UIElement> elementosCombos = new List<UIElement>();
        private List<UIElement> elementosAlimentos = new List<UIElement>();
        private List<UIElement> elementosBebidas = new List<UIElement>();
        private List<UIElement> elementosSnack = new List<UIElement>();
        private bool isThreadActive = true;
        private string pantalla = "C";
        public Combos()
        {
            InitializeComponent();
            DataContext = ((App)Application.Current);
            try
            {
                if (App.ob_diclst.Count > 0)
                {
                    lblnombre.Content = "!HOLA " + App.ob_diclst["Nombre"].ToString() + " " + App.ob_diclst["Apellido"].ToString() + "!";
                }
                else
                {
                    lblnombre.Content = "!HOLA INVITADO¡";
                }
            }
            catch (Exception e)
            { lblnombre.Content = "!HOLA INVITADO¡"; }


            try
            {
                //App.ProductosSeleccionados.Clear();
                CombosConsult();
                //App.ProductosSeleccionados = new List<Producto>();

                DoubleAnimation fadeInAnimation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.5));
                gridPrincipal.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

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

        public async Task LoadDataAsync()
        {
            await Task.Run(() =>
            {
                System.Threading.Thread.Sleep(3000);
            });
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


        private Border clickedBorder;

        private void tapAlimentos_Click(object sender, RoutedEventArgs e)
        {
            RestoreButtonState();

            Button clickedButton = sender as Button;

            clickedBorder = GetButtonBorder(clickedButton);


            if (clickedButton.Name != "tabcombos")
            {
                pantalla = "N";
            }
            else
            {
                pantalla = "C";
            }

            clickedButton.Foreground = new SolidColorBrush(Colors.White);
            clickedButton.Background = new SolidColorBrush(Colors.Red);
            clickedBorder.Background = new SolidColorBrush(Colors.Red);
        }

        private Border GetButtonBorder(Button button)
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

        public async void CombosConsult(string pr_secpro = "", string pr_swtven = "", string pr_tiplog = "", string pr_tbview = "", string Teatro = "0", string Ciudad = "0")
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
                    lc_result = await ob_fncgrl.WebServicesAsync(string.Concat(App.ScoreServices, "scopre/"), lc_srvpar);
                else
                    lc_result = await ob_fncgrl.WebServicesAsync(string.Concat(App.ScoreServices, "scopcf/"), lc_srvpar);

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
                        App.Adicionales = new List<Producto>();
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

                                    case "ADICIONES WEB":
                                        int lc_cntadd = App.Adicionales.Count();

                                        App.Adicionales.Add(item);
                                        App.Adicionales[lc_cntadd].OrdenView = pantallas.Orden;
                                        break;
                                }
                            }
                        }

                        //Validar productos a mostrar combos
                        if (pr_tbview == "" || pr_tbview == "tab-combos")
                        {
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
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        public void CrearCombosYbebidas(List<Producto> productos)
        {
            var ultimoProducto = productos[productos.Count - 1];

            productos.RemoveAt(productos.Count - 1);
            productos.Insert(0, ultimoProducto);
            imagenes.Children.Clear();

            string urlRetailImg = App.UrlRetailImg;

            if (productos != null)
            {
                foreach (var item in productos.OrderBy(o => o.OrdenView))
                {
                    string lc_auxcod = item.Codigo.ToString();
                    string lc_auximg = string.Concat(urlRetailImg, lc_auxcod.Substring(0, lc_auxcod.Length - 2), ".jpg");

                    Border border = new Border
                    {
                        Width = 300,
                        Height = 550,
                        Margin = new Thickness(5),
                        BorderBrush = Brushes.Transparent,
                        BorderThickness = new Thickness(1)
                    };

                    Grid grid = new Grid();
                    border.Child = grid;

                    grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                    grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                    grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                    grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(300) }); // Tamaño fijo para la imagen
                    grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // Ajuste automático
                    grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // Ajuste automático
                    grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(100) }); // Ajuste automático
                    grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // Ajuste automático


                    // Imagen con Border redondeado y sombra
                    var imageBorder = new Border
                    {
                        CornerRadius = new CornerRadius(10), // Esquinas redondeadas para la imagen
                        Margin = new Thickness(10),
                        Effect = new DropShadowEffect
                        {
                            Color = Colors.Black,
                            Direction = 270,
                            ShadowDepth = 5,
                            Opacity = 0.5
                        },
                        OpacityMask = new VisualBrush
                        {
                            Stretch = Stretch.Fill,
                            Visual = new Rectangle
                            {
                                RadiusX = 0.1,
                                RadiusY = 0.1,
                                Width = 1,
                                Height = 1,
                                Fill = Brushes.Black
                            }
                        },
                        Child = new Image
                        {
                            Source = new BitmapImage(new Uri(lc_auximg)),
                            Stretch = Stretch.Uniform,
                            StretchDirection = StretchDirection.Both
                        }
                    };

                    Grid.SetRow(imageBorder, 0);
                    Grid.SetColumnSpan(imageBorder, 3);
                    grid.Children.Add(imageBorder);

                    // Título Combo
                    TextBlock titulocombo = new TextBlock
                    {
                        Text = item.Descripcion.ToUpper(),
                        Foreground = Brushes.Black,
                        Background = Brushes.White,
                        FontSize = 16,
                        FontWeight = FontWeights.Bold,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        TextAlignment = TextAlignment.Center,
                        TextWrapping = TextWrapping.Wrap,
                        Margin = new Thickness(0, 0, 0, 10)
                    };

                    Grid.SetRow(titulocombo, 1);
                    Grid.SetColumnSpan(titulocombo, 3);
                    grid.Children.Add(titulocombo);


                    // Precio
                    TextBlock Precio = new TextBlock
                    {
                        Text = buscarprecio(productos, item.Codigo, "").ToString("C0"),
                        Foreground = Brushes.Black,
                        Background = Brushes.White,
                        FontSize = 12,
                        FontWeight = FontWeights.Bold,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        TextAlignment = TextAlignment.Center,
                        TextWrapping = TextWrapping.Wrap,
                        Margin = new Thickness(0, 0, 0, 10)
                    };
                    Grid.SetRow(Precio, 2);
                    Grid.SetColumnSpan(Precio, 3);
                    grid.Children.Add(Precio);

                    // Descripción
                    TextBlock tituloDescripcion = new TextBlock
                    {
                        Text = item.Descripcion_Web,
                        Foreground = Brushes.Black,
                        Background = Brushes.White,
                        FontSize = 12,
                        FontWeight = FontWeights.Normal,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        TextAlignment = TextAlignment.Center,
                        TextWrapping = TextWrapping.Wrap,
                        Margin = new Thickness(0, 0, 0, 10)
                    };
                    Grid.SetRow(tituloDescripcion, 3);
                    Grid.SetColumnSpan(tituloDescripcion, 3);
                    grid.Children.Add(tituloDescripcion);

                    // Inner Border para los botones
                    Border innerBorder = new Border
                    {
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Width = 200,
                        Margin = new Thickness(0, 10, 0, 0)
                    };

                    Grid.SetRow(innerBorder, 4);
                    Grid.SetColumnSpan(innerBorder, 3);
                    grid.Children.Add(innerBorder);


                    Grid innerGrid = new Grid();
                    innerBorder.Child = innerGrid;

                    innerGrid.ColumnDefinitions.Add(new ColumnDefinition());
                    innerGrid.ColumnDefinitions.Add(new ColumnDefinition());
                    innerGrid.ColumnDefinitions.Add(new ColumnDefinition());

                    // Botón Menos
                    Border minusButtonBorder = new Border
                    {
                        CornerRadius = new CornerRadius(100),
                        Background = new SolidColorBrush(Color.FromRgb(243, 6, 19)),
                        HorizontalAlignment = HorizontalAlignment.Left,
                        Width = 61,
                        Height = 61
                    };
                    Grid.SetColumn(minusButtonBorder, 0);
                    innerGrid.Children.Add(minusButtonBorder);

                    Button minusButton = new Button
                    {
                        Background = Brushes.Transparent,
                        BorderThickness = new Thickness(0),
                        Content = "-",
                        FontFamily = new FontFamily("Myanmar Khyay"),
                        FontSize = 50,
                        Foreground = Brushes.White
                    };
                    minusButton.Click += MinusButton_Click;
                    minusButtonBorder.Child = minusButton;

                    // Etiqueta de Conteo
                    Label countLabel = new Label
                    {
                        FontFamily = new FontFamily("Myanmar Khyay"),
                        FontSize = 32,
                        Content = App.ProductosSeleccionados == null ? "0" :App.ProductosSeleccionados.Where(x => x.Codigo == item.Codigo).Count().ToString(),
                        Name = string.Concat("lbl", lc_auxcod.Substring(0, lc_auxcod.Length - 2)),
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Center
                    };
                    Grid.SetColumn(countLabel, 1);
                    innerGrid.Children.Add(countLabel);

                    // Botón Más
                    Border plusButtonBorder = new Border
                    {
                        CornerRadius = new CornerRadius(100),
                        Background = new SolidColorBrush(Color.FromRgb(243, 6, 19)),
                        HorizontalAlignment = HorizontalAlignment.Right,
                        Width = 61,
                        Height = 61
                    };
                    Grid.SetColumn(plusButtonBorder, 2);
                    innerGrid.Children.Add(plusButtonBorder);

                    Button plusButton = new Button
                    {
                        Background = Brushes.Transparent,
                        BorderThickness = new Thickness(0),
                        Content = "+",
                        FontFamily = new FontFamily("Myanmar Khyay"),
                        FontSize = 50,
                        Foreground = Brushes.White
                    };
                    plusButton.Click += PlusButton_Click;
                    plusButtonBorder.Child = plusButton;

                    imagenes.Children.Add(border);
                }

                // Aquí puedes agregar más lógica si es necesario
            }
        }

        public decimal SelPrecio(int SelectProd, decimal Codigo, string accion)
        {

            var producto = new List<Producto>();
            decimal Precios = 0;

            switch (SelectProd)
            {
                case 1:

                    Precios = buscarprecio(App.CombosWeb, Codigo, accion);
                    elementosCombos.Clear();

                    foreach (UIElement elemento in imagenes.Children)
                    {
                        elementosCombos.Add(elemento);
                    }
                    SelectProd = 1;
                    break;

                case 2:
                    Precios = buscarprecio(App.AlimentosWeb, Codigo, accion);
                    elementosAlimentos.Clear();

                    foreach (UIElement elemento in imagenes.Children)
                    {
                        elementosAlimentos.Add(elemento);
                    }
                    SelectProd = 2;
                    break;

                case 3:
                    Precios = buscarprecio(App.BebidasWeb, Codigo, accion);
                    elementosBebidas.Clear();

                    foreach (UIElement elemento in imagenes.Children)
                    {
                        elementosBebidas.Add(elemento);
                    }
                    SelectProd = 3;
                    break;

                case 4:
                    Precios = buscarprecio(App.SnacksWeb, Codigo, accion);
                    elementosSnack.Clear();

                    foreach (UIElement elemento in imagenes.Children)
                    {
                        elementosSnack.Add(elemento);
                    }
                    SelectProd = 4;
                    break;

                default:
                    break;
            }

            return Precios;
        }

        public decimal buscarprecio(List<Producto> productos, Decimal Codigo, string accion)
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

                                        if (Convert.ToBoolean(frecuenciaBotella) == true)
                                        {
                                            Precios += precioFinalBotella * itecat.Cantidad;
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
                                            Precios += precioFinalComida * itecat.Cantidad;
                                        }

                                        datosFinalesComida.Add((CodioComida, NombreFinalComida, precioFinalComida, frecuenciaComida, itecat.Codigo));
                                    }
                                }

                                if (datosFinalesBotella.Count > 0 && datosFinalesComida.Count > 0)
                                {
                                    condicionCumplida = true;
                                }

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

                    if (accion == "S")
                    {
                        App.ProductosSeleccionados.Add(itepro);
                    }


                    break;
                }
            }

            return Precios;
        }

        private void PlusButton_Click(object sender, RoutedEventArgs e)
        {
            App.CodigoProducto = "";
            Button button = (Button)sender;
            Border parentBorder = (Border)button.Parent;
            Grid innerGrid = (Grid)parentBorder.Parent;
            Label countLabel = (Label)innerGrid.Children[1];

            int currentValue = int.Parse(countLabel.Content.ToString());
            currentValue++;
            countLabel.Content = currentValue.ToString();
            var precio = SelPrecio(SelectProd, Convert.ToDecimal(countLabel.Name.Substring(3)), "S");

            string totalString = totalLabel.Content.ToString().Replace("$", "").Replace("€", "").Replace(".", "").Replace(",", "").Trim();
            decimal totalAnterior = decimal.Parse(totalString);
            decimal nuevoTotal = totalAnterior + precio;
            totalLabel.Content = nuevoTotal.ToString("C0");


            string codigo = countLabel.Name.Substring(3);
            var productoAEliminar = App.ProductosSeleccionados.FirstOrDefault(producto => producto.Codigo == Convert.ToDecimal(countLabel.Name.Substring(3)));
            App.CodigoProducto = countLabel.Name.Substring(3);
           var codigoProducto = Convert.ToDecimal(countLabel.Name.Substring(3));

            if (pantalla == "C")
            {
                ApplyBlurEffect();
                var window = new Combopopup(this, sender);
                window.Cancelled += Popup_Cancelled;
                window.ShowDialog();
                RemoveBlurEffect();
            }
        }

        private void Popup_Cancelled(object sender, EventArgs e)
        {
            // Aquí recibes el sender original que es el Button
            if (sender is Button button)
            {
                DecrementProductCount(button);
            }
        }

        private Button GetSpecificButton()
        {
            // Implementa la lógica para obtener el botón específico que necesitas
            // Por ejemplo, si tienes un contenedor 'imagenes' que contiene todos los botones generados
            foreach (Border border in imagenes.Children)
            {
                Grid grid = border.Child as Grid;
                if (grid != null)
                {
                    foreach (UIElement element in grid.Children)
                    {
                        if (element is Border innerBorder)
                        {
                            Grid innerGrid = innerBorder.Child as Grid;
                            if (innerGrid != null)
                            {
                                foreach (UIElement innerElement in innerGrid.Children)
                                {
                                    if (innerElement is Button button && button.Content.ToString() == "-")
                                    {
                                        return button; // O alguna otra condición específica para encontrar tu botón
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }


        private void ApplyBlurEffect()
        {
            BlurEffect blur = new BlurEffect
            {
                Radius = 10 // Ajusta el radio del desenfoque según tus necesidades
            };
            gridcombos.Effect = blur;
        }

        private void RemoveBlurEffect()
        {
            gridcombos.Effect = null;
        }

        //private void MinusButton_Click(object sender, RoutedEventArgs e)
        //{
        //    Button button = (Button)sender;
        //    Border parentBorder = (Border)button.Parent;
        //    Grid innerGrid = (Grid)parentBorder.Parent;
        //    Label countLabel = (Label)innerGrid.Children[1];
        //    var result = new List<Producto>();
        //    int currentValue = int.Parse(countLabel.Content.ToString());
        //    if (currentValue >= 0)
        //    {
        //        currentValue--;
        //        countLabel.Content = currentValue.ToString();

        //        var precio = SelPrecio(SelectProd, Convert.ToDecimal(countLabel.Name.Substring(3)), "");

        //        var codigoProducto = Convert.ToDecimal(countLabel.Name.Substring(3));
        //        var productoAEliminar = App.ProductosSeleccionados.FirstOrDefault(producto => producto.Codigo == codigoProducto);

        //        if (productoAEliminar != null)
        //        {
        //            string totalString = totalLabel.Content.ToString().Replace("$", "").Replace("€", "").Replace(".", "").Replace(",", "").Trim();
        //            decimal totalAnterior = decimal.Parse(totalString);
        //            decimal nuevoTotal = totalAnterior - precio;
        //            totalLabel.Content = nuevoTotal.ToString("C0");


        //            var productos = App.ProductosSeleccionados;


        //            bool eliminado = false;

        //            foreach (var item in productos)
        //            {
        //                if (item.Codigo == codigoProducto && !eliminado)
        //                {
        //                    eliminado = true; // Marcar que ya hemos eliminado un producto con el código especificado
        //                }
        //                else
        //                {
        //                    result.Add(item); // Añadir el producto a la nueva lista
        //                }
        //            }

        //            App.ProductosSeleccionados = result; // Actualizar la lista de productos seleccionados

        //        }

        //        if (currentValue < 0)
        //        {
        //            countLabel.Content = "0";
        //        }
        //    }
        //}

        private void MinusButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            DecrementProductCount(button);
        }

        private void DecrementProductCount(Button button)
        {
            Border parentBorder = (Border)button.Parent;
            Grid innerGrid = (Grid)parentBorder.Parent;
            Label countLabel = (Label)innerGrid.Children[1];
            ActualizarConteoProducto(countLabel);
        }

        private void ActualizarConteoProducto(Label countLabel)
        {
            var result = new List<Producto>();
            int currentValue = int.Parse(countLabel.Content.ToString());
            if (currentValue >= 0)
            {
                currentValue--;
                countLabel.Content = currentValue.ToString();

                var precio = SelPrecio(SelectProd, Convert.ToDecimal(countLabel.Name.Substring(3)), "");

                var codigoProducto = Convert.ToDecimal(countLabel.Name.Substring(3));
                var productoAEliminar = App.ProductosSeleccionados.FirstOrDefault(producto => producto.Codigo == codigoProducto);

                if (productoAEliminar != null)
                {
                    string totalString = totalLabel.Content.ToString().Replace("$", "").Replace("€", "").Replace(".", "").Replace(",", "").Trim();
                    decimal totalAnterior = decimal.Parse(totalString);
                    decimal nuevoTotal = totalAnterior - precio;
                    totalLabel.Content = nuevoTotal.ToString("C0");

                    var productos = App.ProductosSeleccionados;

                    bool eliminado = false;

                    foreach (var item in productos)
                    {
                        if (item.Codigo == codigoProducto && !eliminado)
                        {
                            eliminado = true;
                        }
                        else
                        {
                            result.Add(item);
                        }
                    }

                    App.ProductosSeleccionados = result;
                    App.EliminarRetailSales(Convert.ToDecimal(App.Secuencia), codigoProducto);
                }

                if (currentValue < 0)
                {
                    countLabel.Content = "0";
                }
            }
        }


        private async void btnSiguiente_Click(object sender, RoutedEventArgs e)
        {

            if (totalLabel.Content.ToString() != "0")
            {
                // Procesa los productos seleccionados
                var productos = App.ProductosSeleccionados;

                foreach (var producto in productos.Where(x => x.Tipo == "P"))
                {
                    producto.KeySecuencia = App.Secuencia;
                    producto.Cantidad = 1;
                    producto.SwitchAdd = "S";
                    foreach (var precio in producto.Precios)
                    {
                        producto.Valor = precio.General.ToString();
                    }
                    App.agregarProducto(producto);
                }

                // Verifica si existen combinaciones seleccionadas
                var ProductosSeleccionados = productos.FirstOrDefault(tip => tip.Tipo == "C");

                var transicion = new transicion();
                transicion.Show();
                this.Close();
                ResumenCompra openWindows = new ResumenCompra(config);
                await openWindows.LoadDataAsync();
                openWindows.Show();
                transicion.Close();

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
                isThreadActive = false;
                App.IsCinefans = true;

                var transicion = new transicion();
                transicion.Show();
                this.Close();
                var openWindows = new AlgoParaComer();
                await openWindows.LoadDataAsync();
                openWindows.Show();
                transicion.Close();
            }
            else
            {
                isThreadActive = false;
                App.IsCinefans = true;


                var transicion = new transicion();
                transicion.Show();
                this.Close();
                var openWindows = new ComoCompra();
                await openWindows.LoadDataAsync();
                openWindows.Show();
                transicion.Close();
            }
        }

        private async void btnSalir_Click(object sender, RoutedEventArgs e)
        {
            isThreadActive = false;



            var transicion = new transicion();
            transicion.Show();
            this.Close();
            var openWindows = new Principal();
            await openWindows.LoadDataAsync();
            openWindows.Show();
            transicion.Close();
        }
    }
}