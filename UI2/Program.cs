using BLL2.Factories;
using BLL2.Services;
using DAL2;
using DAL2.Entities;
using System.Text;

namespace UI2
{
    class Program 
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            using (var initContext = new AppDbContext())
            {
                initContext.Database.EnsureCreated();
            }


            var context = new AppDbContext();
            var service = new ContentService(context);

            // Додати стартові сховища, якщо порожньо
            if (!service.GetStorages().Any())
            {
                service.AddStorage("Головна бібліотека");
                service.AddStorage("Онлайн архів");
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
                        ShowAll(service);
                        break;
                    case "2":
                        AddContent(service);
                        break;
                    case "3":
                        Search(service);
                        break;
                    case "4":
                        Delete(service);
                        break;
                    case "5":
                        ShowStorages(service);
                        break;
                    case "6":
                        AddStorage(service);
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

        static void ShowAll(ContentService service)
        {
            var all = service.GetAll();
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


        static void AddContent(ContentService service)
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

            var storages = service.GetStorages();
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
                _ => throw new Exception("Невірний тип контенту") // тепер ніколи не трапиться
            };

            var content = factory.CreateContent();
            service.AddContent(content, storageId);
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

        static void Search(ContentService service)
        {
            Console.Write("Введіть назву для пошуку: ");
            var query = Console.ReadLine();
            var results = service.SearchByTitle(query!);

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

        static void Delete(ContentService service)
        {
            Console.Write("ID контенту для видалення: ");
            int id = int.Parse(Console.ReadLine()!);
            service.Delete(id);
            Console.WriteLine("Видалено.");
        }

        static void ShowStorages(ContentService service)
        {
            var storages = service.GetStorages();
            foreach (var s in storages)
            {
                Console.WriteLine($"{s.Id}. {s.LocationName} (зберігає {s.ContentLocations.Count} об'єктів)");
            }
        }

        static void AddStorage(ContentService service)
        {
            Console.Write("Назва нового сховища: ");
            var name = Console.ReadLine();
            service.AddStorage(name!);
            Console.WriteLine("Сховище додано.");
        }
    }
}
