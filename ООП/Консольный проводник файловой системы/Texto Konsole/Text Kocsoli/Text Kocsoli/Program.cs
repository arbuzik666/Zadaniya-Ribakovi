using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Text_Kocsoli
{
    class EnhancedFileExplorer
    {
        private static DriveInfo[] allDrives;
        private static string currentDirectory;

        static void Main(string[] args)
        {
            Console.Title = "Улучшенный консольный проводник";
            InitializeExplorer();

            while (true)
            {
                if (currentDirectory == null)
                {
                    DisplayDrivesMenu();
                }
                else
                {
                    DisplayContentMenu();
                }
            }
        }

        static void InitializeExplorer()
        {
            allDrives = DriveInfo.GetDrives();
            currentDirectory = null;
        }

        static void DisplayDrivesMenu()
        {
            Console.Clear();
            Console.WriteLine("=== ДОСТУПНЫЕ ДИСКИ ===");
            Console.WriteLine(new string('=', 60));
            Console.WriteLine("| № | Имя | Тип       | Файловая система | Всего     | Свободно  |");
            Console.WriteLine(new string('=', 60));

            for (int i = 0; i < allDrives.Length; i++)
            {
                var drive = allDrives[i];
                try
                {
                    Console.WriteLine($"| {i + 1,1} | {drive.Name,3} | {drive.DriveType,-9} | " +
                                    $"{drive.DriveFormat,-15} | {FormatSize(drive.TotalSize),-9} | " +
                                    $"{FormatSize(drive.AvailableFreeSpace),-9} |");
                }
                catch
                {
                    Console.WriteLine($"| {i + 1,1} | {drive.Name,3} | {drive.DriveType,-9} | " +
                                    "Н/Д            | Н/Д       | Н/Д       |");
                }
            }
            Console.WriteLine(new string('=', 60));
            Console.WriteLine("\nВыберите диск (1-{0}) или 0 для выхода:", allDrives.Length);
            ProcessDriveSelection();
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
                        Console.WriteLine("\nДиск не готов. Нажмите любую клавишу...");
                        Console.ReadKey();
                    }
                }
            }
        }

        static void DisplayContentMenu()
        {
            Console.Clear();
            Console.WriteLine($"=== СОДЕРЖИМОЕ: {currentDirectory} ===");
            Console.WriteLine(new string('=', 80));
            Console.WriteLine("{0,-50} {1,-20} {2,10}", "Имя", "Тип", "Размер/Дата создания");
            Console.WriteLine(new string('=', 80));

            try
            {
                // Отображение родительской директории
                var parent = Directory.GetParent(currentDirectory);
                if (parent != null)
                {
                    Console.WriteLine("{0,-50} {1,-20} {2,10}", "[..]", "Родительская папка", "");
                }

                // Отображение подкаталогов
                foreach (var dir in Directory.GetDirectories(currentDirectory))
                {
                    var dirInfo = new DirectoryInfo(dir);
                    Console.WriteLine("{0,-50} {1,-20} {2,10}",
                        $"[{dirInfo.Name}]",
                        "Папка",
                        dirInfo.CreationTime.ToShortDateString());
                }

                // Отображение файлов
                foreach (var file in Directory.GetFiles(currentDirectory))
                {
                    var fileInfo = new FileInfo(file);
                    Console.WriteLine("{0,-50} {1,-20} {2,10}",
                        fileInfo.Name,
                        "Файл",
                        $"{FormatFileSize(fileInfo.Length)}");
                }
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("=== ОШИБКА ДОСТУПА ===");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"=== ОШИБКА: {ex.Message} ===");
            }

            Console.WriteLine(new string('=', 80));
            DisplayOperationsMenu();
        }

        static string FormatFileSize(long bytes)
        {
            if (bytes < 1024) return $"{bytes} B";
            if (bytes < 1024 * 1024) return $"{bytes / 1024.0:0.##} KB";
            if (bytes < 1024 * 1024 * 1024) return $"{bytes / (1024.0 * 1024):0.##} MB";
            return $"{bytes / (1024.0 * 1024 * 1024):0.##} GB";
        }

        static void DisplayOperationsMenu()
        {
            Console.WriteLine("\nДОСТУПНЫЕ ОПЕРАЦИИ:");
            Console.WriteLine("1. Открыть папку/файл");
            Console.WriteLine("2. Создать новую папку");
            Console.WriteLine("3. Создать текстовый файл");
            Console.WriteLine("4. Удалить папку/файл");
            Console.WriteLine("5. Вернуться к списку дисков");
            Console.WriteLine("0. Выход");
            Console.Write("\nВыберите действие: ");
            ProcessDirectoryOperation();
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
                    CreateTextFile();
                    break;
                case "4":
                    DeleteItem();
                    break;
                case "5":
                    currentDirectory = null;
                    break;
                case "0":
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("\nНеверный выбор. Нажмите любую клавишу...");
                    Console.ReadKey();
                    break;
            }
        }

        static void OpenItem()
        {
            Console.Write("\nВведите имя папки/файла: ");
            string name = Console.ReadLine();

            if (name == "..")
            {
                MoveToParentDirectory();
                return;
            }

            string path = Path.Combine(currentDirectory, name);

            if (Directory.Exists(path))
            {
                currentDirectory = path;
            }
            else if (File.Exists(path))
            {
                if (Path.GetExtension(path).ToLower() == ".txt")
                {
                    DisplayTextFileContent(path);
                }
                else
                {
                    Console.WriteLine("\nМожно просматривать только текстовые файлы (.txt)");
                    Console.ReadKey();
                }
            }
            else
            {
                Console.WriteLine("\nПапка или файл не найдены");
                Console.ReadKey();
            }
        }

        static void DisplayTextFileContent(string filePath)
        {
            Console.Clear();
            Console.WriteLine($"=== СОДЕРЖИМОЕ ФАЙЛА: {Path.GetFileName(filePath)} ===");
            Console.WriteLine(new string('=', 80));

            try
            {
                string content = File.ReadAllText(filePath);
                Console.WriteLine(content);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ОШИБКА ЧТЕНИЯ: {ex.Message}");
            }

            Console.WriteLine(new string('=', 80));
            Console.WriteLine("\nНажмите любую клавишу для продолжения...");
            Console.ReadKey();
        }

        static void CreateDirectory()
        {
            Console.Write("\nВведите имя новой папки: ");
            string name = Console.ReadLine();
            string path = Path.Combine(currentDirectory, name);

            try
            {
                Directory.CreateDirectory(path);
                Console.WriteLine($"\nПапка '{name}' успешно создана!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nОШИБКА: {ex.Message}");
            }
            Console.ReadKey();
        }

        static void CreateTextFile()
        {
            Console.Write("\nВведите имя нового файла (без расширения): ");
            string name = Console.ReadLine().Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine("\nИмя файла не может быть пустым!");
                Console.ReadKey();
                return;
            }

            string fileName = name.EndsWith(".txt") ? name : $"{name}.txt";
            string path = Path.Combine(currentDirectory, fileName);

            if (File.Exists(path))
            {
                Console.WriteLine("\nФайл с таким именем уже существует!");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("\nВведите содержимое файла (для завершения введите пустую строку):");
            Console.WriteLine(new string('-', 60));

            string content = "";
            string line;
            int lineNumber = 1;

            while (!string.IsNullOrWhiteSpace(line = Console.ReadLine()))
            {
                content += $"{lineNumber++}: {line}{Environment.NewLine}";
            }

            try
            {
                File.WriteAllText(path, content);
                Console.WriteLine(new string('-', 60));
                Console.WriteLine($"\nФайл '{fileName}' успешно создан!");
                Console.WriteLine($"Размер: {FormatFileSize(new FileInfo(path).Length)}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nОШИБКА: {ex.Message}");
            }
            Console.ReadKey();
        }

        static void DeleteItem()
        {
            Console.Write("\nВведите имя папки/файла для удаления: ");
            string name = Console.ReadLine();
            string path = Path.Combine(currentDirectory, name);

            if (!Directory.Exists(path) && !File.Exists(path))
            {
                Console.WriteLine("\nПапка или файл не найдены!");
                Console.ReadKey();
                return;
            }

            Console.Write($"\nВы уверены, что хотите удалить '{name}'? (y/n): ");
            string confirmation = Console.ReadLine().ToLower();

            if (confirmation != "y") return;

            try
            {
                if (Directory.Exists(path))
                {
                    Directory.Delete(path, true);
                    Console.WriteLine($"\nПапка '{name}' успешно удалена!");
                }
                else
                {
                    File.Delete(path);
                    Console.WriteLine($"\nФайл '{name}' успешно удален!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nОШИБКА: {ex.Message}");
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
                Console.WriteLine("\nЭто корневая директория диска!");
                Console.ReadKey();
            }
        }
    }
}
