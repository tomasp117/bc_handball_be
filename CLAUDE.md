# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

BC Handball Backend - ASP.NET Core 8.0 Web API for a handball tournament management system using Clean Architecture with three projects: API (presentation), Core (business logic), and Infrastructure (data access).

**Database**: PostgreSQL (previously MySQL, migrated to Postgres)

## Common Commands

### Running the API

```powershell
# From repository root - using helper script
.\dev.ps1 run

# Or directly
cd .\bc_handball_be.API
dotnet run

# Or from root
dotnet run --project .\bc_handball_be.API\bc_handball_be.API.csproj
```

### Building and Restoring

```powershell
.\dev.ps1 build
.\dev.ps1 restore
.\dev.ps1 clean
```

### Database Migrations (EF Core)

**Prerequisites**: Install/update `dotnet-ef` tool:

```powershell
dotnet tool install --global dotnet-ef
# or
dotnet tool update --global dotnet-ef
```

**Commands**:

```powershell
# Add migration (use helper script)
.\dev.ps1 migration-add <MigrationName>

# Apply migrations
.\dev.ps1 migration-update

# List migrations
.\dev.ps1 migration-list

# Remove last migration
.\dev.ps1 migration-remove

# Direct commands (if needed)
dotnet ef migrations add <Name> --project .\bc_handball_be.Infrastructure\bc_handball_be.Infrastructure.csproj --startup-project .\bc_handball_be.API\bc_handball_be.API.csproj
dotnet ef database update --project .\bc_handball_be.Infrastructure\bc_handball_be.Infrastructure.csproj --startup-project .\bc_handball_be.API\bc_handball_be.API.csproj
```

### Docker

```powershell
# Start containers (Postgres DB + pgAdmin + API)
.\dev.ps1 docker-up

# Rebuild and start
.\dev.ps1 docker-up-build

# Stop containers
.\dev.ps1 docker-down

# View logs
.\dev.ps1 docker-logs

# Check running containers
.\dev.ps1 docker-ps
```

**Docker Services**:

- `postgres_db`: PostgreSQL on port 5432 (devuser/devpass)
- `pgadmin`: pgAdmin on port 5050 (tomasp117@seznam.cz/pcup_handball_is)
- `bc_handball_be`: API on port 5000 (Production mode)
- `mysql_db` and `phpmyadmin`: Legacy MySQL setup (not actively used)

## Architecture

### Three-Layer Clean Architecture

```
Frontend → API → Core ← Infrastructure
          (HTTP) (Business) (Data)
```

**Dependency Rule**: Dependencies flow toward Core. Core references nothing.

### Project Responsibilities

**bc_handball_be.API** (Presentation Layer)

- Controllers: HTTP endpoints
- DTOs: API-specific data transfer objects (optional)
- `Mapping/MappingProfile.cs`: AutoMapper configuration
- `Middleware/`: Custom middleware (exception handling, logging, etc.)
- `Program.cs`: DI registration, middleware, authentication setup
- References: Core only (NOT Infrastructure)

**bc_handball_be.Core** (Business Logic Layer - THE HEART)

- `Entities/`: Domain entities (database models)
- `Interfaces/IServices/`: Service contracts
- `Interfaces/IRepositories/`: Repository contracts
- `Services/`: Business logic implementations
- `Exceptions/`: Custom exception types (NotFoundException, BadRequestException, ValidationException, etc.)
- `DTOs/` or `Models/`: Service-layer models (NOT API-specific)
- References: Nothing (pure business logic)

**bc_handball_be.Infrastructure** (Data Access Layer)

- `Persistence/ApplicationDbContext.cs`: EF Core DbContext
- `Persistence/SeedData.cs`: Development seed data
- `Repositories/`: Repository implementations (simple CRUD only)
- `Migrations/`: EF Core migrations
- `InfrastructureModule.cs`: DbContext registration (PostgreSQL)
- References: Core only

### Critical Architectural Rules

#### ✅ DO:

