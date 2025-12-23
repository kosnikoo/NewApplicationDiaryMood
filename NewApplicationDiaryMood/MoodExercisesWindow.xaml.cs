using Domain.Enum;
using Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace UI
{
    public partial class MoodExercisesWindow : Window
    {
        private readonly IExerciseRepository _exerciseRepository;
        private readonly int _userId;
        private readonly bool _hasSubscription;

        public MoodExercisesWindow(IExerciseRepository exerciseRepository, int userId, bool hasSubscription)
        {
            InitializeComponent();
            _exerciseRepository = exerciseRepository;
            _userId = userId;
            _hasSubscription = hasSubscription;

            LoadMoodTypes();
        }

        private void LoadMoodTypes()
        {
            var moodTypes = new List<MoodTypeInfo>
            {
                new MoodTypeInfo { Type = MoodType.Happy, Display = "😊 Счастливый", Description = "Авторские упражнения для поддержания позитивного настроения" },
                new MoodTypeInfo { Type = MoodType.Sad, Display = "😢 Грустный", Description = "Авторские упражнения для поднятия настроения" },
                new MoodTypeInfo { Type = MoodType.Angry, Display = "😠 Злой", Description = "Авторские упражнения для управления гневом" },
                new MoodTypeInfo { Type = MoodType.Tired, Display = "😴 Уставший", Description = "Авторские упражнения для восстановления энергии" },
                new MoodTypeInfo { Type = MoodType.Anxious, Display = "😰 Тревожный", Description = "Авторские упражнения для снятия тревоги" },
                new MoodTypeInfo { Type = MoodType.Peaceful, Display = "😌 Спокойный", Description = "Авторские упражнения для поддержания спокойствия" },
                new MoodTypeInfo { Type = MoodType.Stressed, Display = "😫 Стрессовый", Description = "Авторские упражнения для снятия стресса" },
                new MoodTypeInfo { Type = MoodType.Depressed, Display = "😞 Подавленный", Description = "Авторские упражнения для выхода из депрессивного состояния" }
            };

            MoodsDataGrid.ItemsSource = moodTypes;
        }

        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            if (MoodsDataGrid.SelectedItem is MoodTypeInfo selected)
            {
                if (!_hasSubscription)
                {
                    MessageBox.Show("Для доступа к авторским упражнениям требуется подписка.",
                        "Подписка", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                var exercisesWindow = new AuthorExercisesWindow(_exerciseRepository, selected.Type);
                exercisesWindow.Owner = this;
                exercisesWindow.ShowDialog();
            }
        }

        private class MoodTypeInfo
        {
            public MoodType Type { get; set; }
            public string Display { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
