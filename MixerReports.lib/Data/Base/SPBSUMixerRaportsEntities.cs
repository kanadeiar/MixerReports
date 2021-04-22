using Microsoft.EntityFrameworkCore;
using MixerReports.lib.Models;

namespace MixerReports.lib.Data.Base
{
    /// <summary> База данных отчетов по заливкам </summary>
    public class SPBSUMixerRaportsEntities : DbContext
    {
        public DbSet<Mix> Mixes { get; set; }
        public SPBSUMixerRaportsEntities(DbContextOptions<SPBSUMixerRaportsEntities> options) : base(options)
        {
        }
    }
}
