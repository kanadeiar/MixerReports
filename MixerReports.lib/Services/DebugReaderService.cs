using System;
using MixerReports.lib.Interfaces;
using MixerReports.lib.Models;

namespace MixerReports.lib.Services
{
    public class DebugReaderService : ISharp7ReaderService
    {
        private bool _nowMixRunning = true; //идет заливка
        private bool _edgeMixRunning = false; //звонок новой заливки
        private Random _random = new Random();

        public int SetSecondsToRead { get; set; } = 210;
        public int AluminiumProp { get; set; } = 20;
        public bool TestConnection(out int error)
        {
            error = 0;
            return true;
        }

        private int _seconds = 0;
        public bool GetMixOnTime(out int secondsBegin, out int error, out Mix mix)
        {
            var outResult = false;
            error = 0;
            mix = null;
            secondsBegin = _seconds;

            if (secondsBegin > SetSecondsToRead && _nowMixRunning) //заливка окончена по времени
            {
                mix = new Mix
                {
                    Number = 1,
                    DateTime = DateTime.Now.AddSeconds(-_seconds),
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
                    SetAluminium1 = (float)_random.NextDouble() * 10.0f + 60.0f / (AluminiumProp + 1),
                    ActAluminium1 = (float)_random.NextDouble() * 10.0f + 60.0f / (AluminiumProp + 1),
                    SetAluminium2 = 0.0f,
                    ActAluminium2 = 0.0f,
                    SandInMud = (float)_random.NextDouble() * 10.0f + 1300.0f,
                    DensitySandMud = (float)_random.NextDouble() * 1.0f + 1.0f,
                    DensityRevertMud = (float)_random.NextDouble() * 1.0f + 1.0f,
                    Normal = true,
                };
                _nowMixRunning = false;
                outResult = true;
            }
            _edgeMixRunning = _nowMixRunning;

            _seconds += 3;
            if (_seconds >= 260)
            {
                _nowMixRunning = true;
                _seconds = 0;
            }
            return outResult;
        }
    }
}
