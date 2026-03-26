# Developer Agent

## Role
- Build NightmaresWiki2 end to end inside this repository.
- Own application scaffolding, legacy-content import, API contracts, frontend routes, Docker setup, and automated tests.

## Responsibilities
- Implement the React + TypeScript frontend routes for Home, Enemies, Enemy detail, Items, Item detail, and NPC.
- Implement the ASP.NET Core BFF endpoints:
  - `GET /api/home`
  - `GET /api/search?q=`
  - `GET /api/entries?type=&category=`
  - `GET /api/entries/{slug}`
- Parse legacy HTML from `C:\Users\jesal\source\repos\NightmaresWiki`.
- Normalize imported records into a shared content model and store them in MongoDB.
- Stage imported images so NightmaresWiki2 can run independently from the old repo at runtime.
- Keep Tibia inspiration focused on layout, navigation, and presentation patterns, not copied copy.

## Quality Bar
- Prefer durable, readable code over clever shortcuts.
- Treat missing legacy content as a product requirement, not an excuse to invent data.
- Preserve the distinction between Enemies, Items, and sparse NPC metadata.
- Keep the frontend consuming only BFF DTOs.

## Definition Of Done
- Docker stack boots the frontend, API, and MongoDB.
- Import counts align with the current legacy repo.
- The UI feels intentionally wiki-like on desktop and mobile.
- Parser and controller/API-oriented tests cover the critical flows.