- Keep all business logic in `Core/Services/` - services work with **Entities**
- Keep entities (domain models) in `Core/Entities/`
- Keep DTOs in `API/DTOs/` (HTTP layer only)
- Let services orchestrate multiple repositories
- Keep repositories as simple CRUD operations that work with Entities
- Keep controllers thin (just HTTP layer + DTO ↔ Entity mapping)
- Map between DTOs and Entities in controllers using AutoMapper
- Register services and repositories in `Program.cs` as `AddScoped`
- Use `Core/Services/Models/` only for specialized internal models (e.g., calculation helpers like `TeamWithAttributes`)

#### ❌ DON'T:

- **NEVER** let Core reference API or Infrastructure
- **NEVER** let repositories call other repositories (move to services)
- **NEVER** put business logic in repositories (move to services)
- **NEVER** put services in Infrastructure (they belong in Core)
- **NEVER** create complex queries in repositories (move to services)
- **NEVER** have services work with DTOs (they should work with Entities)
- **NEVER** reference API DTOs from Core

### Request Flow

1. Frontend → HTTP Request (with DTO in body if POST/PUT)
2. `API/Controllers/*Controller.cs` receives request (receives DTO)
3. **Controller maps DTO → Entity** using `_mapper.Map<Entity>(dto)`
4. Controller calls `IService` with Entity (injected from Core)
5. `Core/Services/*Service.cs`:
   - Validates input (business rules)
   - **Throws custom exceptions** (NotFoundException, BadRequestException, etc.) if validation fails
   - **Orchestrates multiple repositories** as needed
   - Handles transactions
   - Works with **Entities only**, not DTOs
   - Returns Entity to Controller
6. Service calls `IRepository` interfaces (defined in Core)
7. `Infrastructure/Repositories/*Repository.cs`:
   - Executes simple CRUD via EF Core
   - Works with Entities
   - NO business logic
   - NO calling other repositories
   - Returns Entity to Service
8. Results flow back: Repository → Service (Entity) → Controller
9. **Controller maps Entity → DTO** using `_mapper.Map<DTO>(entity)`
10. Controller → HTTP Response (with DTO as JSON)
11. **If exception occurs**: `API/Middleware/GlobalExceptionHandlerMiddleware` catches it, logs it, and returns standardized error response

## Exception Handling

The application uses **centralized exception handling** with custom exceptions and middleware:

### Custom Exceptions (Core/Exceptions)

Services throw domain-specific exceptions that map to HTTP status codes:

- **NotFoundException** → HTTP 404 (use when resource is not found)
- **BadRequestException** → HTTP 400 (use when request data is invalid)
- **ValidationException** → HTTP 422 (use for validation errors with field-level details)
- **UnauthorizedException** → HTTP 401 (use when authentication is required)
- **ForbiddenException** → HTTP 403 (use when user lacks permissions)

**When to use:**

- Throw exceptions **from services** when business rules are violated
- Controllers should **NOT catch** these exceptions - let them bubble up
- Middleware handles conversion to HTTP responses

**Example:**

```csharp
// In Core/Services/TeamService.cs
public async Task<Team> GetTeamByIdAsync(int id)
{
    var team = await _teamRepository.GetByIdAsync(id);
    if (team == null)
        throw new NotFoundException(nameof(Team), id); // Will become HTTP 404

    return team;
}
```

### Global Exception Handler Middleware (API/Middleware)

`GlobalExceptionHandlerMiddleware` automatically:

- Catches all unhandled exceptions
- Maps custom exceptions to appropriate HTTP status codes
- Returns standardized JSON error responses
- Logs exceptions with appropriate severity
- Includes stack traces in Development mode only

**You don't need to manually register this** - it's already configured in `Program.cs`.

## Adding a New Feature (Correct Pattern)

When adding a new entity/feature, follow this order:

