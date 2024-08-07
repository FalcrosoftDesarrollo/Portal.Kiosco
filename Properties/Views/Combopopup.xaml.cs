﻿using APIPortalKiosco.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;


namespace Portal.Kiosco.Properties.Views
{
    public partial class Combopopup : Window
    {
        private readonly IOptions<MyConfig> config;
        private IConfiguration configuration;
        private int ContadorProductos = 1;
        private int ContadorProductosPantallas = 0;

        private string codigocombo = "";

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
        public event EventHandler Cancelled;

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

        private void CenterWindow()
        {
            if (Owner != null)
            {
                Left = Owner.Left + (Owner.Width - ActualWidth) / 2;
                Top = Owner.Top + (Owner.Height - ActualHeight) / 2;
            }
            else
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }
        }

        private void Combopopup_Loaded(object sender, RoutedEventArgs e)
        {
            CenterWindow();
        }

        private void eliminarlista()
        {

            var result = new List<Producto>();

            var productoAEliminar = App.ProductosSeleccionados.FirstOrDefault(producto => producto.Descripcion == lblCombo.Content.ToString());

            if (productoAEliminar != null)
            {

                var productos = App.ProductosSeleccionados;


                bool eliminado = false;

                foreach (var item in productos)
                {
                    if (item.Descripcion == lblCombo.Content.ToString() && !eliminado)
                    {
                        eliminado = true; // Marcar que ya hemos eliminado un producto con el código especificado
                    }
                    else
                    {
                        result.Add(item); // Añadir el producto a la nueva lista
                    }
                }

                App.ProductosSeleccionados = result; // Actualizar la lista de productos seleccionados

            }


        }
        private object _sender;

