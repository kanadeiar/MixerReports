using System;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.EntityFrameworkCore;
using MixerReports.lib.Data.Base;
using MixerReports.lib.Models;

namespace ConsoleAppTestDataBase
{
    class Program
    {
        private static Random _random = new Random();
        static void Main(string[] args)
        {
            RussianConsole();
            Console.WriteLine("Тест базы данных");
            var options = new DbContextOptionsBuilder<SPBSUMixerRaportsEntities>()
                .UseSqlServer(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=SPBSUMixerRaportsDev.DB")
                .Options;
            using (var db = new SPBSUMixerRaportsEntities(options))
            {
                var testdata = Enumerable.Range(1, 10).Select(t 
                    => new Mix
                    {
                        Number = t,
                        DateTime = DateTime.Now.AddSeconds(- _random.Next(1000)),
                        FormNumber = _random.Next(1, 100),
                        RecipeNumber = 0,
                        MixerTemperature = (float)_random.NextDouble() * 10.0f + 50.0f,
                        SetRevertMud = (float)_random.NextDouble() * 10.0f + 1800.0f,
                        ActRevertMud = (float)_random.NextDouble() * 10.0f + 1800.0f,
                        SetSandMud = (float)_random.NextDouble() * 10.0f + 1500.0f,
                        ActSandMud = (float)_random.NextDouble() * 10.0f + 1500.0f,
                        SetColdWater = (float)_random.NextDouble() * 10.0f + 100.0f,
                        ActColdWater = (float)_random.NextDouble() * 10.0f + 100.0f,
                        SetHotWater = (float)_random.NextDouble() * 10.0f + 250.0f,
                        ActHotWater = (float)_random.NextDouble() * 10.0f + 250.0f,
                        SetMixture1 = (float)_random.NextDouble() * 10.0f + 600.0f,
                        ActMixture1 = (float)_random.NextDouble() * 10.0f + 600.0f,
                        SetMixture2 = 0.0f,
                        ActMixture2 = 0.0f,
                        SetCement1 = (float)_random.NextDouble() * 10.0f + 600.0f,
                        ActCement1 = (float)_random.NextDouble() * 10.0f + 600.0f,
                        SetCement2 = 0.0f,
                        ActCement2 = 0.0f,
                        SetAluminium1 = (float)_random.NextDouble() * 10.0f + 60.0f / 21.0f,
                        ActAluminium1 = (float)_random.NextDouble() * 10.0f + 60.0f / 21.0f,
                        SetAluminium2 = 0.0f,
                        ActAluminium2 = 0.0f,
                        SandInMud = (float)_random.NextDouble() * 10.0f + 1300.0f,
                        Normal = true,
                    });
                db.AddRange(testdata);
                db.SaveChanges();
                Console.WriteLine("Данные успешно добавлены");
            }

            

            Console.WriteLine("Нажмите любую кнопку ...");
            Console.ReadLine();
        }
        private static void RussianConsole()
        {
            [DllImport("kernel32.dll")] static extern bool SetConsoleCP(uint pagenum);
            [DllImport("kernel32.dll")] static extern bool SetConsoleOutputCP(uint pagenum);
            SetConsoleCP(65001);        //установка кодовой страницы utf-8 (Unicode) для вводного потока
            SetConsoleOutputCP(65001);  //установка кодовой страницы utf-8 (Unicode) для выводного потока
        }
    }
}
