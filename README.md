# Soccer

Навчальний вебзастосунок на **ASP.NET Core MVC**, побудований за багаторівневою архітектурою з розділенням відповідальності між Web, Business Logic та Data Access шарами.

Проєкт є розвитком прикладу `One-to-Many` і демонструє, як винести роботу з базою даних та бізнес-логіку з контролерів у окремі рівні застосунку.

## Архітектура

Застосунок складається з трьох основних проєктів:

```text
┌─────────────────────────────┐
│          Soccer             │
│       ASP.NET Core MVC      │
│        Presentation         │
└──────────────┬──────────────┘
               │
               ▼
┌─────────────────────────────┐
│        Soccer.BLL           │
│   Business Logic Layer      │
│   Services / DTO / Mapping  │
└──────────────┬──────────────┘
               │
               ▼
┌─────────────────────────────┐
│        Soccer.DAL           │
│     Data Access Layer       │
│ EF Core / Repository / UoW  │
└──────────────┬──────────────┘
               │
               ▼
        ┌─────────────┐
        │ SQL Server  │
        └─────────────┘
```

Таке розділення відповідає принципу **Separation of Concerns**: UI, бізнес-логіка та доступ до даних не змішуються в одному проєкті. Окремі проєкти також забезпечують кращу інкапсуляцію залежностей між шарами. citeturn0search10

## Структура solution

```text
Soccer.sln
│
├── Soccer/
│   ├── Controllers/
│   │   ├── PlayersController.cs
│   │   └── TeamsController.cs
│   ├── Views/
│   ├── Program.cs
│   └── Soccer.csproj
│
├── Soccer.BLL/
│   ├── DTO/
│   │   ├── PlayerDTO.cs
│   │   └── TeamDTO.cs
│   ├── Infrastructure/
│   │   ├── SoccerContextExtensions.cs
│   │   ├── UnitOfWorkServiceExtensions.cs
│   │   └── ValidationException.cs
│   ├── Interfaces/
│   │   └── IEntityService.cs
│   ├── Services/
│   │   ├── PlayerService.cs
│   │   └── TeamService.cs
│   └── Soccer.BLL.csproj
│
└── Soccer.DAL/
    ├── EF/
    │   └── SoccerContext.cs
    ├── Entities/
    │   ├── Player.cs
    │   └── Team.cs
    ├── Interfaces/
    │   ├── IRepository.cs
    │   └── IUnitOfWork.cs
    ├── Repositories/
    │   ├── EFUnitOfWork.cs
    │   ├── PlayerRepository.cs
    │   └── TeamRepository.cs
    └── Soccer.DAL.csproj
```

## Presentation Layer

Проєкт `Soccer` є вебрівнем застосунку та відповідає за взаємодію з користувачем.

Використовується:

- ASP.NET Core MVC;
- Controllers;
- Razor Views;
- Dependency Injection;
- маршрутизація MVC;
- статичні файли.

Контролери `TeamsController` та `PlayersController` не працюють із `DbContext` безпосередньо. Замість цього вони взаємодіють із сервісами бізнес-рівня.

Стандартний маршрут налаштований на:

```text
/Teams/Index
```

що дозволяє відразу відкрити список команд.

## Business Logic Layer

`Soccer.BLL` відповідає за бізнес-логіку застосунку.

У цьому шарі знаходяться:

- DTO;
- інтерфейси сервісів;
- сервіси команд та гравців;
- валідація;
- конфігурація залежностей;
- мапінг Entity → DTO.

Основні сервіси:

```text
IEntityService<TeamDTO>
        │
        └── TeamService

IEntityService<PlayerDTO>
        │
        └── PlayerService
```

Наприклад, `PlayerService` реалізує операції:

```text
Create
Update
Delete
Get
GetAll
```

Сервіс отримує `IUnitOfWork` через Dependency Injection і працює з репозиторіями, не знаючи деталей реалізації Entity Framework Core.

## DTO

Для передачі даних між шарами використовуються Data Transfer Objects:

```text
PlayerDTO
TeamDTO
```

Наприклад, `PlayerDTO` містить дані гравця та може містити назву команди, але вебрівню не потрібно отримувати повний об'єкт Entity Framework.

Це дозволяє не передавати DAL-сутності безпосередньо до View.

## Data Access Layer

`Soccer.DAL` відповідає за доступ до бази даних.

Використовуються:

- Entity Framework Core;
- SQL Server;
- Repository Pattern;
- Unit of Work Pattern;
- Entity-моделі.

Основні сутності:

```text
Team
Player
```

Зв'язок між ними:

```text
Team 1 ─────────── * Player
```

Одна команда може мати багато гравців, а кожен гравець належить одній команді.

## Repository Pattern

Доступ до даних абстрагований через репозиторії:

```text
IRepository<T>
     │
     ├── PlayerRepository
     │
     └── TeamRepository
```

Репозиторії інкапсулюють операції з Entity Framework Core та дозволяють бізнес-рівню працювати з абстракцією, а не з конкретним `DbContext`.

