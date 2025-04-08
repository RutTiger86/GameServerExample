using AuthDB.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthDB.Data
{
    public class AuthDbContext : DbContext
    {

        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
        {
        }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Character> Characters { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Character>()
                .HasOne(c => c.Account)
                .WithMany(a => a.Characters)
                .HasForeignKey(c => c.AccountId)
                .OnDelete(DeleteBehavior.Cascade); // 계정 삭제 시 캐릭터도 삭제
        }
    }
}
