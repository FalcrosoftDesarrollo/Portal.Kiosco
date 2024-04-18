using APIPortalKiosco.Entities;
using Microsoft.Extensions.Configuration;
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
    /// Lógica de interacción para Frame11.xaml
    /// </summary>
    public partial class Combodeluxe1 : Window
    {
        private IConfiguration configuration;
        private int ContadorProductos = 1;
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
        }

        public void CrearCombosYbebidas(List<Producto> productos)
        {

            //imagenes.Children.Clear();

            Producto ob_datpro = new Producto();
            List<(decimal CodigoBotella, string NombreFinalBotella, decimal PrecioFinalBotella, string frecuenciaBotella, decimal categoria)> datosFinalesBotella = new List<(decimal, string, decimal, string, decimal)>();
            List<(decimal CodigoComida, string NombreFinalComida, decimal PrecioFinalComida, string frecuenciaComida, decimal categoria)> datosFinalesComida = new List<(decimal, string, decimal, string, decimal)>();
            int CodigoBebidas = 1244;
            int CodigoBebidas2 = 2444;
            int CodigoComidas = 246;


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
                            totalLabel.Content = (Convert.ToDecimal(totalLabel.Content) + precio).ToString();
                            lblCombo.Content = item.Descripcion;
                            foreach (var itemReceta in item.Receta)
                            {

                                if (itemReceta.RecetaCategoria != null)
                                {
                                    if (CodigoComidas == itemReceta.Codigo)
                                    {
                                        foreach (var itemRecetaCategoria in itemReceta.RecetaCategoria)
                                        {
                                            var radioButton = new RadioButton();
                                            radioButton.Content = itemRecetaCategoria.Descripcion;
                                            radioButton.FontSize = 24; // Establece el tamaño de fuente como un valor numérico
                                            radioButton.HorizontalAlignment = HorizontalAlignment.Left; // Establece la alineación horizontal
                                            radioButton.GroupName = "Comida";
                                            if (itemRecetaCategoria.Frecuente == "True")
                                            {
                                                radioButton.IsChecked = true;
                                            }
                                            radioComidas.Children.Add(radioButton); // Agrega el botón de radio al contenedor
                                        }
                                    }

                                    if (CodigoBebidas2 == itemReceta.Codigo || CodigoBebidas == itemReceta.Codigo)
                                    {
                                        foreach (var itemRecetaCategoria in itemReceta.RecetaCategoria)
                                        {
                                            var radioButton = new RadioButton();
                                            radioButton.Content = itemRecetaCategoria.Descripcion;
                                            radioButton.FontSize = 24; // Establece el tamaño de fuente como un valor numérico
                                            radioButton.HorizontalAlignment = HorizontalAlignment.Left; // Establece la alineación horizontal
                                            radioButton.GroupName = "Bebida";
                                            if (itemRecetaCategoria.Frecuente == "True")
                                            {
                                                radioButton.IsChecked = true;
                                            }
                                            radiobebidas.Children.Add(radioButton); // Agrega el botón de radio al contenedor
                                        }
                                    }
                                }

                            }
                        }
                        itemContador++;
                    }
                    ContadorProductos++;
                }
            }
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

                    //App.ProductosSeleccionados.Add(itepro);

                    break;
                }
            }

            return Precios;

        }

        private async void btnSiguiente_Click(object sender, RoutedEventArgs e)
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

        private async void btnVolver_Click(object sender, RoutedEventArgs e)
        {
            App.ProductosSeleccionados.Clear();
            var openWindow = new Combos();
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