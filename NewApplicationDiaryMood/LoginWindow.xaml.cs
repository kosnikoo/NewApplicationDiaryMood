using Domain;
using Interface;
using System;
using System.Windows;
using System.Windows.Controls;
using UI;

namespace NewApplicationDiaryMood.UI
{
    public partial class LoginWindow : Window
    {
        private readonly IUserRepository _userRepository;
        private User _authenticatedUser;
        public event Action<User> LoginSuccessful;

        public LoginWindow(IUserRepository userRepository)
        {
            InitializeComponent();
            _userRepository = userRepository;

            EmailTextBox.Text = "test@example.com";
            PasswordBox.Password = "password";
            LoginButton.Click += LoginButton_Click;
            RegisterButton.Click += RegisterButton_Click;
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(EmailTextBox.Text) ||
                string.IsNullOrWhiteSpace(PasswordBox.Password))
            {
                MessageBox.Show("Введите email и пароль", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {

                if (_userRepository.Authenticate(EmailTextBox.Text, PasswordBox.Password))
                {
                    _authenticatedUser = _userRepository.GetByEmail(EmailTextBox.Text);

                    if (_authenticatedUser != null)
                    {
                        LoginSuccessful?.Invoke(_authenticatedUser);
                    }
                    else
                    {
                        MessageBox.Show("Ошибка получения данных пользователя", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Неверный email или пароль", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при входе: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            var registrationWindow = new RegistrationWindow(_userRepository);
            registrationWindow.Owner = this;
            registrationWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            if (registrationWindow.ShowDialog() == true)
            {
                MessageBox.Show("Регистрация успешна! Теперь войдите в систему.", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            Console.WriteLine("LoginWindow закрыт");
            base.OnClosed(e);
        }
    }
}