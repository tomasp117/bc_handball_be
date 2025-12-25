# BC Handball Backend - Quick Start Guide

Get up and running in 5 minutes.

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- `dotnet-ef` tool: `dotnet tool install --global dotnet-ef`

## 1. Start Database (Docker)

**Run only the database in Docker** - this gives you PostgreSQL without installing it locally:

```powershell
# Start PostgreSQL + pgAdmin
.\dev docker-up

# Or manually:
docker-compose up postgres_db pgadmin -d
```

**Database access:**

- PostgreSQL: `localhost:5432` (devuser/devpass)
- pgAdmin: `http://localhost:5050` (tomasp117@seznam.cz/pcup_handball_is)

## 2. Run Backend Locally (Recommended for Development)

**Why local?** Hot reload, faster startup, easier debugging.

```powershell
# Run the API locally (rename script to dev.ps1 first)
.\dev run

# Or manually:
cd .\bc_handball_be.API
dotnet run
```

The API will:

- Auto-apply migrations on startup
- Seed development data
- Enable Swagger UI (check console for URL)
- Support hot reload (file changes restart automatically)

## 3. Verify It Works

1. Check console output for the API URL (usually `http://localhost:5000` or `https://localhost:5001`)
2. Open Swagger UI in browser: `http://localhost:5000/swagger`
3. Try a GET endpoint (e.g., `/api/tournaments`)

## 4. Import Sample Data (Optional)

Want to work with real tournament data? Import the sample dataset:

```powershell
# Make sure database is running and migrations are applied
cat final_import.sql | docker exec -i bc_handball_pg psql -U devuser -d handball_is
```

**What you get:**
- 35 Clubs (DHK Zora Olomouc, HC Zlín, etc.)
- 100+ Teams across 5 categories
- 1,200+ Players
- 280+ Matches with full scores
- 10,000+ Match events
- Tournament: Polanka Cup

**To reset database:**
```powershell
# Clear all data
docker exec bc_handball_pg psql -U devuser -d handball_is -c "TRUNCATE handball_is.\"Tournament\" CASCADE;"

# Re-import
cat final_import.sql | docker exec -i bc_handball_pg psql -U devuser -d handball_is
```

## Quick Commands

```powershell
# Development workflow
.\dev run              # Run API locally
.\dev docker-up        # Start DB only
.\dev docker-down      # Stop DB

# Database migrations
.\dev migration-add AddNewFeature
.\dev migration-update

# Build & restore
.\dev build
.\dev restore
```

## Architecture Overview (Quick Reference)

```
Frontend → API (DTOs) → Core (Entities) ← Infrastructure (DB)
           ↓ maps       ↓ business logic   ↓ data access
        Controller → Service → Repository
```

### Key Rules

✅ **DO:**

- DTOs in `API/DTOs/` (HTTP only)
- Entities in `Core/Entities/` (domain models)
- Services work with **Entities**, not DTOs
- Controllers map DTOs ↔ Entities using AutoMapper
- Business logic in Services
- Simple CRUD in Repositories

❌ **DON'T:**

- Never put business logic in Repositories
- Never let Repositories call other Repositories
- Never let Services work with DTOs (use Entities)
- Never let Core reference API

## Project Structure

```
bc_handball_be/
├── bc_handball_be.API/           # HTTP layer
│   ├── Controllers/              # Endpoints (thin, maps DTOs ↔ Entities)
│   ├── DTOs/                     # API data transfer objects
│   └── Mapping/                  # AutoMapper (Entity ↔ DTO)
├── bc_handball_be.Core/          # Business logic
│   ├── Entities/                 # Domain models (DB)
│   ├── Services/                 # Business logic (works with Entities)
│   └── Interfaces/               # Service & Repository contracts
└── bc_handball_be.Infrastructure/ # Data access
    ├── Repositories/             # Simple CRUD (returns Entities)
    ├── Persistence/              # DbContext
    └── Migrations/               # EF Core migrations
```

## Typical Development Workflow

### Adding a New Feature

1. **Create Entity** in `Core/Entities/YourEntity.cs`
2. **Create DTO** in `API/DTOs/YourEntityDTO.cs`
3. **Create Repository** interface + implementation (simple CRUD)
4. **Create Service** interface + implementation (business logic, works with Entity)
5. **Create Controller** (maps DTO ↔ Entity, calls service)
6. **Add AutoMapper mapping** in `API/Mapping/MappingProfile.cs`
7. **Register DI** in `Program.cs`
8. **Create migration**: `.\dev migration-add AddYourEntity`
9. **Apply migration**: `.\dev migration-update`

### Example: Controller Pattern

```csharp
[HttpGet]
public async Task<IActionResult> GetTeams([FromQuery] int categoryId)
{
    // Service returns Entity
    var teams = await _teamService.GetTeamsByCategoryAsync(categoryId);

    // Controller maps Entity → DTO
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
```

