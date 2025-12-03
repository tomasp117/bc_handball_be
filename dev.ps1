# dev.ps1
# PowerShell helper script for common development tasks in bc_handball_be

param(
    [Parameter(Position = 0)]
    [string]$Command = "help",
    
    [Parameter(Position = 1)]
    [string]$Arg1 = ""
)

$ErrorActionPreference = "Stop"
$ApiProject = ".\bc_handball_be.API\bc_handball_be.API.csproj"
$InfraProject = ".\bc_handball_be.Infrastructure\bc_handball_be.Infrastructure.csproj"

function Show-Help {
    Write-Host ""
    Write-Host "=============================" -ForegroundColor Cyan
    Write-Host "  BC Handball BE Dev Helper" -ForegroundColor Cyan
    Write-Host "=============================" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Usage: .\dev.ps1 <command> [args]" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Available Commands:" -ForegroundColor Green
    Write-Host ""
    Write-Host "  run                  - Run the API locally (dotnet run)" -ForegroundColor White
    Write-Host "  restore              - Restore NuGet packages" -ForegroundColor White
    Write-Host "  build                - Build the solution" -ForegroundColor White
    Write-Host "  clean                - Clean build artifacts" -ForegroundColor White
    Write-Host ""
    Write-Host "  migration-add <name> - Add a new EF Core migration" -ForegroundColor White
    Write-Host "  migration-update     - Apply pending migrations to database" -ForegroundColor White
    Write-Host "  migration-list       - List all migrations" -ForegroundColor White
    Write-Host "  migration-remove     - Remove the last migration" -ForegroundColor White
    Write-Host ""
    Write-Host "  docker-up            - Start docker containers (docker-compose up -d)" -ForegroundColor White
    Write-Host "  docker-up-build      - Rebuild and start docker containers" -ForegroundColor White
    Write-Host "  docker-down          - Stop and remove docker containers" -ForegroundColor White
    Write-Host "  docker-restart       - Restart docker containers" -ForegroundColor White
    Write-Host "  docker-logs          - Follow docker logs" -ForegroundColor White
    Write-Host "  docker-ps            - List running containers" -ForegroundColor White
    Write-Host ""
    Write-Host "  help                 - Show this help message" -ForegroundColor White
    Write-Host ""
    Write-Host "Examples:" -ForegroundColor Green
    Write-Host "  .\dev.ps1 run" -ForegroundColor Gray
    Write-Host "  .\dev.ps1 migration-add AddPlayerTable" -ForegroundColor Gray
    Write-Host "  .\dev.ps1 docker-up" -ForegroundColor Gray
    Write-Host ""
}

function Run-Api {
    Write-Host "Starting API..." -ForegroundColor Green
    dotnet run --project $ApiProject
}

function Restore-Packages {
    Write-Host "Restoring NuGet packages..." -ForegroundColor Green
    dotnet restore
}

function Build-Solution {
    Write-Host "Building solution..." -ForegroundColor Green
    dotnet build
}

function Clean-Solution {
    Write-Host "Cleaning solution..." -ForegroundColor Green
    dotnet clean
}

function Add-Migration {
    param([string]$Name)
    
    if ([string]::IsNullOrWhiteSpace($Name)) {
        Write-Host "Error: Migration name is required." -ForegroundColor Red
        Write-Host "Usage: .\dev.ps1 migration-add <MigrationName>" -ForegroundColor Yellow
        exit 1
    }
    
    Write-Host "Adding migration: $Name" -ForegroundColor Green
    dotnet ef migrations add $Name --project $InfraProject --startup-project $ApiProject
}

function Update-Database {
    Write-Host "Applying migrations to database..." -ForegroundColor Green
    dotnet ef database update --project $InfraProject --startup-project $ApiProject
}

function List-Migrations {
    Write-Host "Listing all migrations..." -ForegroundColor Green
    dotnet ef migrations list --project $InfraProject --startup-project $ApiProject
}

function Remove-LastMigration {
    Write-Host "Removing last migration..." -ForegroundColor Yellow
    dotnet ef migrations remove --project $InfraProject --startup-project $ApiProject
}

function Docker-Up {
    Write-Host "Starting docker containers..." -ForegroundColor Green
    docker-compose up -d
}

function Docker-Up-Build {
    Write-Host "Rebuilding and starting docker containers..." -ForegroundColor Green
    docker-compose up --build -d
}

function Docker-Down {
    Write-Host "Stopping docker containers..." -ForegroundColor Green
    docker-compose down
}

function Docker-Restart {
    Write-Host "Restarting docker containers..." -ForegroundColor Green
    docker-compose restart
}

function Docker-Logs {
    Write-Host "Following docker logs (Ctrl+C to exit)..." -ForegroundColor Green
    docker-compose logs -f
}

function Docker-Ps {
    Write-Host "Listing running containers..." -ForegroundColor Green
    Write-Host ""
    docker ps
    Write-Host ""
    Write-Host "Docker Compose services:" -ForegroundColor Cyan
    docker-compose ps
}

# Main script logic
switch ($Command.ToLower()) {
    "run" {
        Run-Api
    }
    "restore" {
        Restore-Packages
    }
    "build" {
        Build-Solution
    }
    "clean" {
        Clean-Solution
    }
    "migration-add" {
        Add-Migration -Name $Arg1
    }
    "migration-update" {
        Update-Database
    }
    "migration-list" {
        List-Migrations
    }
    "migration-remove" {
        Remove-LastMigration
    }
    "docker-up" {
        Docker-Up
    }
    "docker-up-build" {
        Docker-Up-Build
    }
    "docker-down" {
        Docker-Down
    }
    "docker-restart" {
        Docker-Restart
    }
    "docker-logs" {
        Docker-Logs
    }
    "docker-ps" {
        Docker-Ps
    }
    "help" {
        Show-Help
    }
    default {
        Write-Host "Unknown command: $Command" -ForegroundColor Red
        Write-Host ""
        Show-Help
        exit 1
    }
}
