﻿using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Portal.Kiosco.Properties.Views
{
    /// <summary>
    /// Lógica de interacción para Frame13.xaml
    /// </summary>
    public partial class ResumenCompra : Window
    {
        public ResumenCompra()
        {
            InitializeComponent();
            DataContext = ((App)Application.Current);
        }

        private void btnVolver_Click(object sender, RoutedEventArgs e)
        {
            Combodeluxe1 w = new Combodeluxe1();
            this.Close();
            w.ShowDialog();
        }

        private void btnSiguiente_Click(object sender, RoutedEventArgs e)
        {
            //PagoCashback w = new PagoCashback();
            //this.Close();
            //w.ShowDialog();
        }

        private async void btnSalir_Click(object sender, RoutedEventArgs e)
        {
            await App.OpenWindow("Principal");
        }

        private async void btnPagoTarjeta_Click(object sender, RoutedEventArgs e)
        {
            await App.OpenWindow("InstruccionesDatafono");
        }

        private async void btnPagarCash_Click(object sender, RoutedEventArgs e)
        {
            await App.OpenWindow("PagoCashback");
        }
    }
}