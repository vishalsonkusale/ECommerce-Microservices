# E-Commerce Microservices Setup Guide

This README explains how to set up and run the E-Commerce Microservices project locally or with Docker.

## Services And Ports

| Service | Port | Swagger URL |
| --- | ---: | --- |
| API Gateway | `5056` | Not configured |
| Authentication API | `5180` | `http://localhost:5180/swagger` |
| Product API | `5153` | `http://localhost:5153/swagger` |
| Order API | `5152` | `http://localhost:5152/swagger` |
| SQL Server | `1433` | Not applicable |

## Prerequisites

Install these before running the project:

- .NET 8 SDK
- Docker Desktop
- SQL Server or SQL Server Docker container
- EF Core CLI

Install EF Core CLI if it is not already installed:

```bash
dotnet tool install --global dotnet-ef
```

Check installation:

```bash
dotnet --version
docker --version
dotnet ef --version
```

## 1. Start SQL Server

Run SQL Server with Docker:

```bash
docker run --platform linux/amd64 \
  -e "ACCEPT_EULA=Y" \
  -e "MSSQL_SA_PASSWORD=<YOUR_PASSWORD>" \
  -p 1433:1433 \
  --name ecommerce-sql-server \
  -d mcr.microsoft.com/mssql/server:2022-latest
```

If the SQL Server container already exists:

```bash
docker start ecommerce-sql-server
```

Check logs:

```bash
docker logs ecommerce-sql-server
```

## 2. Configure Environment Variables

Use environment variables or a local `.env` file. Do not commit real passwords or production secrets.

For local `dotnet run`:

```env
ConnectionStrings__ECommerceDbConnection=Server=localhost,1433;Database=ECommerceDB;User Id=SA;Password=<YOUR_PASSWORD>;Encrypt=False;TrustServerCertificate=True;
Authentication__Key=<YOUR_JWT_SECRET>
Authentication__Issuer=http://localhost:5180
Authentication__Audience=http://localhost:5180
```

For Docker containers connecting to SQL Server on your host machine:

```env
ConnectionStrings__ECommerceDbConnection=Server=host.docker.internal,1433;Database=ECommerceDB;User Id=SA;Password=<YOUR_PASSWORD>;Encrypt=False;TrustServerCertificate=True;
Authentication__Key=<YOUR_JWT_SECRET>
Authentication__Issuer=http://localhost:5180
Authentication__Audience=http://localhost:5180
```

## 3. Apply Database Migrations

Run these commands from the repository root after SQL Server is running.

Authentication API:

```bash
dotnet ef database update \
  --project Ecommerce.AuthenticationApiSolution/Authentication.infrastructure/Authentication.infrastructure.csproj \
  --startup-project Ecommerce.AuthenticationApiSolution/Authentication.Presentation/Authentication.Presentation.csproj
```

Product API:

```bash
dotnet ef database update \
  --project ECommerce.ProductApiSolution/ProductApi.Infrastructure/ProductApi.Infrastructure.csproj \
  --startup-project ECommerce.ProductApiSolution/ProductApi.Presentation/ProductApi.Presentation.csproj
```

Order API:

```bash
dotnet ef database update \
  --project OrderApiSolution/OrderApi.Infrastructure/OrderApi.Infrastructure.csproj \
  --startup-project OrderApiSolution/OrderApi.Presentation/OrderApi.Presentation.csproj
```

## 4. Run With Docker Compose

Build and start all services:

```bash
docker compose up --build
```

Run in the background:

```bash
docker compose up --build -d
```

Check running containers:

```bash
docker compose ps
```

View logs:

```bash
docker compose logs -f
```

Stop all services:

```bash
docker compose down
```

## 5. Run Locally Without Docker

Open four terminals and run one service in each terminal.

Authentication API:

```bash
dotnet run --project Ecommerce.AuthenticationApiSolution/Authentication.Presentation/Authentication.Presentation.csproj
```

Product API:

```bash
dotnet run --project ECommerce.ProductApiSolution/ProductApi.Presentation/ProductApi.Presentation.csproj
```

Order API:

```bash
dotnet run --project OrderApiSolution/OrderApi.Presentation/OrderApi.Presentation.csproj
```

API Gateway:

```bash
dotnet run --project ECommerce.ApiGateway.Solution/ApiGateway.Presentation/ApiGateway.Presentation.csproj
```

## 6. Open Swagger

After services are running, open:

```text
http://localhost:5180/swagger
http://localhost:5153/swagger
http://localhost:5152/swagger
```

The API Gateway does not currently expose Swagger.

## 7. Test Through API Gateway

Use the API Gateway port for routed requests:

```text
http://localhost:5056/api/authentication/register
http://localhost:5056/api/authentication/login
http://localhost:5056/api/products
http://localhost:5056/api/orders
```

## Useful Docker Commands

Build only the API Gateway image:

```bash
docker build -t ecommerce-api-gateway .
```

Run only the API Gateway container:

```bash
docker run --name ecommerce-api-gateway \
  -p 5056:8080 \
  ecommerce-api-gateway
```

Remove a container:

```bash
docker rm <container-name>
```

Remove an image:

```bash
docker rmi <image-name>
```

## Troubleshooting

If Docker build fails because the Docker daemon is unavailable, start Docker Desktop and try again.

If Swagger does not open, make sure you are using `/swagger` at the end of the API URL.

If APIs cannot connect to SQL Server from Docker, confirm SQL Server is running and available on port `1433`.

If migrations fail, verify the connection string, SQL Server password, and EF Core CLI installation.

If the API Gateway cannot reach downstream services in Docker, check `ECommerce.ApiGateway.Solution/ApiGateway.Presentation/ocelot.Docker.json`.
