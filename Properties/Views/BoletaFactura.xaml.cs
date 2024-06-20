using APIPortalKiosco.Data;
using APIPortalKiosco.Entities;
using Microsoft.Extensions.Options;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace Portal.Kiosco.Properties.Views
{
    public partial class BoletaFactura : Window
    {
        private bool isThreadActive = true;
        private readonly IOptions<MyConfig> config;
        public BoletaFactura(IOptions<MyConfig> config)
        {
            InitializeComponent();
            ListCarrito();
            CargarQr();
            //var diasel = App.DiaSeleccionado;
            //var fechasel = App.FechaSeleccionada;
            //string horaMilitar = App.Pelicula.HoraMilitar;
            //string horaFormateada = horaMilitar.Insert(2, ":");
            //NomHora.Text = horaFormateada; 
            //FechaFac.Text = DateTime.Now.ToString();
            //Sucursal.Text = App.DirEmpresa;
            //NomEmpresa.Text = App.NomEmpresa;
            //NomEmpresa2.Text = App.NomEmpresa;
            //TotalImp.Text = App.IVC == null ? "$0" : "$ " + App.IVC ;
            //IVA.Text = App.IVA == null ? "$0" : "$ " + App.IVA;
            //factura.Text = App.Secuencia;
            //Valorpagado.Text = TotalFac.Text;
            //var combos = App.ProductosSeleccionados;
            //NomPelicula.Text = App.Pelicula.TituloOriginal;
            //NomFormato.Text = App.NomZona + " " + App.NomTarifa2;
            //NomHora.Text = horaFormateada;
            //FechaSeleccionada.Text = App.FechaSeleccionada;
            //NomSala.Text = App.NumSala;
            //DiaSeleccionado.Text = App.DiaSeleccionado;

            //if (App.TipoCompra == "B")
            //{
            //    IVAPorcentaje.Visibility = Visibility.Collapsed;
            //    ImpConsumo.Visibility = Visibility.Collapsed;
            //}
            //else
            //{
            //    PanelPeliculas.Visibility = Visibility.Collapsed;
            //}


            
    
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

        private void ListCarrito()
        {
            #region VARIABLES LOCALES
            decimal lc_secsec = 0;
            var PuntoVenta = App.PuntoVenta;
            var KeyTeatro = App.idCine;
            string secuencia = "";
            #endregion

            //Validar secuencia y asignar valores
            App.TipoCompra = "V";
            secuencia = App.Secuencia;
            List<RetailSales> ListCarritoR = new List<RetailSales>();
            List<ReportSales> ListCarritoB = new List<ReportSales>();

            if (secuencia != null)
            {
                //Obtener productos carrito de compra
                lc_secsec = Convert.ToDecimal(secuencia);
                using (var context = new DataDB(config))
                {
                    var RetailSales = context.RetailSales.Where(x => x.Secuencia == lc_secsec).Where(x => x.PuntoVenta == Convert.ToDecimal(PuntoVenta)).Where(x => x.KeyTeatro == Convert.ToDecimal(KeyTeatro)).ToList();
                    ListCarritoR = RetailSales;
                    var SwitchAddBtn = RetailSales.Any(x => x.SwitchAdd == "S");

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

                //string totalString = TotalFac.Text.ToString().Replace("$", "").Replace("€", "").Replace(".", "").Replace(",", "").Trim();
                //decimal totalAnterior = decimal.Parse(totalString);
                //decimal nuevoTotal = totalAnterior + precio;

                //decimal total = nuevoTotal * cantidad;
                //totalcombos += total;

                //GenerateResumenCategoria("Combos", nombre, precio, cantidad.ToString(), total);
            }

            //TotalFac.Text = totalcombos.ToString("C0");
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

            //ContenedorFac.Children.Add(grid);
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

        public void CargarQr()
        {
            string sala = App.Pelicula.numeroSala;
            string ubicacion = (LayoutAsientos.sillasSeleccionadasArray).ToString();
            string peli = App.Pelicula.Nombre;
            string combo = App.ProductosSeleccionados.ToString();
            string datos = $"Sala: {sala}, ubicacion: {ubicacion}, pelicula:{peli} , combo: {combo}";
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(datos, QRCodeGenerator.ECCLevel.H);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20);

            using (MemoryStream memory = new MemoryStream())
            {
                qrCodeImage.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();
                //imgqr.Source = bitmapimage;
            }
        }
    }
}