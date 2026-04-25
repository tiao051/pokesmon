---
name: backend-expert
description: "Use when: implementing backend endpoints and application logic in this project. Tech: ASP.NET Core net10 minimal API, OpenAPI, C#. Keep architecture simple and evolvable."
---

# Pokemon Backend Guidelines (ASP.NET Core net10)

## Current Reality (Must Follow)
- Framework: ASP.NET Core (minimal API template)
- Target: net10.0
- OpenAPI: Microsoft.AspNetCore.OpenApi is configured
- Nullable and implicit usings are enabled
- Entry point: Program.cs currently holds endpoint mapping

Do not assume this project already uses:
- EF Core
- PostgreSQL
- JWT auth
- Repository pattern
- Controllers
unless those are explicitly added.

## Core Principles
- Keep code simple first, then refactor when complexity appears.
- Respect incremental evolution:
  - start from minimal API route handlers
  - introduce services when logic grows
  - introduce persistence and auth only when needed
- Prefer explicit request/response contracts over ad-hoc anonymous objects for non-trivial endpoints.

## API Design Conventions
- Base all endpoints under /api for new features.
- Use RESTful naming:
  - GET /api/pokemon
  - GET /api/pokemon/{id}
  - POST /api/pokemon
  - PUT /api/pokemon/{id}
  - DELETE /api/pokemon/{id}
- Return consistent status codes:
  - 200/201 for success
  - 400 for validation errors
  - 404 when resource is missing
  - 500 only for unexpected failures

## Minimal API Organization
When Program.cs becomes crowded, split endpoint registration:

```text
backend/
  Program.cs
  Features/
    Pokemon/
      PokemonEndpoints.cs
      PokemonService.cs
      Dtos/
```

Pattern:
- Endpoint files only map routes and convert HTTP concerns.
- Service files contain business logic.
- DTOs define request/response contracts.

## Validation and Error Handling
- Validate request payloads before business logic.
- For simple rules, validate inline.
- For complex request models, create validator classes or dedicated validation methods.
- Return problem details or a consistent error payload shape.

Example error payload:

```json
{
  "error": "Pokemon name is required"
}
```

## OpenAPI Requirements
- Keep OpenAPI enabled in development.
- Name endpoints with WithName for discoverability.
- Add summaries/descriptions where useful for client generation and docs.

## Async and Performance
- Use async for I/O-bound operations.
- Avoid blocking calls such as .Result and .Wait().
- Pass CancellationToken in endpoints/services when operations may be long-running.

## Logging
- Use built-in ILogger for operational visibility.
- Log errors with context (endpoint, entity id, operation).
- Never log secrets.

## Testing Guidance
No backend test project is visible yet.
- If tests are introduced, prefer xUnit.
- Focus tests on business logic in service layer first.
- Keep endpoint tests targeted to routing/status behavior.

## Pitfalls to Avoid
1. Forcing enterprise architecture too early.
2. Mixing business logic directly into large inline endpoint lambdas.
3. Returning inconsistent error response formats.
4. Introducing infra dependencies without clear project need.
5. Assuming auth/database stack exists when it is not configured.

## Quick Checklist Before Finalizing
- Is this aligned with current net10 minimal API setup?
- Are endpoint contracts clear and typed enough?
- Are validation and status codes explicit?
- Is logic placed in a maintainable location?
- Did we avoid assumptions about missing infrastructure?
