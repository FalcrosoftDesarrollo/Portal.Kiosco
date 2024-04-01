using APIPortalKiosco.Entities;
using Portal.Kiosco.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml.Linq;

namespace Portal.Kiosco.Properties.Views
{
    public partial class SeleccionarFuncion : Window
    {
        public SeleccionarFuncion()
        {
            InitializeComponent();
            CargarHorariosDesdeXml(App.Pelicula.Id,"", "Normal"); // Reemplaza "ID_DE_LA_PELICULA_A_BUSCAR" con el ID real de la película que deseas buscar
        }

        private void CargarHorariosDesdeXml(string pr_keypel, string pr_fecprg = "", String pr_tippel = "")
        {
            try
            {

                string teatro = App.ObtenerValorDeConfiguracion("TeaDefault");
                // Ruta del archivo XML
                string rutaXml = "https://scorecoorp.procinal.com/mobilecomjson//nuevaweb/variableCARTELERA_TEA_PEL.aspx?teatro=xxx&pelicula=yyy";

                DateTime dt_fechoy = DateTime.Now;

                var helper = new Helper();

                if (string.IsNullOrEmpty(pr_fecprg))
                    pr_fecprg = dt_fechoy.ToString("yyyyMMdd");

                // Construir la URL completa con el valor de la sesión del teatro
                string url = rutaXml;

                XDocument xdoc = XDocument.Load("https://scorecoorp.procinal.com/mobilecomjson//nuevaweb/variable41CARTELERA310_7163.xml");

                var ob_fechas = (
               from pelicula in xdoc.Descendants("pelicula")
               let idAttr = pelicula.Attribute("id")?.Value
               let lc_auxipel_inner = (idAttr?.Length >= 8 && idAttr?.Length <= 10) ? idAttr.Substring(0, idAttr.Length - 5) : string.Empty
               where pelicula.Attribute("tipo")?.Value == pr_tippel && lc_auxipel_inner == (pr_keypel.Length >= 5 ? pr_keypel.Substring(0, pr_keypel.Length - 5) : pr_keypel)
               from cinema in pelicula.Descendants("cinema")
               where cinema.Attribute("id")?.Value == "310"
               from dia in pelicula.Descendants("DiasDisponiblesTodosCinemas").Descendants("dia")
               let auxFec = dia.Attribute("univ")?.Value
               where !string.IsNullOrEmpty(auxFec)
               let dtAuxFec = DateTime.ParseExact(auxFec, "yyyyMMdd", CultureInfo.InvariantCulture)
               group new { dtAuxFec, auxFec } by dtAuxFec.Date into grouped
               select new DateCartelera
               {
                   DiaLt = helper.DiaMes(grouped.Key.DayOfWeek.ToString(), "D"),
                   Flags = (pr_fecprg == grouped.First().auxFec) ? "S" : "N",
                   FecDt = grouped.Key,
                   FecSt = grouped.First().auxFec,
                   DiaNb = grouped.Key.ToString("dd"),
                   MesLt = helper.DiaMes(grouped.First().auxFec.Substring(4, 2), "M")
               }
           ).OrderBy(o => o.FecDt).ToList();
                 
                int fila = 0;
                int columna = 0;

                // Lógica para obtener las fechas de la película desde el XML
                List<DateCartelera> fechasPelicula = ob_fechas;

                foreach (var fecha in fechasPelicula)
                {
                    // Crear el Border
                    Border nuevoBorde = new Border
                    {
                        Margin = new Thickness(20, 30, 10, 0),
                        Height = 50,
                        Width = 150,
                        Background = new SolidColorBrush(Color.FromRgb(229, 229, 229)), // Color de fondo #E5E5E5
                        CornerRadius = new CornerRadius(10),
                    };

                    // Crear el Label dentro del Border
                    Label nuevoLabel = new Label
                    {
                        Content = fecha.DiaLt + ", " + fecha.DiaNb, // Contenido del Label
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        FontFamily = new FontFamily("Poppins Regular"), // Fuente
                        FontSize = 20, // Tamaño de la fuente
                    };

                    // Agregar el Label al Border
                    nuevoBorde.Child = nuevoLabel;

                    // Agregar el Border al contenedor ContenedorHorarios
                    Grid.SetRow(nuevoBorde, fila);
                    Grid.SetColumn(nuevoBorde, columna);
                    ContenedorHorarios.Children.Add(nuevoBorde);

                    // Actualizar fila y columna para la siguiente iteración
                    columna++;
                    if (columna >= 4)
                    {
                        columna = 0;
                        fila++;
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al cargar películas desde el archivo XML: " + ex.Message);
            }
        }


    }
}
