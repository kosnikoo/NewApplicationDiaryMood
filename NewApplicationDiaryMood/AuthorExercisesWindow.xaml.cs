using Domain.Enum;
using Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace UI
{
    public partial class AuthorExercisesWindow : Window
    {
        private readonly IExerciseRepository _exerciseRepository;
        private readonly MoodType _selectedMoodType;

        public AuthorExercisesWindow(IExerciseRepository exerciseRepository, MoodType moodType)
        {
            InitializeComponent();
            _exerciseRepository = exerciseRepository;
            _selectedMoodType = moodType;

            LoadExercises();
            UpdateTitle();
        }

        private void LoadExercises()
        {
            try
            {
                // Получаем все упражнения для данного типа настроения
                var allExercises = _exerciseRepository.GetByMoodType(_selectedMoodType);

                if (allExercises.Any())
                {
                    ExercisesListBox.ItemsSource = allExercises;
                }
                else
                {
                    ExercisesListBox.ItemsSource = null;
                    ExerciseTitleTextBlock.Text = "Нет упражнений";
                    ExerciseDescriptionTextBlock.Text = "Для этого типа настроения пока не добавлены упражнения.";
                    InstructionsTextBlock.Text = "";
                    DurationTextBlock.Text = "";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки упражнений: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateTitle()
        {
            Title = $"Авторские упражнения для настроения: {_selectedMoodType}";
        }

        private void ExercisesListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (ExercisesListBox.SelectedItem is Domain.Exercise exercise)
            {
                ExerciseTitleTextBlock.Text = $"🔒 {exercise.Title}"; // Добавляем значок замка
                ExerciseDescriptionTextBlock.Text = exercise.Description;
                InstructionsTextBlock.Text = $"Инструкции:\n{exercise.Instructions}";
                DurationTextBlock.Text = $"Длительность: {exercise.DurationMinutes} минут";
            }
        }
    }
}
