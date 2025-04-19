#pragma warning disable 

using BLL2.Factories;
using BLL2.Services;
using DAL2;
using DAL2.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Text;
using DAL2.Entities;

namespace UI2
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            using (var initContext = new AppDbContext())
            {
                initContext.Database.EnsureCreated();
            }

            var context = new AppDbContext();
            var unitOfWork = new UnitOfWork(context);
            var service = new ContentService(unitOfWork);

            if (!(await service.GetStoragesAsync()).Any())
            {
                await service.AddStorageAsync("Головна бібліотека");
                await service.AddStorageAsync("Онлайн архів");
            }

            while (true)
            {
                Console.Clear();
                Console.WriteLine("==== Бібліотека контенту ====");
                Console.WriteLine("1. Показати весь контент");
                Console.WriteLine("2. Додати контент");
                Console.WriteLine("3. Пошук контенту за назвою");
                Console.WriteLine("4. Видалити контент");
                Console.WriteLine("5. Показати всі сховища");
                Console.WriteLine("6. Додати нове сховище");
                Console.WriteLine("0. Вийти");
                Console.Write("Вибір: ");

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        await ShowAll(service);
                        break;
                    case "2":
                        await AddContent(service);
                        break;
                    case "3":
                        await Search(service);
                        break;
                    case "4":
                        await Delete(service);
                        break;
                    case "5":
                        await ShowStorages(service);
                        break;
                    case "6":
                        await AddStorage(service);
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Невідомий вибір.");
                        break;
                }

                Console.WriteLine("\nНатисніть будь-яку клавішу для продовження...");
                Console.ReadKey();
            }
        }

        static async Task ShowAll(ContentService service)
        {
            var all = await service.GetAllAsync();
            Console.WriteLine("Весь контент:");

            if (!all.Any())
            {
                Console.WriteLine("Контент відсутній.");
                return;
            }

            foreach (var item in all)
            {
                var location = item.Location?.Storage?.LocationName ?? "невідомо";
                Console.WriteLine($"{item.Id}. {item.Title} ({item.GetType().Name}) - {item.Format}, розташування: {location}");
            }
        }

        static async Task AddContent(ContentService service)
        {
            Console.WriteLine("Тип контенту (book/audio/video/document):");
            var type = Console.ReadLine()?.Trim().ToLower();

            if (type is not ("book" or "audio" or "video" or "document"))
            {
                Console.WriteLine("Неправильний тип контенту. Спробуйте ще раз.");
                return;
            }

            Console.Write("Назва: ");
            var title = Console.ReadLine();

            Console.Write("Формат (наприклад, PDF, MP3...): ");
            var format = Console.ReadLine();

            var storages = await service.GetStoragesAsync();
            if (!storages.Any())
            {
                Console.WriteLine("Немає доступних сховищ.");
                return;
            }

            Console.WriteLine("Оберіть сховище:");
            for (int i = 0; i < storages.Count; i++)
                Console.WriteLine($"{i + 1}. {storages[i].LocationName}");

            if (!int.TryParse(Console.ReadLine(), out int storageIndex) || storageIndex < 1 || storageIndex > storages.Count)
            {
                Console.WriteLine("Невірний вибір сховища.");
                return;
            }

            int storageId = storages[storageIndex - 1].Id;

            ContentFactory factory = type switch
            {
                "book" => CreateBookFactory(title!, format!),
                "audio" => CreateAudioFactory(title!, format!),
                "video" => CreateVideoFactory(title!, format!),
                "document" => CreateDocumentFactory(title!, format!),
                _ => throw new Exception("Невірний тип контенту")
            };

            var content = factory.CreateContent();
            await service.AddContentAsync(content, storageId);
            Console.WriteLine("Контент додано!");
        }

        static BookFactory CreateBookFactory(string title, string format)
        {
            Console.Write("Автор: ");
            var author = Console.ReadLine();
            Console.Write("Кількість сторінок: ");
            int pages = int.Parse(Console.ReadLine()!);
            return new BookFactory(title, author!, pages, format);
        }

        static AudioFactory CreateAudioFactory(string title, string format)
        {
            Console.Write("Виконавець: ");
            var artist = Console.ReadLine();
            Console.Write("Тривалість (хв): ");
            double duration = double.Parse(Console.ReadLine()!);
            return new AudioFactory(title, artist!, duration, format);
        }

        static VideoFactory CreateVideoFactory(string title, string format)
        {
            Console.Write("Режисер: ");
            var director = Console.ReadLine();
            Console.Write("Висота (px): ");
            int height = int.Parse(Console.ReadLine()!);
            return new VideoFactory(title, director!, height, format);
        }

        static DocumentFactory CreateDocumentFactory(string title, string format)
        {
            Console.Write("Автор: ");
            var author = Console.ReadLine();
            Console.Write("Опис: ");
            var desc = Console.ReadLine();
            return new DocumentFactory(title, author!, desc!, format);
        }

        static async Task Search(ContentService service)
        {
            Console.Write("Введіть назву для пошуку: ");
            var query = Console.ReadLine();
            var results = await service.SearchByTitleAsync(query!);

            if (!results.Any())
            {
                Console.WriteLine("Нічого не знайдено.");
                return;
            }

            foreach (var item in results)
            {
                Console.WriteLine($"{item.Id}. {item.Title} ({item.GetType().Name}) - {item.Format}");
            }
        }

        static async Task Delete(ContentService service)
        {
            Console.Write("ID контенту для видалення: ");
            int id = int.Parse(Console.ReadLine()!);
            await service.DeleteAsync(id);
            Console.WriteLine("Видалено.");
        }

        static async Task ShowStorages(ContentService service)
        {
            var storages = await service.GetStoragesAsync();
            foreach (var s in storages)
            {
                Console.WriteLine($"{s.Id}. {s.LocationName} (зберігає {s.ContentLocations?.Count ?? 0} об'єктів)");
            }
        }

        static async Task AddStorage(ContentService service)
        {
            Console.Write("Назва нового сховища: ");
            var name = Console.ReadLine();
            await service.AddStorageAsync(name!);
            Console.WriteLine("Сховище додано.");
        }
    }
}
