using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// Lógica de interacción para ConsultaCineFans.xaml
    /// </summary>
    public partial class ConsultaCineFans : Window
    {
        public ConsultaCineFans()
        {
            InitializeComponent();
        }


        private void Image_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
    }
}
