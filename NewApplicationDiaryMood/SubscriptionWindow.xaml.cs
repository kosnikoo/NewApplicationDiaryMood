using Domain;
using Interface;
using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace UI
{
    public partial class SubscriptionWindow : Window
    {
        private readonly IUserRepository _userRepository;
        private readonly int _userId;
        private bool _hasActiveSubscription = false;

        public SubscriptionWindow(IUserRepository userRepository, int userId)
        {
            InitializeComponent();
            _userRepository = userRepository;
            _userId = userId;

            Loaded += Window_Loaded;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CheckSubscriptionStatus();

            if (!_hasActiveSubscription)
            {
                CardNumberTextBox.TextChanged += CardNumberTextBox_TextChanged;
                ExpiryTextBox.TextChanged += ExpiryTextBox_TextChanged;

                AutoFillTestData();
            }
        }

        private void CheckSubscriptionStatus()
        {
            try
            {
                var user = _userRepository.GetById(_userId);
                if (user != null && user.SubActivation)
                {
                    _hasActiveSubscription = true;
                    ShowActiveSubscriptionView();
                }
                else
                {
                    _hasActiveSubscription = false;
                    ShowPaymentView();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка проверки статуса подписки: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ShowActiveSubscriptionView()
        {
            Title = "Информация о подписке";
            TitleTextBlock.Text = "⭐ Премиум Подписка (активна)";
            DescriptionTextBlock.Text = "Вам доступны все функции приложения";
            CardInputPanel.Visibility = Visibility.Collapsed;
            ActiveSubscriptionPanel.Visibility = Visibility.Visible;
            ConfirmPaymentButton.Content = "Подписка активна ✓";
            ConfirmPaymentButton.Background = System.Windows.Media.Brushes.Gray;
            ConfirmPaymentButton.IsEnabled = false;
            CancelButton.Content = "Закрыть";
        }

        private void ShowPaymentView()
        {
            Title = "Оформление подписки";
            TitleTextBlock.Text = "⭐ Премиум Подписка";
            DescriptionTextBlock.Text = "Получите доступ ко всем функциям:";
            CardInputPanel.Visibility = Visibility.Visible;
            ActiveSubscriptionPanel.Visibility = Visibility.Collapsed;
            ConfirmPaymentButton.Content = "Оплатить 299₽";
            ConfirmPaymentButton.Background = System.Windows.Media.Brushes.Green;
            ConfirmPaymentButton.IsEnabled = true;
            CancelButton.Content = "Отмена";
        }

        private void AutoFillTestData()
        {
            CardNumberTextBox.Text = "4242 4242 4242 4242";
            ExpiryTextBox.Text = "12/25";
            CVCTextBox.Text = "123";
        }


        private void CardNumberTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string text = CardNumberTextBox.Text;
            string digitsOnly = Regex.Replace(text, @"[^\d]", "");

            if (digitsOnly.Length > 16)
                digitsOnly = digitsOnly.Substring(0, 16);

            string formatted = "";
            for (int i = 0; i < digitsOnly.Length; i++)
            {
                if (i > 0 && i % 4 == 0)
                    formatted += " ";
                formatted += digitsOnly[i];
            }

            if (text != formatted)
            {
                CardNumberTextBox.Text = formatted;
                CardNumberTextBox.CaretIndex = formatted.Length;
            }
        }

        private void ExpiryTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string text = ExpiryTextBox.Text;
            string digitsOnly = Regex.Replace(text, @"[^\d]", "");

            if (digitsOnly.Length > 4)
                digitsOnly = digitsOnly.Substring(0, 4);

            if (digitsOnly.Length >= 2)
            {
                string month = digitsOnly.Substring(0, 2);
                string year = digitsOnly.Length > 2 ? digitsOnly.Substring(2) : "";
                string formatted = $"{month}{(year.Length > 0 ? "/" + year : "")}";

                if (text != formatted)
                {
                    ExpiryTextBox.Text = formatted;
                    ExpiryTextBox.CaretIndex = formatted.Length;
                }
            }
        }

        private void ConfirmPaymentButton_Click(object sender, RoutedEventArgs e)
        {
            string cardDigits = Regex.Replace(CardNumberTextBox.Text, @"[^\d]", "");
            if (cardDigits.Length != 16)
            {
                MessageBox.Show("Ошибка: номер карты должен содержать 16 цифр",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                CardNumberTextBox.Focus();
                return;
            }

            string expiry = ExpiryTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(expiry))
            {
                MessageBox.Show("Ошибка: введите срок действия карты",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                ExpiryTextBox.Focus();
                return;
            }

            if (!Regex.IsMatch(expiry, @"^\d{2}/\d{2}$"))
            {
                MessageBox.Show("Ошибка: введите срок действия в формате ММ/ГГ (например: 12/25)",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                ExpiryTextBox.Focus();
                return;
            }

            string cvc = CVCTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(cvc) || !Regex.IsMatch(cvc, @"^\d{3,4}$"))
            {
                MessageBox.Show("Ошибка: CVC код должен содержать 3 или 4 цифры",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                CVCTextBox.Focus();
                return;
            }

            try
            {
                var user = _userRepository.GetById(_userId);
                if (user != null)
                {
                    user.SubActivation = true;

                    if (_userRepository.Update(user))
                    {
                        MessageBox.Show("✅ Подписка успешно активирована!",
                            "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        DialogResult = true;
                        Close();
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при обновлении данных",
                            "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Пользователь не найден",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            if (CardNumberTextBox != null)
                CardNumberTextBox.TextChanged -= CardNumberTextBox_TextChanged;

            if (ExpiryTextBox != null)
                ExpiryTextBox.TextChanged -= ExpiryTextBox_TextChanged;

            base.OnClosed(e);
        }

    }
}