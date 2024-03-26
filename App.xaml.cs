using APIPortalKiosco.Entities;
using APIPortalWebMed.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Portal.Kiosco
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static bool IsUserAuthenticated { get; set; }
        public static CineFansData DatosCineFans { get; set; }
        public static bool IsBoleteriaConfiteria { get; set; } = false;
        public static bool IsCinefans { get; set; } = false;
        public static List<Pelicula> Peliculas = new List<Pelicula>();
        public static Pelicula Pelicula = new Pelicula();
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            DatosCineFans = new CineFansData();
            Peliculas = new List<Pelicula>();
            Pelicula = new  Pelicula();
        }

        public static string ObtenerValorDeConfiguracion(string nombreConfiguracion)
        {
            return ConfigurationManager.AppSettings[nombreConfiguracion];
        }
    }
}
