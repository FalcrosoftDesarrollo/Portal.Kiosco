using APIPortalKiosco.Entities;
using APIPortalWebMed.Entities;
using System;
using System.Collections.Generic;
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
            this.KeyDown += Principal_KeyDown;
            App.Pelicula = new Pelicula();
            App.ProductosSeleccionados = new List<Producto>();
            DoubleAnimation fadeInAnimation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.5));
            gridPrincipal.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);
        
        }

        public static Principal GetInstance()
        {
            if (instance == null)
            {
                instance = new Principal();
            }
            return instance;
        }

        private void Principal_KeyDown(object sender, KeyEventArgs e)
        {
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
            App.TipoCompra = "B";
            isThreadActive = false;
            App.IsBoleteriaConfiteria = false;
            ComoCompra openWindows = new ComoCompra();
            openWindows.Show();
            this.Close();
        }

        private async void btnConfiteria_Click(object sender, RoutedEventArgs e)
        {
            App.TipoCompra = "P";
            isThreadActive = false;
            App.IsBoleteriaConfiteria = true;
            ComoCompra openWindows = new ComoCompra();
            openWindows.Show();
            this.Close();
        }
    }
}