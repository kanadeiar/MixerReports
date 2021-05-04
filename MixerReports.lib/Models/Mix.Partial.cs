using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MixerReports.lib.Models
{
    public partial class Mix
    {
        [NotMapped] public string NormalStr => (Normal) ? "Норма" : "Ошибка";

        [NotMapped] public string UndersizedStr => (Undersized) ? "Недоросток" : String.Empty;

        [NotMapped] public string OvergroundStr => (Overground) ? "Переросток" : string.Empty;

        [NotMapped] public string BoiledStr => (Boiled) ? "Закипел" : string.Empty;

        [NotMapped] public string OtherStr => (Other) ? "Другое" : string.Empty;

        [NotMapped] public string IsMudStr => (IsMud) ? "Сброс в шлам" : string.Empty;

        [NotMapped] public string IsExperimentStr => (IsExperiment) ? "Эксперимент" : string.Empty;

        /// <summary> Характеристики в виде строки </summary>
        [NotMapped]
        public string CharsStr
        {
            get
            {
                List<string> strs = new List<string>();
                if (Undersized) strs.Add(UndersizedStr);
                if (Overground) strs.Add(OvergroundStr);
                if (Boiled) strs.Add(BoiledStr);
                if (Other) strs.Add(OtherStr);
                if (IsMud) strs.Add(IsMudStr);
                if (IsExperiment) strs.Add(IsExperimentStr);
                if (strs.Count > 1)
                    return strs.Aggregate((m, n) => m + ',' + n);
                else if (strs.Count > 0)
                    return strs.First();
                else
                    return string.Empty;
            }
        }
    }
}
