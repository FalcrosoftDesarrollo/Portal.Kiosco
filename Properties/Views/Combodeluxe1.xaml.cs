using APIPortalKiosco.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Org.BouncyCastle.Asn1.Mozilla;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace Portal.Kiosco.Properties.Views
{

    public partial class Combodeluxe1 : Window
    {
        private readonly IOptions<MyConfig> config;
        private IConfiguration configuration;
        private int ContadorProductos = 1;
        private List<(decimal CodigoBotella, string NombreFinalBotella, decimal PrecioFinalBotella, string frecuenciaBotella, decimal categoria, int cantidad)> datosFinalesBotella = new List<(decimal, string, decimal, string, decimal, int)>();
        private List<(decimal CodigoComida, string NombreFinalComida, decimal PrecioFinalComida, string frecuenciaComida, decimal categoria, int cantidad)> datosFinalesComida = new List<(decimal, string, decimal, string, decimal, int)>();
        private bool isThreadActive = true;
        private List<Producto> productosCambiados = new List<Producto>();

        public Combodeluxe1()
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
            CrearCombosYbebidas(App.ProductosSeleccionados);
            ProductosAdicionales();
            DoubleAnimation fadeInAnimation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.5));
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

        public void CrearCombosYbebidas(List<Producto> productos)
        {

            radiobebidas.Children.Clear();
            radioComidas.Children.Clear();
            radioAdicionales.Children.Clear();

            Producto ob_datpro = new Producto();

            int CodigoBebidas = 1244;
            int CodigoBebidas2 = 2444;
            int CodigoComidas = 246;


            string appSettingsPath = "C:\\FALCROSOFT\\PROCINAL\\Portal.Kiosco\\appsettings.json";

            var builder = new ConfigurationBuilder()
                .AddJsonFile(appSettingsPath, optional: true, reloadOnChange: true);

            configuration = builder.Build();

            var myConfigSection = configuration.GetSection("MyConfig");

            string urlRetailImg = myConfigSection["UrlRetailImg"];

            if (productos != null)
            {
                var itemContador = 1;
                radioComidas.Children.Clear();
                radiobebidas.Children.Clear();
                foreach (var item in productos)
                {
                    if (item.Tipo == "C")
                    {
                        if (ContadorProductos == itemContador)
                        {
                            string lc_auxcod = item.Codigo.ToString();
                            string lc_auximg = string.Concat(urlRetailImg, lc_auxcod.Substring(0, lc_auxcod.Length - 2), ".jpg");

                            ImgCombo.Source = new BitmapImage(new Uri(lc_auximg));
                            var precio = buscarprecio(productos, Convert.ToDecimal(item.Codigo));

                            totalLabel.Content = precio.ToString("C0");

                            lblCombo.Content = item.Descripcion + " #" + ContadorProductos.ToString();
                            foreach (var itemReceta in item.Receta)
                            {
                                var cantidad = itemReceta.Cantidad;

                                for (int lc_variii = 0; lc_variii < cantidad; lc_variii++)
                                {
                                    if (itemReceta.RecetaCategoria != null)
                                    {
                                        if (CodigoComidas == itemReceta.Codigo)
                                        {
                                            foreach (var itemRecetaCategoria in itemReceta.RecetaCategoria)
                                            {
                                                var radioButton = new RadioButton();
                                                radioButton.Content = itemRecetaCategoria.Descripcion;
                                                radioButton.Name = "v" + lc_variii.ToString() + Convert.ToInt32(itemRecetaCategoria.Codigo).ToString();
                                                radioButton.FontSize = 24; // Establece el tamaño de fuente como un valor numérico
                                                radioButton.HorizontalAlignment = HorizontalAlignment.Left; // Establece la alineación horizontal
                                                radioButton.GroupName = "Comida" + lc_variii.ToString();
                                                if (itemRecetaCategoria.Frecuente == "True")
                                                {
                                                    radioButton.IsChecked = true;
                                                }
                                                radioComidas.Children.Add(radioButton); // Agrega el botón de radio al contenedor

                                                // Agregar el evento Checked
                                                radioButton.Checked += RadioButton_Checked;
                                            }
                                        }

                                        if (CodigoBebidas2 == itemReceta.Codigo || CodigoBebidas == itemReceta.Codigo)
                                        {
                                            foreach (var itemRecetaCategoria in itemReceta.RecetaCategoria)
                                            {
                                                var radioButton = new RadioButton();
                                                radioButton.Content = itemRecetaCategoria.Descripcion;
                                                radioButton.FontSize = 24;
                                                radioButton.Name = "v" + lc_variii.ToString() + Convert.ToInt32(itemRecetaCategoria.Codigo).ToString();
                                                radioButton.HorizontalAlignment = HorizontalAlignment.Left;
                                                radioButton.GroupName = "Bebida" + lc_variii.ToString();
                                                if (itemRecetaCategoria.Frecuente == "True")
                                                {
                                                    radioButton.IsChecked = true;
                                                }
                                                radiobebidas.Children.Add(radioButton);

                                                // Agregar el evento Checked
                                                radioButton.Checked += RadioButton_Checked;
                                            }

                                        }
                                    }


                                }

                            }
                        }
                        itemContador++;
                    }

                }
                ContadorProductos++;
            }
            ProductosAdicionales();
        }

        public void ProductosAdicionales()
        {
            var snacks = App.SnacksWeb;

            foreach (var itemRecetaCategoria in snacks)
            {
                var radioButton = new RadioButton();
                radioButton.Content = itemRecetaCategoria.Descripcion;
                radioButton.Name = "v" + Convert.ToInt32(itemRecetaCategoria.Codigo).ToString();
                radioButton.FontSize = 24; // Establece el tamaño de fuente como un valor numérico
                radioButton.HorizontalAlignment = HorizontalAlignment.Left; // Establece la alineación horizontal
                radioButton.GroupName = "Adicionales";
                radioAdicionales.Children.Add(radioButton); // Agrega el botón de radio al contenedor
                // Agregar el evento Checked
                radioButton.Checked += RadioButton_Checked;
            }
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            var radioButton = (RadioButton)sender;
            var descripcion = radioButton.Content.ToString();
            var codigo = radioButton.Name.Substring(1);

            var preciodefault = datosFinalesComida
    .Where(x => x.frecuenciaComida == "default")
    .Sum(x => x.PrecioFinalComida);
            decimal preciobebida = 0;
            decimal preciocomida = 0;
            decimal precioAdicionales = 0;

            var opcionSeleccionadabebidas = 0;
            var opcionSeleccionadacomidas = 0;


            List<(decimal CodigoBotella, string NombreFinalBotella, decimal PrecioFinalBotella, string frecuenciaBotella, decimal categoria, int cantidad)> opcionSeleccionadabebidasDetalle = new List<(decimal, string, decimal, string, decimal, int)>();
            List<(decimal CodigoComida, string NombreFinalComida, decimal PrecioFinalComida, string frecuenciaComida, decimal categoria, int cantidad)> opcionSeleccionadacomidasDetalle = new List<(decimal, string, decimal, string, decimal, int)>();

            foreach (var child in radioComidas.Children)
            {
                if (child is RadioButton radioButtonComida && radioButtonComida.IsChecked == true)
                {
                    opcionSeleccionadacomidas = Convert.ToInt32(radioButtonComida.Name.Substring(2));
                    preciocomida += datosFinalesComida.FirstOrDefault(x => x.CodigoComida == opcionSeleccionadacomidas).PrecioFinalComida;
                    var producto = datosFinalesComida.FirstOrDefault(x => x.CodigoComida == opcionSeleccionadacomidas);
                    opcionSeleccionadacomidasDetalle.Add(producto);
                }
            }

            foreach (var child in radiobebidas.Children)
            {

                if (child is RadioButton radioButtonBebidas)
                {
                    if (radioButtonBebidas.IsChecked == true)
                    {
                        opcionSeleccionadabebidas = Convert.ToInt32(radioButtonBebidas.Name.Substring(2));

                        preciobebida += datosFinalesBotella.FirstOrDefault(x => x.CodigoBotella == opcionSeleccionadabebidas).PrecioFinalBotella;
                        var producto = datosFinalesBotella.FirstOrDefault(x => x.CodigoBotella == opcionSeleccionadabebidas);
                        opcionSeleccionadabebidasDetalle.Add(producto);
                    }
                }
            }

            foreach (var child in radioAdicionales.Children)
            {
                if (child is RadioButton radioButtonAdicionales && radioButtonAdicionales.IsChecked == true)
                {
                    // Se encontró un RadioButton seleccionado
                    string opcionSeleccionada = radioButtonAdicionales.Name.Substring(1);
                    precioAdicionales = buscarPrecioAdicionales(opcionSeleccionada);

                }
            }


            var datosselecionado = string.Empty;
            foreach (var productocambiado in App.ProductosSeleccionados)
            {
                var productonew = new Producto();

                if (opcionSeleccionadabebidasDetalle.Count == 1 && opcionSeleccionadacomidasDetalle.Count == 1)
                {

                    productonew = productocambiado;
                    productonew.Valor = (preciodefault + preciocomida + preciobebida + precioAdicionales).ToString();
                    productonew.CanCategoria_1 = 1;
                    productonew.Cantidad = 1;

                    productonew.Cantidad1 = 1;
                    productonew.Cantidad11 = 0;
                    productonew.Cantidad111 = 0;
                    productonew.Cantidad1111 = 0;


                    productonew.ProCantidad_1 = 1;
                    productonew.ProCantidad_2 = 0;
                    productonew.ProCategoria_1 = opcionSeleccionadabebidasDetalle.FirstOrDefault().categoria;
                    productonew.ProProducto_1 = opcionSeleccionadabebidasDetalle.FirstOrDefault().CodigoBotella;
                    productonew.ProProducto_2 = 0;
                    productonew.ProProducto_3 = 0;
                    productonew.ProProducto_4 = 0;
                    productonew.ProProducto_5 = 0;

                    productonew.NombreEli = App.NombreEli;
                    productonew.EmailEli = App.EmailEli;

                    productonew.Tipo = "C";
                    productonew.TipoCompra = "P";
                    productonew.TelefonoEli = App.TelefonoEli;
                    productonew.SwitchAdd = "N";
                    productonew.SwtVenta = "V";
                    productonew.Receta = null;

                    foreach (var bebidas in opcionSeleccionadabebidasDetalle)
                    {
                        datosselecionado = bebidas.CodigoBotella.ToString() + "-" + bebidas.NombreFinalBotella.ToString() + "+Categoria:" + bebidas.categoria + "-Precio:" + bebidas.PrecioFinalBotella;
                    }

                    productonew.Check1 = datosselecionado;

                }

                productosCambiados.Add(productonew);


            }

            totalLabel.Content = (preciodefault + preciocomida + preciobebida + precioAdicionales).ToString("C0");

        }


        public decimal buscarPrecioAdicionales(string opcionSeleccionada)
        {
            decimal Precios = 0;
            var adicionales = App.SnacksWeb;

            foreach (var item in adicionales)
            {
                if (item.Codigo == Convert.ToDecimal(opcionSeleccionada))
                {
                    foreach (var precios in item.Precios)
                    {
                        Precios = precios.General;
                    }
                }

            }

            return Precios;
        }

        public decimal buscarprecio(List<Producto> productos, Decimal Codigo)
        {
            datosFinalesBotella = new List<(decimal, string, decimal, string, decimal, int)>();
            datosFinalesComida = new List<(decimal, string, decimal, string, decimal, int)>();

            List<Producto> ob_return = new List<Producto>();
            Dictionary<string, object> ob_diclst = new Dictionary<string, object>();

            Secuencia ob_scopre = new Secuencia();
            Producto ob_datpro = new Producto();
            General ob_fncgrl = new General();
            int CodigoBebidas = 1244;
            int CodigoBebidas2 = 2444;
            int CodigoComidas = 246;

            decimal Precios = 0;
            var itemContador = 1;
            foreach (var itepro in productos)
            {
                if (itemContador == ContadorProductos)
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
                                    Precios += (itempre.General * itepro.Cantidad);
                                };
                                datosFinalesComida.Add((itepro.Codigo, itepro.Descripcion, Precios, "true", itepro.Codigo, Convert.ToInt32(itepro.Cantidad)));

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
                                            datosFinalesComida.Add((itemprer.Codigo, itemprer.Descripcion, itempre.General, "default", itemprer.Codigo, Convert.ToInt32(itepro.Cantidad)));
                                            Precios += (itempre.General * itemprer.Cantidad);
                                        };
                                    }
                                }

                                ob_datpro.Precios = precio_Combo;

                                bool condicionCumplida = false;

                                foreach (var itecat in itepro.Receta)
                                {
                                    var cantidad = itecat.Cantidad;


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
                                                Precios += precioFinalBotella * itecat.Cantidad;
                                            }

                                            datosFinalesBotella.Add((CodioBotella, NombreFinalBotella, precioFinalBotella, frecuenciaBotella, itecat.Codigo, 1));

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

                                            datosFinalesComida.Add((CodioComida, NombreFinalComida, precioFinalComida, frecuenciaComida, itecat.Codigo, 1));

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
                        break;

                    }
                }
                itemContador++;
            }

            return Precios;

        }

        private async void btnSiguiente_Click(object sender, RoutedEventArgs e)
        {

            if (ContadorProductos > App.ProductosSeleccionados.Count())
            {
                App.
                isThreadActive = false;
                ResumenCompra openWindows = new ResumenCompra(config);
                openWindows.Show();
                this.Close();
            }
            else
            {
                CrearCombosYbebidas(App.ProductosSeleccionados);
                ProductosAdicionales();
            }
        }

        private async void btnVolver_Click(object sender, RoutedEventArgs e)
        {
            isThreadActive = false;
            App.ProductosSeleccionados.Clear();
            Combos openWindows = new Combos();
            openWindows.Show();
            this.Close();
        }

        private async void btnSalir_Click(object sender, RoutedEventArgs e)
        {
            isThreadActive = false;
            Principal openWindows = new Principal();
            openWindows.Show();
            this.Close();
        }
    }
}