## Unit of Work

Для координації роботи декількох репозиторіїв використовується патерн **Unit of Work**.

Інтерфейс `IUnitOfWork` надає доступ до:

```csharp
IRepository<Team> Teams
IRepository<Player> Players
```

а також метод:

```csharp
Task Save()
```

Тому кілька змін можна виконати через різні репозиторії, а потім централізовано зберегти їх одним викликом.

```text
                 IUnitOfWork
                      │
             ┌────────┴────────┐
             ▼                 ▼
        Teams repo        Players repo
             │                 │
             └────────┬────────┘
                      ▼
                    Save()
                      │
                      ▼
               SaveChangesAsync()
```

Це допомагає централізувати збереження змін і не розповсюджувати виклики `SaveChangesAsync()` по всьому застосунку.

## AutoMapper

У `Soccer.BLL` використовується **AutoMapper** для перетворення Entity у DTO.

Приклад логіки:

```text
Player Entity
      │
      ▼
   AutoMapper
      │
      ▼
PlayerDTO
```

Для `Player` додатково налаштовано отримання назви команди:

```csharp
.ForMember(
    d => d.Team,
    o => o.MapFrom(
        s => s.Team != null ? s.Team.Name : null
    )
);
```

Таким чином View отримує необхідні дані без прямої залежності від Entity Framework-моделі.

## Dependency Injection

У `Program.cs` реєструються основні залежності:

```csharp
builder.Services.AddSoccerContext(connection);
builder.Services.AddUnitOfWorkService();

builder.Services.AddTransient<
    IEntityService<TeamDTO>,
    TeamService>();

builder.Services.AddTransient<
    IEntityService<PlayerDTO>,
    PlayerService>();
```

У результаті залежності будуються приблизно так:

```text
Controller
    │
    ▼
IEntityService<T>
    │
    ▼
BLL Service
    │
    ▼
IUnitOfWork
    │
    ▼
Repository
    │
    ▼
Entity Framework Core
    │
    ▼
SQL Server
```

## Технології

- **C#**
- **.NET 10**
- **ASP.NET Core MVC**
- **Entity Framework Core 10**
- **Microsoft SQL Server**
- **AutoMapper 16**
- **Razor Views**
- **Repository Pattern**
- **Unit of Work Pattern**
- **Dependency Injection**
- **DTO**

DAL використовує `Microsoft.EntityFrameworkCore.SqlServer` версії `10.0.1`, BLL використовує `AutoMapper` версії `16.0.0`, а всі три проєкти орієнтовані на `net10.0`. fileciteturn6file0 fileciteturn7file0 fileciteturn8file0

## Налаштування підключення

Рядок підключення до SQL Server отримується з конфігурації:

```csharp
string? connection =
    builder.Configuration.GetConnectionString(
        "DefaultConnection");
```

Приклад конфігурації:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=Soccer;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

Змініть параметри `Server` та автентифікації відповідно до вашого SQL Server.

Не зберігайте паролі та інші секрети безпосередньо в Git-репозиторії.

## Запуск проєкту

### Клонування

```bash
git clone https://github.com/sunmeat/aspnetcore_layers.git
cd aspnetcore_layers
```

### Перевірка .NET SDK

Для запуску потрібен **.NET 10 SDK**:

```bash
dotnet --version
```

### Налаштування бази даних

Переконайтеся, що SQL Server доступний, а `DefaultConnection` у конфігурації вказує на потрібну базу даних.

### Відновлення залежностей

```bash
dotnet restore
```

### Запуск

З кореня solution:

```bash
dotnet run --project Soccer
```

Після запуску відкрийте адресу, яку покаже ASP.NET Core.

Стартовий маршрут:

```text
/Teams/Index
```

## CRUD

Для команд та гравців реалізовані стандартні операції:

```text
Create
Read
Update
Delete
```

При цьому потік виконання проходить через усі рівні:

```text
HTTP Request
     │
     ▼
Controller
     │
     ▼
BLL Service
     │
     ▼
Unit of Work
     │
     ▼
Repository
     │
     ▼
Entity Framework Core
     │
     ▼
SQL Server
```

Таким чином Controller не містить логіку доступу до бази даних, а DAL не залежить від MVC.

## Навчальна мета

Проєкт демонструє практичне застосування:

- багаторівневої архітектури;
- Separation of Concerns;
- ASP.NET Core MVC;
- Entity Framework Core;
- SQL Server;
- Repository Pattern;
- Unit of Work Pattern;
- Dependency Injection;
- DTO;
- AutoMapper;
- CRUD;
- зв'язку One-to-Many;
- асинхронної роботи з базою даних.

Проєкт можна використовувати як навчальний приклад переходу від простого MVC-застосунку, де Controller безпосередньо працює з `DbContext`, до більш структурованої багаторівневої архітектури.

## Ліцензія

У репозиторії міститься `LICENSE.txt`. Умови використання визначаються відповідно до ліцензії проєкту.
