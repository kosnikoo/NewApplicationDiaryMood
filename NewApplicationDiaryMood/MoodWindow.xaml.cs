using Domain;
using Domain.Enum;
using Interface;
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

namespace UI
{
    public partial class MoodWindow : Window
    {
        private readonly IMoodRepository _moodRepository;
        private readonly int _userId;

        public MoodWindow(IMoodRepository moodRepository, int userId)
        {
            InitializeComponent();
            _moodRepository = moodRepository;
            _userId = userId;

            InitializeControls();
        }

        private void InitializeControls()
        {
            MoodTypeComboBox.ItemsSource = Enum.GetValues(typeof(MoodType))
                .Cast<MoodType>()
                .Where(m => m != MoodType.None)
                .Select(m => new
                {
                    Value = m,
                    Display = GetMoodDisplayName(m)
                });

            MoodTypeComboBox.DisplayMemberPath = "Display";
            MoodTypeComboBox.SelectedValuePath = "Value";
            MoodTypeComboBox.SelectedIndex = 0;

            DatePicker.SelectedDate = DateTime.Now;

            IntensitySlider.ValueChanged += IntensitySlider_ValueChanged;
            UpdateIntensityText();
        }

        private string GetMoodDisplayName(MoodType moodType)
        {
            return moodType switch
            {
                MoodType.Happy => "😊 Счастливый",
                MoodType.Sad => "😢 Грустный",
                MoodType.Angry => "😠 Злой",
                MoodType.Tired => "😴 Уставший",
                MoodType.Anxious => "😰 Тревожный",
                MoodType.Peaceful => "😌 Спокойный",
                MoodType.Stressed => "😫 Стрессовый",
                MoodType.Depressed => "😞 Подавленный",
                _ => moodType.ToString()
            };
        }

        private void IntensitySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            UpdateIntensityText();
        }

        private void UpdateIntensityText()
        {
            IntensityValueText.Text = $"{(int)IntensitySlider.Value}/10";
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MoodTypeComboBox.SelectedItem == null)
                {
                    MessageBox.Show("Выберите тип настроения", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (DatePicker.SelectedDate == null)
                {
                    MessageBox.Show("Выберите дату", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var mood = new Mood
                {
                    UserId = _userId,
                    MoodType = (MoodType)MoodTypeComboBox.SelectedValue,
                    MoodQuantity = (int)IntensitySlider.Value,
                    EntryDate = DatePicker.SelectedDate.Value
                };

                _moodRepository.Add(mood);
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения настроения: {ex.Message}", "Ошибка",
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