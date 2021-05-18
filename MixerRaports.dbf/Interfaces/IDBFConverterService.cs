using System.Collections.Generic;
using MixerReports.lib.Models;

namespace MixerRaports.dbf.Interfaces
{
    public interface IDBFConverterService
    {
        ICollection<Mix> GetMixesFromDBF(string fileName);
    }
}
