## bc_handball_be — Backend (README)

This repository contains the backend for the BC Handball application. Below you'll find the overall structure, an explanation of each project (`.API`, `.Core`, `.Infrastructure`), how dependency injection and mapping are wired, how to run the code locally, Docker guidance, database migration commands, and the typical request workflow.

## Repository high-level structure

- `bc_handball_be.sln` — solution file
- `bc_handball_be.API/` — Web API project (controllers, API-specific DTOs, mapping, Program.cs)
  - `Controllers/` — API endpoints (HTTP layer)
  - `DTOs/` — API-specific data transfer objects (optional, for API-specific concerns)
  - `Mapping/MappingProfile.cs` — AutoMapper configuration mapping between Core models and API DTOs
  - `Program.cs` — application entrypoint and dependency injection configuration
- `bc_handball_be.Core/` — core domain logic, entities, service contracts and models
  - `Interfaces/` — service and repository interfaces (contracts)
  - `Services/` — business logic implementations (orchestrate repositories, validation, business rules)
  - `Entities/` — domain entities (database models)
  - `Models/` or `DTOs/` — service layer models/DTOs (used by services and controllers)
- `bc_handball_be.Infrastructure/` — persistence and external implementation
  - `Persistence/` — EF Core DbContext and configuration
  - `Repositories/` — repository implementations (simple CRUD, no business logic, no calling other repositories)
  - `Migrations/` — EF Core migrations

Other files:

- `docker-compose.yml` — docker-compose orchestration (DB, maybe API)
- `Dockerfile` — Dockerfile for the API container

## What each project is for

### Dependency flow (Critical!)

```
Frontend → API → Core ← Infrastructure
         (outer) → (inner) ← (data access)
```

**Rule**: Dependencies flow toward Core (the inner layer).

- ✅ API references Core (controllers use service interfaces)
- ✅ Infrastructure references Core (repositories implement interfaces defined in Core)
- ✅ Core references nothing (it only defines interfaces and entities)
- ❌ Core should NEVER reference API
- ❌ Core should NEVER reference Infrastructure
- ❌ Repositories should NEVER call other repositories

**Key concept**: Core defines `IRepository` interfaces, Infrastructure implements them. Services in Core depend on the _interface_ (`IPlayerRepository`), not the concrete implementation (`PlayerRepository`).

---

- `bc_handball_be.API` (the web API / presentation layer)

  - Contains all Controllers that expose HTTP endpoints to the frontend.
  - **May** hold API-specific DTO classes if needed (e.g., with validation attributes, API-specific shapes).
  - Contains mapping configuration (AutoMapper `MappingProfile`) which translates between Core models and API DTOs.
  - `Program.cs` is where services are configured and registered in the DI container (AddScoped, AddTransient, AddSingleton) and middleware, authentication, and the web host are set up.
  - **Dependencies**: References `Core` (never references `Infrastructure` directly in production code).

- `bc_handball_be.Core` (business logic layer)

  - **The heart of the application** - contains all business logic, domain entities, and service contracts.
  - Defines interfaces (contracts) for services (`IPlayerService`, `ITeamService`, etc.) and repositories (`IPlayerRepository`, etc.).
  - Contains **service implementations** in `Services/` folder - these orchestrate multiple repositories, implement validation, business rules, and cross-cutting concerns.
  - Contains domain **entities** in `Entities/` (the actual database models).
  - **Should contain Models/DTOs** used by services - these are service-layer data contracts (NOT tied to API or HTTP).
  - **Dependencies**: References nothing (or only lightweight packages like AutoMapper abstractions). Never references API or Infrastructure.
  - **Responsibilities**:
    - Define service interfaces and implement business logic
    - Orchestrate multiple repositories (e.g., a service can call multiple repositories)
    - Validate input, implement business rules, handle transactions
    - Return domain models or Core DTOs back to controllers

- `bc_handball_be.Infrastructure` (data access layer)
  - Implements low-level details: database access (Entity Framework Core), concrete repositories, migrations and any external system integration.
  - Repositories here implement repository interfaces declared in `Core.Interfaces`.
  - **Repositories should be simple**: only CRUD operations for a single entity/aggregate.
  - **Repositories should NEVER**:
    - Call other repositories (move that logic to services)
    - Contain business logic (move to services)
    - Implement complex queries spanning multiple aggregates (move to services)
  - **Dependencies**: References `Core` to implement its interfaces.
  - This layer should be the only place that knows about EF Core directly.

## Typical request flow (Front-end → DB)

