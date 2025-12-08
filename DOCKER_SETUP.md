# Docker Setup Guide

## Running with Docker Compose (Recommended)

The project is set up with `docker-compose.yaml` which orchestrates everything you need:

```bash
cd RecipiesAPI
docker-compose up --build
```

This will:
1. **Start PostgreSQL** database on port `5433` (host) → `5432` (container)
2. **Run migrations** automatically using Entity Framework Core
3. **Start the API** on `http://localhost:5000`

### What each service does:

- **postgres**: PostgreSQL 15 database
  - Database: `Recipies`
  - User: `postgres`
  - Password: `124578`
  - Port: `5433` on your machine

- **migrate**: Runs `dotnet ef database update` to apply migrations

- **webapi**: Your RecipiesAPI application
  - Accessible at `http://localhost:5000`
  - Built from the Dockerfile

### Useful commands:

```bash
# Run in detached mode (background)
docker-compose up -d

# View logs
docker-compose logs -f

# Stop containers
docker-compose down

# Stop and remove volumes (fresh database)
docker-compose down -v

# Rebuild after code changes
docker-compose up --build
```

## Running just the Dockerfile

If you want to run only the API container (assuming you have a database elsewhere):

```bash
cd RecipiesAPI
docker build -t recipiesapi .
docker run -p 5000:80 -e ConnectionStrings__DefaultConnection="YourConnectionString" recipiesapi
```

The docker-compose approach is recommended since it handles the database and migrations automatically!