## Docker vs Local Development

### Recommended Setup (Development)

- ✅ **Database in Docker** - Easy setup, isolated environment
- ✅ **API runs locally** - Hot reload, faster startup, easier debugging

```powershell
# Terminal 1: Database
.\dev docker-up

# Terminal 2: API
.\dev run
```

### Full Docker Setup (Production)

- Both API and DB in Docker
- Slower for development (no hot reload)
- Use for production deployment

```powershell
.\dev docker-up-build
```

## Troubleshooting

**"Cannot connect to database"**

- Check Docker is running: `docker ps`
- Verify connection string in `appsettings.json`
- Restart DB: `.\dev docker-restart`

**"Migration pending"**

- Apply migrations: `.\dev migration-update`

**"Port already in use"**

- API: Check `appsettings.json` for configured ports
- DB: Change port in `docker-compose.yml` if 5432 is taken

## Next Steps

- Read [README.md](README.md) for detailed architecture explanation
- Review [CLAUDE.md](CLAUDE.md) for development guidelines
- Check existing services in `Core/Services/` for patterns
- Review AutoMapper configs in `API/Mapping/MappingProfile.cs`

## Authentication

JWT tokens configured. To authenticate in Swagger:

1. Call `/api/auth/login` with credentials
2. Copy the token from response
3. Click "Authorize" button in Swagger UI
4. Enter: `Bearer YOUR_TOKEN_HERE`

## Production Deployment (Azure)

### Architecture Overview

```
Azure Static Web Apps (Frontend) → Azure Container Apps (Backend Docker) → Azure Database for PostgreSQL
      FREE tier                      Docker Hub image                    Flexible Server (cheapest)
```

### Cost-Effective Azure Setup (Student Budget)

**Tournament Management Strategy:** High traffic during tournaments (3 days), dormant rest of year.

| Service      | Configuration                           | "Tournament Mode" (3 Days)                                  | "Rest of Year" Mode                                                                                      |
| ------------ | --------------------------------------- | ----------------------------------------------------------- | -------------------------------------------------------------------------------------------------------- |
| **Frontend** | Azure Static Web Apps                   | Handles high traffic automatically (Global CDN). FREE.      | FREE.                                                                                                    |
| **Backend**  | Azure Container Apps                    | Set `min-replicas=1` for these 3 days so it never sleeps.   | Set `min-replicas=0`. Scales to zero. **Cost: $0**.                                                      |
| **Database** | Azure PostgreSQL Flexible Server (B1MS) | Running and ready. **FREE** (first 12 months with student). | **Stop** server in portal when not in use. **Cost: $0**. After 12 months: Stop manually to stop billing. |

### Deployment Strategy

1. **Backend**: Build Docker image → Push to Docker Hub → Deploy to Azure Container Apps
2. **Frontend**: Push to GitHub → Azure Static Web Apps auto-deploys (FREE CI/CD)
3. **Database**: Provision Azure PostgreSQL Flexible Server B1MS → Update connection string

### Estimated Monthly Cost

**During first 12 months (Student Credits + Free Tier):**

- Frontend: **$0** (Static Web Apps free tier)
- Backend: **$0** (only runs during tournaments, ~$2-3 per tournament for 3 days)
- Database: **$0** (FREE for 12 months with Azure for Students)
- **Total: ~$0-10/year** (only tournament days!) 🎉

**After 12 months (Still Optimized):**

- Frontend: **$0** (always free)
- Backend: **$2-5 per tournament** (3 days × 3-4 tournaments/year = $6-20/year)
- Database: **$12/month** but can be **stopped** when not in use (stop 10 months = ~$24/year)
- **Total: ~$30-45/year** for a tournament management system!

### Scaling Strategy (Before Tournament)

**3 days before tournament:**

```bash
# Start database in Azure Portal (if stopped)
# Set Container Apps min-replicas to 1
az containerapp update \
  --name bc-handball-api \
  --resource-group handball-rg \
  --min-replicas 1
```

**After tournament (cleanup):**

```bash
# Scale backend to zero
az containerapp update \
  --name bc-handball-api \
  --resource-group handball-rg \
  --min-replicas 0

# Stop database in Azure Portal (manual)
```

### Cost Optimization Tips

1. **Azure Container Apps** - Perfect for sporadic workloads (tournament days only)
2. **Stop PostgreSQL** - Manually stop when dormant (portal or CLI)
3. **Docker Hub** - Free public repository for images
4. **Azure for Students** - $100 credit + 12 months FREE PostgreSQL B1MS
5. **Static Web Apps** - Always FREE, handles CDN and SSL automatically
6. **Auto-scale to zero** - Backend costs $0 when nobody is using it

## Help

Run `.\dev` (or `.\dev.ps1`) without arguments to see all available commands.

For issues: Check `docker-compose logs -f` for database logs.
