# NightmaresWiki2

NightmaresWiki2 is a Tibia-inspired wiki rebuild for the Nightmares game universe. It uses:

- React + TypeScript for the frontend
- ASP.NET Core as a backend-for-frontend API
- MongoDB for stored imported content
- Docker Desktop for local orchestration

## Structure

- `src/NightmaresWiki.Api`: ASP.NET Core BFF, importer, Mongo persistence, static media hosting
- `src/NightmaresWiki.Frontend`: React app with routes for Home, Enemies, Items, and NPC
- `tests/NightmaresWiki.Api.Tests`: parser and controller-level tests
- `Orchestrator.md`, `developer.md`, `UX-designer.md`, `Reviewer.md`: agent team operating guides

## Runtime model

The API imports legacy HTML and images from `C:\Users\jesal\source\repos\NightmaresWiki`, normalizes the content, stores it in MongoDB, and serves image assets from `/media/imported/*`.

## Docker

Run the stack with:

```powershell
docker compose up --build
```

Services:

- Frontend: `http://localhost:4173`
- API: `http://localhost:8080`
- MongoDB: `mongodb://localhost:27017`

## Notes

- The legacy source currently contains 3 enemy detail pages, 32 item detail pages, and no true NPC detail pages.
- The frontend expects standard Node/Vite tooling, but the Docker setup can build it even if Node is not installed on the host.
