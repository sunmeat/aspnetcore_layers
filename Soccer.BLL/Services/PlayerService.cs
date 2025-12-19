using Soccer.BLL.DTO;
using Soccer.DAL.Entities;
using Soccer.DAL.Interfaces;
using Soccer.BLL.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;
using System.ComponentModel.DataAnnotations;

namespace Soccer.BLL.Services
{
    public class PlayerService : IEntityService<PlayerDTO>
    {
        IUnitOfWork Database { get; set; } // юніт оф ворк для доступу до репозиторіїв, буде створений через DI

        public PlayerService(IUnitOfWork uow)
        {
            Database = uow;
        }

        public async Task Create(PlayerDTO playerDto)
        {
            var player = new Player // приклад ручного мапінгу DTO в Entity
            {
                Id = playerDto.Id,
                Name = playerDto.Name,
                Age = playerDto.Age,
                Position = playerDto.Position,
                TeamId = playerDto.TeamId
            };

            await Database.Players.Create(player); // створюємо сутність гравця
            await Database.Save(); // зберігаємо зміни в БД
        }

        public async Task Update(PlayerDTO playerDto)
        {
            var player = new Player
            {
                Id = playerDto.Id,
                Name = playerDto.Name,
                Age = playerDto.Age,
                Position = playerDto.Position,
                TeamId = playerDto.TeamId
            };

            Database.Players.Update(player); // оновлюємо сутність
            await Database.Save(); // зберігаємо зміни
        }

        public async Task Delete(int id)
        {
            await Database.Players.Delete(id); // видаляємо гравця за ідентифікатором
            await Database.Save(); // зберігаємо зміни
        }

        public async Task<PlayerDTO> Get(int id)
        {
            var player = await Database.Players.Get(id); // отримуємо гравця за ідентифікатором із БД
            if (player == null)
                throw new ValidationException("Немає такого гравця!"); // викидаємо виключення, якщо гравця не знайдено

            return new PlayerDTO
            {
                Id = player.Id,
                Name = player.Name,
                Age = player.Age,
                Position = player.Position,
                TeamId = player.TeamId,
                Team = player.Team?.Name // назва команди, якщо вона завантажена
            };
        }

        // automapper дозволяє проеціювати одну модель на іншу, що зменшує обсяг коду та спрощує програму
        public async Task<IEnumerable<PlayerDTO>> GetAll()
        {
            // конфігурація з двома параметрами (другий — ILoggerFactory)
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Player, PlayerDTO>()
                    .ForMember(d => d.Team, o => o.MapFrom(s => s.Team != null ? s.Team.Name : null)); // вью не потрібне посилання на команду, тільки її назва
                /* d => d.Team — вказує на властивість Team у цільовому типі (PlayerDTO), куди потрібно записати значення
                 * o => o.MapFrom(s => s.Team != null ? s.Team.Name : null) — визначає джерело значення:
                 * з вихідного об’єкта (Player) береться властивість Team, і якщо вона не null, то мапиться лише її Name (рядок), інакше — null
                 */
            },
            NullLoggerFactory.Instance); // логування не передбачене, поки що :)

            // створюємо мапер і мапимо дані
            return config.CreateMapper()
                         .Map<IEnumerable<PlayerDTO>>(await Database.Players.GetAll());
        }
    }
}