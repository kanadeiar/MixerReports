using System;
using System.Runtime.InteropServices;
using System.Timers;
using MixerReports.lib.Interfaces;
using MixerReports.lib.Models;
using MixerReports.lib.Services;
using Sharp7;

namespace ConsoleAppMixerReports
{
    class Program
    {
        private static Timer _timer;
        //private static ISharp7ReaderService _sharp7ReaderService;
        private static ISharp7MixReaderService _service;
        static void Main(string[] args)
        {
            RussianConsole();
            Console.WriteLine("Тестирование контроллерой части сервиса отчетов по заливкам");

            _service = new Sharp7EasyMixReaderService(new S7Client{ConnTimeout = 5_000, RecvTimeout = 5_000});
            //_sharp7ReaderService = new Sharp7ReaderService();
            //_sharp7ReaderService = new DebugReaderService();
            //_service = new Sharp7MixReaderService();
            Console.WriteLine(_service.TestConnection(out int error)
                ? "Соединение с контроллером успешно установлено"
                : $"Ошибка установки соединения с контроллером: {error}");

            _timer = new Timer(3_000);
            _timer.Elapsed += TimerOnElapsed;
            _timer.Start();




            Console.WriteLine("Нажмите любую кнопку ...");
            Console.ReadLine();
        }
        private static void PrintDatas(Mix row)
        {
            Console.WriteLine($" {row.DateTime:g} {row.Number} Форма №{row.FormNumber} {row.MixerTemperature}, " +
                              $"обратный шлам: {row.SetRevertMud} {row.ActRevertMud} песчаный шлам: {row.SetSandMud} {row.ActSandMud} \n" +
                              $"холодная вода: {row.SetColdWater} {row.ActColdWater} горячая вода {row.SetHotWater} {row.ActHotWater} " +
                              $"ипв1: {row.SetMixture1} {row.ActMixture1} ипв2: {row.SetMixture2} {row.ActMixture2} \n" +
                              $"цемент1: {row.SetCement1} {row.ActCement1} цемент2: {row.SetCement2} {row.ActCement2} " +
                              $"алюминий1: {row.SetAluminium1} {row.ActAluminium1} алюминий2: {row.SetAluminium2} {row.ActAluminium2} песок в шламе: {row.SandInMud}\n" +
                              $"норма: {row.Normal} комментарий: {row.Comment}");
        }
        private static void TimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            _timer.Enabled = false;
            //try
            //{
                if (_service.TryNewMixTick(out int seconds, out int error, out Mix mix))
                {
                    PrintDatas(mix);
                }
                Console.Title = $"Заливка - {seconds} секунд, ошибка - {error}";

                //if (_sharp7ReaderService.GetMixOnTime(out int seconds, out int error, out Mix mix)) PrintDatas(mix);
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine("Ошибка чтения заливки: " + ex.Message);
            //    throw;
            //}
            _timer.Enabled = true;
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
