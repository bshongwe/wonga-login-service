#!/bin/bash

set -euo pipefail

# ---------------------------------------
# Build script for Wonga Login Service
# ---------------------------------------

log() { echo "[$(date '+%Y-%m-%d %H:%M:%S')] $1"; }
error() { echo "❌ ERROR: $1" >&2; exit 1; }

log "🚀 Building Wonga Login Service..."

# ---------------------------------------
# Build frontend
# ---------------------------------------
log "📦 Building frontend..."
[ -d "wonga-login-service-client" ] || error "Frontend directory not found!"
cd wonga-login-service-client
npm install || error "npm install failed"
npm run build || error "npm build failed"
cd ..

# ---------------------------------------
# Build backend
# ---------------------------------------
log "📦 Building backend..."
[ -d "wonga-login-service-server" ] || error "Backend directory not found!"
cd wonga-login-service-server
dotnet restore || error "dotnet restore failed"
dotnet build --configuration Release || error "dotnet build failed"
cd ..

# ---------------------------------------
# Run tests
# ---------------------------------------
log "🧪 Running tests..."
[ -d "wonga-login-service-server-tests/WongaLoginService.Tests" ] || error "Tests directory not found!"
cd wonga-login-service-server-tests/WongaLoginService.Tests
dotnet test || error "Tests failed"
cd ../..

# ---------------------------------------
# Build Docker containers
# ---------------------------------------
log "🐳 Building Docker containers..."
docker-compose build || error "Docker build failed"

# ---------------------------------------
# Done
# ---------------------------------------
log "✅ Build complete!"
log "Run 'docker-compose up' to start the application"
