using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.Models
{
    public class Car
    {
        private string make;    // private - доступ только внутри класса Car
        private string model;   // private - доступ только внутри класса Car

        /// Год выпуска - public, доступен из любого места
        public int Year { get; set; }

        /// Устанавливает значения make и model
        internal void SetMakeAndModel(string make, string model)
        {
            this.make = make;
            this.model = model;
        }

        /// Выводит информацию об автомобиле
        protected virtual void DisplayInfo()
        {
            Console.WriteLine($"Автомобиль: {make} {model}, {Year} года выпуска");
        }
    }

    /// Класс электромобиля, наследуется от Car
    /// Добавляет информацию о емкости батареи
    public class ElectricCar : Car
    {
        private double batteryCapacity; 

        /// Public метод - доступен из любого места
        /// Устанавливает емкость батареи
        public void SetBatteryCapacity(double capacity)
        {
            batteryCapacity = capacity;
        }

        /// Переопределенный protected метод - доступен внутри класса и производных классов
        /// Добавляет вывод информации о батарее
        protected override void DisplayInfo()
        {
            base.DisplayInfo(); // Вызываем метод базового класса
            Console.WriteLine($"Емкость батареи: {batteryCapacity} kWh");
        }

        /// Public метод для вывода информации
        public void ShowInfo()
        {
            DisplayInfo();
        }
    }
}

namespace MyApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var car = new MyApp.Models.Car();


            car.Year = 2020; 
            car.SetMakeAndModel("Toyota", "Camry"); 

            // Создаем электромобиль
            var electricCar = new MyApp.Models.ElectricCar();
            electricCar.Year = 2022;
            electricCar.SetMakeAndModel("Tesla", "Model 3"); 
            electricCar.SetBatteryCapacity(75.5); 

            electricCar.ShowInfo();

            Console.ReadKey();
        }
    }
}
