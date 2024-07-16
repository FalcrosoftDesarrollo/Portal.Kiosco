using APIPortalKiosco.Data;
using APIPortalKiosco.Entities;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace Portal.Kiosco.Properties.Views
{
    public partial class Entradas : Window
    {
        private readonly IOptions<MyConfig> config;
        public Entradas(string dianomre, string hora, string salas, string ubicacione,string fecha)
        {
          
            InitializeComponent();
            txtpelicula.Text = App.Pelicula.TituloOriginal; 
            txthora.Text = hora;
            txtfecha.Text = fecha;
            txtubicacion.Text = ubicacione;
            txtsala.Text = salas;
            txtdia.Text = dianomre;
            txtformato.Text = App.Pelicula.Formato;




        }

    }
}
