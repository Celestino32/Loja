# Loja Virtual (Angola)

Loja virtual de produtos eletrónicos, operada por uma única empresa com equipa interna
(Admin/Gerente/Vendedor). Backend em ASP.NET Core (Clean Architecture), frontend em Next.js,
PostgreSQL e Docker.

> Este repositório está na **Fase 0 (fundação)**: autenticação, catálogo de produtos/categorias,
> storefront e painel administrativo básico já funcionam de ponta a ponta. Carrinho/checkout,
> pagamentos (Multicaixa Express/EMIS via AppyPay), faturação fiscal (SAF-T AO) e frete por
> zona ainda serão implementados em fases seguintes.

## Stack

- **Backend**: ASP.NET Core 10, Clean Architecture (Domain/Application/Infrastructure/Api),
  EF Core + PostgreSQL, ASP.NET Identity + JWT, MediatR, FluentValidation.
- **Frontend**: Next.js 16 (App Router), React 19, TypeScript, Tailwind CSS 4.
- **Infra**: Docker Compose (PostgreSQL, API, Web, pgAdmin opcional).

## Como correr com Docker (recomendado)

Pré-requisitos: Docker Desktop instalado e em execução.

```bash
cp .env.example .env      # ajuste os valores se necessário
docker compose up --build
```

- Loja: http://localhost:3000
- API + Swagger: http://localhost:5050/swagger
- PostgreSQL: localhost:5432
- pgAdmin (opcional): `docker compose --profile tools up -d pgadmin` → http://localhost:5433

Na primeira subida, a API aplica as migrations e semeia dados de exemplo (categorias, produtos
e um utilizador Admin).

**Login inicial (Admin)**: `admin@lojavirtual.ao` / `Trocar@123!` — altere a senha em produção
via variáveis `SEED_ADMIN_EMAIL` / `SEED_ADMIN_PASSWORD` no `.env` antes do primeiro arranque.

## Como correr localmente sem Docker

### Backend

```bash
cd backend
dotnet run --project src/LojaVirtual.Api
```

Requer um PostgreSQL local acessível conforme `src/LojaVirtual.Api/appsettings.Development.json`.
A API sobe em `http://localhost:5050` (Swagger em `/swagger`) e aplica migrations/seed
automaticamente em ambiente de desenvolvimento.

### Frontend

```bash
cd frontend
cp .env.example .env.local   # já aponta para http://localhost:5050
npm install
npm run dev
```

A loja sobe em `http://localhost:3000`.

## Estrutura do repositório

```
backend/
  src/
    LojaVirtual.Domain          # entidades e regras de negócio puras
    LojaVirtual.Application     # casos de uso (CQRS/MediatR), validação
    LojaVirtual.Infrastructure  # EF Core, Identity, seed
    LojaVirtual.Api             # controllers, autenticação JWT, Swagger
  tests/
    LojaVirtual.UnitTests
    LojaVirtual.IntegrationTests
frontend/
  src/app/(storefront)/         # loja pública: home, produtos, login, registo
  src/app/admin/                # painel administrativo (protegido por role)
  src/app/api/auth/             # route handlers que fazem a ponte com a API (cookie httpOnly)
  src/components/               # componentes de storefront e admin
  src/lib/                      # cliente HTTP, sessão/autenticação, tipos, formatação
docker-compose.yml
```

## Papéis e permissões

- **Admin**: acesso total, único papel que pode criar contas de funcionários (`/admin/funcionarios`).
- **Gerente**: gestão de catálogo (produtos/categorias/estoque).
- **Vendedor**: gestão de catálogo (produtos/estoque).
- **Cliente**: conta própria (self-signup em `/registar`), sem acesso ao painel administrativo.

Não há autocadastro de staff — apenas o Admin cria contas de Gerente/Vendedor.

## Testes

```bash
cd backend
dotnet test
```

## Próximas fases (roadmap)

1. Carrinho, checkout e máquina de estados de pedidos.
2. Pagamentos: Multicaixa Express via AppyPay/IZI Pay, Referência de Pagamento, Transferência Bancária.
3. Faturação fiscal completa (numeração sequencial imutável, NIF, IVA 14%, segunda via, export SAF-T (AO)).
4. Frete por província/município e Click & Collect.
5. Avaliações de produtos e cupons de desconto.
6. Relatórios administrativos (vendas, produtos mais vendidos, estoque baixo).
