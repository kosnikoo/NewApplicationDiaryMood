using Domain;
using Domain.Enum;
using Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlServer
{
    public class MoodRepository : IMoodRepository
    {
        private readonly AppDbContext _context;
        // Конструктор принимает DbContext через dependency injection
        public MoodRepository(AppDbContext context)
        {
            _context = context;
        }
        public int Add(Mood entity)
        {
            // Add() добавляет сущность в отслеживание EF Core
            _context.Mood.Add(entity);
            // SaveChanges() генерирует и выполняет SQL INSERT
            // EF Core автоматически присваивает Id после сохранения
            _context.SaveChanges();
            return entity.ID;
        }
        public Mood? GetById(int id)
        {
            // Find() ищет сущность по первичному ключу
            // Сначала проверяет кэш, затем выполняет SELECT
            // Возвращает null, если не найдена
            return _context.Mood.Find(id);
        }
        public List<Mood> GetAll(MoodFilter filter)
        {
            // AsQueryable() создаёт LINQ-запрос (ещё не выполнен)
            var query = _context.Mood.AsQueryable();
            // ВАЖНО: Если сущность имеет навигационные свойства (связи),
            // используйте Include() для их загрузки:
            // var query = _context.<Сущности>
            // .Include(x => x.Master) // Загружаем связанную сущность
            // .AsQueryable();
            // Добавляем фильтры — формируется SQL WHERE
            if (filter.StartDate.HasValue)
                query = query.Where(x => x.Date >= filter.StartDate.Value);
            if (filter.EndDate.HasValue)
                query = query.Where(x => x.Date <= filter.EndDate.Value);
            // ToList() выполняет запрос к БД и возвращает результат
            return query.ToList();
        }
        public bool Update(Mood entity)
        {
            // Сначала находим существующую сущность
            var existing = _context.Mood.Find(entity.ID);
            if (existing == null)
                return false;
            // Копируем все свойства (кроме Id) из entity в existing
            existing.CopyFrom(entity);
            // SaveChanges() генерирует SQL UPDATE только для изменённых полей
            _context.SaveChanges();
            return true;
        }
        public bool Delete(int id)
        {
            var entity = _context.Mood.Find(id);
            if (entity == null)
                return false;
            // Remove() помечает сущность для удаления
            _context.Mood.Remove(entity);
            // SaveChanges() генерирует и выполняет SQL DELETE
            _context.SaveChanges();
            return true;
        }
    }
}
