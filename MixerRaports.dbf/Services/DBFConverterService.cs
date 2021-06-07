using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MixerRaports.dbf.Interfaces;
using MixerReports.lib.Models;
using NDbfReaderEx;

namespace MixerRaports.dbf.Services
{
    public class DBFConverterService : IDBFConverterService
    {
        public ICollection<Mix> GetMixesFromDBF(string fileName)
        {
            if (!File.Exists(fileName))
                throw new FileNotFoundException($"Файла {fileName} не существует!");
            var result = new List<Mix>();
            using (DbfTable table = DbfTable.Open(fileName, Encoding.GetEncoding(437)))
            {
                for (int i = 0; i < table.recCount - 3; i++)
                {
                    var row = table.GetRow(i + 3);
                    var mix = new Mix
                    {
                        Number = int.Parse(row.GetString("0001")),
                        DateTime = DateTime.Parse(row.GetString("0002")),
                        FormNumber = int.Parse(row.GetString("0003")),
                        RecipeNumber = int.Parse(row.GetString("0004")),
                        MixerTemperature = float.Parse(row.GetString("0005")),
                        SetRevertMud = float.Parse(row.GetString("0006")),
                        ActRevertMud = float.Parse(row.GetString("0007")),
                        SetSandMud = float.Parse(row.GetString("0008")),
                        ActSandMud = float.Parse(row.GetString("0009")),
                        SetColdWater = float.Parse(row.GetString("0010")),
                        ActColdWater = float.Parse(row.GetString("0011")),
                        SetHotWater = float.Parse(row.GetString("0012")),
                        ActHotWater = float.Parse(row.GetString("0013")),
                        SetMixture1 = float.Parse(row.GetString("0014")),
                        ActMixture1 = float.Parse(row.GetString("0015")),
                        SetMixture2 = float.Parse(row.GetString("0016")),
                        ActMixture2 = float.Parse(row.GetString("0017")),
                        SetCement1 = float.Parse(row.GetString("0022")),
                        ActCement1 = float.Parse(row.GetString("0023")),
                        SetCement2 = float.Parse(row.GetString("0024")),
                        ActCement2 = float.Parse(row.GetString("0025")),
                        SetAluminium1 = float.Parse(row.GetString("0026")),
                        ActAluminium1 = float.Parse(row.GetString("0027")),
                        SetAluminium2 = float.Parse(row.GetString("0028")),
                        ActAluminium2 = float.Parse(row.GetString("0029")),
                        SandInMud = float.Parse(row.GetString("0060")),
                        DensityRevertMud = float.Parse(row.GetString("0038")),
                        DensitySandMud = float.Parse(row.GetString("0039")),
                        Normal = true,
                    };
                    if (i > 0 && mix.SetRevertMud < 0.1 && mix.ActRevertMud < 0.1 && mix.SetSandMud < 0.1 &&
                        mix.ActSandMud < 0.1 && mix.SetColdWater < 0.1 && mix.ActColdWater < 0.1)
                    {
                        mix.SetRevertMud = result[i - 1].SetRevertMud;
                        mix.ActRevertMud = result[i - 1].ActRevertMud;
                        mix.SetSandMud = result[i - 1].SetSandMud;
                        mix.ActSandMud = result[i - 1].ActSandMud;
                        mix.SetColdWater = result[i - 1].SetColdWater;
                        mix.ActColdWater = result[i - 1].ActColdWater;
                        mix.SetHotWater = result[i - 1].SetHotWater;
                        mix.ActHotWater = result[i - 1].ActHotWater;
                        mix.SetMixture1 = result[i - 1].SetMixture1;
                        mix.ActMixture1 = result[i - 1].ActMixture1;
                        mix.SetMixture2 = result[i - 1].SetMixture2;
                        mix.ActMixture2 = result[i - 1].ActMixture2;
                        mix.SetCement1 = result[i - 1].SetCement1;
                        mix.ActCement1 = result[i - 1].ActCement1;
                        mix.SetCement2 = result[i - 1].SetCement2;
                        mix.ActCement2 = result[i - 1].ActCement2;
                        mix.SetAluminium1 = result[i - 1].SetAluminium1;
                        mix.ActAluminium1 = result[i - 1].ActAluminium1;
                        mix.SetAluminium2 = result[i - 1].SetAluminium2;
                        mix.ActAluminium2 = result[i - 1].ActAluminium2;
                        mix.SandInMud = result[i - 1].SandInMud;
                        mix.DensityRevertMud = result[i - 1].DensityRevertMud;
                        mix.DensitySandMud = result[i - 1].DensitySandMud;
                    }
                    result.Add(mix);
                }
            }
            return result;
        }
    }
}
