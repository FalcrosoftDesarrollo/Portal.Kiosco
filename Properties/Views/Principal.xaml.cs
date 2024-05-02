using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Input;

namespace Portal.Kiosco.Properties.Views
{
    public partial class Principal : Window
    {
        public Principal()
        {
            InitializeComponent();
            App.IsFecha = false;
            this.KeyDown += Principal_KeyDown; // Manejador de eventos para teclas presionadas

        }

        private void Principal_KeyDown(object sender, KeyEventArgs e)
        {
            // Verificar si se presionó Control + Escape
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                if (e.Key == Key.D)
                {
                    // Cerrar la ventana
                    this.Close();
                }
            }
        }

        private async void btnBoleteria_Click(object sender, RoutedEventArgs e)
        {
            App.IsBoleteriaConfiteria = false;
            var comoComprarWindow = new ComoCompra();
            DoubleAnimation fadeOutAnimation = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.5));
            this.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);
            await Task.Delay(300); 
            this.Visibility = Visibility.Collapsed;
            comoComprarWindow.Background = Brushes.White;
            comoComprarWindow.Show();
            this.Close();
            DoubleAnimation fadeInAnimation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.5));
            comoComprarWindow.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);
        }

        private async void btnConfiteria_Click(object sender, RoutedEventArgs e)
        {
            App.IsBoleteriaConfiteria = true;
            var comoComprarWindow = new ComoCompra();
            DoubleAnimation fadeOutAnimation = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.5));
            this.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);
            await Task.Delay(300);
            this.Visibility = Visibility.Collapsed;
            comoComprarWindow.Background = Brushes.White;
            comoComprarWindow.Show();
            this.Close();
            DoubleAnimation fadeInAnimation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.5));
            comoComprarWindow.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);
        }
    }
}