        public Combopopup(Window owner, object sender)
        {
            InitializeComponent();
            _sender = sender;
            DataContext = ((App)Application.Current);
            CrearCombosYbebidas(App.ProductosSeleccionados, _sender);
            DoubleAnimation fadeInAnimation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.5));
            gridPrincipal.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);
            Owner = owner;
            WindowStartupLocation = WindowStartupLocation.Manual;
            Left = owner.Left + (owner.Width - ActualWidth) / 2;
            Top = owner.Top + (owner.Height - ActualHeight) / 2;
           
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

            Loaded += Combopopup_Loaded;

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

        private decimal CodigoProducto;

        public void CrearCombosYbebidas(List<Producto> productos, object sender)
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

                foreach (var item in productos.Where(x => x.Tipo == "C" && x.Codigo == Convert.ToDecimal(App.CodigoProducto)))
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

                            //ImgCombo.Source = new BitmapImage(new Uri(lc_auximg));
                            var precio = buscarprecio(productos, Convert.ToDecimal(item.Codigo));

                            //totalLabel.Content = precio.ToString("C0");
                            precioAntesAdicionales = precio;
                            lblCombo.Content = item.Descripcion;

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
                                                radioButton.FontSize = 24;
                                                radioButton.HorizontalAlignment = HorizontalAlignment.Left;
                                                radioButton.GroupName = "Comida" + lc_variii.ToString();
                                                if (itemRecetaCategoria.Frecuente == "True")
                                                {
                                                    radioButton.IsChecked = true;
                                                }
                                                radioComidas.Children.Add(radioButton);

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
            var snacks = App.Adicionales;

            foreach (var itemRecetaCategoria in snacks)
            {
                if (itemRecetaCategoria.Descripcion != "Gafas 3D")
                {
                    var checkBox = new CheckBox();
                    checkBox.Content = itemRecetaCategoria.Descripcion;
                    checkBox.Name = "v" + Convert.ToInt32(itemRecetaCategoria.Codigo).ToString();
                    checkBox.FontSize = 24;
                    checkBox.HorizontalAlignment = HorizontalAlignment.Left;
                    checkBox.Checked += CheckBox_Checked;
                    checkBox.Unchecked += CheckBox_Unchecked;

                    checkBoxAdicionales.Children.Add(checkBox);
                }
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
            var snacks = App.Adicionales;
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

            //totalLabel.Content = (precioAdicionales + precioAntesAdicionales).ToString("C0");

        }

        private async void btnSiguiente_Click(object sender, RoutedEventArgs e)
        {
            var transicion = new transicion();
            transicion.Show();

            await Task.Delay(3);

            int contadorPantallas = App.ProductosSeleccionados.Count() - App.ProductosSeleccionados.Where(x => x.Tipo == "P").Count();
            if (ContadorProductosPantallas <= contadorPantallas)
            {
                ProductosAdicionales();
                await ProductosModificadosAsync();
                await ProductosAdicionalesSeleccionAsync();
                contadorProdModificados++;

                if (ContadorProductosPantallas == contadorPantallas)
                {
                    isThreadActive = false;
                    transicion.Close();
                    this.Close();
                }
                else
                {

                    CrearCombosYbebidas(App.ProductosSeleccionados,_sender);
                    transicion.Close();
                    this.Close();
                }

                return;
            }
        }

        public async Task ProductosAdicionalesSeleccionAsync()
        {
            await Task.Run(() =>
            {
                Application.Current.Dispatcher.Invoke(() => {
                    ProductosAdicionalesSeleccion();
                });
            });
        }

        public void ProductosAdicionalesSeleccion()
        {
            int contadorAdicionales = 1;
            var adicionales = new Adiciones();
            var snacks = App.Adicionales;
            adicionales.Secuencia = Convert.ToDecimal(App.Secuencia);
            adicionales.Tipo = "P";

            foreach (var itemRecetaCategoria in snacks)
            {
                foreach (var child in checkBoxAdicionales.Children)
                {
                    if (child is CheckBox checkBox && checkBox.IsChecked == true)
                    {
                        if (checkBox.Name.Substring(1) == Convert.ToInt32(itemRecetaCategoria.Codigo).ToString())
                        {
                            string cantidadProp = $"Cantidad_{contadorAdicionales}";
                            string codigoProp = $"Codigo_{contadorAdicionales}";
                            string descripcionProp = $"Descripcion_{contadorAdicionales}";
                            string precioProp = $"Precio_{contadorAdicionales}";

                            adicionales.GetType().GetProperty(descripcionProp)?.SetValue(adicionales, itemRecetaCategoria.Descripcion);

                            var codigoValue = itemRecetaCategoria.Codigo;
                            adicionales.GetType().GetProperty(codigoProp)?.SetValue(adicionales, codigoValue);

                            var precioGeneral = itemRecetaCategoria.Precios.FirstOrDefault()?.General;
                            if (precioGeneral != null)
                            {
                                var precioValue = precioGeneral;
                                adicionales.GetType().GetProperty(precioProp)?.SetValue(adicionales, precioValue);
                            }

                            adicionales.GetType().GetProperty(cantidadProp)?.SetValue(adicionales, 1m);

                            // Usa Dispatcher para agregar el producto en el hilo de la UI
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                App.AddProduct(adicionales);
                            });

                            break;
                        }
                    }
                }
            }
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            var radioButton = (RadioButton)sender;
            var descripcion = radioButton.Content.ToString();
            var codigo = radioButton.Name.Substring(1);
            var snacks = App.Adicionales;
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

            //totalLabel.Content = (preciodefault + preciocomida + preciobebida + precioAdicionales).ToString("C0");
        }

        public async Task ProductosModificadosAsync()
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
                    // Mueve la lógica pesada a Task.Run si es necesario
                    await Task.Run(() =>
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
                            productonew = UpdateProduct(productocambiado, preciodefault + bebidaDetalle1.PrecioFinalBotella + bebidaDetalle2.PrecioFinalBotella, 2, bebidaDetalle1, null, bebidaDetalle2, null);
                        }
                        else if (opcionSeleccionadabebidasDetalle.Count == 1 && opcionSeleccionadacomidasDetalle.Count == 1)
                        {
                            var bebidaDetalle = opcionSeleccionadabebidasDetalle.FirstOrDefault();
                            var comidaDetalle = opcionSeleccionadacomidasDetalle.FirstOrDefault();
                            productonew = UpdateProduct(productocambiado, preciodefault + bebidaDetalle.PrecioFinalBotella + comidaDetalle.PrecioFinalComida, 1, bebidaDetalle, comidaDetalle, null, null);
                        }
                        else if (opcionSeleccionadabebidasDetalle.Count == 2 && opcionSeleccionadacomidasDetalle.Count == 2)
                        {
                            var bebidaDetalle1 = opcionSeleccionadabebidasDetalle[0];
                            var bebidaDetalle2 = opcionSeleccionadabebidasDetalle[1];
                            var comidaDetalle1 = opcionSeleccionadacomidasDetalle[0];
                            var comidaDetalle2 = opcionSeleccionadacomidasDetalle[1];
                            productonew = UpdateProduct(productocambiado, preciodefault + bebidaDetalle1.PrecioFinalBotella + bebidaDetalle2.PrecioFinalBotella + comidaDetalle1.PrecioFinalComida + comidaDetalle2.PrecioFinalComida, 2, bebidaDetalle1, comidaDetalle1, bebidaDetalle2, comidaDetalle2);
                        }
                        else if (opcionSeleccionadabebidasDetalle.Count == 2 && opcionSeleccionadacomidasDetalle.Count == 1)
                        {
                            var bebidaDetalle1 = opcionSeleccionadabebidasDetalle[0];
                            var bebidaDetalle2 = opcionSeleccionadabebidasDetalle[1];
                            var comidaDetalle1 = opcionSeleccionadacomidasDetalle[0];
                            productonew = UpdateProduct(productocambiado, preciodefault + bebidaDetalle1.PrecioFinalBotella + bebidaDetalle2.PrecioFinalBotella + comidaDetalle1.PrecioFinalComida, 2, bebidaDetalle1, comidaDetalle1, bebidaDetalle2);
                        }

                        productonew.KeySecuencia = App.Secuencia;
                        productonew.Descripcion = productocambiado.Descripcion;
                        productonew.SwitchAdd = "S";
                        productonew.Codigo = CodigoProducto;
                        productonew.Cantidad = 1;

                        // Utiliza un método asincrónico para agregar productos si es posible
                        Dispatcher.Invoke(() => App.agregarProducto(productonew));
                    });

                    itemContador++;
                }
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
            var adicionales = App.Adicionales;

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



        private async void btnVolver_Click(object sender, RoutedEventArgs e)
        {

            isThreadActive = false;
            Cancelled?.Invoke(_sender, EventArgs.Empty);
            this.Close();
        }

        public async Task LoadDataAsync()
        {
            await Task.Run(() =>
            {
                System.Threading.Thread.Sleep(3000);
            });
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