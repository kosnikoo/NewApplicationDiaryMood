using Domain;
using System;
using System.Windows;

namespace UI
{
    public partial class NotificationWindow : Window
    {
        private readonly Notification _notification;

        public NotificationWindow(Notification notification)
        {
            InitializeComponent();
            _notification = notification;
            LoadNotification();
        }

        private void LoadNotification()
        {
            TitleTextBlock.Text = _notification.NotTitle;
            DescriptionTextBlock.Text = _notification.NotDescription;
            RecommendationTextBlock.Text = _notification.NotRecommendation;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
