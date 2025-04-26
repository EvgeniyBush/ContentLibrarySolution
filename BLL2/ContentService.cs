#pragma warning disable 

using AutoMapper;
using BLL2.DTO;
using DAL2.Entities;
using DAL2.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BLL2.Services
{
    public class ContentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ContentService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task AddContentAsync(ContentDto contentDto, int storageId)
        {
            if (contentDto == null)
                throw new ArgumentNullException(nameof(contentDto));

            var storage = await _unitOfWork.Storages.GetByIdAsync(storageId)
                ?? throw new InvalidOperationException("Сховище не знайдено.");

            Content content = contentDto switch
            {
                BookDto bookDto => _mapper.Map<Book>(bookDto),
                AudioDto audioDto => _mapper.Map<Audio>(audioDto),
                VideoDto videoDto => _mapper.Map<Video>(videoDto),
                DocumentDto documentDto => _mapper.Map<Document>(documentDto),
                _ => throw new InvalidOperationException("Невідомий тип контенту")
            };

            await _unitOfWork.Contents.AddAsync(content);
            await _unitOfWork.CompleteAsync();

            var location = new ContentLocation
            {
                ContentId = content.Id,
                StorageId = storage.Id
            };

            await _unitOfWork.ContentLocations.AddAsync(location);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<List<ContentDto>> GetAllAsync()
        {
            var contents = await _unitOfWork.Contents.GetAllAsync();
            var contentDtos = contents.Select(content => content switch
            {
                Book book => (ContentDto)_mapper.Map<BookDto>(book),
                Audio audio => (ContentDto)_mapper.Map<AudioDto>(audio),
                Video video => (ContentDto)_mapper.Map<VideoDto>(video),
                Document document => (ContentDto)_mapper.Map<DocumentDto>(document),
                _ => throw new InvalidOperationException("Невідомий тип контенту")
            }).ToList();

            await AttachStorageInfoAsync(contentDtos);
            return contentDtos;
        }

        public async Task<ContentDto?> GetByIdAsync(int id)
        {
            var content = await _unitOfWork.Contents.GetByIdAsync(id);
            if (content == null) return null;

            ContentDto contentDto = content switch
            {
                Book book => (ContentDto)_mapper.Map<BookDto>(book),
                Audio audio => (ContentDto)_mapper.Map<AudioDto>(audio),
                Video video => (ContentDto)_mapper.Map<VideoDto>(video),
                Document document => (ContentDto)_mapper.Map<DocumentDto>(document),
                _ => throw new InvalidOperationException("Невідомий тип контенту")
            };

            await AttachStorageInfoAsync(contentDto);
            return contentDto;
        }

        public async Task<List<ContentDto>> SearchByTitleAsync(string title)
        {
            var contents = await _unitOfWork.Contents.FindAsync(c =>
                EF.Functions.Like(c.Title, $"%{title}%"));

            var contentDtos = contents.Select(content => content switch
            {
                Book book => (ContentDto)_mapper.Map<BookDto>(book),
                Audio audio => (ContentDto)_mapper.Map<AudioDto>(audio),
                Video video => (ContentDto)_mapper.Map<VideoDto>(video),
                Document document => (ContentDto)_mapper.Map<DocumentDto>(document),
                _ => throw new InvalidOperationException("Невідомий тип контенту")
            }).ToList();

            await AttachStorageInfoAsync(contentDtos);
            return contentDtos;
        }

        public async Task DeleteAsync(int id)
        {
            var content = await _unitOfWork.Contents.GetByIdAsync(id);
            if (content == null) return;

            _unitOfWork.Contents.Remove(content);
            await _unitOfWork.CompleteAsync();
        }

        public async Task UpdateTitleAsync(int id, string newTitle)
        {
            var content = await _unitOfWork.Contents.GetByIdAsync(id);
            if (content == null) return;

            content.Title = newTitle;
            await _unitOfWork.CompleteAsync();
        }

        public async Task<List<StorageDto>> GetStoragesAsync()
        {
            var storages = await _unitOfWork.Storages.GetAllAsync();
            return _mapper.Map<List<StorageDto>>(storages);
        }

        public async Task AddStorageAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Назва сховища не може бути порожньою.", nameof(name));

            var storage = new Storage { LocationName = name };
            await _unitOfWork.Storages.AddAsync(storage);
            await _unitOfWork.CompleteAsync();
        }

        private async Task AttachStorageInfoAsync(ContentDto contentDto)
        {
            if (contentDto == null) return;

            var location = (await _unitOfWork.ContentLocations
                .FindAsync(cl => cl.ContentId == contentDto.Id))
                .FirstOrDefault();

            if (location != null)
            {
                var storage = await _unitOfWork.Storages.GetByIdAsync(location.StorageId);
                contentDto.Storage = _mapper.Map<StorageDto>(storage);
            }
        }

        private async Task AttachStorageInfoAsync(List<ContentDto> contentDtos)
        {
            foreach (var contentDto in contentDtos)
            {
                await AttachStorageInfoAsync(contentDto);
            }
        }
    }
}
