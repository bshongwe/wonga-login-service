# Wonga Login Service

A secure, full-stack authentication platform with Next.js frontend and .NET backend, featuring JWT-based authentication, PostgreSQL database, and comprehensive security hardening.

## 🎯 Features

- ✅ **Secure Authentication**: JWT tokens with httpOnly cookies
- ✅ **Password Security**: BCrypt hashing with salt rounds
- ✅ **Security Hardening**: CWE-209 compliant (no stack trace exposure)
- ✅ **Database Migrations**: EF Core with automatic schema updates
- ✅ **Responsive UI**: Modern design with Tailwind CSS
- ✅ **API Documentation**: Swagger/OpenAPI integration
- ✅ **Rate Limiting**: Protection against brute force attacks
- ✅ **Global Error Handling**: Sanitized error responses
- ✅ **CORS Protection**: Configured for cross-origin security
- ✅ **Docker Support**: Full containerization for all services

## 📁 Project Structure

```
wonga-login-service/
├── README.md
├── docker-compose.yml
├── build.sh
├── wonga-login-service-client/          # Next.js Frontend
│   ├── Dockerfile
│   ├── next.config.mjs
│   ├── package.json
│   ├── tsconfig.json
│   ├── tailwind.config.ts
│   ├── .env
│   ├── app/
│   │   ├── layout.tsx
│   │   ├── page.tsx                     # Home page with Wonga logo
│   │   ├── login/page.tsx               # Login form with logo
│   │   ├── register/page.tsx            # Registration form with logo
│   │   ├── user-details/page.tsx        # Protected user profile
│   │   ├── api/                         # Next.js API routes (proxy)
│   │   │   └── auth/
│   │   │       ├── login/route.ts
│   │   │       ├── register/route.ts
│   │   │       ├── logout/route.ts
│   │   │       └── session/route.ts
│   │   └── styling/
│   │       └── globals.css
│   ├── components/
│   │   ├── Navbar.tsx                   # Navigation with Wonga logo
│   │   ├── Sidebar.tsx
│   │   ├── AppLayout.tsx
│   │   └── ui/
│   │       ├── Button.tsx
│   │       └── Input.tsx
│   ├── contexts/
│   │   └── AuthContext.tsx
│   ├── lib/
│   │   ├── api.ts
│   │   ├── schemas.ts
│   │   └── utils.ts
│   └── public/
│       └── wonga-logo.svg
│
└── wonga-login-service-server/          # .NET Backend
    ├── Dockerfile
    ├── Program.cs                       # Auto-migration on startup
    ├── WongaLoginService.csproj
    ├── appsettings.json
    ├── Controllers/
    │   ├── AuthController.cs            # Login/Register (CWE-209 hardened)
    │   └── UserController.cs            # User details (CWE-209 hardened)
    ├── Services/
    │   ├── AuthService.cs
    │   └── JwtService.cs
    ├── Middleware/
    │   ├── GlobalExceptionHandlerMiddleware.cs
    │   └── RateLimitingMiddleware.cs
    ├── Models/
    │   └── User.cs
    ├── DTOs/
    │   └── AuthDTOs.cs
    ├── Data/
    │   └── AppDbContext.cs
    └── Migrations/
        ├── 20260306025951_InitialCreate.cs
        ├── 20260306025951_InitialCreate.Designer.cs
        └── AppDbContextModelSnapshot.cs
```

## 🚀 Tech Stack

### Frontend:
- **Framework**: Next.js 14 (App Router)
- **Language**: TypeScript
- **Styling**: Tailwind CSS
- **Form Handling**: React Hook Form + Zod validation
- **State Management**: React Context API
- **HTTP Client**: Fetch API with custom wrapper
- **Icons**: Lucide React
- **Image Optimization**: Next.js Image component

### Backend:
- **Framework**: .NET 8 Web API
- **Language**: C#
- **Authentication**: JWT Bearer tokens
- **Password Hashing**: BCrypt.Net-Next
- **API Documentation**: Swagger/Swashbuckle

### Database:
- **Database**: PostgreSQL 15
- **ORM**: Entity Framework Core 8.0
- **Migration Strategy**: Code-First with auto-migration on startup
- **Provider**: Npgsql.EntityFrameworkCore.PostgreSQL

### Security:
- **JWT**: 1-hour token expiry with secure httpOnly cookies
- **Password**: BCrypt hashing with automatic salt generation
- **CORS**: Configured for localhost:3000 origin
- **Cookie Security**: SameSite=Strict, HttpOnly, Secure flags
- **Error Handling**: CWE-209 compliant (no stack trace exposure)
- **Rate Limiting**: Request throttling middleware
- **Input Validation**: Data annotations + Zod schemas

