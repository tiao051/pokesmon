---
name: frontend-expert
description: "Use when: building or refactoring Nuxt frontend features. Tech: Nuxt 4, Vue 3, Vue Router, TypeScript. Follow KISS/DRY and align with current project structure."
---

# Pokemon Frontend Guidelines (Nuxt 4 + Vue 3)

## Current Reality (Must Follow)
- Framework: Nuxt 4
- Runtime: Vue 3
- Language: TypeScript (prefer strict typing for props, composables, API models)
- Router: file-based routing from Nuxt (Vue Router under the hood)
- Current folder layout is lightweight:
  - app/app.vue
  - app/pages/
  - app/components/
  - app/layouts/
  - public/
  - nuxt.config.ts
- Do not propose React Native, Expo, NativeWind, AsyncStorage, or mobile navigation patterns.

## Working Principles
- Keep implementation simple and production-friendly (KISS).
- Prefer Nuxt conventions over custom abstractions.
- Build small, composable units:
  - pages for route-level UI
  - components for reusable UI
  - composables for reusable stateful logic
  - server/api for backend-for-frontend endpoints when needed
- Avoid adding libraries unless the project clearly needs them.

## Recommended Nuxt Structure
Use this structure when adding new features:

```text
frontend/
  app/
    app.vue
    pages/
    components/
    composables/
    layouts/
    middleware/
  server/
    api/
  public/
  nuxt.config.ts
```

Notes:
- Keep shared types in app/types or app/models when domain grows.
- Keep API client logic in composables/services, not in template blocks.

## UI and Styling
- Preserve the visual language already used in the project.
- For new UI:
  - prioritize responsive layout (mobile first)
  - avoid giant single-file components
  - separate presentational and data-fetch concerns when possible
- If no CSS framework is configured, use scoped CSS/CSS modules with clear naming.

## Data Fetching Pattern
Prefer Nuxt-native data utilities:
- useFetch for straightforward endpoint calls.
- useAsyncData for SSR-friendly data loading and caching semantics.
- $fetch in composables/services for reusable API calls.

Example composable pattern:

```ts
// app/composables/usePokemon.ts
export function usePokemon(id: string) {
  return useAsyncData(`pokemon-${id}`, () => $fetch(`/api/pokemon/${id}`))
}
```

## Error and Loading States
Every async view should handle:
- pending/loading UI
- empty state
- error state with clear user-facing message

Do not expose raw server stack traces in UI.

## Frontend-Backend Contract
- Keep request/response types explicit.
- Backend is source of truth for business rules.
- Validate critical form input on frontend for UX, and still validate on backend.

## Testing Guidance
Current project does not show a configured test runner yet.
- If adding tests, prefer Vitest + @vue/test-utils for unit tests.
- Add tests only when requested or when introducing non-trivial logic.
- Do not invent a test setup that is not installed.

## Pitfalls to Avoid
1. Mixing mobile app patterns into Nuxt web code.
2. Putting data-fetch logic directly inside template expressions.
3. Tight coupling between UI components and transport layer.
4. Over-engineering state management before complexity exists.
5. Adding dependencies for problems Nuxt already solves.

## Quick Checklist Before Finalizing
- Is this aligned with Nuxt 4 conventions?
- Is route/component/composable responsibility clear?
- Are loading/empty/error states handled?
- Is TypeScript typing clear enough for future changes?
- Did we avoid unnecessary dependencies?
