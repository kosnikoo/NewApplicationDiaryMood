using Domain;
using Domain.Enum;
using System;
using System.Collections.Generic;

namespace Interface
{
    public interface IMoodRepository
    {
        int Add(Mood mood);
        Mood? GetById(int id);
        List<Mood> GetAll(MoodFilter filter);
        List<Mood> GetByUserId(int userId, MoodFilter filter);
        bool Update(Mood mood);
        bool Delete(int id);
        Dictionary<MoodType, int> GetMoodStatistics(int userId, MoodFilter filter);

        // Новые методы
        Mood? GetByUserIdAndDate(int userId, DateTime date);
        Mood? GetLastMood(int userId);
        int GetMoodCountLastDays(int userId, int days);
        Dictionary<DayOfWeek, int> GetMoodStatisticsByDayOfWeek(int userId, MoodFilter filter);
        double GetAverageMoodIntensity(int userId, MoodFilter filter);
        (MoodType Mood, int Count)? GetMostFrequentMood(int userId, MoodFilter filter);
        List<(int Year, int Month, int Count)> GetMonthlyStatistics(int userId, MoodFilter filter);
        bool HasMoodToday(int userId);
        int GetCurrentStreak(int userId);
        int GetBestStreak(int userId);
        List<DateTime> GetDaysWithGoodMood(int userId, int threshold = 7, MoodFilter filter = null);
        bool DeleteAllUserMoods(int userId);
        List<Mood> ExportUserMoods(int userId);
        bool HasSameMoodTypeOnDate(int userId, MoodType moodType, DateTime date);
    }
}