### DevOps:
- **Containerization**: Docker + Docker Compose
- **Frontend Container**: Node.js 20 Alpine
- **Backend Container**: .NET 8 Runtime (multi-stage build)
- **Database Container**: PostgreSQL 15 Alpine
- **Networking**: Isolated Docker bridge network
- **Build Script**: `build.sh` for CI/CD readiness

## 🏗️ Getting Started

### Prerequisites
- **Docker** & **Docker Compose** (recommended)
- OR:
  - Node.js 18+
  - .NET SDK 8.0+
  - PostgreSQL 15+

### Quick Start with Docker (Recommended)

1. **Clone the repository:**
   ```bash
   git clone https://github.com/bshongwe/wonga-login-service.git
   cd wonga-login-service
   ```

2. **Configure environment variables:**
   ```bash
   # Frontend environment is already configured in .env
   # Backend uses environment variables from docker-compose.yml
   ```

3. **Start all services:**
   ```bash
   docker-compose up --build
   ```

4. **Access the application:**
   - Frontend: http://localhost:3000
   - Backend API: http://localhost:5001
   - Swagger Docs: http://localhost:5001/swagger
   - PostgreSQL: localhost:5432

### Manual Setup (Without Docker)

#### Backend Setup:

1. **Install PostgreSQL and create database:**
   ```bash
   # Using psql
   createdb WongaLoginDb
   ```

2. **Update connection string in `wonga-login-service-server/appsettings.json`:**
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Host=localhost;Database=WongaLoginDb;Username=postgres;Password=YourPassword"
   }
   ```

3. **Navigate to backend directory:**
   ```bash
   cd wonga-login-service-server
   ```

4. **Restore NuGet packages:**
   ```bash
   dotnet restore
   ```

5. **Run migrations (automatic on startup):**
   ```bash
   dotnet run
   ```
   The backend will automatically apply migrations on startup and listen on http://localhost:5000

#### Frontend Setup:

1. **Navigate to frontend directory:**
   ```bash
   cd wonga-login-service-client
   ```

2. **Install dependencies:**
   ```bash
   npm install
   ```

3. **Update `.env` file:**
   ```bash
   NEXT_PUBLIC_API_URL=http://localhost:5000/api
   ```

4. **Run development server:**
   ```bash
   npm run dev
   ```
   Frontend will be available at http://localhost:3000

## 📝 Available Scripts

### Frontend (`wonga-login-service-client`)
```bash
npm run dev        # Start development server (http://localhost:3000)
npm run build      # Build for production
npm run start      # Start production server
npm run lint       # Run ESLint
npm run lint:fix   # Auto-fix ESLint issues
```

### Backend (`wonga-login-service-server`)
```bash
dotnet run                    # Run in development mode
dotnet build                  # Build the project
dotnet publish -c Release     # Build for production
dotnet ef migrations add <Name>   # Create new migration
dotnet ef database update     # Apply migrations manually
```

### Docker Commands
```bash
docker-compose up --build     # Build and start all services
docker-compose up -d          # Start in detached mode
docker-compose down           # Stop all services
docker-compose logs -f        # View logs
docker-compose ps             # List running services
```

## 🐳 Docker

The project includes full Docker support with multi-container orchestration:

### Services:
- **Frontend**: Next.js app on port 3000
- **Backend**: .NET Web API on port 5001 (mapped from internal 5000)
- **Database**: PostgreSQL 15 on port 5432

### Docker Configuration:

```yaml
# docker-compose.yml includes:
- Isolated Docker network (wonga-network)
- PostgreSQL with persistent volume
- Auto-configured environment variables
- Health checks and service dependencies
```

### Quick Commands:
```bash
# Start all services
docker-compose up --build

# Start in background
docker-compose up -d

# View logs
docker logs wonga-login-backend-local -f
docker logs wonga-login-service-db-1 -f

# Rebuild specific service
docker-compose up --build backend

# Stop all services
docker-compose down

# Remove volumes (reset database)
docker-compose down -v
```

## 🔐 API Endpoints

### Authentication Endpoints

#### POST `/api/auth/register`
Register a new user account.

**Request Body:**
```json
{
  "username": "johndoe",
  "email": "john@example.com",
  "password": "SecurePass123!"
}
```

**Response (200):**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": {
    "id": "f009be08-5e1c-41c5-86e9-6b31d9261db6",
    "username": "johndoe",
    "email": "john@example.com",
    "createdAt": "2026-03-06T03:09:07.002Z"
  }
}
```

#### POST `/api/auth/login`
Authenticate existing user.

**Request Body:**
```json
{
  "email": "john@example.com",
  "password": "SecurePass123!"
}
```

