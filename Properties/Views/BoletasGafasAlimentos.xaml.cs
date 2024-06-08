using APIPortalKiosco.Entities;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Animation;

namespace Portal.Kiosco.Properties.Views
{
    public partial class BoletasGafasAlimentos : Window
    {
        private readonly IOptions<MyConfig> config;
        private bool isThreadActive = true;

        public BoletasGafasAlimentos()
        {
            InitializeComponent();
            DataContext = ((App)Application.Current);
            if (App.ob_diclst.Count > 0)
            {
                lblnombre.Content = "¡HOLA " + App.ob_diclst["Nombre"].ToString() + " " + App.ob_diclst["Apellido"].ToString() + "!";
            }
            else
            {
                lblnombre.Content = "¡HOLA INVITADO!";
            }

            DoubleAnimation fadeInAnimation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.5));
            gridPrincipal.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);

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


        private void btnImprimir_Click(object sender, RoutedEventArgs e)
        {
            var compra = App.TipoCompra;
            Window ventanaSecundaria = new BoletaFactura(config);

            UIElement contenidoVisual = ventanaSecundaria.Content as UIElement;

            ventanaSecundaria.Content = null;

            ImpresionDirectaWPF impresion = new ImpresionDirectaWPF(contenidoVisual);

            impresion.ImprimirDirecto();
        }

        private void PrintDocument()
        {
            PrintDialog printDialog = new PrintDialog();

            if (printDialog.ShowDialog() == true)
            {
                FlowDocument document = new FlowDocument();

                document.Blocks.Add(new Paragraph(new Run("Colombiana de Cines S.A. Pro:cinal Parque Fabricato")));
                document.Blocks.Add(new Paragraph(new Run("NIT: 900.090.098-0")));
                document.Blocks.Add(new Paragraph(new Run("Matriz:")));
                document.Blocks.Add(new Paragraph(new Run("Colombiana de Cines S.A.")));
                document.Blocks.Add(new Paragraph(new Run("Sucursal:")));
                document.Blocks.Add(new Paragraph(new Run("Cra 50 N 38 A-185, local 04284")));

                var Fecha = DateTime.Now.ToString("dd(MM/yyyy MM:ss:FF"); 

                document.Blocks.Add(new Paragraph(new Run("Detalle del documento: 328-014-000874403")));
                document.Blocks.Add(new Paragraph(new Run("Fecha: "+ Fecha)));
                document.Blocks.Add(new Paragraph(new Run("Resolución de facturación:")));
                document.Blocks.Add(new Paragraph(new Run("18764056988493 del 10/11/2023")));
                document.Blocks.Add(new Paragraph(new Run("desde PF 748662 hasta PF 2000000")));
                document.Blocks.Add(new Paragraph(new Run("Factura de Venta: PF 000874403")));
                document.Blocks.Add(new Paragraph(new Run("Cliente: CONSUMIDOR FINAL")));
                document.Blocks.Add(new Paragraph(new Run("Cant Producto       Im      Precio     Total")));
                document.Blocks.Add(new Paragraph(new Run("1    Perro caliente         8.00      $14,900.00      $14,900.00")));
                document.Blocks.Add(new Paragraph(new Run("Subtotal: 8%        $13,796.30")));
                document.Blocks.Add(new Paragraph(new Run("Subtotal: 0%        $0.00")));
                document.Blocks.Add(new Paragraph(new Run("IMPOCONSUMO 8%  $1,103.70")));
                document.Blocks.Add(new Paragraph(new Run("TOTAL: $14,900.00")));
                document.Blocks.Add(new Paragraph(new Run("Valor pagado en:")));
                document.Blocks.Add(new Paragraph(new Run("Efectivo: $14,900.00")));
                document.PagePadding = new Thickness(50);
                document.ColumnGap = 0;
                document.ColumnWidth = printDialog.PrintableAreaWidth;

                printDialog.PrintDocument(((IDocumentPaginatorSource)document).DocumentPaginator, "Impresión de prueba");
            }
        }

        private void btnSiguiente_Click(object sender, RoutedEventArgs e)
        {
            isThreadActive = false;
            CorreoTecladoFlotante openWindows = new CorreoTecladoFlotante(config);
            openWindows.Show();
            this.Close();
        }

        private void btnVolver_Click(object sender, RoutedEventArgs e)
        {
            isThreadActive = false;
            InstruccionesDatafono openWindows = new InstruccionesDatafono();
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

        private async void btnEnviar_Click(object sender, RoutedEventArgs e)
        {
            isThreadActive = false;
            CorreoTecladoFlotante openWindows = new CorreoTecladoFlotante(config);
            openWindows.Show();
            this.Close();
        }
    }
}