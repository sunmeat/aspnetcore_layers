using Soccer.DAL.EF;            
using Soccer.DAL.Interfaces;   
using Soccer.DAL.Entities;   

namespace Soccer.DAL.Repositories 
{
    /*
     * патерн unit of work спрощує роботу з різними репозиторіями та гарантує,
     * що всі репозиторії використовують один і той же контекст даних.
    */
    public class EFUnitOfWork : IUnitOfWork
    {
        private SoccerContext db; 
        private PlayerRepository playerRepository; // поле для кешування репозиторію гравців
        private TeamRepository teamRepository;     // поле для кешування репозиторію команд
        public EFUnitOfWork(SoccerContext context) // конструктор, що приймає готовий контекст
        {
            db = context; // ініціалізація поля контекстом, переданим через di
        }
        public IRepository<Team> Teams // властивість для доступу до репозиторію команд
        {
            get
            {
                if (teamRepository == null) // перевірка, чи вже створено репозиторій
                    teamRepository = new TeamRepository(db); // створення репозиторію команд за потреби (lazy initialization)
                return teamRepository; // повернення єдиного екземпляра репозиторію
            }
        }
        public IRepository<Player> Players // властивість для доступу до репозиторію гравців
        {
            get
            {
                if (playerRepository == null) // перевірка, чи вже створено репозиторій
                    playerRepository = new PlayerRepository(db); // створення репозиторію гравців за потреби
                return playerRepository; // повернення єдиного екземпляра репозиторію
            }
        }
        public async Task Save() // асинхронний метод для збереження всіх змін
        {
            await db.SaveChangesAsync(); // виклик асинхронного збереження змін у контексті бази даних
        }
    }
}

/* // можна було б і так зробити, вийде навіть краще:
using Soccer.DAL.EF;
using Soccer.DAL.Interfaces;
using Soccer.DAL.Entities;

namespace Soccer.DAL.Repositories
{
    public class EFUnitOfWork : IUnitOfWork
    {
        private readonly SoccerContext _db;
        private readonly IRepository<Team> _teams;
        private readonly IRepository<Player> _players;

        public EFUnitOfWork(
            SoccerContext db,
            IRepository<Team> teams,
            IRepository<Player> players) // всі реалізації будуть надані інфраструктурою асп нет, не треба буде нью
        {
            _db = db;
            _teams = teams;
            _players = players;
        }

        public IRepository<Team> Teams => _teams;
        public IRepository<Player> Players => _players;

        public async Task Save()
        {
            await _db.SaveChangesAsync();
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
} */ // але тоді і інфраструктурі середнього рівня треба зареєструвати репозиторії: (Soccer.BLL Infrastructure SoccerContextExtensions.cs)
/*
public static void AddSoccerDal(this IServiceCollection services)
        {
            services.AddScoped<IRepository<Team>, TeamRepository>();
            services.AddScoped<IRepository<Player>, PlayerRepository>();

            services.AddScoped<IUnitOfWork, EFUnitOfWork>();
        }
*/ // ну і ще в Program.cs буде таке:
// builder.Services.AddSoccerDal(); // <-- реєструє репозиторії + UnitOfWork
