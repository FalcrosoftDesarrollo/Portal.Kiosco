using APIPortalKiosco.Data;
using APIPortalKiosco.Entities;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Portal.Kiosco.Properties.Views
{
    public partial class ResumenCompra : Window
    {
        private readonly IOptions<MyConfig> config;
        private bool isThreadActive = true;
        public ResumenCompra(IOptions<MyConfig> config)
        {
            InitializeComponent();
            ListCarrito();
            DataContext = ((App)Application.Current);
            this.config = config;
            var validacion = App.Pelicula;
            if (validacion != null && validacion.Nombre != null)
            {
                //contenedorImagen.Child = CargarImagen(App.Pelicula.Imagen);
            }
            if (App.ob_diclst.Count > 0)
            {
                lblnombre.Content = "¡HOLA " + App.ob_diclst["Nombre"].ToString() + " " + App.ob_diclst["Apellido"].ToString() + "!";
            }
            else
            {
                lblnombre.Content = "¡HOLA INVITADO!";
                borderPagarCash.Visibility = Visibility.Hidden;
            }

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

        private async void btnVolver_Click(object sender, RoutedEventArgs e)
        {
            isThreadActive = false;
            var openWindows = new Combos();
            this.Close();
            openWindows.Show();
        }

        private async void btnSalir_Click(object sender, RoutedEventArgs e)
        {
            isThreadActive = false;
            Principal openWindows = new Principal();
            this.Close();
            openWindows.Show();
        }

        private async void btnPagoTarjeta_Click(object sender, RoutedEventArgs e)
        {
            isThreadActive = false;

            decimal totalDecimal;
            if (decimal.TryParse(TotalResumen.Content.ToString(), NumberStyles.Currency, CultureInfo.GetCultureInfo("es-CO"), out totalDecimal))
            {
                var total = Convert.ToString(Convert.ToInt32(totalDecimal));
                App.TotalPagar = total.ToString();

            }
            else
            {
                Console.WriteLine("No se pudo convertir la cadena en un valor decimal.");
            }

            StartMonitoringDatafono();
            var openWindows = new InstruccionesDatafono();

            openWindows.Show();
            this.Close();

        }

        private Thread monitorThread;
        private void StartMonitoringDatafono()
        {

            monitorThread = new Thread(() =>
            {
                while (true)
                {
                    if (App.ResponseDatafono == "00")
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            BoletasGafasAlimentos boletasGafasAlimentos = new BoletasGafasAlimentos();
                            boletasGafasAlimentos.Show();

                            this.Close();
                        });

                        return;
                    }

                    Thread.Sleep(1000);
                }
            });

            monitorThread.IsBackground = true;
            monitorThread.Start();

        }

        private async void btnPagarCash_Click(object sender, RoutedEventArgs e)
        {


            App.CineFans();
            var saldo = App.Saldo;
            var total = App.TotalPagar;
            if (Convert.ToDecimal(App.Saldo) < Convert.ToDecimal(App.TotalPagar))
            {
                MessageBox.Show("Ups! Tu cashback disponible es inferior al monto total de la compra", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            }
            else if (Convert.ToDecimal(App.Saldo) >= Convert.ToDecimal(App.TotalPagar))
            {
                isThreadActive = false;

                var transicion = new transicion();
                transicion.Show();
                this.Close();
                var openWindows = new PagoCashback();
                await openWindows.LoadDataAsync();
                openWindows.Show();
                transicion.Close();
            }
        }

        public void ListCarrito()
        {
            #region VARIABLES LOCALES
            decimal lc_secsec = 0;
            var PuntoVenta = App.PuntoVenta;
            var KeyTeatro = App.idCine;
            string secuencia = "";
            #endregion
            GenerateResumenCategoriaLimpiar();
            App.TipoCompra = "V";
            secuencia = App.Secuencia;
            List<RetailSales> ListCarritoR = new List<RetailSales>();
            List<ReportSales> ListCarritoB = new List<ReportSales>();

            if (secuencia != null)
            {
                lc_secsec = Convert.ToDecimal(secuencia);
                using (var context = new DataDB(config))
                {
                    try
                    {
                        var RetailSales = context.RetailSales.Where(x => x.Secuencia == lc_secsec).Where(x => x.PuntoVenta == Convert.ToDecimal(PuntoVenta)).Where(x => x.KeyTeatro == Convert.ToDecimal(KeyTeatro)).ToList();
                        ListCarritoR = RetailSales;
                        var SwitchAddBtn = RetailSales.Any(x => x.SwitchAdd == "S");
                    }
                    catch (Exception e) { }
                    try
                    {
                        var ReportSales = context.ReportSales.Where(x => x.Secuencia == lc_secsec.ToString()).Where(x => x.KeyPunto == PuntoVenta).Where(x => x.KeyTeatro == KeyTeatro).ToList();
                        ListCarritoB = ReportSales;
                    }
                    catch (Exception e) { }
                }

                if (ListCarritoB.Count != 0 && ListCarritoR.Count == 0)
                    App.TipoCompra = "B";
                if (ListCarritoB.Count == 0 && ListCarritoR.Count != 0)
                    App.TipoCompra = "P";
                if (ListCarritoB.Count != 0 && ListCarritoR.Count != 0)
                    App.TipoCompra = "M";
            }

            GenerateResumen(ListCarritoR, ListCarritoB);
        }



        public async void GenerateResumen(List<RetailSales> ListCarritoR, List<ReportSales> ListCarritoB)
        {
            decimal totalcombos = 0;
            decimal codigo = 0;

            if (App.Pelicula.Nombre == null)
            {

                ContenedorBoletas.Visibility = Visibility.Collapsed;
                ContenedorBoletas.Visibility = Visibility.Collapsed;
                ContenedorBoletasPrecio.Visibility = Visibility.Collapsed;
                ContenedorBoletasCantidad.Visibility = Visibility.Collapsed;
                ContenedorBoletasTotal.Visibility = Visibility.Collapsed;

            }
            else
            {
                GenerateResumenCategoria("Boletas", App.Pelicula.TituloOriginal == null || App.Pelicula.TituloOriginal == "" ? "Sin Pelicula" : App.Pelicula.TituloOriginal, App.ValorTarifa, App.CantidadBoletas.ToString(), App.CantidadBoletas * App.ValorTarifa, "");
            }
            totalcombos += (App.CantidadBoletas * App.ValorTarifa);

     
                if (App.CantidadGafas == 0)
                {
                    borderimagengafas.Visibility = Visibility.Collapsed;
                    borderEliminarGafas.Visibility = Visibility.Collapsed;
                    ContenedorGafas.Visibility = Visibility.Collapsed;
                    ContenedorGafasPrecio.Visibility = Visibility.Collapsed;
                    ContenedorGafasCantidad.Visibility = Visibility.Collapsed;
                    ContenedorGafasTotal.Visibility = Visibility.Collapsed;

                }

            GenerateResumenCategoria("Gafas", "Gafas", App.PrecioUnitario == 0 ? 0 : App.PrecioUnitario, App.CantidadGafas.ToString() == "" ? "0" : App.CantidadGafas.ToString(), App.CantidadGafas == 0 ? 0 : (App.CantidadGafas * App.PrecioUnitario), "");

            totalcombos += 0;

            var combos = ListCarritoR;

            var combosAgrupados = combos.GroupBy(c => c.KeyProducto);


            //decimal totalcombos = 0;
        
            var pelicula = App.Pelicula;
            var gafas = App.CantidadGafas;
            var combosseleccionados = combosAgrupados.Count();

            if (pelicula.Nombre == null && gafas == 0 && combosseleccionados == 0)
            {
                MessageBox.Show("Sin productos para la venta", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                await Task.Delay(2);
                var transicion = new transicion();
                transicion.Show();
                this.Close();
                var openWindows = new Principal();
                await openWindows.LoadDataAsync();
                openWindows.Show();
                transicion.Close();
            }

            foreach (var grupoCombos in combosAgrupados)
            {
                var groupedItems = grupoCombos
                    .GroupBy(item => new { item.Descripcion, item.Precio, item.KeyProducto })
                    .Select(g => new
                    {
                        Descripcion = g.Key.Descripcion,
                        Precio = g.Key.Precio,
                        Cantidad = g.Count(),
                        KeyProducto = g.Key.KeyProducto
                    });

                foreach (var group in groupedItems)
                {
                    string nombre = group.Descripcion;
                    decimal precio = group.Precio;
                    int cantidad = group.Cantidad;
                    decimal KeyProducto = group.KeyProducto;


                    // Calcula el total actual (remueve caracteres no numéricos y convierte a decimal)
                    string totalString = TotalResumen.Content.ToString().Replace("$", "").Replace("€", "").Replace(".", "").Replace(",", "").Trim();


                    // Calcula el nuevo total sumando el precio del ítem
                    decimal nuevoTotal = (precio * cantidad);

                    // Multiplica el nuevo total por la cantidad
                    decimal total = nuevoTotal;

                    // Suma el total al total de combinaciones
                    totalcombos += total;

                    // Genera el resumen de categoría
                    GenerateResumenCategoria("Combos", nombre, precio, cantidad.ToString(), total, KeyProducto.ToString());
                }
            }

            TotalResumen.Content = totalcombos.ToString("C0");

            App.TotalPagar = totalcombos.ToString();
        }
        //private void GenerateResumenCategoria(string categoria, string nombre, decimal valor, string cantidad, decimal total, string codigo)
        //{
        //    Label CreateLabel(string content)
        //    {
        //        return new Label
        //        {
        //            Content = content,
        //            FontFamily = new FontFamily("Myanmar Khyay"),
        //            FontSize = 24,
        //            VerticalContentAlignment = VerticalAlignment.Center,
        //            HorizontalAlignment = HorizontalAlignment.Center
        //        };
        //    }

        //    switch (categoria)
        //    {
        //        case "Boletas":
        //            ContenedorBoletas.Children.Add(CreateLabel(nombre));
        //            ContenedorBoletasPrecio.Children.Add(CreateLabel(valor.ToString("C0")));
        //            ContenedorBoletasCantidad.Children.Add(CreateLabel(cantidad));
        //            ContenedorBoletasTotal.Children.Add(CreateLabel(total.ToString("C0")));
        //            break;

        //        case "Gafas":
        //            ContenedorGafas.Children.Add(CreateLabel(nombre));
        //            ContenedorGafasPrecio.Children.Add(CreateLabel(valor.ToString("C0")));
        //            ContenedorGafasCantidad.Children.Add(CreateLabel(cantidad));
        //            ContenedorGafasTotal.Children.Add(CreateLabel(total.ToString("C0")));
        //            break;

        //        case "Combos":

        //            string lc_auximg = string.Concat(App.UrlRetailImg, codigo, ".jpg");
        //            ContenedorResumenImagen.Child = CargarImagen(lc_auximg);
        //            ContenedorResumen.Children.Add(CreateLabel(nombre));
        //            ContenedorResumenPrecio.Children.Add(CreateLabel(valor.ToString("C0")));
        //            ContenedorResumenCantidad.Children.Add(CreateLabel(cantidad));
        //            ContenedorResumenTotal.Children.Add(CreateLabel(total.ToString("C0")));
        //            break;
        //    }
        //}


        private void GenerateResumenCategoria(string categoria, string nombre, decimal valor, string cantidad, decimal total, string codigo)
        {
            Label CreateLabel(string content)
            {
                return new Label
                {
                    Content = content,
                    FontFamily = new FontFamily("Myanmar Khyay"),
                    FontSize = 24,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center
                };
            }

            // Crear una nueva fila si la categoría es "Combos"
            if (categoria == "Combos")
            {
                // Añadir una nueva fila para los combos
                mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                int newRow = mainGrid.RowDefinitions.Count - 1;

                // Crear imagen para combos
                string lc_auximg = string.Concat(App.UrlRetailImg, codigo, ".jpg");
                StackPanel comboImage = CargarImagen(lc_auximg); // Necesitas implementar CargarImagen

                // Crear un Grid para la columna de Total con dos filas
                Grid totalGrid = new Grid();
                totalGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                totalGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                // Añadir el Label y el botón de eliminar a este Grid
                totalGrid.Children.Add(CreateLabel(total.ToString("C0")));
                Grid.SetRow(totalGrid.Children[0], 0); // Label en la primera fila

                Border eliminarBorder = new Border
                {
                    Height = 15,
                    Width = 80,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F30613")),
                    CornerRadius = new CornerRadius(5)
                };

                Button eliminarButton = new Button
                {
                    Background = Brushes.Transparent,
                    Name = "btnEliminar",
                    Content = "Eliminar",
                    FontSize = 10,
                    FontFamily = new FontFamily("Myanmar Khyay"),
                    Foreground = Brushes.White,
                    BorderThickness = new Thickness(0),
                };

                eliminarButton.Click += btnEliminar_Click;
                eliminarBorder.Child = eliminarButton;

                totalGrid.Children.Add(eliminarBorder);
                Grid.SetRow(totalGrid.Children[1], 1); // Botón en la segunda fila

                // Añadir la imagen y el Grid con el Label y el botón al Grid principal
                AddToGrid(mainGrid, newRow, 0, comboImage);
                AddToGrid(mainGrid, newRow, 1, CreateLabel(nombre));
                AddToGrid(mainGrid, newRow, 2, CreateLabel(valor.ToString("C0")));
                AddToGrid(mainGrid, newRow, 3, CreateLabel(cantidad));
                AddToGrid(mainGrid, newRow, 4, totalGrid); // Añadir el Grid con el Label y el botón a la columna 4
            }
            else
            {
                switch (categoria)
                {
                    case "Boletas":
                        ContenedorBoletas.Children.Add(CreateLabel(nombre));
                        ContenedorBoletas.Children.Add(CreateLabel(nombre));
                        ContenedorBoletasPrecio.Children.Add(CreateLabel(valor.ToString("C0")));
                        ContenedorBoletasCantidad.Children.Add(CreateLabel(cantidad));
                        ContenedorBoletasTotal.Children.Add(CreateLabel(total.ToString("C0")));
                        break;

                    case "Gafas":
                        ContenedorGafas.Children.Add(CreateLabel(nombre));
                        ContenedorGafasPrecio.Children.Add(CreateLabel(valor.ToString("C0")));
                        ContenedorGafasCantidad.Children.Add(CreateLabel(cantidad));
                        ContenedorGafasTotal.Children.Add(CreateLabel(total.ToString("C0")));
                        break;
                }
            }
        }


        private void AddToGrid(Grid grid, int row, int column, UIElement element)
        {
            Grid.SetRow(element, row);
            Grid.SetColumn(element, column);
            grid.Children.Add(element);
        }




        private void GenerateResumenCategoriaLimpiar()
        {
            // Crear una lista de elementos a eliminar para evitar modificaciones durante la iteración
            var elementsToRemove = new List<UIElement>();

            // Recorrer los elementos hijos del Grid
            foreach (UIElement child in mainGrid.Children)
            {
                // Verificar la fila del elemento
                int row = Grid.GetRow(child);

                // Si la fila es mayor o igual a 3, añadir el elemento a la lista para eliminar
                if (row >= 3)
                {
                    elementsToRemove.Add(child);
                }
            }

            // Eliminar los elementos de la lista
            foreach (UIElement element in elementsToRemove)
            {
                mainGrid.Children.Remove(element);
            }

            // Limpiar otros contenedores específicos si los usas
            ContenedorBoletas.Children.Clear();
            ContenedorBoletasPrecio.Children.Clear();
            ContenedorBoletasCantidad.Children.Clear();
            ContenedorBoletasTotal.Children.Clear();
            ContenedorGafas.Children.Clear();
            ContenedorGafasPrecio.Children.Clear();
            ContenedorGafasCantidad.Children.Clear();
            ContenedorGafasTotal.Children.Clear();
        }


        public StackPanel CargarImagen(string imagen)
        {

            var stackPanel = new StackPanel
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            var imagenBorde = new Border
            {
                Width = 120,
                Height = 120,
                CornerRadius = new CornerRadius(5), // Esquinas redondeadas para la imagen
                ClipToBounds = true,
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
                    Width = 120,
                    Height = 120,
                    Stretch = Stretch.Fill,
                    Source = new BitmapImage(new Uri(imagen))
                }
            };

            stackPanel.Children.Add(imagenBorde);

            return stackPanel;

        }

        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {


            ApplyBlurEffect();
            var open = new ResumenCompraEliminar(config, this);
            open.ShowDialog();
            RemoveBlurEffect();
        }

        private void ApplyBlurEffect()
        {
            BlurEffect blur = new BlurEffect
            {
                Radius = 10 // Ajusta el radio del desenfoque según tus necesidades
            };
            gridPrincipal.Effect = blur;
        }

        private void RemoveBlurEffect()
        {
            gridPrincipal.Effect = null;
        }
    }
}