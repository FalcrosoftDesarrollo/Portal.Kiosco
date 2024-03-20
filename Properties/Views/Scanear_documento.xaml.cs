using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Portal.Kiosco
{
    /// <summary>
    /// Lógica de interacción para Scanear_documento.xaml
    /// </summary>
    public partial class Scanear_documento : Window
    {
        public Scanear_documento()
        {
            InitializeComponent();
        }
        private void Scanear_documento_Loaded(object sender, RoutedEventArgs e)
        {

        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