1. Frontend issues an HTTP request to an endpoint (e.g., POST /api/team).
2. The **Controller** in `bc_handball_be.API/Controllers` receives the request. It may map API-specific DTOs to Core models if needed.
3. The Controller calls an **IService** (from `Core.Interfaces`) via constructor-injected dependency.
4. The **Service implementation** (in `Core/Services`) executes business logic:
   - Validates inputs and applies business rules
   - **Orchestrates multiple repositories** if needed (e.g., checking permissions, coordinating data from multiple sources)
   - Handles transactions spanning multiple repository calls
   - Transforms data between entities and Core models/DTOs
5. The Service calls one or more **IRepository** interfaces (from `Core.Interfaces`) to persist or query data.
6. The concrete **Repository implementation** (in `Infrastructure/Repositories`) uses EF Core DbContext to run **simple CRUD operations**.
   - Repositories should NOT call other repositories (this logic belongs in services)
   - Repositories should NOT contain business logic
7. Results bubble back: Repository → Service → Controller → HTTP response returned to the frontend.

**In short**: Frontend → Controller → IService → Service (orchestrates) → IRepository (simple CRUD) → EF Core → Database.

### Key architectural principles:

- **Controllers**: Thin HTTP layer, delegates to services
- **Services**: Fat business logic layer, orchestrates repositories, handles transactions
- **Repositories**: Thin data access, simple CRUD only, no business logic, no calling other repositories
- **Dependency flow**: API → Core ← Infrastructure (Core references nothing)

## Dependency Injection and Program.cs

- `Program.cs` is the main place where:
  - services.AddScoped/AddTransient/AddSingleton registrations are made for controllers, services, repositories,
  - AutoMapper and EF Core DbContext are configured,
  - Middleware (authentication, authorization, CORS, logging) are configured.

If you want to add a new service:

1. Define an interface in `bc_handball_be.Core/Interfaces`, e.g., `IMyService`.
2. Implement it in `bc_handball_be.Core/Services` (business logic lives in Core, not Infrastructure).
3. If you need DTOs/models for the service, create them in `bc_handball_be.Core/Models` or `bc_handball_be.Core/DTOs`.
4. Register implementation in `Program.cs` (e.g., `services.AddScoped<IMyService, MyService>();`)
5. Inject `IMyService` into any Controller constructor.

**Important notes:**

- Services live in `.Core/Services/` (not in Infrastructure)
- DTOs used by services should live in `.Core/Models/` or `.Core/DTOs/` (not in API)
- API can have its own DTOs in `.API/DTOs/` for API-specific concerns, then map to Core models in controllers
- Services can call multiple repositories - this is the correct place for orchestration logic
- Repositories should never call other repositories

## How to run the API locally (dotnet)

From the repository root you can either change directory to the API project and run:

PowerShell example (recommended):

```powershell
cd .\bc_handball_be.API
dotnet run
```

Or run directly from the solution root specifying the project:

```powershell
dotnet run --project .\bc_handball_be.API\bc_handball_be.API.csproj
```

This starts the web host and listens on the configured URL(s) (see `appsettings.json` or output logged by `dotnet run`).

## Docker (start, restart, check running instances)

- Install Docker Desktop (Windows) if you don't have it: https://www.docker.com/get-started
- To bring up services defined in `docker-compose.yml` (background):

```powershell
docker-compose up -d
```

- To rebuild images and start fresh:

```powershell
docker-compose up --build -d
```

- To stop and remove containers:

```powershell
docker-compose down
```

- To restart containers managed by compose:

```powershell
docker-compose restart
```

- To check running containers and their status:

```powershell
docker ps
docker-compose ps
```

- To follow logs from services:

```powershell
docker-compose logs -f
```

Note: If the API is exposed as a container, docker-compose will start it too (if defined). If the compose file only runs a DB service, run the API locally with `dotnet run` while the DB runs in Docker.

## Database migrations (EF Core)

Prerequisites:

- Make sure `dotnet-ef` CLI is available. Install or update globally if needed:

```powershell
dotnet tool install --global dotnet-ef
# or to update
dotnet tool update --global dotnet-ef
```

- Ensure the `bc_handball_be.Infrastructure` project has the `Microsoft.EntityFrameworkCore.Design` package installed (check the `.csproj`).

Typical commands (from repo root):

Add a migration (name it meaningfully):

```powershell
dotnet ef migrations add AddSomething --project .\bc_handball_be.Infrastructure\bc_handball_be.Infrastructure.csproj --startup-project .\bc_handball_be.API\bc_handball_be.API.csproj
```

Apply (update) the database:

```powershell
dotnet ef database update --project .\bc_handball_be.Infrastructure\bc_handball_be.Infrastructure.csproj --startup-project .\bc_handball_be.API\bc_handball_be.API.csproj
```

