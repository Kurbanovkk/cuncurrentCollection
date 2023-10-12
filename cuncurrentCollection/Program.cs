using System.Collections.Concurrent;

namespace cuncurrentCollection
{
    internal class Program
    {
        static ConcurrentDictionary<string, int> library = new ConcurrentDictionary<string, int>();
        static void Main(string[] args)
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            var percentageUpdateTask = Task.Run(() => UpdateBookPercentage(cancellationTokenSource.Token));
            while (true)
            {
                Console.WriteLine("Меню: 1 - добавить книгу; 2 - вывести список непрочитанного; 3 - выйти");
                string choice = Console.ReadLine();

                if (choice == "1")
                {
                    Console.Write("Введите название книги: ");
                    string? bookName = Console.ReadLine();
                        if (library.TryAdd(bookName, 0))
                        {
                            Console.WriteLine($"Книга '{bookName}' добавлена.");
                        }
                        else
                        {
                            Console.WriteLine($"Книга '{bookName}' уже существует.");
                        }
                }
                else if (choice == "2")
                {
                    Console.WriteLine("Список непрочитанных книг:");
                    foreach (var book in library)
                    {
                        Console.WriteLine($"{book.Key} - {book.Value}%");
                    }
                }
                else if (choice == "3")
                {
                    cancellationTokenSource.Cancel();
                    percentageUpdateTask.Wait();
                    break;
                }
            }
        }

        static async Task UpdateBookPercentage(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                foreach (var book in library)
                {
                    if (book.Value < 100)
                    {
                        library.TryUpdate(book.Key, book.Value + 1, book.Value);
                    }
                    else
                    {
                        library.TryRemove(book.Key, out _);
                    }
                }
                await Task.Delay(1000);
            }
        }
    }
}