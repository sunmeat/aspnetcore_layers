using Microsoft.Extensions.DependencyInjection;
using Soccer.DAL.Interfaces;
using Soccer.DAL.Repositories;

namespace Soccer.BLL.Infrastructure
{
    public static class UnitOfWorkServiceExtensions
    {
        public static void AddUnitOfWorkService(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, EFUnitOfWork>(); // реєструємо unit of work з реалізацією на entity framework
        }
    }
}