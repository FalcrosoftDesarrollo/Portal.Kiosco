using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Portal.Kiosco.Properties.Views
{
    public partial class transicion : Window
    {
        public     transicion()
        {
            InitializeComponent(); 

        }


        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DoubleAnimation fadeInAnimation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(2));
            gridPrincipal.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);
            await ShowProcessingAsync();
        }

        private void CreateSpinnerAnimation()
        {
            DoubleAnimation rotateAnimation = new DoubleAnimation
            {
                From = 0,
                To = 360,
                Duration = new Duration(TimeSpan.FromSeconds(1)),
                RepeatBehavior = RepeatBehavior.Forever
            };

            RotateTransform rotateTransform = new RotateTransform();
            SpinnerCanvas.RenderTransform = rotateTransform;
            SpinnerCanvas.RenderTransformOrigin = new Point(0.5, 0.5);
            rotateTransform.BeginAnimation(RotateTransform.AngleProperty, rotateAnimation);
        }

        private async Task ShowProcessingAsync()
        {
            SpinnerCanvas.Visibility = Visibility.Visible;
            DotsTextBlock.Visibility = Visibility.Visible;
            var dotsStoryboard = (Storyboard)FindResource("DotsAnimation");
            CreateSpinnerAnimation();
            dotsStoryboard.Begin();

            // Simular una operación asíncrona, si es necesario
            await Task.Delay(100);
        }

        private async Task HideProcessingAsync()
        {
            SpinnerCanvas.Visibility = Visibility.Collapsed;
            DotsTextBlock.Visibility = Visibility.Collapsed;
            var dotsStoryboard = (Storyboard)FindResource("DotsAnimation");
            dotsStoryboard.Stop();

            // Simular una operación asíncrona, si es necesario
            await Task.Delay(100);
        }



    }
}
