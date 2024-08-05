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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Portal.Kiosco.Properties.Views
{
    public partial class ResumenCompraEliminar : Window
    {
        private readonly IOptions<MyConfig> config;
        private bool isThreadActive = true;

        private ResumenCompra resumenCompra;


        public ResumenCompraEliminar(IOptions<MyConfig> config, ResumenCompra resumenCompra)
        {
            InitializeComponent();
            ListCarrito();
            DataContext = ((App)Application.Current);
            this.config = config;
            var validacion = App.Pelicula;
            this.resumenCompra = resumenCompra;

            Loaded += Combopopup_Loaded;

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

        private void Combopopup_Loaded(object sender, RoutedEventArgs e)
        {
            CenterWindow();
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
            Combodeluxe1 openWindows = new Combodeluxe1();
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

        private void ListCarrito()
        {
            #region VARIABLES LOCALES
            decimal lc_secsec = 0;
            var PuntoVenta = App.PuntoVenta;
            var KeyTeatro = App.idCine;
            string secuencia = "";
            #endregion

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
        public void GenerateResumen(List<RetailSales> ListCarritoR, List<ReportSales> ListCarritoB)
        {
            decimal totalcombos = 0;
            totalcombos += 0;

            var combos = ListCarritoR;
            var combosAgrupados = combos.GroupBy(c => c.KeyProducto);

            // Clear existing children
            MainStackPanel.Children.Clear();

            foreach (var grupoCombos in combosAgrupados)
            {
                var groupedItems = grupoCombos
                    .GroupBy(item => new { item.Descripcion, item.Precio, item.KeyProducto })
                    .Select(g => new
                    {
                        Descripcion = g.Key.Descripcion,
                        Precio = g.Key.Precio,
                        Cantidad = g.Count(),
                        Codigo = g.Key.KeyProducto
                    });

                foreach (var group in groupedItems)
                {
                    string nombre = group.Descripcion;
                    decimal precio = group.Precio;
                    int cantidad = group.Cantidad;
                    decimal total = precio * cantidad;

                    GenerateResumenCategoria("Combos", nombre, precio, cantidad.ToString(), total, group.Codigo);
                }
            }

            App.TotalPagar = totalcombos.ToString();
        }

        private void GenerateResumenCategoria(string categoria, string nombre, decimal valor, string cantidad, decimal total, decimal codigo)
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

            CheckBox CreateCheckBox()
            {
                return new CheckBox
                {
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center
                    
                };
            }

            // Crear el contenedor para los elementos
            Grid container = new Grid();
            container.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            container.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(120) });
            container.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            container.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(40) });

            switch (categoria)
            {
                case "Combos":
                    // Crear los elementos
                    CheckBox checkBox = CreateCheckBox();
                    checkBox.Name = "c" + codigo.ToString();
                    string lc_auximg = string.Concat(App.UrlRetailImg, codigo.ToString(), ".jpg");
                    StackPanel image = CargarImagen(lc_auximg);
                    Label nameLabel = CreateLabel(nombre);
                    string imageneliminar = "C:\\FALCROSOFT\\PROCINAL\\Portal.Kiosco\\Properties\\Resources\\eliminar.png";


                    var deleteButton = new StackPanel
                    {
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Center

                    };

                    var imagenBorde = new Border
                    {
                        Name = "b" + codigo.ToString(),
                        Width = 20,
                        Height = 20,
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
                            Width = 20,
                            Height = 20,
                            Stretch = Stretch.Fill,
                            Source = new BitmapImage(new Uri(imageneliminar))
                        }
                    };

                    deleteButton.Children.Add(imagenBorde);

                    // Configurar eventos
                    imagenBorde.MouseLeftButtonDown += DeleteButton_Click;

                    // Añadir los elementos al contenedor
                    Grid.SetColumn(checkBox, 0);
                    Grid.SetColumn(image, 1);
                    Grid.SetColumn(nameLabel, 2);
                    Grid.SetColumn(deleteButton, 3);

                    container.Children.Add(checkBox);
                    container.Children.Add(image);
                    container.Children.Add(nameLabel);
                    container.Children.Add(deleteButton);

                    // Añadir el contenedor al StackPanel principal
                    MainStackPanel.Children.Add(container);
                    break;
            }
        }

        private void DeleteButton_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border)
            {
                string codigo = border.Name.Substring(1); // Obtener el código desde el nombre del Border
                                                          // Implementar la lógica de eliminación basada en el código
                App.EliminarRetailSales(Convert.ToDecimal(App.Secuencia), Convert.ToDecimal(codigo));

                // Encontrar el Grid contenedor y eliminarlo del StackPanel
                Grid parentGrid = FindParent<Grid>(border);
                if (parentGrid != null)
                {
                    MainStackPanel.Children.Remove(parentGrid);
                }
            }
        }

        // Método helper para encontrar el padre de un tipo específico
        private T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);
            if (parentObject == null) return null;

            T parent = parentObject as T;
            if (parent != null)
            {
                return parent;
            }
            else
            {
                return FindParent<T>(parentObject);
            }
        }
          

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            resumenCompra.ListCarrito();
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
        private void lblBorrarCombos_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            bool foundAny = false;

            foreach (var child in MainStackPanel.Children)
            {
                if (child is Grid grid)
                {
                    foreach (var gridChild in grid.Children)
                    {
                        if (gridChild is CheckBox checkBox && checkBox.IsChecked == true)
                        {

                            foreach (var producto in App.ProductosSeleccionados)
                            {
                                if (checkBox.Name.Substring(1) == Convert.ToInt32(producto.Codigo).ToString())
                                {
                                    App.EliminarRetailSales(Convert.ToDecimal(App.Secuencia), producto.Codigo);
                                    foundAny = true;
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            if (foundAny == true)
            {
                this.Close();
                resumenCompra.ListCarrito();
            }
            else 
            {
                MessageBox.Show("No tiene productos seleccionados para eliminar", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void SeleccionarTodos_Checked(object sender, RoutedEventArgs e)
        {
            bool foundAny = false;

            foreach (var child in MainStackPanel.Children)
            {
                if (child is Grid grid)
                {
                    foreach (var gridChild in grid.Children)
                    {
                        if (gridChild is CheckBox checkBox )
                        { 

                            foreach (var producto in App.ProductosSeleccionados)
                            {
                                if (checkBox.Name.Substring(1) == Convert.ToInt32(producto.Codigo).ToString())
                                {
                                    checkBox.IsChecked = true;
                                    foundAny = true;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
             
        }


    }
}