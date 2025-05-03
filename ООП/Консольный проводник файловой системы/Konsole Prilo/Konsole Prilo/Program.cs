using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Konsole_Prilo
{
    class FileExplorer
    {
        private static string currentDirectory = Directory.GetCurrentDirectory();

        static void Main(string[] args)
        {
            Console.WriteLine("Простой файловый проводник");
            Console.WriteLine("Текущая директория: " + currentDirectory);
            Console.WriteLine();

            while (true)
            {
                DisplayDirectoryContents();
                DisplayMenu();
                ProcessUserChoice();
            }
        }

        static void DisplayDirectoryContents()
        {
            Console.WriteLine("Содержимое директории:");
            Console.WriteLine(new string('-', 50));

            try
            {
                // Вывод подкаталогов
                foreach (var directory in Directory.GetDirectories(currentDirectory))
                {
                    Console.WriteLine($"[Папка] {Path.GetFileName(directory)}");
                }

                // Вывод файлов
                foreach (var file in Directory.GetFiles(currentDirectory))
                {
                    Console.WriteLine($"[Файл]  {Path.GetFileName(file)}");
                }
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("Нет доступа к этой директории");
            }

            Console.WriteLine(new string('-', 50));
        }

        static void DisplayMenu()
        {
            Console.WriteLine("\nМеню:");
            Console.WriteLine("1. Открыть папку/файл");
            Console.WriteLine("2. Создать новую папку");
            Console.WriteLine("3. Создать новый файл");
            Console.WriteLine("4. Удалить папку/файл");
            Console.WriteLine("5. Вернуться в родительскую папку");
            Console.WriteLine("6. Выход");
            Console.Write("Выберите действие: ");
        }

        static void ProcessUserChoice()
        {
            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    OpenItem();
                    break;
                case "2":
                    CreateDirectory();
                    break;
                case "3":
                    CreateFile();
                    break;
                case "4":
                    DeleteItem();
                    break;
                case "5":
                    MoveToParentDirectory();
                    break;
                case "6":
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("Неверный выбор. Попробуйте снова.");
                    break;
            }
        }

        static void OpenItem()
        {
            Console.Write("Введите имя папки/файла: ");
            string name = Console.ReadLine();
            string path = Path.Combine(currentDirectory, name);

            if (Directory.Exists(path))
            {
                currentDirectory = path;
                Console.Clear();
                Console.WriteLine($"Перешли в папку: {currentDirectory}");
            }
            else if (File.Exists(path))
            {
                if (Path.GetExtension(path).ToLower() == ".txt")
                {
                    Console.WriteLine("\nСодержимое файла:");
                    Console.WriteLine(new string('-', 50));
                    Console.WriteLine(File.ReadAllText(path));
                    Console.WriteLine(new string('-', 50));
                }
                else
                {
                    Console.WriteLine("Можно просматривать только текстовые файлы (.txt)");
                }
            }
            else
            {
                Console.WriteLine("Папка или файл не найдены");
            }
        }

        static void CreateDirectory()
        {
            Console.Write("Введите имя новой папки: ");
            string name = Console.ReadLine();
            string path = Path.Combine(currentDirectory, name);

            try
            {
                Directory.CreateDirectory(path);
                Console.WriteLine($"Папка '{name}' создана");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        static void CreateFile()
        {
            Console.Write("Введите имя нового файла (с расширением .txt): ");
            string name = Console.ReadLine();

            if (!name.EndsWith(".txt"))
            {
                name += ".txt";
            }

            string path = Path.Combine(currentDirectory, name);

            Console.WriteLine("Введите содержимое файла (для завершения введите пустую строку):");
            string content = "";
            string line;

            while (!string.IsNullOrWhiteSpace(line = Console.ReadLine()))
            {
                content += line + Environment.NewLine;
            }

            try
            {
                File.WriteAllText(path, content);
                Console.WriteLine($"Файл '{name}' создан");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        static void DeleteItem()
        {
            Console.Write("Введите имя папки/файла для удаления: ");
            string name = Console.ReadLine();
            string path = Path.Combine(currentDirectory, name);

            Console.Write($"Вы уверены, что хотите удалить '{name}'? (y/n): ");
            string confirmation = Console.ReadLine().ToLower();

            if (confirmation != "y") return;

            try
            {
                if (Directory.Exists(path))
                {
                    Directory.Delete(path, true);
                    Console.WriteLine($"Папка '{name}' удалена");
                }
                else if (File.Exists(path))
                {
                    File.Delete(path);
                    Console.WriteLine($"Файл '{name}' удален");
                }
                else
                {
                    Console.WriteLine("Папка или файл не найдены");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        static void MoveToParentDirectory()
        {
            DirectoryInfo parent = Directory.GetParent(currentDirectory);
            if (parent != null)
            {
                currentDirectory = parent.FullName;
                Console.Clear();
                Console.WriteLine($"Перешли в папку: {currentDirectory}");
            }
            else
            {
                Console.WriteLine("Это корневая директория, нельзя подняться выше");
            }
        }
    }
}
