using Domain;
using Interface;
using System;
using System.Text.RegularExpressions;
using System.Windows;

namespace UI
{
    public partial class RegistrationWindow : Window
    {
        private readonly IUserRepository _userRepository;

        public RegistrationWindow(IUserRepository userRepository)
        {
            InitializeComponent();
            _userRepository = userRepository;

            FirstNameTextBox.Text = "";
            LastNameTextBox.Text = "";
            EmailTextBox.Text = "";
            PhoneTextBox.Text = "";
            PasswordBox.Password = "";
            ConfirmPasswordBox.Password = "";
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
                return regex.IsMatch(email);
            }
            catch
            {
                return false;
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(FirstNameTextBox.Text))
            {
                MessageBox.Show("Введите имя");
                FirstNameTextBox.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(LastNameTextBox.Text))
            {
                MessageBox.Show("Введите фамилию");
                LastNameTextBox.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(EmailTextBox.Text) || !IsValidEmail(EmailTextBox.Text))
            {
                MessageBox.Show("Введите корректный email");
                EmailTextBox.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(PasswordBox.Password) || PasswordBox.Password.Length < 6)
            {
                MessageBox.Show("Пароль должен содержать минимум 6 символов");
                PasswordBox.Focus();
                return;
            }
            if (PasswordBox.Password != ConfirmPasswordBox.Password)
            {
                MessageBox.Show("Пароли не совпадают");
                PasswordBox.Focus();
                return;
            }

            var existingUser = _userRepository.GetByEmail(EmailTextBox.Text);
            if (existingUser != null)
            {
                MessageBox.Show("Пользователь с таким email уже существует");
                EmailTextBox.Focus();
                return;
            }

            try
            {
                var user = new User
                {
                    FirstName = FirstNameTextBox.Text.Trim(),
                    LastName = LastNameTextBox.Text.Trim(),
                    Email = EmailTextBox.Text.Trim(),
                    Phone = PhoneTextBox.Text.Trim(),
                    HashPassword = PasswordBox.Password,
                    SubActivation = false,
                    RegistrationDate = DateTime.Now
                };

                _userRepository.Add(user);
                MessageBox.Show("Регистрация успешна! Теперь войдите в систему.");
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка регистрации: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}