using APIPortalKiosco.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Portal.Kiosco.Properties.Views
{
    public partial class ResumenCompra : Window
    {
        public ResumenCompra()
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

            GenerateResumen();

        }

        private void btnVolver_Click(object sender, RoutedEventArgs e)
        {
            Combodeluxe1 w = new Combodeluxe1();
            this.Close();
            w.ShowDialog();
        }

        private void btnSiguiente_Click(object sender, RoutedEventArgs e)
        {
            //PagoCashback w = new PagoCashback();
            //this.Close();
            //w.ShowDialog();
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

        private async void btnPagoTarjeta_Click(object sender, RoutedEventArgs e)
        {

            var openWindow = new InstruccionesDatafono();
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

        private async void btnPagarCash_Click(object sender, RoutedEventArgs e)
        {

            var openWindow = new PagoCashback();
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

        public void GenerateResumen()
        {
            // Crear un nuevo Grid
            Grid gridBoletas = new Grid();

            // Crear definiciones de columna
            for (int i = 0; i < 9; i++)
            {
                ColumnDefinition columnDefinition = new ColumnDefinition();
                // Asignar el ancho de la columna según corresponda
                if (i == 0)
                    columnDefinition.Width = new GridLength(28);
                else if (i == 1)
                    columnDefinition.Width = new GridLength(539);
                else if (i == 2)
                    columnDefinition.Width = new GridLength(0);
                else if (i == 3)
                    columnDefinition.Width = new GridLength(105);
                else if (i == 4)
                    columnDefinition.Width = new GridLength(185);
                else if (i == 5)
                    columnDefinition.Width = new GridLength(100);
                else if (i == 6)
                    columnDefinition.Width = new GridLength(118);
                else if (i == 7)
                    columnDefinition.Width = new GridLength(110);
                // La última columna no tiene un ancho específico

                gridBoletas.ColumnDefinitions.Add(columnDefinition);
            }

            // Crear los bordes con etiquetas dentro
            for (int i = 1; i <= 4; i++)
            {
                Border border = new Border();
                Grid.SetColumn(border, i * 2 - 1); // Colocar el borde en columnas impares (1, 3, 5)
                Label label = new Label();
                label.FontFamily = new FontFamily("Myanmar Khyay");
                label.FontSize = 24;
                label.VerticalContentAlignment = VerticalAlignment.Center;

                // Configurar contenido y márgenes según corresponda
                if (i == 1)
                {
                    label.Content =  App.Pelicula.Nombre == null || App.Pelicula.Nombre == "" ? "Sin Pelicula" : App.Pelicula.Nombre;
                    label.Margin = new Thickness(0, 0, -78, 0);
                }
                else if (i == 2)
                {
                    label.Content = "0";
                    label.Margin = new Thickness(0, 22, -44, 23);
                }
                else if (i == 3)
                {
                    label.Content = "0";
                }
                else if (i == 4)
                {
                    label.Content = "0";
                }

                border.Child = label;
                gridBoletas.Children.Add(border);
            }

            // Agregar el Grid a donde corresponda
            // Supongamos que hay un contenedor llamado "contenedor" en tu ventana XAML
            ContenedorResumen.Children.Add(gridBoletas);

            // Crear un nuevo Grid
            Grid gridGafas = new Grid();

            // Crear definiciones de columna
            for (int i = 0; i < 9; i++)
            {
                ColumnDefinition columnDefinition = new ColumnDefinition();
                // Asignar el ancho de la columna según corresponda
                if (i == 0)
                    columnDefinition.Width = new GridLength(28);
                else if (i == 1)
                    columnDefinition.Width = new GridLength(539);
                else if (i == 2)
                    columnDefinition.Width = new GridLength(0);
                else if (i == 3)
                    columnDefinition.Width = new GridLength(105);
                else if (i == 4)
                    columnDefinition.Width = new GridLength(185);
                else if (i == 5)
                    columnDefinition.Width = new GridLength(100);
                else if (i == 6)
                    columnDefinition.Width = new GridLength(118);
                else if (i == 7)
                    columnDefinition.Width = new GridLength(110);
                // La última columna no tiene un ancho específico

                gridGafas.ColumnDefinitions.Add(columnDefinition);
            }

            // Crear los bordes con etiquetas dentro
            for (int i = 1; i <= 4; i++)
            {
                Border border = new Border();
                Grid.SetColumn(border, i * 2 - 1); // Colocar el borde en columnas impares (1, 3, 5)
                Label label = new Label();
                label.FontFamily = new FontFamily("Myanmar Khyay");
                label.FontSize = 24;
                label.VerticalContentAlignment = VerticalAlignment.Center;

                // Configurar contenido y márgenes según corresponda
                if (i == 1)
                {
                    label.Content = "Gafas";
                    label.Margin = new Thickness(0, 0, -78, 0);
                }
                else if (i == 2)
                {

                    label.Content = "0";
                    label.Margin = new Thickness(0, 22, -44, 23);
                }
                else if (i == 3)
                {
                    label.Content = "0";
                }
                else if (i == 4)
                {
                    label.Content = "0";
                    total += Convert.ToDecimal(label.Content);
                }

                border.Child = label;
                gridGafas.Children.Add(border);
            }

            // Agregar el Grid a donde corresponda
            // Supongamos que hay un contenedor llamado "contenedor" en tu ventana XAML
            ContenedorResumen.Children.Add(gridGafas);
            var combos = App.ProductosSeleccionados;
            foreach (var combosseleccionados in combos)
            {

                // Crear un nuevo Grid
                Grid grid = new Grid();

                // Crear definiciones de columna
                for (int i = 0; i < 9; i++)
                {
                    ColumnDefinition columnDefinition = new ColumnDefinition();
                    // Asignar el ancho de la columna según corresponda
                    if (i == 0)
                        columnDefinition.Width = new GridLength(28);
                    else if (i == 1)
                        columnDefinition.Width = new GridLength(539);
                    else if (i == 2)
                        columnDefinition.Width = new GridLength(0);
                    else if (i == 3)
                        columnDefinition.Width = new GridLength(105);
                    else if (i == 4)
                        columnDefinition.Width = new GridLength(185);
                    else if (i == 5)
                        columnDefinition.Width = new GridLength(100);
                    else if (i == 6)
                        columnDefinition.Width = new GridLength(118);
                    else if (i == 7)
                        columnDefinition.Width = new GridLength(110);
                    // La última columna no tiene un ancho específico

                    grid.ColumnDefinitions.Add(columnDefinition);
                }

                // Crear los bordes con etiquetas dentro
                for (int i = 1; i <= 4; i++)
                {
                    Border border = new Border();
                    Grid.SetColumn(border, i * 2 - 1); // Colocar el borde en columnas impares (1, 3, 5)
                    Label label = new Label();
                    label.FontFamily = new FontFamily("Myanmar Khyay");
                    label.FontSize = 24;
                    label.VerticalContentAlignment = VerticalAlignment.Center;

                    // Configurar contenido y márgenes según corresponda
                    if (i == 1)
                    {
                        label.Content = buscarNombre(combos, combosseleccionados.Codigo);
                        label.Margin = new Thickness(0, 0, -78, 0);
                    }
                    else if (i == 2)
                    {
                        label.Content = buscarprecio(combos, combosseleccionados.Codigo);
                        label.Margin = new Thickness(0, 22, -44, 23);
                    }
                    else if (i == 3)
                    {
                        label.Content = "1";
                    }
                    else if (i == 4)
                    {
                        label.Content = buscarprecio(combos, combosseleccionados.Codigo);

                        total += Convert.ToDecimal(label.Content);
                    }

                    border.Child = label;
                    grid.Children.Add(border);
                }

                // Agregar el Grid a donde corresponda
                // Supongamos que hay un contenedor llamado "contenedor" en tu ventana XAML
                ContenedorResumen.Children.Add(grid);
            }


            TotalResumen.Content = "TOTAL A PAGAR: " + total.ToString().Replace("$", "").Replace("€", "").Replace(".", "").Replace(",", "").Trim();
        }
        public string buscarNombre(List<Producto> productos, Decimal Codigo)
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
            var nombre = "";

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
                            nombre = itepro.Descripcion;
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
                            nombre = itepro.Descripcion;
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

            return nombre;

        }

        public decimal total = 0;
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



                    break;
                }
            }
          
            return Precios;

        }
    }

}