Notes:

- The `--project` points to where the EF DbContext/migrations live (typically `Infrastructure`).
- The `--startup-project` points to the app that configures services and the connection string (typically `API`).
- Ensure the connection string in `bc_handball_be.API/appsettings.json` (or environment) is correct before running `database update`.

## End-to-end example: Full request flow

Below is a simplified example showing how a request travels through the layers with **correct architectural patterns**. We'll create a small feature to get a team by ID.

### 1. Entity (bc_handball_be.Core/Entities/Team.cs)

**Entities are domain models (database models) in Core**

```csharp
namespace bc_handball_be.Core.Entities
{
    public class Team
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public int ClubId { get; set; }
        public Club Club { get; set; }
        // navigation properties, other properties...
    }
}
```

### 2. API DTO (bc_handball_be.API/DTOs/TeamDTO.cs)

**DTOs live in API project (HTTP layer only)**

```csharp
namespace bc_handball_be.API.DTOs
{
    public class TeamDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } // Enriched from navigation property
        public int ClubId { get; set; }
        // API-specific properties...
    }

    public class TeamCreateDTO
    {
        public string Name { get; set; }
        public int CategoryId { get; set; }
        public int ClubId { get; set; }
    }
}
```

### 3. Repository Interface (bc_handball_be.Core/Interfaces/IRepositories/ITeamRepository.cs)

**Simple CRUD only - no business logic!**

```csharp
using bc_handball_be.Core.Entities;

namespace bc_handball_be.Core.Interfaces.IRepositories
{
    public interface ITeamRepository
    {
        Task<Team?> GetByIdAsync(int id);
        Task<IEnumerable<Team>> GetAllAsync();
        Task<IEnumerable<Team>> GetByCategoryIdAsync(int categoryId);
        Task AddAsync(Team team);
        Task UpdateAsync(Team team);
        Task DeleteAsync(int id);
        // NO complex queries, NO calling other repositories
    }
}
```

### 4. Repository Implementation (bc_handball_be.Infrastructure/Repositories/TeamRepository.cs)

**Keep it simple - just data access!**

```csharp
using bc_handball_be.Core.Entities;
using bc_handball_be.Core.Interfaces.IRepositories;
using bc_handball_be.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace bc_handball_be.Infrastructure.Repositories
{
    public class TeamRepository : ITeamRepository
    {
        private readonly ApplicationDbContext _context;

        public TeamRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Team?> GetByIdAsync(int id)
        {
            return await _context.Teams
                .Include(t => t.Category)
                .Include(t => t.Club)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<IEnumerable<Team>> GetByCategoryIdAsync(int categoryId)
        {
            return await _context.Teams
                .Include(t => t.Category)
                .Include(t => t.Club)
                .Where(t => t.CategoryId == categoryId)
                .ToListAsync();
        }

        public async Task AddAsync(Team team)
        {
            await _context.Teams.AddAsync(team);
            await _context.SaveChangesAsync();
        }

        // NO business logic here!
        // NO calling other repositories!
    }
}
```

### 5. Service Interface (bc_handball_be.Core/Interfaces/IServices/ITeamService.cs)

**Services work with Entities, not DTOs**

```csharp
using bc_handball_be.Core.Entities;

namespace bc_handball_be.Core.Interfaces.IServices
{
    public interface ITeamService
    {
        Task<Team?> GetTeamByIdAsync(int id);
        Task<IEnumerable<Team>> GetTeamsByCategoryAsync(int categoryId);
        Task AddTeamAsync(Team team);
        // other service methods...
    }
}
```

### 6. Service Implementation (bc_handball_be.Core/Services/TeamService.cs)

**Business logic and orchestration happens here! Works with Entities.**

