# Wonga Login Service

A full-stack login and registration platform with Next.js frontend and .NET backend.

## 📁 Project Structure

```
wonga-login-service/
├── README.md
├── docker-compose.yml
├── build.sh
├── .gitignore
├── .eslintrc.json
├── .eslintignore
├── wonga-login-service-client/          # Next.js Frontend
│   ├── Dockerfile
│   ├── next.config.mjs
│   ├── package.json
│   ├── tsconfig.json
│   ├── tailwind.config.ts
│   ├── postcss.config.js
│   ├── .env.example
│   ├── app/
│   │   ├── globals.css
│   │   ├── layout.tsx
│   │   ├── page.tsx
│   │   ├── register/page.tsx
│   │   ├── login/page.tsx
│   │   └── user-details/page.tsx
│   └── components/
└── wonga-login-service-server/          # .NET Backend
    └── (To be implemented)
```

## 🚀 Tech Stack

### Frontend:
- Next.js 14 (App Router + TypeScript + Tailwind + React Hook Form + Zod)

### Backend:
- .NET 8 Web API (C#)

### Authentication and User modules:
- Completely separated

### Database:
- PostgreSQL + EF Core Code-First + migrations

### Auth:
- JWT (1-hour expiry) + BCrypt password hashing

### Containerisation:
- Frontend, backend, PostgreSQL
- Runs with a single `docker-compose up --build`

### Tests:
- Unit + Integration (xUnit + Moq + WebApplicationFactory)

### Build script:
- `build.sh` for CI/CD readiness

### Documentation:
- Comprehensive README + Swagger

## 🏗️ Getting Started

### Prerequisites
- Node.js 18+
- .NET SDK 8.0+
- Docker & Docker Compose
- PostgreSQL (for local development)

### Installation

1. **Install frontend dependencies:**
   ```bash
   cd wonga-login-service-client
   npm install
   ```

2. **Set up environment variables:**
   ```bash
   cp wonga-login-service-client/.env.example wonga-login-service-client/.env
   ```

3. **Run development server:**
   ```bash
   npm run dev
   ```

### Build & Deploy

```bash
# Make build script executable
chmod +x build.sh

# Run build
./build.sh

# Start with Docker Compose
docker-compose up --build
```

## 📝 Available Scripts

### Frontend
- `npm run dev` - Start development server
- `npm run build` - Build for production
- `npm run start` - Start production server
- `npm run lint` - Run ESLint
- `npm run lint:fix` - Auto-fix ESLint issues

## 🐳 Docker

The project includes Docker support for both frontend and backend:

```bash
docker-compose up -d
```

Services:
- **Frontend:** http://localhost:3000
- **Backend:** http://localhost:5000
- **Database:** localhost:5432 (PostgreSQL)

## 📄 License

Private - All rights reserved