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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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
            var openWindows = new BoletasGafasAlimentos();
          
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
                PagoCashback openWindows = new PagoCashback();
                this.Close();
                openWindows.Show();
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
            decimal codigo = 0;
            //string nombre = "";
            //decimal precio = 0;
            //decimal cantidad = 0;

            if (App.Pelicula.Nombre != "")
            {
                GenerateResumenCategoria("Boletas", App.Pelicula.TituloOriginal == null || App.Pelicula.TituloOriginal == "" ? "Sin Pelicula" : App.Pelicula.TituloOriginal, App.ValorTarifa, App.CantidadBoletas.ToString(), App.CantidadBoletas * App.ValorTarifa);
            }
            totalcombos += (App.CantidadBoletas * App.ValorTarifa);

            if (App.CantidadGafas.ToString() != "0")
            {
                GenerateResumenCategoria("Gafas", "Gafas", App.PrecioUnitario, App.CantidadGafas.ToString(), (App.CantidadGafas * App.PrecioUnitario));
            }

            
            totalcombos += 0;

            var combos = ListCarritoR;

            var combosAgrupados = combos.GroupBy(c => c.KeyProducto);


            //decimal totalcombos = 0;

            foreach (var grupoCombos in combosAgrupados)
            {
                var groupedItems = grupoCombos
                    .GroupBy(item => new { item.Descripcion, item.Precio })
                    .Select(g => new
                    {
                        Descripcion = g.Key.Descripcion,
                        Precio = g.Key.Precio,
                        Cantidad = g.Count()
                    });

                foreach (var group in groupedItems)
                {
                    string nombre = group.Descripcion;
                    decimal precio = group.Precio;
                    int cantidad = group.Cantidad;

                    // Calcula el total actual (remueve caracteres no numéricos y convierte a decimal)
                    string totalString = TotalResumen.Content.ToString().Replace("$", "").Replace("€", "").Replace(".", "").Replace(",", "").Trim();
                    decimal totalAnterior = decimal.Parse(totalString);

                    // Calcula el nuevo total sumando el precio del ítem
                    decimal nuevoTotal = totalAnterior + (precio * cantidad);

                    // Multiplica el nuevo total por la cantidad
                    decimal total = nuevoTotal;

                    // Suma el total al total de combinaciones
                    totalcombos += total;

                    // Genera el resumen de categoría
                    GenerateResumenCategoria("Combos", nombre, precio, cantidad.ToString(), total);
                }
            }

            TotalResumen.Content = totalcombos.ToString("C0");

            App.TotalPagar = totalcombos.ToString();
        }

        private void GenerateResumenCategoria(string categoria, string nombre, decimal valor, string cantidad, decimal total)
        {
            Grid grid = new Grid();
            var columnWidths = new double[] { 28, 840, 0, 185, 50, 185, 50, 185 };

            foreach (var width in columnWidths)
            {
                ColumnDefinition columnDefinition = new ColumnDefinition
                {
                    Width = new GridLength(width)
                };
                grid.ColumnDefinitions.Add(columnDefinition);
            }

            string[] contents = { nombre, valor.ToString("C0"), cantidad, total.ToString("C0") };
            double[] margins = { 0, -78, 0, 0 };

            for (int i = 0; i < contents.Length; i++)
            {
                Border border = new Border
                {
                    BorderBrush = Brushes.Black
                };
                Grid.SetColumn(border, (i * 2) + 1);

                Label label = new Label
                {
                    Content = contents[i],
                    FontFamily = new FontFamily("Myanmar Khyay"),
                    FontSize = 24,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(0, 0, margins[i], 0)
                };

                if (i > 0)
                {
                    label.HorizontalAlignment = HorizontalAlignment.Center;
                }

                border.Child = label;
                grid.Children.Add(border);
            }

            switch (categoria)
            {
                case "Boletas":
                    ContenedorBoletas.Children.Add(grid);
                    break;
                case "Gafas":
                    ContenedorGafas.Children.Add(grid);
                    break;
                case "Combos":
                    ContenedorResumen.Children.Add(grid);
                    break;
            }
        }

    }
}