```csharp
using bc_handball_be.Core.Entities;
using bc_handball_be.Core.Interfaces.IRepositories;
using bc_handball_be.Core.Interfaces.IServices;

namespace bc_handball_be.Core.Services
{
    public class TeamService : ITeamService
    {
        private readonly ITeamRepository _teamRepository;
        private readonly ICategoryRepository _categoryRepository; // Service can call multiple repositories!

        public TeamService(
            ITeamRepository teamRepository,
            ICategoryRepository categoryRepository)
        {
            _teamRepository = teamRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<Team?> GetTeamByIdAsync(int id)
        {
            // Business logic: validate
            if (id <= 0)
                return null;

            // Get team from repository (returns Entity)
            var team = await _teamRepository.GetByIdAsync(id);
            return team; // Returns Entity to controller
        }

        public async Task<IEnumerable<Team>> GetTeamsByCategoryAsync(int categoryId)
        {
            // Business validation
            var category = await _categoryRepository.GetByIdAsync(categoryId);
            if (category == null)
                throw new ArgumentException("Category not found");

            // Service orchestrates multiple repositories
            return await _teamRepository.GetByCategoryIdAsync(categoryId);
        }

        public async Task AddTeamAsync(Team team)
        {
            // Business validation
            if (string.IsNullOrEmpty(team.Name))
                throw new ArgumentException("Team name is required");

            // Validate category exists
            var category = await _categoryRepository.GetByIdAsync(team.CategoryId);
            if (category == null)
                throw new ArgumentException("Invalid category");

            // Save entity
            await _teamRepository.AddAsync(team);
        }

        // Services can orchestrate complex operations across multiple repositories
        // Services contain ALL business logic
        // Services work with ENTITIES, not DTOs
    }
}
```

### 7. Controller (bc_handball_be.API/Controllers/TeamController.cs)

**Controller maps between DTOs (API) and Entities (Core)**

```csharp
using AutoMapper;
using bc_handball_be.API.DTOs;
using bc_handball_be.Core.Entities;
using bc_handball_be.Core.Interfaces.IServices;
using Microsoft.AspNetCore.Mvc;

namespace bc_handball_be.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TeamController : ControllerBase
    {
        private readonly ITeamService _teamService;
        private readonly IMapper _mapper;

        public TeamController(ITeamService teamService, IMapper mapper)
        {
            _teamService = teamService;
            _mapper = mapper;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTeam(int id)
        {
            // Service returns Entity
            var team = await _teamService.GetTeamByIdAsync(id);
            if (team == null)
                return NotFound();

            // Controller maps Entity → DTO for response
            var teamDto = _mapper.Map<TeamDTO>(team);
            return Ok(teamDto);
        }

        [HttpGet]
        public async Task<IActionResult> GetTeamsByCategory([FromQuery] int categoryId)
        {
            // Service returns Entities
            var teams = await _teamService.GetTeamsByCategoryAsync(categoryId);

            // Controller maps Entities → DTOs for response
            var teamDtos = _mapper.Map<List<TeamDTO>>(teams);
            return Ok(teamDtos);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTeam([FromBody] TeamCreateDTO dto)
        {
            // Controller maps DTO → Entity
            var team = _mapper.Map<Team>(dto);

            // Service works with Entity
            await _teamService.AddTeamAsync(team);

            return CreatedAtAction(nameof(GetTeam), new { id = team.Id }, team);
        }

        // Controllers are thin - just HTTP concerns and DTO ↔ Entity mapping
        // All business logic is in services
    }
}
```

### 8. Dependency Injection Registration (bc_handball_be.API/Program.cs)

```csharp
// Inside Program.cs, register repositories and services:
builder.Services.AddScoped<ITeamRepository, TeamRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ITeamService, TeamService>();
// Services are in Core, Repositories are in Infrastructure
```

### 9. AutoMapper Profile (bc_handball_be.API/Mapping/MappingProfile.cs)

**Maps between Entities (Core) and DTOs (API)**

```csharp
using AutoMapper;
using bc_handball_be.API.DTOs;
using bc_handball_be.Core.Entities;

namespace bc_handball_be.API.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Map between Entity (Core) and DTO (API)
            CreateMap<Team, TeamDTO>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name));

            CreateMap<TeamCreateDTO, Team>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Category, opt => opt.Ignore())
                .ForMember(dest => dest.Club, opt => opt.Ignore());
        }
    }
}
```

### Request flow summary

1. Frontend calls `GET /api/team/5`
2. `TeamController.GetTeam(5)` is invoked
3. Controller calls `_teamService.GetTeamByIdAsync(5)` (service returns Entity)
4. `TeamService` (in Core):
   - Validates input (business logic)
   - Calls `_teamRepository.GetByIdAsync(5)` (simple CRUD)
   - **Can orchestrate multiple repositories** if needed (e.g., checking permissions, loading related data)
   - Returns `Team` **Entity** to Controller
5. `TeamRepository` uses EF Core: `_context.Teams.Include(...).FirstOrDefaultAsync(...)` (just data access)
6. Database returns the Team entity
7. Repository returns entity to Service
8. Service returns entity to Controller
9. **Controller maps Entity → DTO** using `_mapper.Map<TeamDTO>(team)`
10. Controller returns `200 OK` with DTO as JSON to Frontend

**This demonstrates the CORRECT pattern:**

