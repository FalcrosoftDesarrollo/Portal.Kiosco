using System.Windows;
using System.Windows.Input;

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