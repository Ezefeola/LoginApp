using AppLogin.Models;
using Microsoft.EntityFrameworkCore;

namespace AppLogin.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(tb =>
            {
                tb.HasKey(col => col.IdUser);
                tb.Property(col => col.IdUser)
                .UseIdentityColumn()
                .ValueGeneratedOnAdd();

                tb.Property(col => col.FullName).HasMaxLength(50);
                tb.Property(col => col.Email).HasMaxLength(50);
                tb.Property(col => col.Password).HasMaxLength(50);
            });

            modelBuilder.Entity<User>().ToTable("User");
        }
    }
}
