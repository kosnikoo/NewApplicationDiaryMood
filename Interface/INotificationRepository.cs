using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface
{
    public interface INotificationRepository
    {
        List<Notification> GetByUserId(int userId);
        Notification GetById(int id);
        int Add(Notification notification);
        bool Update(Notification notification);
        bool Delete(int id);
        void GenerateRecommendationNotifications(int userId, IMoodRepository moodRepository, ICourseRepository courseRepository);
    }
}
