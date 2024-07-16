using APIPortalKiosco.Data;
using APIPortalKiosco.Entities;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
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
        private List<ReportSales> ListCarritoB = new List<ReportSales>();
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


            if (secuencia != null)
            {
                lc_secsec = Convert.ToDecimal(secuencia);
                using (var context = new DataDB(config))
                {

                    try
                    {
                        var ReportSales = context.ReportSales.Where(x => x.Secuencia == lc_secsec.ToString()).Where(x => x.KeyPunto == PuntoVenta).Where(x => x.KeyTeatro == KeyTeatro).ToList();
                        ListCarritoB = ReportSales;
                    }
                    catch (Exception e) { }
                }

            }

        }

        private void btnImprimir_Click(object sender, RoutedEventArgs e)
        {
            var compra = App.TipoCompra;

            // Crear una instancia de la ventana secundaria y obtener su contenido visual
            Window ventanaSecundaria = new BoletaFactura(config);
            UIElement contenidoVisualVentanaSecundaria = ventanaSecundaria.Content as UIElement;

            // Desconectar el contenido visual de la ventana secundaria de su padre
            ventanaSecundaria.Content = null;

            // Crear una instancia de ImpresionDirectaWPF con el contenido visual de la ventana secundaria
            ImpresionDirectaWPF impresionVentanaSecundaria = new ImpresionDirectaWPF(contenidoVisualVentanaSecundaria);

            // Intentar imprimir el contenido de la ventana secundaria
            impresionVentanaSecundaria.ImprimirDirecto();
            ListCarrito();
            // Crear una instancia de la ventana de entradas y obtener su contenido visual
            foreach (var boleta in ListCarritoB)
            {
                string[] ubicacionesArray = boleta.SelUbicaciones.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);


                foreach (var ubicacion in ubicacionesArray)
                {
                    var dianomre = boleta.NombreFec;
                    string[] partesUbicacion = ubicacion.Split('_');
                    string columna = partesUbicacion[2]; // Obtener la columna (H)
                    string numeroAsiento = partesUbicacion[3]; // Obtener el número de asiento (p)



                    var hora = boleta.HorProg  ; var salas = boleta.KeySala; string ubicacione = numeroAsiento  + "-" + columna;
                    // Crear una instancia de la ventana de entradas y obtener su contenido visual
                    Window ventanaEntradas = new Entradas(dianomre, boleta.NombreHor, salas, ubicacione, boleta.FecProg);
                    UIElement contenidoVisualEntradas = ventanaEntradas.Content as UIElement;

                    // Desconectar el contenido visual de la ventana de entradas de su padre
                    ventanaEntradas.Content = null;

                    // Crear una instancia de ImpresionDirectaWPF con el contenido visual de la ventana de entradas
                    ImpresionDirectaWPF impresionVentanaEntradas = new ImpresionDirectaWPF(contenidoVisualEntradas);

                    // Intentar imprimir el contenido de la ventana de entradas
                    impresionVentanaEntradas.ImprimirDirecto();
                }
            }
           
            // Mostrar la ventana principal
            Principal openWindows = new Principal();
            openWindows.Show();

            // Cerrar la ventana actual
            this.Close();
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