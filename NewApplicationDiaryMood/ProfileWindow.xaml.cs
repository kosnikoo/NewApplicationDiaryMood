using Domain;
using Interface;
using System;
using System.Text.RegularExpressions;
using System.Windows;

namespace UI
{
    public partial class ProfileWindow : Window
    {
        private readonly IUserRepository _userRepository;
        private readonly int _userId;
        private User _user;

        public ProfileWindow(IUserRepository userRepository, int userId)
        {
            InitializeComponent();
            _userRepository = userRepository;
            _userId = userId;

            LoadUserData();
        }

        private void LoadUserData()
        {
            _user = _userRepository.GetById(_userId);
            if (_user != null)
            {
                FirstNameTextBox.Text = _user.FirstName;
                LastNameTextBox.Text = _user.LastName;
                EmailTextBox.Text = _user.Email;
                PhoneTextBox.Text = _user.Phone;

                RegistrationDateTextBlock.Text = _user.RegistrationDate.ToString("dd.MM.yyyy HH:mm");

                if (_user.SubActivation)
                {
                    SubscriptionStatusTextBlock.Text = "⭐ Подписка активна";
                    SubscriptionStatusTextBlock.Foreground = System.Windows.Media.Brushes.Green;
                }
                else
                {
                    SubscriptionStatusTextBlock.Text = "❌ Подписка не активна";
                    SubscriptionStatusTextBlock.Foreground = System.Windows.Media.Brushes.Red;
                }

                UserIdTextBlock.Text = _user.Id.ToString();
            }
        }

        private void ChangePasswordButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(CurrentPasswordBox.Password))
                {
                    MessageBox.Show("Введите текущий пароль", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    CurrentPasswordBox.Focus();
                    return;
                }

                if (CurrentPasswordBox.Password != _user.HashPassword)
                {
                    MessageBox.Show("Текущий пароль неверен", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    CurrentPasswordBox.Clear();
                    CurrentPasswordBox.Focus();
                    return;
                }
                if (string.IsNullOrWhiteSpace(NewPasswordBox.Password))
                {
                    MessageBox.Show("Введите новый пароль", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    NewPasswordBox.Focus();
                    return;
                }

                if (NewPasswordBox.Password.Length < 6)
                {
                    MessageBox.Show("Новый пароль должен содержать минимум 6 символов", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    NewPasswordBox.Focus();
                    return;
                }
                if (NewPasswordBox.Password != ConfirmNewPasswordBox.Password)
                {
                    MessageBox.Show("Новый пароль и подтверждение не совпадают", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    ConfirmNewPasswordBox.Clear();
                    ConfirmNewPasswordBox.Focus();
                    return;
                }
                if (NewPasswordBox.Password == CurrentPasswordBox.Password)
                {
                    MessageBox.Show("Новый пароль должен отличаться от текущего", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    NewPasswordBox.Focus();
                    return;
                }

                var oldPassword = _user.HashPassword;
                _user.HashPassword = NewPasswordBox.Password;

                if (_userRepository.Update(_user))
                {
                    MessageBox.Show("Пароль успешно изменен", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);

                    CurrentPasswordBox.Clear();
                    NewPasswordBox.Clear();
                    ConfirmNewPasswordBox.Clear();
                }
                else
                {
                    _user.HashPassword = oldPassword;
                    MessageBox.Show("Не удалось изменить пароль", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при смене пароля: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}