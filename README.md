My first mind plan draft:

# Tech Stack
## Frontend:
- Next.js 14 (App Router + TypeScript + Tailwind + React Hook Form + Zod)

---

## Backend:
- .NET 8 Web API (C#)

---

## Authentication and User modules
- completely separated

---

## Database:
- PostgreSQL + EF Core Code-First + migrations

---

## Auth:
- JWT (1-hour expiry) + BCrypt password hashing

---

## Containerisation:
- frontend, backend, Postgres
- runs with a single docker compose up --build

---

## Tests:
- Unit + Integration (xUnit + Moq + WebApplicationFactory).

---

## Build script:
- build.sh for CI/CD readiness

---

## Documentation:
- Comprehensive README + Swagger.