**Response (200):**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": {
    "id": "f009be08-5e1c-41c5-86e9-6b31d9261db6",
    "username": "johndoe",
    "email": "john@example.com",
    "createdAt": "2026-03-06T03:09:07.002Z"
  }
}
```

**Note:** JWT token is automatically set as httpOnly cookie (`auth_token`) for security.

### User Endpoints

#### GET `/api/user/details` 🔒 (Protected)
Get current authenticated user details.

**Headers:**
```
Cookie: auth_token=<jwt_token>
```

**Response (200):**
```json
{
  "id": "f009be08-5e1c-41c5-86e9-6b31d9261db6",
  "username": "johndoe",
  "email": "john@example.com",
  "createdAt": "2026-03-06T03:09:07.002Z"
}
```

## 🔒 Security Features

### Implemented Security Measures:

1. **CWE-209 Compliance**: No stack trace exposure in production
   - All exceptions caught and logged server-side
   - Generic error messages returned to clients
   - Sanitized validation error messages

2. **Authentication Security**:
   - JWT tokens with 1-hour expiry
   - HttpOnly cookies (XSS protection)
   - SameSite=Strict (CSRF protection)
   - Secure flag for HTTPS

3. **Password Security**:
   - BCrypt hashing with automatic salt
   - Minimum 8 characters required
   - Password complexity validation

4. **Input Validation**:
   - Server-side validation (Data Annotations)
   - Client-side validation (Zod schemas)
   - SQL injection protection (EF Core parameterization)

5. **Rate Limiting**:
   - Request throttling middleware
   - Protection against brute force attacks

6. **CORS Configuration**:
   - Restricted to localhost:3000 origin
   - Credentials allowed for cookie transmission

7. **Database Security**:
   - Parameterized queries via EF Core
   - No raw SQL execution
   - Connection string in environment variables

### Security Best Practices:

- ✅ Never expose stack traces to clients
- ✅ Use httpOnly cookies for JWT storage
- ✅ Implement proper error handling
- ✅ Validate all user inputs
- ✅ Hash passwords with BCrypt
- ✅ Use HTTPS in production (Secure cookie flag)
- ✅ Log security events server-side
- ✅ Implement rate limiting
- ✅ Use environment variables for secrets

## 🎨 UI Features

- **Wonga Branding**: Official Wonga logo displayed on:
  - Navigation bar
  - Home page
  - Login form
  - Registration form

- **Responsive Design**: Mobile-first approach with Tailwind CSS
- **Form Validation**: Real-time client-side validation
- **Error Handling**: User-friendly error messages
- **Loading States**: Visual feedback during async operations

## 📚 API Documentation

Swagger documentation is available at:
- **Development**: http://localhost:5001/swagger
- **Production**: Disabled for security

### Testing with Swagger:

1. Navigate to http://localhost:5001/swagger
2. Use the `/api/auth/register` or `/api/auth/login` endpoint
3. Copy the JWT token from the response
4. Click the "Authorize" button (🔒)
5. Enter the token (without "Bearer" prefix)
6. Test protected endpoints

## 🛠️ Troubleshooting

### Common Issues:

**1. Port 5000 already in use:**
```bash
# Find process using port 5000
lsof -i :5000

# Kill the process
kill -9 <PID>

# Or use port 5001 (already configured)
```

**2. Database connection errors:**
```bash
# Check if PostgreSQL is running
docker ps | grep postgres

# View database logs
docker logs wonga-login-service-db-1

# Reset database
docker-compose down -v
docker-compose up --build
```

**3. Frontend not connecting to backend:**
- Verify `NEXT_PUBLIC_API_URL` in `.env` is set to `http://localhost:5001/api`
- Restart Next.js dev server after changing `.env`
- Check CORS configuration in backend

**4. Migration issues:**
```bash
# Remove existing migrations
rm -rf wonga-login-service-server/Migrations/*

# Create new migration
docker run --rm -v "$(pwd):/src" -w /src mcr.microsoft.com/dotnet/sdk:8.0 \
  bash -c "dotnet tool install --global dotnet-ef && \
  export PATH=\"\$PATH:/root/.dotnet/tools\" && \
  dotnet ef migrations add InitialCreate"

# Rebuild backend
docker-compose up --build backend
```

## 🚀 Deployment

### Production Considerations:

1. **Environment Variables:**
   - Set `ASPNETCORE_ENVIRONMENT=Production`
   - Use strong JWT secret key
   - Configure production database connection
   - Set `NODE_ENV=production` for frontend

2. **Security:**
   - Enable HTTPS
   - Update CORS to production domain
   - Use secure cookie flags
   - Disable Swagger in production

3. **Database:**
   - Use managed PostgreSQL service
   - Enable SSL connections
   - Regular backups
   - Connection pooling

4. **Monitoring:**
   - Set up application logging
   - Monitor API performance
   - Track security events
   - Set up alerts

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/logo`)
3. Commit your changes (`git commit -m 'Add Wonga logo'`)
4. Push to the branch (`git push origin feature/logo`)
5. Open a Pull Request

## 📄 License

Private - All rights reserved