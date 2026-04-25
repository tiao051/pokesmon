# Pokemon Project — Claude Instructions

A learning project pairing an ASP.NET Core minimal API with a Nuxt 4 frontend. Keep solutions simple and let architecture grow with real needs, not anticipated ones.

## Stack (current reality)

- **Backend**: ASP.NET Core, `net10.0`, minimal API template. `Microsoft.AspNetCore.OpenApi` is configured. Nullable + implicit usings enabled. Entry point is `backend/Program.cs`.
- **Frontend**: Nuxt `^4.4.2`, Vue `^3.5`, `vue-router`, TypeScript. File-based routing. Layout is `app/app.vue`, `app/pages/`, `app/components/`, `app/layouts/`.
- **Solution**: `pokemon.sln` at the repo root references `backend/backend.csproj`.

Do **not** assume the project has EF Core, PostgreSQL, JWT auth, controllers, a repository pattern, a state library (Pinia/Zustand), a CSS framework, or a test runner unless those are visibly added. If a task needs one, propose adding it before scaffolding code that depends on it.

## Repository Layout

```
pokemon/
  backend/                ASP.NET Core minimal API (net10)
    Program.cs
    backend.csproj
  frontend/               Nuxt 4 app
    app/
      app.vue
      pages/
      components/
      layouts/
    nuxt.config.ts
    package.json
  pokemon.sln
  .agents/                Legacy Copilot-style instructions (mirrored into CLAUDE.md + .claude/skills)
  .claude/skills/         Claude-native skills (backend-expert, frontend-expert)
```

Always use paths relative to the repo root.

## Core Principles

1. **KISS** — reach for the smallest thing that works. Refactor when complexity actually appears.
2. **YAGNI** — no speculative abstractions, services, or layers. Three similar lines beats a premature helper.
3. **DRY (only when there's real duplication)** — don't centralize logic that has only one caller.
4. **Backend is the source of truth** — domain calculations and validation live in the API; the frontend consumes them.
5. **Clean naming** — descriptive over clever. Tests, types, and routes should read clearly.

## Specialized Skills

For deeper guidance, use the matching skill:

- `backend-expert` — ASP.NET Core net10 minimal API conventions, endpoint layout, validation, OpenAPI.
- `frontend-expert` — Nuxt 4 + Vue 3 conventions, routing, data fetching with `useFetch`/`useAsyncData`, composables.

These skills live under `.claude/skills/` (and are mirrored in `.agents/skills/` for the Copilot tooling).

## Engineering Standards

### Backend (`backend/`)

- Stay on minimal API until route handlers genuinely outgrow `Program.cs`. When that happens, split into `Features/<Area>/` with `*Endpoints.cs`, `*Service.cs`, and `Dtos/`.
- New endpoints under `/api`. RESTful naming and consistent status codes (`200/201`, `400`, `404`, `500`).
- Explicit request/response DTOs for non-trivial endpoints — avoid anonymous objects in handlers.
- Validate inputs before business logic; return a consistent error shape (e.g. `{ "error": "..." }` or ProblemDetails).
- Keep OpenAPI on in development. Use `WithName(...)` and add summaries where they aid client/doc generation.
- Async for I/O. Never `.Result` / `.Wait()`. Pass `CancellationToken` through long-running paths.
- Use `ILogger<T>`. Log with context (endpoint, id, operation). Never log secrets.

### Frontend (`frontend/`)

- Stick to Nuxt conventions: `pages/` for routes, `components/` for reusable UI, `composables/` for stateful logic, `server/api/` for BFF endpoints when they appear.
- Prefer `useFetch` / `useAsyncData` for data loading; use `$fetch` inside composables/services for reusable calls.
- Every async view handles loading, empty, and error states. Don't surface raw stack traces to the user.
- Keep components small. Separate presentational and data-fetch concerns. No data-fetch logic inside template expressions.
- TypeScript: type props, composable return values, and API models. Share types under `app/types/` once the domain grows.
- Don't import React Native, Expo, NativeWind, or AsyncStorage patterns — this is a web app.

## Operational Rules

- **Small, scoped changes.** No broad refactors without explicit ask.
- **Ask first** when requirements are ambiguous or the edge cases matter.
- **No hardcoded mock data in UI** — use empty states until the API is wired.
- **No emoji** in production code or docs unless explicitly requested.
- **Don't add tests** unless asked or the change is non-trivial. There's no test runner configured yet; if one is added, prefer xUnit (backend) and Vitest + `@vue/test-utils` (frontend).

## Common Commands

- Run backend: `dotnet run --project backend`
- Build backend: `dotnet build`
- Run frontend dev server: `npm run dev` (from `frontend/`)
- Build frontend: `npm run build` (from `frontend/`)

## Pitfalls to Avoid

1. Forcing enterprise patterns (repositories, mediators, CQRS) onto a tiny minimal API.
2. Mixing business logic into giant inline endpoint lambdas.
3. Returning inconsistent error shapes across endpoints.
4. Adding infra dependencies (DB, auth, queues) before there is a clear feature need.
5. Mobile/native patterns leaking into the Nuxt frontend.
6. Centralizing state management before complexity exists.
