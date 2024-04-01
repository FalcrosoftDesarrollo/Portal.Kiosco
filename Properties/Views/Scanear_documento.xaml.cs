using Portal.Kiosco.Properties.Views;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Portal.Kiosco
{
    /// <summary>
    /// Lógica de interacción para Scanear_documento.xaml
    /// </summary>
    public partial class Scanear_documento : Window
    {
        public Scanear_documento()
        {
            InitializeComponent();
        }
        private void Scanear_documento_Loaded(object sender, RoutedEventArgs e)
        {

        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnIngresaDocumento_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void TextDocumento_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {
                    var openWindow = new IngresoDePassword();
                    DoubleAnimation fadeOutAnimation = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.5));
                    this.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);
                    await Task.Delay(300);
                    this.Visibility = Visibility.Collapsed;
                    openWindow.Background = Brushes.White;
                    openWindow.Show();
                    DoubleAnimation fadeInAnimation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.5));
                    openWindow.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);
                    App.DatosCineFans.Documento = TextDocumento.Text;
                }
            }
            catch (Exception ex)
            {
                var errorWindow = new ErrorGeneral();
                errorWindow.Owner = this;
                errorWindow.Closed += (s, args) =>
                {
                    this.Visibility = Visibility.Visible;
                };
                errorWindow.ShowDialog();
            }
        }
    }
}
