# Pokémon

A learning project that pairs an ASP.NET Core minimal API with a Nuxt 4 frontend, themed as a Pokémon-card "gallery" storefront (`PokéGogh`). The architecture is intentionally small — features are added when there is a real need, not in anticipation of one.

## Stack

**Backend** — `backend/`
- ASP.NET Core, target framework `net10.0`
- Minimal API template (`Program.cs`)
- `Microsoft.AspNetCore.OpenApi`
- Nullable + implicit usings enabled

**Frontend** — `frontend/`
- Nuxt `^4.4.2`, Vue `^3.5`, `vue-router`
- TypeScript
- File-based routing (`app/pages/`), components in `app/components/`, composables in `app/composables/`, mock data in `app/data/`

There is no database, auth, ORM, state library, CSS framework, or test runner wired up yet. Pages currently render from in-memory mock data under `frontend/app/data/`.

## Repository layout

```
pokemon/
  backend/                  ASP.NET Core minimal API
    Program.cs
    backend.csproj
  frontend/                 Nuxt 4 app
    app/
      app.vue
      pages/                index, login, products/, cart, checkout/
      components/           AppHeader, ProductCard, CartLineItem, …
      composables/          useCart, useToast, …
      data/                 mock product catalog
    nuxt.config.ts
    package.json
  pokemon.sln
  CLAUDE.md                 instructions for Claude Code
  .claude/skills/           backend-expert, frontend-expert
```

## Prerequisites

- .NET SDK matching `net10.0`
- Node.js (LTS recommended) and `npm`

## Getting started

### Backend

```bash
dotnet build
dotnet run --project backend
```

OpenAPI is enabled in development.

### Frontend

```bash
cd frontend
npm install
npm run dev
```

Build for production:

```bash
npm run build
```

## Conventions

- **KISS / YAGNI** — smallest thing that works; no speculative abstractions.
- **Backend is the source of truth** for domain calculations and validation; the frontend consumes them.
- **Backend** — new endpoints under `/api`, RESTful naming, explicit DTOs for non-trivial endpoints, consistent error shape, `async` for I/O, `ILogger<T>` with context.
- **Frontend** — Nuxt conventions (`pages/`, `components/`, `composables/`, `server/api/` when needed), `useFetch` / `useAsyncData` for loading, every async view handles loading/empty/error states, components stay small, props and API models are typed.
- **No hardcoded mock data in UI views** once the API is wired — use empty states until then.

See `CLAUDE.md` for the full set of project rules, and `.claude/skills/backend-expert` / `frontend-expert` for deeper, area-specific guidance.

## Status

Early-stage. The frontend has the gallery, product detail, cart, checkout, and login screens scaffolded against mock data. The backend is the default minimal-API template — domain endpoints have not been added yet.
