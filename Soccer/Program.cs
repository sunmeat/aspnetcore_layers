using Microsoft.EntityFrameworkCore;
using Soccer.BLL.Interfaces;
using Soccer.BLL.Services;
using Soccer.BLL.Infrastructure;
using Soccer.BLL.DTO;

var builder = WebApplication.CreateBuilder(args);

// отримуємо рядок підключення з конфігурації (звісно, там треба налаштувати правильне джерело в appsettings.json)
string? connection = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddSoccerContext(connection); // реєструємо контекст бази даних
builder.Services.AddUnitOfWorkService(); // реєструємо юніт оф ворк
builder.Services.AddTransient<IEntityService<TeamDTO>, TeamService>(); // реєструємо сервіс команд
builder.Services.AddTransient<IEntityService<PlayerDTO>, PlayerService>(); // реєструємо сервіс гравців

// додаємо сервіси mvc
builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseStaticFiles(); // запити до статичних файлів у wwwroot

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Teams}/{action=Index}/{id?}"); // налаштовуємо стандартний маршрут

app.Run();