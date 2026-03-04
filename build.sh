#!/bin/bash

# Build script for Wonga Login Service
echo "🚀 Building Wonga Login Service..."

# Build frontend
echo "📦 Building frontend..."
cd wonga-login-service-client
npm install
npm run build
cd ..

# Build backend
echo "📦 Building backend..."
cd wonga-login-service-server
dotnet restore
dotnet build --configuration Release
cd ..

# Build Docker containers
echo "🐳 Building Docker containers..."
docker-compose build

echo "✅ Build complete!"
echo "Run 'docker-compose up' to start the application"
