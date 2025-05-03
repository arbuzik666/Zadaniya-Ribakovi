using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disk_Manager
{
    class DiskManager
    {
        private static DriveInfo[] allDrives;
        private static string currentDirectory;

        static void Main(string[] args)
        {
            Console.Title = "Дисковый менеджер";
            allDrives = DriveInfo.GetDrives();
            currentDirectory = null;

            while (true)
            {
                if (currentDirectory == null)
                {
                    DisplayDrives();
                    ProcessDriveSelection();
                }
                else
                {
                    DisplayDirectoryContents();
                    ProcessDirectoryOperation();
                }
            }
        }

        static void DisplayDrives()
        {
            Console.Clear();
            Console.WriteLine("Доступные диски:");
            Console.WriteLine(new string('-', 60));
            Console.WriteLine("| №  | Имя | Тип       | ФС       | Всего     | Свободно  |");
            Console.WriteLine(new string('-', 60));

            for (int i = 0; i < allDrives.Length; i++)
            {
                var drive = allDrives[i];
                try
                {
                    Console.WriteLine($"| {i + 1,2} | {drive.Name,-4} | {drive.DriveType,-9} | " +
                                    $"{drive.DriveFormat,-8} | {FormatSize(drive.TotalSize),-9} | " +
                                    $"{FormatSize(drive.AvailableFreeSpace),-9} |");
                }
                catch
                {
                    Console.WriteLine($"| {i + 1,2} | {drive.Name,-4} | {drive.DriveType,-9} | " +
                                    "Н/Д      | Н/Д       | Н/Д       |");
                }
            }
            Console.WriteLine(new string('-', 60));
            Console.WriteLine("\nВыберите диск (1-{0}) или 0 для выхода:", allDrives.Length);
        }

        static string FormatSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len /= 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }

        static void ProcessDriveSelection()
        {
            if (int.TryParse(Console.ReadLine(), out int choice))
            {
                if (choice == 0)
                {
                    Environment.Exit(0);
                }
                else if (choice > 0 && choice <= allDrives.Length)
                {
                    var selectedDrive = allDrives[choice - 1];
                    if (selectedDrive.IsReady)
                    {
                        currentDirectory = selectedDrive.RootDirectory.FullName;
                    }
                    else
                    {
                        Console.WriteLine("Диск не готов. Нажмите любую клавишу...");
                        Console.ReadKey();
                    }
                }
            }
        }

        static void DisplayDirectoryContents()
        {
            Console.Clear();
            Console.WriteLine($"Текущая директория: {currentDirectory}");
            Console.WriteLine(new string('-', 60));
            Console.WriteLine("Содержимое:");
            Console.WriteLine(new string('-', 60));

            try
            {
                // Отображение подкаталогов
                foreach (var dir in Directory.GetDirectories(currentDirectory))
                {
                    var dirInfo = new DirectoryInfo(dir);
                    Console.WriteLine($"[Папка]  {dirInfo.Name,-50} {dirInfo.CreationTime}");
                }

                // Отображение файлов
                foreach (var file in Directory.GetFiles(currentDirectory))
                {
                    var fileInfo = new FileInfo(file);
                    Console.WriteLine($"[Файл]   {fileInfo.Name,-50} {fileInfo.Length,10} байт");
                }
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("Нет доступа к этой директории");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }

            Console.WriteLine(new string('-', 60));
            DisplayDirectoryMenu();
        }

        static void DisplayDirectoryMenu()
        {
            Console.WriteLine("\nМеню:");
            Console.WriteLine("1. Открыть папку/файл");
            Console.WriteLine("2. Создать новую папку");
            Console.WriteLine("3. Создать новый файл");
            Console.WriteLine("4. Удалить папку/файл");
            Console.WriteLine("5. Вернуться в родительскую папку");
            Console.WriteLine("6. Вернуться к списку дисков");
            Console.WriteLine("0. Выход");
            Console.Write("Выберите действие: ");
        }

        static void ProcessDirectoryOperation()
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
                    currentDirectory = null;
                    break;
                case "0":
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("Неверный выбор. Нажмите любую клавишу...");
                    Console.ReadKey();
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
            }
            else if (File.Exists(path))
            {
                if (Path.GetExtension(path).ToLower() == ".txt")
                {
                    Console.WriteLine("\nСодержимое файла:");
                    Console.WriteLine(new string('-', 50));
                    Console.WriteLine(File.ReadAllText(path));
                    Console.WriteLine(new string('-', 50));
                    Console.WriteLine("Нажмите любую клавишу...");
                    Console.ReadKey();
                }
                else
                {
                    Console.WriteLine("Можно просматривать только текстовые файлы (.txt)");
                    Console.ReadKey();
                }
            }
            else
            {
                Console.WriteLine("Папка или файл не найдены");
                Console.ReadKey();
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
            Console.ReadKey();
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
            Console.ReadKey();
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
            Console.ReadKey();
        }

        static void MoveToParentDirectory()
        {
            DirectoryInfo parent = Directory.GetParent(currentDirectory);
            if (parent != null)
            {
                currentDirectory = parent.FullName;
            }
            else
            {
                Console.WriteLine("Это корневая директория, нельзя подняться выше");
                Console.ReadKey();
            }
        }
    }
}
