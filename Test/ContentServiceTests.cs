#pragma warning disable 

using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using BLL2.DTO;
using BLL2.Services;
using DAL2.Entities;
using DAL2.Repositories;
using Xunit;

namespace BLL2.Tests
{
    public class ContentServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IMapper> _mockMapper;
        private readonly ContentService _contentService;

        public ContentServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _contentService = new ContentService(_mockUnitOfWork.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task AddContentAsync_ValidContent_ShouldAddContent()
        {
            // Arrange
            var contentDto = new BookDto { Title = "Test Book" };
            var storageId = 1;
            var storage = new Storage { Id = storageId, LocationName = "Storage1" };
            var book = new Book { Id = 1, Title = "Test Book" };

            _mockUnitOfWork.Setup(u => u.Storages.GetByIdAsync(storageId))
                .ReturnsAsync(storage);
            _mockMapper.Setup(m => m.Map<Book>(contentDto))
                .Returns(book);
            _mockUnitOfWork.Setup(u => u.Contents.AddAsync(It.IsAny<Book>()))
                .Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(u => u.ContentLocations.AddAsync(It.IsAny<ContentLocation>()))
                .Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(u => u.CompleteAsync())
                .ReturnsAsync(1);

            // Act
            await _contentService.AddContentAsync(contentDto, storageId);

            // Assert
            _mockUnitOfWork.Verify(u => u.Contents.AddAsync(It.IsAny<Book>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.ContentLocations.AddAsync(It.IsAny<ContentLocation>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.AtLeastOnce());
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllContents()
        {
            // Arrange
            var contents = new List<Content>
            {
                new Book { Id = 1, Title = "Book 1" },
                new Audio { Id = 2, Title = "Audio 1" }
            };

            _mockUnitOfWork.Setup(u => u.Contents.GetAllAsync())
                .ReturnsAsync(contents);
            _mockMapper.Setup(m => m.Map<BookDto>(It.IsAny<Book>()))
                .Returns((Book src) => new BookDto { Id = src.Id, Title = src.Title, Storage = null });
            _mockMapper.Setup(m => m.Map<AudioDto>(It.IsAny<Audio>()))
                .Returns((Audio src) => new AudioDto { Id = src.Id, Title = src.Title, Storage = null });

            // Mock storage info
            _mockUnitOfWork.Setup(u => u.ContentLocations.FindAsync(It.IsAny<Expression<Func<ContentLocation, bool>>>()))
                .ReturnsAsync(new List<ContentLocation>());
            _mockUnitOfWork.Setup(u => u.Storages.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Storage)null);

            // Act
            var result = await _contentService.GetAllAsync();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(result, c => c.Title == "Book 1");
            Assert.Contains(result, c => c.Title == "Audio 1");
        }

        [Fact]
        public async Task GetByIdAsync_ContentExists_ShouldReturnContent()
        {
            // Arrange
            var content = new Book { Id = 1, Title = "Test Book" };
            _mockUnitOfWork.Setup(u => u.Contents.GetByIdAsync(1))
                .ReturnsAsync(content);
            _mockMapper.Setup(m => m.Map<BookDto>(content))
                .Returns(new BookDto { Id = 1, Title = "Test Book", Storage = null });

            // Mock storage info
            _mockUnitOfWork.Setup(u => u.ContentLocations.FindAsync(It.IsAny<Expression<Func<ContentLocation, bool>>>()))
                .ReturnsAsync(new List<ContentLocation>());
            _mockUnitOfWork.Setup(u => u.Storages.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Storage)null);

            // Act
            var result = await _contentService.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test Book", result.Title);
        }

        [Fact]
        public async Task DeleteAsync_ExistingContent_ShouldDeleteContent()
        {
            // Arrange
            var content = new Book { Id = 1, Title = "Test Book" };
            _mockUnitOfWork.Setup(u => u.Contents.GetByIdAsync(1))
                .ReturnsAsync(content);
            _mockUnitOfWork.Setup(u => u.CompleteAsync())
                .ReturnsAsync(1);

            // Act
            await _contentService.DeleteAsync(1);

            // Assert
            _mockUnitOfWork.Verify(u => u.Contents.Remove(content), Times.Once);
            _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateTitleAsync_ValidContent_ShouldUpdateTitle()
        {
            // Arrange
            var content = new Book { Id = 1, Title = "Old Title" };
            var newTitle = "New Title";

            _mockUnitOfWork.Setup(u => u.Contents.GetByIdAsync(1))
                .ReturnsAsync(content);
            _mockUnitOfWork.Setup(u => u.CompleteAsync())
                .ReturnsAsync(1);

            // Act
            await _contentService.UpdateTitleAsync(1, newTitle);

            // Assert
            Assert.Equal(newTitle, content.Title);
            _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Once);
        }

        [Fact]
        public async Task GetStoragesAsync_ShouldReturnAllStorages()
        {
            // Arrange
            var storages = new List<Storage>
            {
                new Storage { Id = 1, LocationName = "Storage1" },
                new Storage { Id = 2, LocationName = "Storage2" }
            };

            _mockUnitOfWork.Setup(u => u.Storages.GetAllAsync())
                .ReturnsAsync(storages);
            _mockMapper.Setup(m => m.Map<List<StorageDto>>(storages))
                .Returns(new List<StorageDto>
                {
                    new StorageDto { Id = 1, LocationName = "Storage1" },
                    new StorageDto { Id = 2, LocationName = "Storage2" }
                });

            // Act
            var result = await _contentService.GetStoragesAsync();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(result, s => s.LocationName == "Storage1");
            Assert.Contains(result, s => s.LocationName == "Storage2");
        }

        [Fact]
        public async Task AddStorageAsync_ValidStorage_ShouldAddStorage()
        {
            // Arrange
            var storageName = "Storage A";
            var storage = new Storage { LocationName = storageName };

            _mockUnitOfWork.Setup(u => u.Storages.AddAsync(It.IsAny<Storage>()))
                .Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(u => u.CompleteAsync())
                .ReturnsAsync(1);

            // Act
            await _contentService.AddStorageAsync(storageName);

            // Assert
            _mockUnitOfWork.Verify(u => u.Storages.AddAsync(It.Is<Storage>(s =>
                s.LocationName == storageName)), Times.Once);
            _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Once);
        }

        [Fact]
        public async Task AttachStorageInfoAsync_ValidContentDto_ShouldAttachStorageInfo()
        {
            // Arrange
            var contentDto = new BookDto { Id = 1 };
            var location = new ContentLocation { ContentId = 1, StorageId = 1 };
            var storage = new Storage { Id = 1, LocationName = "Storage1" };

            _mockUnitOfWork.Setup(u => u.ContentLocations.FindAsync(
                It.IsAny<Expression<Func<ContentLocation, bool>>>()))
                .ReturnsAsync(new List<ContentLocation> { location });
            _mockUnitOfWork.Setup(u => u.Storages.GetByIdAsync(1))
                .ReturnsAsync(storage);
            _mockMapper.Setup(m => m.Map<StorageDto>(storage))
                .Returns(new StorageDto { Id = 1, LocationName = "Storage1" });

            // Act
            await _contentService.AttachStorageInfoAsync(contentDto);

            // Assert
            Assert.NotNull(contentDto.Storage);
            Assert.Equal("Storage1", contentDto.Storage.LocationName);
        } 
    }
}