1. **Create Entity** in `Core/Entities/YourEntity.cs` (database model)
2. **Create API DTO(s)** in `API/DTOs/YourEntityDTO.cs` (for HTTP requests/responses only)
3. **Create Repository Interface** in `Core/Interfaces/IRepositories/IYourEntityRepository.cs` (simple CRUD only, works with Entities)
4. **Implement Repository** in `Infrastructure/Repositories/YourEntityRepository.cs` (data access only, returns Entities)
5. **Create Service Interface** in `Core/Interfaces/IServices/IYourEntityService.cs` (methods work with Entities)
6. **Implement Service** in `Core/Services/YourEntityService.cs` (business logic + orchestration, works with Entities)
7. **Create Controller** in `API/Controllers/YourEntityController.cs` (inject service + AutoMapper)
8. **In Controller**: Map DTOs ↔ Entities using `_mapper.Map<>()`, call service with Entities
9. **Add AutoMapper mappings** in `API/Mapping/MappingProfile.cs` for Entity ↔ DTO
10. **Register in DI** (`Program.cs`):
    ```csharp
    builder.Services.AddScoped<IYourEntityRepository, YourEntityRepository>();
    builder.Services.AddScoped<IYourEntityService, YourEntityService>();
    ```
11. **Create Migration**: `.\dev.ps1 migration-add AddYourEntity`
12. **Apply Migration**: `.\dev.ps1 migration-update`

## Key Technologies

- **Framework**: ASP.NET Core 8.0
- **Database**: PostgreSQL (via Npgsql.EntityFrameworkCore.PostgreSQL)
- **ORM**: Entity Framework Core 8.0
- **Mapping**: AutoMapper
- **Authentication**: JWT Bearer tokens
- **API Docs**: Swagger/Swashbuckle
- **Password Hashing**: BCrypt.Net-Next

## Authentication & Authorization

- JWT authentication configured in `Program.cs`
- JWT settings in `appsettings.json` under `JwtSettings:Secret`
- `IAuthService` and `AuthService` handle authentication logic
- Swagger UI includes Bearer token authentication

## Development Workflow

1. Ensure Docker is running (for PostgreSQL)
2. Start database: `.\dev.ps1 docker-up`
3. Run API: `.\dev.ps1 run`
4. API runs with automatic migration and seed data (in Development mode)
5. Swagger UI available at the configured URL (check console output)

## Important Notes

- **Seed data** runs automatically in Development mode (see `Program.cs` lines 143-155)
- **CORS** is configured to allow any origin in Development (`AllowFrontend` policy)
- Connection string in `appsettings.json` points to PostgreSQL
- Docker production setup uses `ASPNETCORE_ENVIRONMENT=Production`
- Old MySQL migrations are in `Infrastructure/Migrations_old_mySql/` (excluded from build)
- Static files served from `wwwroot/` (images volume-mounted in Docker)

## Core Domain Entities

Main entities (in `Core/Entities/`): Tournament, TournamentInstance, Team, Category, Group, Match, Club, Player, Coach, Person, Event, Lineup, LineupPlayer.

**Important**: Services work directly with these Entities, not DTOs. DTOs only exist in the API layer for HTTP communication. Review existing services in `Core/Services/` for patterns before adding new features.

**Specialized Service Models** (in `Core/Services/Models/`): TeamWithAttributes, GroupAssignmentVariant, UnassignedMatch, GroupStanding, PlaceholderTeam, etc. These are internal models used only for complex business logic calculations within services - they are NOT replacements for general DTOs.

## Entity Domain Model

For a complete class diagram showing all entity relationships, see the **Entity Domain Model (Class Diagram)** section in `README.md`.

**Key Entity Structure:**

- **BaseEntity**: Abstract base class providing `Id` property to all entities
- **BasePersonRole**: Abstract base for all person roles (Player, Coach, Referee, ClubAdmin)
- **Tournament → TournamentInstance**: Tournament editions (years)
- **TournamentInstance → Category → Group**: Competition hierarchy
- **Club → Team → Player/Coach**: Club roster management
- **Team ↔ Group**: Many-to-many via `TeamGroup` join table
- **Match**: Links teams, referees, groups; tracks events and lineups
- **Person → BasePersonRole**: One person can have multiple roles
- **ClubRegistration**: Tracks club tournament registrations with package selections

The domain model follows **Clean Architecture** principles with all entities in `Core/Entities/` and organized into subdirectories:

- `Actors/super/`: Person entity
- `Actors/sub/`: Person role entities (Player, Coach, Referee, ClubAdmin)
- `Actors/`: BasePersonRole abstract class
- `IdentityField/`: BaseEntity abstract class
- Root: Core tournament entities (Tournament, Team, Match, etc.)
