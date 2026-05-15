# VitrineBilhar

Base inicial do SaaS **VitrineBilhar** com .NET 8, EF Core, PostgreSQL e multi-tenant.

## Estrutura

- `src/VitrineBilhar.Domain`
- `src/VitrineBilhar.Application`
- `src/VitrineBilhar.Infrastructure`
- `src/VitrineBilhar.API`
- `tests/VitrineBilhar.Domain.Tests`
- `tests/VitrineBilhar.Application.Tests`
- `tests/VitrineBilhar.API.IntegrationTests`

## Rodando local com Docker

```bash
docker compose up --build
```

API: `http://localhost:8080`

- Swagger: `http://localhost:8080/swagger`
- Health: `http://localhost:8080/health`
- Catálogo público: `GET http://localhost:8080/api/catalog/demo/products`

## Rodando local com dotnet

1. Suba o banco PostgreSQL:

```bash
docker compose up -d postgres
```

2. Rode a API:

```bash
dotnet run --project src/VitrineBilhar.API
```

## Migrações EF Core

```bash
dotnet ef migrations add InitialCreate --project src/VitrineBilhar.Infrastructure --startup-project src/VitrineBilhar.API
dotnet ef database update --project src/VitrineBilhar.Infrastructure --startup-project src/VitrineBilhar.API
```

> A aplicação também usa `EnsureCreated` em desenvolvimento quando ainda não houver migrações.

## Multi-tenancy (MVP)

Resolução de tenant:
1. Claims JWT (`tenant_id`, `tenant_slug`) para rotas autenticadas.
2. Subdomínio (`{tenantSlug}.seu-dominio.com`).
3. Header `X-Tenant-Id` (apenas desenvolvimento quando habilitado via `MultiTenant:AllowHeaderTenantIdInDevelopment=true`).

Pipeline:
`UseAuthentication()` -> `TenantMiddleware` -> `UseAuthorization()`

## Seed mínimo

No primeiro start, é criado:
- Tenant: `demo`
- Produto ativo: `Taco Profissional`