- ✅ Controller is thin (just HTTP + DTO mapping)
- ✅ Service contains business logic and orchestrates multiple repositories
- ✅ Service works with **Entities**, not DTOs
- ✅ Repository is simple CRUD only
- ✅ **DTOs live in API project only** (HTTP layer)
- ✅ **Entities live in Core/Entities/** (domain models)
- ✅ **AutoMapper maps Entity ↔ DTO** in controllers
- ✅ Service can call multiple repositories
- ✅ Repository never calls another repository

## Quick examples and tips

- **Add a new feature (correct architecture):**

  1. Create Entity in `bc_handball_be.Core/Entities/` (database model).
  2. Create API DTO(s) in `bc_handball_be.API/DTOs/` (for HTTP requests/responses only).
  3. Create Repository interface in `Core/Interfaces/IRepositories/` - keep it simple (CRUD only, works with Entities).
  4. Implement Repository in `Infrastructure/Repositories/` - just data access, no business logic, returns Entities.
  5. Create Service interface in `Core/Interfaces/IServices/` - methods work with Entities.
  6. Implement Service in `Core/Services/` - business logic lives here, works with Entities, orchestrates multiple repositories.
  7. Add Controller in `bc_handball_be.API/Controllers` - inject service and AutoMapper.
  8. In Controller: map DTOs ↔ Entities using `_mapper.Map<>()`, call service with Entities.
  9. Add AutoMapper mappings in `API/Mapping/MappingProfile.cs` for Entity ↔ DTO.
  10. Register in `Program.cs`: `services.AddScoped<IMyService, MyService>();` and `services.AddScoped<IMyRepository, MyRepository>();`.

- **Mapping**: Use `API/Mapping/MappingProfile.cs` for AutoMapper:

  - Map `Entity ↔ DTO` (Entity from Core/Entities, DTO from API/DTOs)
  - Mapping happens **in controllers only**
  - Services work with Entities, not DTOs

- **Remember**:
  - ✅ Services in `.Core/Services/` work with **Entities** and can call multiple repositories
  - ✅ DTOs live **only in API/DTOs/** (HTTP layer)
  - ✅ Entities live in `.Core/Entities/` (domain models)
  - ✅ Controllers map DTOs ↔ Entities using AutoMapper
  - ✅ Controllers are thin - just HTTP + DTO mapping + delegate to services
  - ❌ Repositories should NEVER call other repositories
  - ❌ Repositories should NOT contain business logic
  - ❌ Services should NOT work with DTOs (only Entities)
  - ❌ Core should NEVER reference API

## Troubleshooting

- If `dotnet run` fails with missing dependencies, run `dotnet restore` first.
- If `dotnet ef` cannot find the DbContext, ensure the `--project` and `--startup-project` flags point to the correct projects and that `Program.cs` properly configures the DbContext.
- If Docker containers are unhealthy, check logs `docker-compose logs -f` and inspect the specific container logs.

## Summary

Main ideas:

- `bc_handball_be.API` is the presentation layer (controllers, HTTP concerns, DTOs for HTTP only)
- `bc_handball_be.Core` is the **heart** - business logic, services, and entities (domain models)
- `bc_handball_be.Infrastructure` is data access only - simple repositories, EF Core, migrations
- **Dependency flow**: API → Core ← Infrastructure (Core references nothing!)
- **Services** (in Core) work with **Entities**, orchestrate multiple repositories, and contain all business logic
- **Repositories** (in Infrastructure) are simple CRUD only - work with Entities, no business logic, no calling other repositories
- **DTOs** live only in API project - controllers map between DTOs (API) and Entities (Core) using AutoMapper
- **Entities** are the domain models in Core/Entities/ - this is what services and repositories work with

## Architectural rules summary

### ✅ DO:

- Keep services in `.Core/Services/` and have them work with **Entities**
- Keep entities (domain models) in `.Core/Entities/`
- Keep DTOs in `.API/DTOs/` (HTTP layer only)
- Let services call multiple repositories (orchestration)
- Keep repositories simple (CRUD only, work with Entities)
- Keep controllers thin (just HTTP layer + DTO ↔ Entity mapping)
- Map between Entities and API DTOs in controllers using AutoMapper
- Use `.Core/Services/Models/` only for specialized internal service models (e.g., complex calculation helpers)

### ❌ DON'T:

- Never let Core reference API
- Never let repositories call other repositories
- Never put business logic in repositories
- Never put services in Infrastructure
- Never have services work with DTOs (they should work with Entities)
- Never reference `.API.DTOs` from Core

## Helper script

Use the `dev.ps1` PowerShell script at the repository root for common development tasks (run API, migrations, docker commands). Run `.\dev.ps1` to see available commands.
