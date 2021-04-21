using System;
using System.Runtime.InteropServices;
using MixerReports.lib;

namespace ConsoleAppMixerReports
{
    class Program
    {
        static void Main(string[] args)
        {
            RussianConsole();
            Console.WriteLine("Тестирование контроллерой части сервиса отчетов по заливкам");

            var sharp7Reader = new Sharp7Reader("10.0.57.10");
            if (sharp7Reader.TestConnection(out int error))
            {
                Console.WriteLine("Соединение с контроллером установлено");
            }
            else
            {
                Console.WriteLine($"Ошибка: {error}");
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
