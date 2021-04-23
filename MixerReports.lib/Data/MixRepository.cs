using System;
using System.Collections.Generic;
using System.Linq;
using MixerReports.lib.Data.Base;
using MixerReports.lib.Models;

namespace MixerReports.lib.Data
{
    /// <summary> Репозиторий заливок </summary>
    public class MixRepository : BaseRepository<Mix>
    {
        /// <summary> Получение данных только за последнюю неделю </summary>
        public new IEnumerable<Mix> GetAll() => Context.Mixes.Where(m => m.DateTime >= DateTime.Now.AddDays(- 7));

        public MixRepository(SPBSUMixerRaportsEntities entities) : base(entities)
        {
        }

    }
}
