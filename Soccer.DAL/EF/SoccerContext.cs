using Microsoft.EntityFrameworkCore; 
using Soccer.DAL.Entities; // підключення простору імен з сутностями моделі даних (team та player)

namespace Soccer.DAL.EF
{
    public class SoccerContext : DbContext 
    {
        public DbSet<Team> Teams { get; set; } // властивість для доступу до таблиці команд у базі даних
        public DbSet<Player> Players { get; set; } 
        public SoccerContext(DbContextOptions<SoccerContext> options) : base(options)                                         
        {
            Database.EnsureCreated();
        }
    }
}