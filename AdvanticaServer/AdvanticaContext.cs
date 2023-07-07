using AdvanticaServer.Entities;
using Microsoft.EntityFrameworkCore;

namespace AdvanticaServer
{
    public class AdvanticaContext : DbContext
    {
        public DbSet<Worker> Workers { get; set; }
        public DbSet<Sex> Sex { get; set; }

        public AdvanticaContext() { }
        public AdvanticaContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Sex>()
                .HasData(Enum.GetValues<AdvanticaServer.Protos.Sex>()
                .Cast<AdvanticaServer.Protos.Sex>()
                .Select(s => new Sex()
                {
                    Id = (int)s,
                    Name = s.ToString(),
                }));
        }
    }
}
