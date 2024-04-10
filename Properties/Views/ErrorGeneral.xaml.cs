using System.Windows;



namespace Portal.Kiosco.Properties.Views
{
    /// <summary>
    /// Interaction logic for ErrorGeneral.xaml
    /// </summary>
    public partial class ErrorGeneral : Window
    {
        public ErrorGeneral()
        {
            InitializeComponent();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
