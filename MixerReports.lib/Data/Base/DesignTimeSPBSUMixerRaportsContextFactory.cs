using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MixerReports.lib.Data.Base
{
    public class DesignTimeSPBSUMixerRaportsContextFactory : IDesignTimeDbContextFactory<SPBSUMixerRaportsEntities>
    {
        public SPBSUMixerRaportsEntities CreateDbContext(string[] args)
        {
            var options = new DbContextOptionsBuilder<SPBSUMixerRaportsEntities>()
                .UseSqlServer(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=SPBSUMixerRaportsDev.DB").Options;
            return new(options);
        }
    }
}
