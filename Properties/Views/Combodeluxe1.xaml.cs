using APIPortalKiosco.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Org.BouncyCastle.Asn1.Mozilla;
using Org.BouncyCastle.Tls.Crypto.Impl.BC;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
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
        private int ContadorProductosPantallas = 0;

        private List<Bebida> datosFinalesBotella = new List<Bebida>();
        private List<Comida> datosFinalesComida = new List<Comida>();

        private List<Bebida> datosFinalesBotellaRadio = new List<Bebida>();
        private List<Comida> datosFinalesComidaRadio = new List<Comida>();

        private bool isThreadActive = true;
        private int contadorProdModificados = 1;
        private List<Bebida> opcionSeleccionadabebidasDetalle = new List<Bebida>();
        private List<Comida> opcionSeleccionadacomidasDetalle = new List<Comida>();
        private decimal preciodefault = 0;
        private decimal preciobebida = 0;
        private decimal preciocomida = 0;
        private decimal precioAdicionales = 0;
        private decimal productoactual = 0;

        private decimal precioAntesAdicionales = 0;

        public class Bebida
        {
            public decimal CodigoCombo { get; set; }
            public decimal CodigoBotella { get; set; }
            public string NombreFinalBotella { get; set; }
            public decimal PrecioFinalBotella { get; set; }
            public string frecuenciaBotella { get; set; }
            public decimal categoria { get; set; }
            public int cantidad { get; set; }


        }
        public class Comida
        {
            public decimal CodigoCombo { get; set; }
            public decimal CodigoComida { get; set; }
            public string NombreFinalComida { get; set; }
            public decimal PrecioFinalComida { get; set; }
            public string frecuenciaComida { get; set; }
            public decimal categoria { get; set; }
            public int cantidad { get; set; }


        }

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
                lblnombre.Content = "!HOLA INVITADO¡";
            }

            CrearCombosYbebidas(App.ProductosSeleccionados);


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
        private decimal CodigoProducto;
        public void CrearCombosYbebidas(List<Producto> productos)
        {

            radiobebidas.Children.Clear();
            radioComidas.Children.Clear();
            checkBoxAdicionales.Children.Clear();
            
            Producto ob_datpro = new Producto();

            int CodigoBebidas = 1244;
            int CodigoBebidas2 = 2444;
            int CodigoComidas = 246;
            datosFinalesComidaRadio = new List<Comida>();
            datosFinalesBotellaRadio = new List<Bebida>();

            string urlRetailImg = App.UrlRetailImg;
            decimal Precios = 0;
            if (productos != null)
            {
                var itemContador = 1;
                radioComidas.Children.Clear();
                radiobebidas.Children.Clear();
                foreach (var item in productos)
                {
                    foreach (var itemReceta in item.Receta)
                    {
                        if (itemReceta.Tipo == "P")
                        {

                            if (ContadorProductos == itemContador)
                            {
                                CodigoProducto = item.Codigo;
                                ob_datpro.Codigo = itemReceta.Codigo;
                                ob_datpro.Descripcion = itemReceta.Descripcion;
                                ob_datpro.Tipo = itemReceta.Tipo;
                                ob_datpro.Precios = itemReceta.Precios;



                                foreach (var itempre in itemReceta.Precios)
                                {
                                    Precios = (itempre.General * itemReceta.Cantidad);

                                    var comida = new Comida()
                                    {
                                        CodigoCombo = item.Codigo,
                                        CodigoComida = itemReceta.Codigo,
                                        NombreFinalComida = itemReceta.Descripcion,
                                        PrecioFinalComida = Precios,
                                        frecuenciaComida = "default",
                                        categoria = itemReceta.Codigo,
                                        cantidad = Convert.ToInt32(itemReceta.Cantidad)

                                    };

                                    datosFinalesComidaRadio.Add((comida));
                                };
                            }
                        }
                    }

                    if (item.Tipo == "C")
                    {
                        if (ContadorProductos == itemContador)
                        {
                            string lc_auxcod = item.Codigo.ToString();
                            string lc_auximg = string.Concat(urlRetailImg, lc_auxcod.Substring(0, lc_auxcod.Length - 2), ".jpg");

                            ImgCombo.Source = new BitmapImage(new Uri(lc_auximg));
                            var precio = buscarprecio(productos, Convert.ToDecimal(item.Codigo));

                            totalLabel.Content = precio.ToString("C0");
                            precioAntesAdicionales = precio;
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


                                                radioButton.Checked += RadioButton_Checked;

                                                var CodioComida = itemRecetaCategoria.Codigo;
                                                var NombreFinalComida = itemRecetaCategoria.Descripcion.ToString();
                                                var precioFinalComida = itemRecetaCategoria.Precios.Sum(precio => precio.General);
                                                var frecuenciaComida = itemRecetaCategoria.Frecuente.ToString();


                                                var comidar = new Comida()
                                                {
                                                    CodigoCombo = item.Codigo,
                                                    CodigoComida = CodioComida,
                                                    NombreFinalComida = NombreFinalComida,
                                                    PrecioFinalComida = precioFinalComida,
                                                    frecuenciaComida = frecuenciaComida,
                                                    categoria = itemReceta.Codigo,
                                                    cantidad = Convert.ToInt32(itemRecetaCategoria.Cantidad)

                                                };

                                                datosFinalesComidaRadio.Add(comidar);

                                                //lc_variii++;
                                            }
                                        }

                                        if (CodigoBebidas2 == itemReceta.Codigo || CodigoBebidas == itemReceta.Codigo)
                                        {
                                            //lc_variii = 1;
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

                                                radioButton.Checked += RadioButton_Checked;





                                                var CodioBotella = itemRecetaCategoria.Codigo;
                                                var NombreFinalBotella = itemRecetaCategoria.Descripcion.ToString();
                                                var precioFinalBotella = itemRecetaCategoria.Precios.Sum(precio => precio.General);
                                                var frecuenciaBotella = itemRecetaCategoria.Frecuente.ToString();

                                                var bebida = new Bebida()
                                                {
                                                    CodigoCombo = item.Codigo,
                                                    CodigoBotella = CodioBotella,
                                                    NombreFinalBotella = NombreFinalBotella,
                                                    PrecioFinalBotella = precioFinalBotella,
                                                    frecuenciaBotella = frecuenciaBotella,
                                                    categoria = itemReceta.Codigo,
                                                    cantidad = Convert.ToInt32(itemRecetaCategoria.Cantidad)

                                                };

                                                datosFinalesBotellaRadio.Add(bebida);

                                                //lc_variii++;
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
                ContadorProductosPantallas++;
            }
            ProductosAdicionales();
        }

        public void ProductosAdicionales()
        {
            var snacks = App.SnacksWeb;

            foreach (var itemRecetaCategoria in snacks)
            {
                var checkBox = new CheckBox();
                checkBox.Content = itemRecetaCategoria.Descripcion;
                checkBox.Name = "v" + Convert.ToInt32(itemRecetaCategoria.Codigo).ToString();
                checkBox.FontSize = 24; // Establece el tamaño de fuente como un valor numérico
                checkBox.HorizontalAlignment = HorizontalAlignment.Left; // Establece la alineación horizontal
                checkBox.Checked += CheckBox_Checked;
                checkBox.Unchecked += CheckBox_Unchecked;

                checkBoxAdicionales.Children.Add(checkBox); // Agrega el botón de radio al contenedor
                                                            // Agregar el evento Checked

            }
        }


        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            buscarprecioadicionales();
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            buscarprecioadicionales();
        }

        public void buscarprecioadicionales()
        {
            var snacks = App.SnacksWeb;
            precioAdicionales = 0;
            foreach (var itemRecetaCategoria in snacks)
            {
                foreach (var child in checkBoxAdicionales.Children)
                {
                    if (child is CheckBox checkBoxAdicionales && checkBoxAdicionales.IsChecked == true)
                    {
                        if (checkBoxAdicionales.Name.Substring(1) == Convert.ToInt32(itemRecetaCategoria.Codigo).ToString())
                        {
                            precioAdicionales += itemRecetaCategoria.Precios.FirstOrDefault().General;

                        }
                    }
                }
            }

            totalLabel.Content = (precioAdicionales + precioAntesAdicionales).ToString("C0");

        }


        public void ProductosAdicionalesSeleccion()
        {
            int contadorAdicionales = 1;
            var adicionales = new Adiciones();
            var snacks = App.SnacksWeb;
            adicionales.Secuencia = Convert.ToDecimal(App.Secuencia);
            adicionales.Tipo = "P";
            adicionales.SwitchVenta = "";

            foreach (var itemRecetaCategoria in snacks)
            {
                if (contadorAdicionales > 6) break;

                string cantidadProp = $"Cantidad_{contadorAdicionales}";
                string codigoProp = $"Codigo_{contadorAdicionales}";
                string descripcionProp = $"Descripcion_{contadorAdicionales}";
                string precioProp = $"Precio_{contadorAdicionales}";

                // Siempre asignar el código, descripción y precio
                adicionales.GetType().GetProperty(codigoProp)?.SetValue(adicionales, itemRecetaCategoria.Codigo);
                adicionales.GetType().GetProperty(descripcionProp)?.SetValue(adicionales, itemRecetaCategoria.Descripcion);
                var precioGeneral = itemRecetaCategoria.Precios.FirstOrDefault()?.General;
                if (precioGeneral != null)
                {
                    adicionales.GetType().GetProperty(precioProp)?.SetValue(adicionales, Convert.ToDecimal(precioGeneral));
                }

                // Asignar cantidad solo si el CheckBox correspondiente está seleccionado
                foreach (var child in checkBoxAdicionales.Children)
                {
                    if (child is CheckBox checkBoxAdicionales && checkBoxAdicionales.IsChecked == true)
                    {
                        if (checkBoxAdicionales.Name.Substring(1) == Convert.ToInt32(itemRecetaCategoria.Codigo).ToString())
                        {
                            adicionales.GetType().GetProperty(cantidadProp)?.SetValue(adicionales, 1m);
                            break;
                        }
                    }
                }
                contadorAdicionales++;
            }

            App.AddProduct(adicionales);

        }




        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            var radioButton = (RadioButton)sender;
            var descripcion = radioButton.Content.ToString();
            var codigo = radioButton.Name.Substring(1);
            var snacks = App.SnacksWeb;
            preciodefault = datosFinalesComidaRadio
            .Where(x => x.frecuenciaComida == "default")
            .Sum(x => x.PrecioFinalComida);

            var opcionSeleccionadabebidas = 0;
            var opcionSeleccionadacomidas = 0;
            opcionSeleccionadabebidasDetalle.Clear();
            opcionSeleccionadacomidasDetalle.Clear();
            preciobebida = 0;
            preciocomida = 0;
            precioAdicionales = 0;
            foreach (var child in radioComidas.Children)
            {
                if (child is RadioButton radioButtonComida && radioButtonComida.IsChecked == true)
                {
                    opcionSeleccionadacomidas = Convert.ToInt32(radioButtonComida.Name.Substring(2));
                    preciocomida += datosFinalesComidaRadio.FirstOrDefault(x => x.CodigoComida == opcionSeleccionadacomidas).PrecioFinalComida;
                    var producto = datosFinalesComidaRadio.FirstOrDefault(x => x.CodigoComida == opcionSeleccionadacomidas);
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

                        preciobebida += datosFinalesBotellaRadio.FirstOrDefault(x => x.CodigoBotella == opcionSeleccionadabebidas).PrecioFinalBotella;
                        var producto = datosFinalesBotellaRadio.FirstOrDefault(x => x.CodigoBotella == opcionSeleccionadabebidas);
                        opcionSeleccionadabebidasDetalle.Add(producto);
                    }
                }
            }


            foreach (var itemRecetaCategoria in snacks)
            {
                foreach (var child in checkBoxAdicionales.Children)
                {
                    if (child is CheckBox checkBoxAdicionales && checkBoxAdicionales.IsChecked == true)
                    {
                        if (checkBoxAdicionales.Name.Substring(1) == Convert.ToInt32(itemRecetaCategoria.Codigo).ToString())
                        {
                            precioAdicionales += itemRecetaCategoria.Precios.FirstOrDefault().General;

                        }
                    }
                }
            }


            totalLabel.Content = (preciodefault + preciocomida + preciobebida + precioAdicionales).ToString("C0");
        }

        public void ProductosModificados()
        {
            if (opcionSeleccionadabebidasDetalle.Count == 0 && opcionSeleccionadacomidasDetalle.Count == 0)
            {
                var preciodefault = datosFinalesComidaRadio
                    .Where(x => x.frecuenciaComida == "default")
                    .Sum(x => x.PrecioFinalComida);

                var opcionSeleccionadabebidas = 0;
                var opcionSeleccionadacomidas = 0;
                opcionSeleccionadabebidasDetalle.Clear();
                opcionSeleccionadacomidasDetalle.Clear();

                foreach (var child in radioComidas.Children)
                {
                    if (child is RadioButton radioButtonComida && radioButtonComida.IsChecked == true)
                    {
                        opcionSeleccionadacomidas = Convert.ToInt32(radioButtonComida.Name.Substring(2));
                        var producto = datosFinalesComidaRadio.FirstOrDefault(x => x.CodigoComida == opcionSeleccionadacomidas);
                        if (producto != null)
                        {
                            preciocomida += producto.PrecioFinalComida;
                            opcionSeleccionadacomidasDetalle.Add(producto);
                        }
                    }
                }

                foreach (var child in radiobebidas.Children)
                {
                    if (child is RadioButton radioButtonBebidas && radioButtonBebidas.IsChecked == true)
                    {
                        opcionSeleccionadabebidas = Convert.ToInt32(radioButtonBebidas.Name.Substring(2));
                        var producto = datosFinalesBotellaRadio.FirstOrDefault(x => x.CodigoBotella == opcionSeleccionadabebidas);
                        if (producto != null)
                        {
                            preciobebida += producto.PrecioFinalBotella;
                            opcionSeleccionadabebidasDetalle.Add(producto);
                        }
                    }
                }
            }

            preciodefault = datosFinalesComidaRadio
                    .Where(x => x.frecuenciaComida == "default")
                    .Sum(x => x.PrecioFinalComida);



            var datosselecionado = string.Empty;
            var productosSeleccionados = App.ProductosSeleccionados;
            var itemContador = 1;

            foreach (var productocambiado in productosSeleccionados)
            {
                var productonew = new Producto();



                if (contadorProdModificados == itemContador)
                {
                    if (opcionSeleccionadabebidasDetalle.Count == 1 && opcionSeleccionadacomidasDetalle.Count == 0)
                    {
                        var bebidaDetalle = opcionSeleccionadabebidasDetalle.FirstOrDefault();
                        productonew = UpdateProduct(productocambiado, preciodefault + bebidaDetalle.PrecioFinalBotella, 1, bebidaDetalle, null, null, null);
                    }
                    else if (opcionSeleccionadabebidasDetalle.Count == 2 && opcionSeleccionadacomidasDetalle.Count == 0)
                    {

                        var bebidaDetalle1 = opcionSeleccionadabebidasDetalle[0];
                        var bebidaDetalle2 = opcionSeleccionadabebidasDetalle[1];
                        productonew = UpdateProduct(productocambiado, datosFinalesComidaRadio
                    .Where(x => x.frecuenciaComida == "default")
                    .Sum(x => x.PrecioFinalComida) + bebidaDetalle1.PrecioFinalBotella + bebidaDetalle2.PrecioFinalBotella, 2, bebidaDetalle1, null, bebidaDetalle2, null);
                    }
                    else if (opcionSeleccionadabebidasDetalle.Count == 1 && opcionSeleccionadacomidasDetalle.Count == 1)
                    {
                        var bebidaDetalle = opcionSeleccionadabebidasDetalle.FirstOrDefault();
                        var comidaDetalle = opcionSeleccionadacomidasDetalle.FirstOrDefault();
                        productonew = UpdateProduct(productocambiado, datosFinalesComidaRadio
                    .Where(x => x.frecuenciaComida == "default")
                    .Sum(x => x.PrecioFinalComida) + bebidaDetalle.PrecioFinalBotella + comidaDetalle.PrecioFinalComida, 1, bebidaDetalle, comidaDetalle, null, null);
                    }
                    else if (opcionSeleccionadabebidasDetalle.Count == 2 && opcionSeleccionadacomidasDetalle.Count == 2)
                    {
                        var bebidaDetalle1 = opcionSeleccionadabebidasDetalle[0];
                        var bebidaDetalle2 = opcionSeleccionadabebidasDetalle[1];
                        var comidaDetalle1 = opcionSeleccionadacomidasDetalle[0];
                        var comidaDetalle2 = opcionSeleccionadacomidasDetalle[1];
                        productonew = UpdateProduct(productocambiado, datosFinalesComidaRadio
                    .Where(x => x.frecuenciaComida == "default")
                    .Sum(x => x.PrecioFinalComida) + bebidaDetalle1.PrecioFinalBotella + bebidaDetalle2.PrecioFinalBotella + comidaDetalle1.PrecioFinalComida + comidaDetalle2.PrecioFinalComida, 2, bebidaDetalle1, comidaDetalle1, bebidaDetalle2, comidaDetalle2);
                    }

                    productonew.KeySecuencia = App.Secuencia;
                    productonew.SwitchAdd = "S";
                    productonew.Codigo = CodigoProducto;
                    App.agregarProducto(productonew);
                }
                itemContador++;
            }
        }

        private Producto UpdateProduct(Producto originalProduct, decimal finalPrice, int bebidaCount, Bebida bebidaDetalle1, Comida comidaDetalle1 = null, dynamic bebidaDetalle2 = null, dynamic comidaDetalle2 = null)
        {
            var newProduct = new Producto
            {
                Valor = finalPrice.ToString(),
                Descripcion = originalProduct.Descripcion,
                CanCategoria_1 = bebidaCount,
                CanCategoria_2 = comidaDetalle1 != null ? 1 : 0,
                CanCategoria_3 = bebidaCount > 1 ? 1 : 0,
                CanCategoria_4 = comidaDetalle2 != null ? 1 : 0,
                CanCategoria_5 = 0,
                Cantidad = 1,
                Cantidad1 = 1,
                Cantidad11 = bebidaCount > 1 ? 1 : 0,
                Cantidad111 = 0,
                Cantidad1111 = 0,
                Cantidad2 = comidaDetalle1 != null ? 1 : 0,
                Cantidad22 = comidaDetalle2 != null ? 1 : 0,
                Cantidad222 = 0,
                Cantidad2222 = 0,
                Cantidad3 = 0,
                Cantidad33 = 0,
                Cantidad333 = 0,
                Cantidad3333 = 0,
                Cantidad4 = 0,
                Cantidad44 = 0,
                Cantidad444 = 0,
                Cantidad4444 = 0,
                Cantidad5 = 0,
                Cantidad55 = 0,
                Cantidad555 = 0,
                Cantidad5555 = 0,
                Check1 = $"{bebidaDetalle1.CodigoBotella}-{bebidaDetalle1.NombreFinalBotella}+Categoria:{bebidaDetalle1.categoria}-Precio:{bebidaDetalle1.PrecioFinalBotella}",
                Check11 = bebidaDetalle2 != null ? $"{bebidaDetalle2.CodigoBotella}-{bebidaDetalle2.NombreFinalBotella}+Categoria:{bebidaDetalle2.categoria}-Precio:{bebidaDetalle2.PrecioFinalBotella}" : null,
                Check2 = comidaDetalle1 != null ? $"{comidaDetalle1.CodigoComida}-{comidaDetalle1.NombreFinalComida}+Categoria:{comidaDetalle1.categoria}-Precio:{comidaDetalle1.PrecioFinalComida}" : null,
                Check22 = comidaDetalle2 != null ? $"{comidaDetalle2.CodigoComida}-{comidaDetalle2.NombreFinalComida}+Categoria:{comidaDetalle2.categoria}-Precio:{comidaDetalle2.PrecioFinalComida}" : null,
                ProCantidad_1 = 1,
                ProCantidad_2 = bebidaDetalle2 != null ? 1 : 0,
                ProCantidad_3 = 0,
                ProCantidad_4 = 0,
                ProCantidad_5 = 0,
                ProCategoria_1 = bebidaDetalle1.categoria,
                ProCategoria_2 = comidaDetalle1?.categoria ?? 0,
                ProCategoria_3 = bebidaDetalle2?.categoria ?? 0,
                ProCategoria_4 = comidaDetalle2?.categoria ?? 0,
                ProCategoria_5 = 0,
                ProProducto_1 = bebidaDetalle1.CodigoBotella,
                ProProducto_2 = bebidaDetalle2?.CodigoBotella ?? 0,
                ProProducto_3 = 0,
                ProProducto_4 = 0,
                ProProducto_5 = 0,
                OrdenView = 0,
                NombreEli = App.NombreEli,
                EmailEli = App.EmailEli,
                KeyTeatro = App.idCine,
                KeySecuencia = "0",
                Tipo = "C",
                TipoCompra = "P",
                TelefonoEli = App.TelefonoEli,
                SwitchAdd = "N",
                SwtVenta = "V"
            };
            return newProduct;
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
            datosFinalesBotella = new List<Bebida>();
            datosFinalesComida = new List<Comida>();

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
                                    var comida = new Comida()
                                    {

                                        CodigoComida = itepro.Codigo,
                                        NombreFinalComida = itepro.Descripcion,
                                        PrecioFinalComida = Precios,
                                        frecuenciaComida = "true",
                                        categoria = itepro.Codigo,
                                        cantidad = Convert.ToInt32(itepro.Cantidad)

                                    };
                                    Precios = (itempre.General * itepro.Cantidad);
                                    datosFinalesComida.Add((comida));
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

                                            var comidac = new Comida()
                                            {

                                                CodigoComida = itepro.Codigo,
                                                NombreFinalComida = itepro.Descripcion,
                                                PrecioFinalComida = Precios,
                                                frecuenciaComida = "default",
                                                categoria = itepro.Codigo,
                                                cantidad = Convert.ToInt32(itepro.Cantidad)

                                            };

                                            datosFinalesComida.Add((comidac));
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

                                            var bebida = new Bebida()
                                            {

                                                CodigoBotella = CodioBotella,
                                                NombreFinalBotella = NombreFinalBotella,
                                                PrecioFinalBotella = precioFinalBotella,
                                                frecuenciaBotella = frecuenciaBotella,
                                                categoria = itecat.Codigo,
                                                cantidad = 1

                                            };

                                            datosFinalesBotella.Add(bebida);

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

                                            var comidar = new Comida()
                                            {

                                                CodigoComida = CodioComida,
                                                NombreFinalComida = NombreFinalComida,
                                                PrecioFinalComida = precioFinalComida,
                                                frecuenciaComida = frecuenciaComida,
                                                categoria = itecat.Codigo,
                                                cantidad = 1

                                            };

                                            datosFinalesComida.Add(comidar);

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
          

            if (ContadorProductosPantallas <= (App.ProductosSeleccionados.Count()))
            {
                ProductosAdicionales();
                ProductosModificados();
                ProductosAdicionalesSeleccion();
                contadorProdModificados++;
                if (ContadorProductosPantallas == (App.ProductosSeleccionados.Count()))
                {
                    isThreadActive = false;
                    ResumenCompra openWindows = new ResumenCompra(config);
                    openWindows.Show();
                    this.Close();
                }
                else 
                {
                    CrearCombosYbebidas(App.ProductosSeleccionados);
                }
                
                return;
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