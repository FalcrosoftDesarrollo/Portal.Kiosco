using System;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace Portal.Kiosco.Properties.Views
{
    public partial class Principal : Window
    {
        private bool isThreadActive = true;
        private static Principal instance;

        public Principal()
        {
            InitializeComponent();
            ((App)Application.Current).ResetearTimer();
            App.IsFecha = false;
            this.KeyDown += Principal_KeyDown; // Manejador de eventos para teclas presionadas
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

        public static Principal GetInstance()
        {
            if (instance == null)
            {
                instance = new Principal();
            }
            return instance;
        }

        private void ComprobarTiempo()
        {
            if (App._tiempoRestanteGlobal == "00:00")
            {
                this.Dispatcher.Invoke(() =>
                {
                    if (this.Visibility == Visibility.Hidden)
                    {
                        this.Show();
                        this.Activate();
                    }
                });
            }
        }

        private void Principal_KeyDown(object sender, KeyEventArgs e)
        {
            // Verificar si se presionó Control + Escape
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                if (e.Key == Key.D)
                {
                    this.Close();
                }
            }
        }

        private async void btnBoleteria_Click(object sender, RoutedEventArgs e)
        {
            isThreadActive = false;
            App.IsBoleteriaConfiteria = false;
            ComoCompra w = new ComoCompra();
            this.Close();
            w.ShowDialog();
            
        }

        private async void btnConfiteria_Click(object sender, RoutedEventArgs e)
        {
            isThreadActive = false;
            App.IsBoleteriaConfiteria = true;
            ComoCompra w = new ComoCompra();
            this.Close();
            w.ShowDialog();
        }
    }
}