using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Soccer.DAL.EF;

namespace Soccer.BLL.Infrastructure
{
    public static class SoccerContextExtensions
    {
        public static void AddSoccerContext(this IServiceCollection services, string? connection)
        {

            /// <summary>
            /// Реєструє SoccerContext у контейнері залежностей (DI).
            /// Метод розміщено саме в шарі BLL (Business Logic Layer), тому що:
            /// - Шар презентації (наприклад, Controllers у веб-додатку) не повинен знати про існування Entity Framework чи конкретної БД — це порушує принцип розділення відповідальностей.
            /// - А шар DAL (Data Access Layer) не має доступу до IServiceCollection, бо реєстрація сервісів відбувається на рівні запуску додатка (Program.cs або Startup.cs), а DAL — це бібліотека класів, яка не містить точки входу.
            /// Тому реєстрація контексту винесена в окремий infrastructure-шар всередині BLL, який виступає "мостом" між DAL і рівнем запуску додатка.
            /// </summary>
            services.AddDbContext<SoccerContext>(options =>
                options.UseSqlServer(connection)); // реєструємо контекст для роботи з sql server
        }
    }
}