using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;


namespace Portal.Kiosco.Properties.Views
{
    /// <summary>
    /// Lógica de interacción para ComoCompra.xaml
    /// </summary>
    public partial class ComoCompra : Window
    {
        private DispatcherTimer timer;
        private int contador;
        public ComoCompra()
        {
            InitializeComponent();
            contador = 10; // 15 minutos en segundos

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private async void btnCinefans_Click(object sender, RoutedEventArgs e)
        {
            App.IsCinefans = false;
            var openWindow = new Scanear_documento();
            DoubleAnimation fadeOutAnimation = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.5));
            this.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);
            await Task.Delay(300);
            this.Visibility = Visibility.Collapsed;
            openWindow.Background = Brushes.White;
            openWindow.Show();
            DoubleAnimation fadeInAnimation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.5));
            openWindow.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);
        }

        private async void btnInvitado_Click(object sender, RoutedEventArgs e)
        {
            App.IsCinefans = true;
            var openWindow = new Cartelera();
            DoubleAnimation fadeOutAnimation = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.5));
            this.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);
            await Task.Delay(300);
            this.Visibility = Visibility.Collapsed;
            openWindow.Background = Brushes.White;
            openWindow.Show();
            DoubleAnimation fadeInAnimation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.5));
            openWindow.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);
        }

        private async void btnVolverComoCompra_Click(object sender, RoutedEventArgs e)
        {
            var openWindow = new Principal();
            DoubleAnimation fadeOutAnimation = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.5));
            this.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);
            await Task.Delay(300);
            this.Visibility = Visibility.Collapsed;
            openWindow.Background = Brushes.White;
            openWindow.Show();
            DoubleAnimation fadeInAnimation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.5));
            openWindow.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);
        }

        private async void Timer_Tick(object sender, EventArgs e)
        {
            contador--;
            if (contador == 0)
            {
                timer.Stop();
                var openWindow=new Principal();
                DoubleAnimation fadeOutAnimation = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.5));
                this.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);
                await Task.Delay(300);
                this.Visibility = Visibility.Collapsed;
                openWindow.Background = Brushes.White;
                openWindow.Show();
                DoubleAnimation fadeInAnimation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.5));
                openWindow.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);
            }
        }
    }

     
}
