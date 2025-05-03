using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3_Zadanie_1
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("Введите розничную цену за 1 кг (в рублях):");
            double C = double.Parse(Console.ReadLine());

            Console.WriteLine("Введите процент скидки на опт (p):");
            double p = double.Parse(Console.ReadLine());

            Console.WriteLine("Введите текущий курс доллара (D):");
            double D = double.Parse(Console.ReadLine());

            // Рассчитываем оптовую цену (на p% ниже розничной)
            double wholesalePrice = C * (100 - p) / 100;

            Console.WriteLine("\nВес | Розничная цена | Оптовая цена | Розничная ($) | Оптовая ($)");
            Console.WriteLine("-----------------------------------------------------------");

            // Выводим цены для веса 5, 10, 15, ..., 50 кг
            for (int weight = 5; weight <= 50; weight += 5)
            {
                double retailTotal = weight * C;
                double wholesaleTotal = weight * wholesalePrice;

                double retailDollars = retailTotal / D;
                double wholesaleDollars = wholesaleTotal / D;

                Console.WriteLine($"{weight,3} кг | {retailTotal,13:F2} руб | {wholesaleTotal,11:F2} руб | " +
                                 $"{retailDollars,11:F2} $ | {wholesaleDollars,9:F2} $");
            }
        }
    }
}
