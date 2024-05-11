using APIPortalKiosco.Entities;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Portal.Kiosco.Properties.Views
{
    /// <summary>
    /// Lógica de interacción para Frame16.xaml
    /// </summary>
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
                lblnombre.Content = "!HOLA " + App.ob_diclst["Nombre"].ToString() + " " + App.ob_diclst["Apellido"].ToString();
            }
            else
            {
                lblnombre.Content = "!HOLA INVITADO";
            }

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

        private void ComprobarTiempo()
        {
            if (App._tiempoRestanteGlobal == "00:00")
            {
                this.Dispatcher.Invoke(() =>
                {
                    Principal principal = Application.Current.Windows.OfType<Principal>().FirstOrDefault();
                    if (principal != null)
                    {
                        this.Close();
                        principal.Show();
                    }
                    else
                    {

                        Principal p = new Principal();
                        this.Close();
                        p.Show();
                    }
                });
            }
        }

        private void btnImprimir_Click(object sender, RoutedEventArgs e)
        {
            // Crear una instancia de la ventana secundaria
            Window ventanaSecundaria = new BoletaFactura();

            // Extraer el contenido visual de la ventana secundaria
            UIElement contenidoVisual = ventanaSecundaria.Content as UIElement;

            // Desconectar el contenido visual de la ventana secundaria
            ventanaSecundaria.Content = null;

            // Crear una instancia de la clase de impresión y pasarle el contenido visual
            ImpresionDirectaWPF impresion = new ImpresionDirectaWPF(contenidoVisual);

            // Llamar al método para imprimir directamente el contenido visual
            impresion.ImprimirDirecto();
        }

        private void PrintDocument()
        {
            // Crear un diálogo de impresión
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                // Crear el documento a imprimir
                FlowDocument document = new FlowDocument();

                document.Blocks.Add(new Paragraph(new Run("Colombiana de Cines S.A. Pro:cinal Parque Fabricato")));
                document.Blocks.Add(new Paragraph(new Run("NIT: 900.090.098-0")));

                // Detalles de la matriz y sucursal
                document.Blocks.Add(new Paragraph(new Run("Matriz:")));
                document.Blocks.Add(new Paragraph(new Run("Colombiana de Cines S.A.")));
                document.Blocks.Add(new Paragraph(new Run("Sucursal:")));
                document.Blocks.Add(new Paragraph(new Run("Cra 50 N 38 A-185, local 04284")));

                // Detalles de la factura

                // App.ProductosSeleccionados
                var Fecha = DateTime.Now.ToString("dd(MM/yyyy MM:ss:FF"); 
                document.Blocks.Add(new Paragraph(new Run("Detalle del documento: 328-014-000874403")));
                document.Blocks.Add(new Paragraph(new Run("Fecha: "+ Fecha)));
                document.Blocks.Add(new Paragraph(new Run("Resolución de facturación:")));
                document.Blocks.Add(new Paragraph(new Run("18764056988493 del 10/11/2023")));
                document.Blocks.Add(new Paragraph(new Run("desde PF 748662 hasta PF 2000000")));

                // Detalles de los artículos comprados
                document.Blocks.Add(new Paragraph(new Run("Factura de Venta: PF 000874403")));
                document.Blocks.Add(new Paragraph(new Run("Cliente: CONSUMIDOR FINAL")));
                document.Blocks.Add(new Paragraph(new Run("Cant Producto       Im      Precio     Total")));
                
                document.Blocks.Add(new Paragraph(new Run("1    Perro caliente         8.00      $14,900.00      $14,900.00")));

                // Totales y pago
                document.Blocks.Add(new Paragraph(new Run("Subtotal: 8%        $13,796.30")));
                document.Blocks.Add(new Paragraph(new Run("Subtotal: 0%        $0.00")));
                document.Blocks.Add(new Paragraph(new Run("IMPOCONSUMO 8%  $1,103.70")));
                document.Blocks.Add(new Paragraph(new Run("TOTAL: $14,900.00")));

                document.Blocks.Add(new Paragraph(new Run("Valor pagado en:")));
                document.Blocks.Add(new Paragraph(new Run("Efectivo: $14,900.00")));
                
                // Especificar características del documento, como margen, orientación, etc.
                document.PagePadding = new Thickness(50);
                document.ColumnGap = 0;
                document.ColumnWidth = printDialog.PrintableAreaWidth;

                // Imprimir el documento
                printDialog.PrintDocument(((IDocumentPaginatorSource)document).DocumentPaginator, "Impresión de prueba");
            }
        }

        private void btnSiguiente_Click(object sender, RoutedEventArgs e)
        {
            isThreadActive = false;
            CorreoTecladoFlotante w = new CorreoTecladoFlotante(config);
            this.Close();
            w.ShowDialog();
        }

        private void btnVolver_Click(object sender, RoutedEventArgs e)
        {
            isThreadActive = false;
            InstruccionesDatafono w = new InstruccionesDatafono();
            this.Close();
            w.ShowDialog();
        }

        private async void btnSalir_Click(object sender, RoutedEventArgs e)
        {
            isThreadActive = false;
            Principal w = new Principal();
            this.Close();
            w.ShowDialog();

        }

        private async void btnEnviar_Click(object sender, RoutedEventArgs e)
        {
            isThreadActive = false;
            CorreoTecladoFlotante w = new CorreoTecladoFlotante(config);
            this.Close();
            w.ShowDialog();
        }


    }
}