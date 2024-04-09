using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Portal.Kiosco.Properties.Views
{
    /// <summary>
    /// Lógica de interacción para Principal.xaml
    /// </summary>
    public partial class Principal : Window
    {
        public Principal()
        {
            InitializeComponent();
            App.IsFecha = false;
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
            DoubleAnimation fadeInAnimation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.5));
            comoComprarWindow.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);
        }
    }
}
