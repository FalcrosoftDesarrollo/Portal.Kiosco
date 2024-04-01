using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;

namespace Portal.Kiosco.Properties.Views
{
    /// <summary>
    /// Interaction logic for transicion.xaml
    /// </summary>
    public partial class transicion : Window
    {
        public transicion()
        {
            InitializeComponent();
            // Asociar evento Loaded para iniciar la animación
            Loaded += Transicion_Loaded;
        }

        private async void Transicion_Loaded(object sender, RoutedEventArgs e)
        {
            // Animación de desvanecimiento
            DoubleAnimation fadeOutAnimation = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.5));
            BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);

            // Esperar un breve momento antes de cerrar la ventana
            await Task.Delay(500);

            // Cerrar la ventana de transición
            Close();
        }
    }

}
