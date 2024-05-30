using APIPortalKiosco.Data;
using APIPortalKiosco.Entities;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Portal.Kiosco.Properties.Views
{
    public partial class CorreoTecladoFlotante : Window
    {
        private readonly IOptions<MyConfig> config;
        private bool isThreadActive = true;

        public CorreoTecladoFlotante(IOptions<MyConfig> config)
        {
            InitializeComponent();
            DataContext = ((App)Application.Current);
            this.config = config;

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

        private async void btnObtenerDatos_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string email = TextCorreoElectronico.Text;

                if (string.IsNullOrWhiteSpace(email))
                {
                    MessageBox.Show("!Ups, aún falta ingresar el correo electrónico!", "Notificación", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                else if (!IsValidEmail(email))
                {
                    MessageBox.Show("El correo electrónico ingresado no es válido.", "Notificación", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                else if (IsValidEmail(email))
                {
                    EnvioCorreo(email);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EnvioCorreo(string correo)
        {
            string lc_fectra = string.Empty;
            string lc_valtra = string.Empty;
            string lc_idsepy = string.Empty;
            string lc_status = string.Empty;
            string lc_coreli = correo;
            string lc_jsnrst = string.Empty;
            string lc_objson = string.Empty;
            string lc_srvpar = string.Empty;
            string lc_refepy = string.Empty;
            string lc_bankpy = string.Empty;
            string lc_urlcor = App.UrlCorreo;
            string session = "";
            string status = "";
            decimal lc_secsec = Convert.ToDecimal(App.Secuencia);
            decimal lc_keytea = Convert.ToDecimal(App.idCine);
            decimal lc_puntea = Convert.ToDecimal(App.PuntoVenta);

            List<OrderItem> ob_ordite = new List<OrderItem>();
            Dictionary<string, string> ob_diclst = new Dictionary<string, string>();

            TransactionSales ob_repsle = new TransactionSales();
            General ob_fncgrl = new General();

            try
            {
                lc_urlcor = lc_urlcor.Replace("#xxx", lc_keytea.ToString());
                lc_urlcor = lc_urlcor.Replace("#yyy", lc_puntea.ToString());
                lc_urlcor = lc_urlcor.Replace("#zzz", lc_secsec.ToString());
                lc_urlcor = lc_urlcor.Replace("#ccc", correo);

                using (var context = new DataDB(config))
                {
                    var rs = context.RetailSales.Where(x => x.Secuencia == lc_secsec).Where(x => x.PuntoVenta == lc_puntea).Where(x => x.KeyTeatro == lc_keytea).Where(x => x.Tipo == "C").ToList();
                    List<RetailSales> retailsales = rs
                        .GroupBy(l => l.KeyProducto)
                        .Select(cl => new RetailSales
                        {
                            Descripcion = cl.First().Descripcion,
                            Cantidad = cl.Sum(c => c.Cantidad),
                            Precio = cl.Sum(c => c.Precio)
                        }).ToList();

                    foreach (var vr_itevta in retailsales)
                    {
                        ob_ordite.Add(new OrderItem
                        {
                            Precio = vr_itevta.Precio,
                            Cantidad = Convert.ToInt32(vr_itevta.Cantidad),
                            Descripcion = vr_itevta.Descripcion
                        });
                    }

                    rs = context.RetailSales.Where(x => x.Secuencia == lc_secsec).Where(x => x.PuntoVenta == lc_puntea).Where(x => x.KeyTeatro == lc_keytea).Where(x => x.Tipo != "C").ToList();

                    foreach (var vr_itevta in rs)
                    {
                        ob_ordite.Add(new OrderItem
                        {
                            Precio = vr_itevta.Precio * Convert.ToInt32(vr_itevta.Cantidad),
                            Cantidad = Convert.ToInt32(vr_itevta.Cantidad),
                            Descripcion = vr_itevta.Descripcion
                        });
                    }
                }

                if (lc_secsec != null)
                {
                    try
                    {
                        var request = (HttpWebRequest)WebRequest.Create(lc_urlcor);
                        request.GetResponse();
                        var openWindows = new Principal();
                        openWindows.Show();
                        this.Close();

                    }
                    catch (Exception)
                    {
                        string EnvioCorreo = "Fallo envío de correo compra APROBADA, por favor comunicarse con el teatro.";
                    }
                }
            }
            catch (Exception lc_syserr)
            {

            }
        }

        private bool IsValidEmail(string email)
        {
            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            Regex regex = new Regex(pattern);
            return regex.IsMatch(email);
        }

        private void KeyButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button != null)
            {
                string character = button.Content.ToString();

                if (character == "Del")
                {
                    if (TextCorreoElectronico.Text.Length > 0)
                    {
                        TextCorreoElectronico.Text = TextCorreoElectronico.Text.Substring(0, TextCorreoElectronico.Text.Length - 1);
                    }
                }
                else
                {
                    TextCorreoElectronico.Text += character;
                }
            }
        }

        private async void btnVolver_Click(object sender, RoutedEventArgs e)
        {
            isThreadActive = false;
            BoletasGafasAlimentos openWindows = new BoletasGafasAlimentos();
            this.Close();
            openWindows.Show();
        }

        private void btnSalir_Click(object sender, RoutedEventArgs e)
        {
            isThreadActive = false;
            var openWindows = new Principal();
            this.Close();
            openWindows.Show();
        }
    }
}