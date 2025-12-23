using Domain.Enum;
using Interface;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// <summary>
    /// Логика взаимодействия для CoursesWindow.xaml
    /// </summary>
    public partial class CoursesWindow : Window
    {
        private readonly ICourseRepository _courseRepository;

        public CoursesWindow(ICourseRepository courseRepository)
        {
            InitializeComponent();
            _courseRepository = courseRepository;

            InitializeCategories();
            LoadAllCourses();
        }

        private void InitializeCategories()
        {
            CategoryComboBox.ItemsSource = System.Enum.GetValues(typeof(MoodType))
                .Cast<MoodType>()
                .Where(m => m != MoodType.None)
                .Select(m => new
                {
                    Value = m,
                    Display = m.ToString()
                });

            CategoryComboBox.DisplayMemberPath = "Display";
            CategoryComboBox.SelectedValuePath = "Value";
            CategoryComboBox.SelectedIndex = 0;
        }

        private void LoadAllCourses()
        {
            try
            {
                var courses = _courseRepository.GetAll();
                CoursesDataGrid.ItemsSource = courses;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки курсов: {ex.Message}");
            }
        }

        private void CategoryComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (CategoryComboBox.SelectedValue is MoodType category)
            {
                try
                {
                    var courses = _courseRepository.GetByCategory(category);
                    CoursesDataGrid.ItemsSource = courses;
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show($"Ошибка фильтрации курсов: {ex.Message}");
                }
            }
        }

        private void ShowAllButton_Click(object sender, RoutedEventArgs e)
        {
            LoadAllCourses();
            CategoryComboBox.SelectedIndex = -1;
        }

        private void DetailsButton_Click(object sender, RoutedEventArgs e)
        {
            if (CoursesDataGrid.SelectedItem is Domain.Course course)
            {
                MessageBox.Show(
                    $"Название: {course.Title}\n\n" +
                    $"Категория: {course.Category}\n\n" +
                    $"Описание: {course.Description}\n\n" +
                    $"Ссылка: {course.Link}",
                    "Информация о курсе"
                );
            }
        }
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
}
