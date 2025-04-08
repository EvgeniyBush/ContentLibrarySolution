using DAL2;
using DAL2.Entities;
using Microsoft.EntityFrameworkCore;

namespace BLL2.Services
{
    public class ContentService
    {
        private readonly AppDbContext _context;

        public ContentService(AppDbContext context)
        {
            _context = context;
        }

        // Додавання контенту
        public void AddContent(Content content, int storageId)
        {
            var storage = _context.Storages.FirstOrDefault(s => s.Id == storageId);
            if (storage == null) throw new Exception("Сховище не знайдено");

            _context.Contents.Add(content);
            _context.SaveChanges(); // Зберігаємо, щоб згенерувався ID

            var location = new ContentLocation
            {
                ContentId = content.Id,
                StorageId = storage.Id
            };

            _context.ContentLocations.Add(location);
            _context.SaveChanges();
        }


        // Отримати всі
        public List<Content> GetAll()
        {
            return _context.Contents.Include(c => c.Location).ThenInclude(l => l.Storage).ToList();
        }

        // Пошук по назві
        public List<Content> SearchByTitle(string title)
        {
            return _context.Contents
                .Where(c => EF.Functions.Like(c.Title, $"%{title}%"))
                .Include(c => c.Location)
                .ThenInclude(l => l.Storage)
                .ToList();
        }

        // Видалення
        public void Delete(int id)
        {
            var content = _context.Contents.Find(id);
            if (content == null) return;

            _context.Contents.Remove(content);
            _context.SaveChanges();
        }

        // Отримання за ID
        public Content? GetById(int id)
        {
            return _context.Contents
                .Include(c => c.Location)
                .ThenInclude(l => l.Storage)
                .FirstOrDefault(c => c.Id == id);
        }

        // Оновлення (наприклад, зміна назви)
        public void UpdateTitle(int id, string newTitle)
        {
            var content = _context.Contents.Find(id);
            if (content == null) return;

            content.Title = newTitle;
            _context.SaveChanges();
        }

        // Отримання усіх сховищ
        public List<Storage> GetStorages()
        {
            return _context.Storages.Include(s => s.ContentLocations).ToList();
        }

        // Додавання сховища
        public void AddStorage(string name)
        {
            _context.Storages.Add(new Storage { LocationName = name });
            _context.SaveChanges();
        }
    }
}
