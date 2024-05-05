using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Portal.Kiosco.Properties.Views
{
    /// <summary>
    /// Lógica de interacción para Frame17.xaml
    /// </summary>
    public partial class CorreoTecladoFlotante : Window
    {
        public CorreoTecladoFlotante()
        {
            InitializeComponent();
            DataContext = ((App)Application.Current);
            DoubleAnimation fadeInAnimation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.5));
            gridPrincipal.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);
        }

        private async void btnObtenerDatos_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(TextCorreoElectronico.Text))
                {
                    MessageBox.Show("!Ups, aún falta ingresar el correo electrónico!", "Notificación", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                string email = TextCorreoElectronico.Text;
                if (!IsValidEmail(email))
                {
                    MessageBox.Show("El correo electrónico ingresado no es válido.", "Notificación", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool IsValidEmail(string email)
        {
            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            Regex regex = new Regex(pattern);
            return regex.IsMatch(email);
        }


        private void KeyButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button != null)
            {
                string character = button.Content.ToString();

                if (character == "Del")
                {
                    if (TextCorreoElectronico.Text.Length > 0)
                    {
                        TextCorreoElectronico.Text = TextCorreoElectronico.Text.Substring(0, TextCorreoElectronico.Text.Length - 1);
                    }
                }
                else
                {
                    TextCorreoElectronico.Text += character;
                }
            }
        }


        private void btnVolver_Click(object sender, RoutedEventArgs e)
        {
            BoletasGafasAlimentos w = new BoletasGafasAlimentos();
            this.Close();
            w.ShowDialog();
        }
    }
}