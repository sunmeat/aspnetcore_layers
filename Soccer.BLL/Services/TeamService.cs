using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;
using Soccer.BLL.DTO;
using Soccer.BLL.Interfaces;
using Soccer.DAL.Entities;
using Soccer.DAL.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Soccer.BLL.Services
{
    public class TeamService : IEntityService<TeamDTO>
    {
        IUnitOfWork Database { get; set; } // юніт оф ворк для доступу до репозиторіїв

        public TeamService(IUnitOfWork uow)
        {
            Database = uow;
        }

        public async Task Create(TeamDTO teamDto)
        {
            var team = new Team
            {
                Id = teamDto.Id,
                Name = teamDto.Name,
                Coach = teamDto.Coach
            };

            await Database.Teams.Create(team); // створюємо сутність команди
            await Database.Save(); // зберігаємо зміни
        }

        public async Task Update(TeamDTO teamDto)
        {
            var team = new Team
            {
                Id = teamDto.Id,
                Name = teamDto.Name,
                Coach = teamDto.Coach
            };

            Database.Teams.Update(team); // оновлюємо сутність
            await Database.Save(); // зберігаємо зміни
        }

        public async Task Delete(int id)
        {
            await Database.Teams.Delete(id); // видаляємо команду за ідентифікатором
            await Database.Save(); // зберігаємо зміни
        }

        public async Task<TeamDTO> Get(int id)
        {
            var team = await Database.Teams.Get(id);
            if (team == null)
                throw new ValidationException("Немає такого клуба!"); // викидаємо виключення, якщо команду не знайдено

            return new TeamDTO
            {
                Id = team.Id,
                Name = team.Name,
                Coach = team.Coach
            };
        }

        public async Task<IEnumerable<TeamDTO>> GetAll()
        {
            var config = new MapperConfiguration(cfg =>
                cfg.CreateMap<Team, TeamDTO>(), NullLoggerFactory.Instance); // конфігурація automapper для проекції Team в TeamDTO
            // мепінг простіше, тому що властивості збігаються за назвою та типами
            return config.CreateMapper()
                         .Map<IEnumerable<TeamDTO>>(await Database.Teams.GetAll()); // мапимо всі сутності команд на dto
        }
    }
}