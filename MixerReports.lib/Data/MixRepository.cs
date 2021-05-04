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
        public MixRepository(SPBSUMixerRaportsEntities entities) : base(entities)
        {
        }
    }
}
