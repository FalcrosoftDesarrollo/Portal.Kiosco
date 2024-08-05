using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Options;
using APIPortalKiosco.Data;
using APIPortalKiosco.Entities;
using System.Collections.Generic;
using System;

namespace Portal.Kiosco.Properties.Views
{
    public partial class BoletaFactura : UserControl
    {
        private bool isThreadActive = true;
        private readonly IOptions<MyConfig> config;

        public BoletaFactura(IOptions<MyConfig> config)
        {
            InitializeComponent();
            ListCarrito();
             

            var diasel = App.DiaSeleccionado;
            var fechasel = App.FechaSeleccionada;
            string horaMilitar = App.Pelicula.HoraMilitar;
            string horaFormateada = horaMilitar == null ? "" : horaMilitar.Insert(2, ":");
         
            FechaFac.Text = DateTime.Now.ToString();
            Sucursal.Text = App.DirEmpresa;
            NomEmpresa.Text = App.NomEmpresa;
            NomEmpresa2.Text = App.NomEmpresa;
            TotalImp.Text = App.IVC == null ? "$0" : "$ " + App.IVC;
            IVA.Text = App.IVA == null ? "$0" : "$ " + App.IVA;
            Valorpagado.Text = TotalFac.Text;
            var combos = App.ProductosSeleccionados;


            if (App.TipoCompra == "B" || App.TipoCompra == "M")
            {
                ImpConsumo.Visibility = Visibility.Collapsed;
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

                    var window = Window.GetWindow(this);
                    if (window != null)
                    {
                        window.Close();
                    }
                });
            }

            return isMainWindowOpen;
        }

        private void ListCarrito()
        {
            decimal lc_secsec = 0;
            var PuntoVenta = App.PuntoVenta;
            var KeyTeatro = App.idCine;
            string secuencia = "";

            App.TipoCompra = "V";
            secuencia = App.Secuencia;
            List<RetailSales> ListCarritoR = new List<RetailSales>();
            List<ReportSales> ListCarritoB = new List<ReportSales>();

            if (secuencia != null)
            {
                lc_secsec = Convert.ToDecimal(secuencia);
                using (var context = new DataDB(config))
                {
                    var RetailSales = context.RetailSales.Where(x => x.Secuencia == lc_secsec).Where(x => x.PuntoVenta == Convert.ToDecimal(PuntoVenta)).Where(x => x.KeyTeatro == Convert.ToDecimal(KeyTeatro)).ToList();
                    ListCarritoR = RetailSales;

                    var ReportSales = context.ReportSales.Where(x => x.Secuencia == lc_secsec.ToString()).Where(x => x.KeyPunto == PuntoVenta).Where(x => x.KeyTeatro == KeyTeatro).ToList();
                    ListCarritoB = ReportSales;
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
            string nombre = "";
            decimal precio = 0;
            decimal cantidad = 0;

            if (App.Pelicula.Nombre != "")
            {
                GenerateResumenCategoria("Boletas", App.Pelicula.Nombre == null || App.Pelicula.Nombre == "" ? "Sin Pelicula" : App.Pelicula.Nombre, App.ValorTarifa, App.CantidadBoletas.ToString(), App.CantidadBoletas * App.ValorTarifa);
            }
            totalcombos += (App.CantidadBoletas * App.ValorTarifa);

            if (App.CantidadGafas.ToString() != "0")
            {
                GenerateResumenCategoria("Gafas", "Gafas", App.PrecioUnitario, App.CantidadGafas.ToString(), (App.CantidadGafas * App.PrecioUnitario));
            }

            totalcombos += 0;

            var combos = ListCarritoR;

            var combosAgrupados = combos.GroupBy(c => c.KeyProducto);

            foreach (var grupoCombos in combosAgrupados)
            {
                foreach (var item in grupoCombos)
                {
                    codigo = item.KeyProducto;
                    nombre = item.Descripcion;
                    precio = item.Precio;
                    cantidad = item.Cantidad;
                }

                string totalString = TotalFac.Text.ToString().Replace("$", "").Replace("€", "").Replace(".", "").Replace(",", "").Trim();
                decimal totalAnterior = decimal.Parse(totalString);
                decimal nuevoTotal = totalAnterior + precio;

                decimal total = nuevoTotal * cantidad;
                totalcombos += total;

                GenerateResumenCategoria("Combos", nombre, precio, cantidad.ToString(), total);
            }

            TotalFac.Text = totalcombos.ToString("C0");
        }

        private void GenerateResumenCategoria(string categoria, string nombre, decimal valor, string cantidad, decimal total)
        {
            Grid grid = new Grid();

            for (int i = 0; i < 6; i++)
            {
                ColumnDefinition columnDefinition = new ColumnDefinition();

                if (i == 0)
                    columnDefinition.Width = new GridLength(1, GridUnitType.Auto);
                else
                    columnDefinition.Width = new GridLength(1, GridUnitType.Star);

                grid.ColumnDefinitions.Add(columnDefinition);
            }

            bool cantidadZeroFound = false;
            for (int i = 0; i < 5; i++)
            {
                if (cantidad == "0" && cantidadZeroFound)
                    continue;

                if (cantidad == "0")
                {
                    cantidadZeroFound = true;
                    continue;
                }

                Border border = new Border();
                Grid.SetColumn(border, i);
                Label label = new Label();
                label.FontFamily = new System.Windows.Media.FontFamily("Myanmar Khyay");
                label.FontSize = 12;
                label.VerticalContentAlignment = VerticalAlignment.Center;
                border.BorderBrush = System.Windows.Media.Brushes.Black;

                if (i == 0)
                {
                    label.Content = cantidad;
                }
                else if (i == 1)
                {
                    label.Content = nombre;
                }
                else if (i == 2)
                {
                    label.Content = "0";
                }
                else if (i == 3)
                {
                    label.Content = valor.ToString("C0");
                }
                else if (i == 4)
                {
                    label.Content = total.ToString("C0");
                }

                label.HorizontalAlignment = HorizontalAlignment.Center;
                border.Child = label;
                grid.Children.Add(border);
            }

            ContenedorFac.Children.Add(grid);
        }

        public string buscarNombre(List<Producto> productos, decimal Codigo)
        {
            var nombre = productos.FirstOrDefault(p => p.Codigo == Codigo)?.Descripcion;
            return nombre ?? string.Empty;
        }

        public decimal buscarprecio(List<Producto> productos, decimal Codigo)
        {
            var precio = productos.FirstOrDefault(p => p.Codigo == Codigo)?.Precios.Sum(p => p.General) ?? 0;
            return precio;
        }
    }
}
