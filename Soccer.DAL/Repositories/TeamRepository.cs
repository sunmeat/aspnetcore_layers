using Microsoft.EntityFrameworkCore; 
using Soccer.DAL.Entities;           
using Soccer.DAL.Interfaces;         
using Soccer.DAL.EF;                 
namespace Soccer.DAL.Repositories    
{
    public class TeamRepository : IRepository<Team> 
    {
        private SoccerContext db; 

        public TeamRepository(SoccerContext context) 
        {
            this.db = context; 
        }

        public async Task<IEnumerable<Team>> GetAll() // отримання всіх команд
        {
            return await db.Teams.ToListAsync(); // повертаємо список всіх команд без додаткового завантаження пов'язаних даних
        }

        public async Task<Team?> Get(int id) // отримання команди за ідентифікатором
        {
            Team? team = await db.Teams.FindAsync(id); // пошук за первинним ключем асинхронно
            return team; // повертаємо знайдену команду або null
        }

        public async Task<Team?> Get(string name) // отримання команди за назвою
        {
            var teams = await db.Teams.Where(a => a.Name == name).ToListAsync(); // фільтруємо за точним збігом назви
            Team? team = teams?.FirstOrDefault(); // беремо першу знайдену команду
            return team; // повертаємо команду або null
        }

        public async Task Create(Team team) // створення нової команди
        {
            await db.Teams.AddAsync(team); // додаємо сутність до dbset асинхронно (зміни зберігаються пізніше)
        }

        public void Update(Team team) // оновлення команди
        {
            db.Entry(team).State = EntityState.Modified; // явно позначаємо сутність як змінену для відстеження EF
            // при виклику SaveChanges() фреймворк згенерує і виконає SQL-команду UPDATE для оновлення відповідного запису в БД
        }

        public async Task Delete(int id) // видалення команди за ідентифікатором
        {
            Team? team = await db.Teams.FindAsync(id); // пошук за первинним ключем
            if (team != null) // перевірка на існування
                db.Teams.Remove(team); // видаляємо сутність з dbset (зміни застосовуються при savechanges)
        }
    }
}