#pragma warning disable 

using DAL2.Entities;
using DAL2.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BLL2.Services
{
    public class ContentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ContentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // Додавання контенту
        public async Task AddContentAsync(Content content, int storageId)
        {
            var storage = await _unitOfWork.Storages.GetByIdAsync(storageId);
            if (storage == null)
                throw new Exception("Сховище не знайдено");

            await _unitOfWork.Contents.AddAsync(content);
            await _unitOfWork.CompleteAsync(); // Зберігаємо, щоб згенерувався ID

            var location = new ContentLocation
            {
                ContentId = content.Id,
                StorageId = storage.Id
            };

            await _unitOfWork.ContentLocations.AddAsync(location);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<List<Content>> GetAllAsync()
        {
            var contents = await _unitOfWork.Contents.GetAllAsync();
            return contents
               .Select(c =>
               {
                   var location = _unitOfWork
                       .ContentLocations
                       .FindAsync(cl => cl.ContentId == c.Id)
                       .Result
                       .FirstOrDefault();

                   if (location != null)
                   {
                       location.Storage = _unitOfWork.Storages
                           .GetByIdAsync(location.StorageId)
                           .Result;

                       c.Location = location;
                   }

                   return c;
               })


                .ToList();
        }

        public async Task<List<Content>> SearchByTitleAsync(string title)
        {
            var result = await _unitOfWork.Contents.FindAsync(c => EF.Functions.Like(c.Title, $"%{title}%"));
            return result.ToList();
        }

        public async Task DeleteAsync(int id)
        {
            var content = await _unitOfWork.Contents.GetByIdAsync(id);
            if (content == null) return;

            _unitOfWork.Contents.Remove(content);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<Content?> GetByIdAsync(int id)
        {
            return await _unitOfWork.Contents.GetByIdAsync(id);
        }

        public async Task UpdateTitleAsync(int id, string newTitle)
        {
            var content = await _unitOfWork.Contents.GetByIdAsync(id);
            if (content == null) return;

            content.Title = newTitle;
            await _unitOfWork.CompleteAsync();
        }

        public async Task<List<Storage>> GetStoragesAsync()
        {
            var storages = await _unitOfWork.Storages.GetAllAsync();
            return storages.ToList();
        }

        public async Task AddStorageAsync(string name)
        {
            var storage = new Storage { LocationName = name };
            await _unitOfWork.Storages.AddAsync(storage);
            await _unitOfWork.CompleteAsync();
        }
    }
}
