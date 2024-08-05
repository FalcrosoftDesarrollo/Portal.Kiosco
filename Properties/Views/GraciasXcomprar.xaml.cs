using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Portal.Kiosco.Properties.Views
{
    /// <summary>
    /// Interaction logic for GraciasXcomprar.xaml
    /// </summary>
    public partial class GraciasXcomprar : Window
    {
        private DispatcherTimer timer;

        public GraciasXcomprar()
        {
            InitializeComponent();
            InitializeTimer();
        }

        private void InitializeTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(5);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();
            ReturnToMainWindow();
        }

        private void ReturnToMainWindow()
        {
            var mainWindow = new Principal();  
            mainWindow.Show();
            this.Close();
        }

        private void btnSalir_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            ReturnToMainWindow();
        }

        public async Task LoadDataAsync()
        {
            await Task.Run(() =>
            {
                System.Threading.Thread.Sleep(3000);
            });
        }
    }
}