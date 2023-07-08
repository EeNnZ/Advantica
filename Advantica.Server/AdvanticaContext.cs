using Advantica.Server.Entities;
using Microsoft.EntityFrameworkCore;

namespace Advantica.Server
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
                .HasData(Enum.GetValues<Advantica.Server.Protos.Sex>()
                .Cast<Advantica.Server.Protos.Sex>()
                .Select(s => new Sex()
                {
                    Id = (int)s,
                    Name = s.ToString(),
                }));
        }
    }
}
