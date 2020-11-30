using Charts.Shared.Data.Context.Dictionary;
using Microsoft.EntityFrameworkCore;

namespace Charts.Shared.Data.Context
{
    public class DataContext : DbContext
    {
        public DataContext()
        {
        }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<User> Users { get; set; }
        
        public DbSet<Role> Roles { get; set; }
        public DbSet<Application> Applications { get; set; }
        public DbSet<DicContractors> DicContractors { get; set; }
        public DbSet<DicDefect> DicDefect { get; set; }
        public DbSet<DicRepairPlace> DicRepairPlace { get; set; }
        public DbSet<Remarks> Remarks { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<DocInformation> DocInformation { get; set; }
        public DbSet<TTSFile> TTSFile { get; set; }
        
        public DbSet<ApplicationTask> ApplicationTask { get; set; }
        
        public DbSet<ApplicationHistory> ApplicationHistory { get; set; }
        

    }
}
