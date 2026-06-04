# syntax=docker/dockerfile:1.4
FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
WORKDIR /src

# Copy projects and restore dependencies to cache layer
COPY ["apps/api/src/ONENET.Domain/ONENET.Domain.csproj", "apps/api/src/ONENET.Domain/"]
COPY ["apps/api/src/ONENET.Application/ONENET.Application.csproj", "apps/api/src/ONENET.Application/"]
COPY ["apps/api/src/ONENET.Infrastructure/ONENET.Infrastructure.csproj", "apps/api/src/ONENET.Infrastructure/"]
COPY ["apps/api/src/ONENET.WebAPI/ONENET.WebAPI.csproj", "apps/api/src/ONENET.WebAPI/"]

RUN dotnet restore "apps/api/src/ONENET.WebAPI/ONENET.WebAPI.csproj"

# Copy the rest of the source code
COPY apps/api/src/ apps/api/src/

# Publish the WebAPI application
WORKDIR /src/apps/api/src/ONENET.WebAPI
RUN dotnet publish "ONENET.WebAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false --no-restore

# --- RUNTIME STAGE ---
FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS final
WORKDIR /app
EXPOSE 8080

# Install curl for docker-compose healthcheck
RUN apk add --no-cache curl

# Create non-root user and group for security
RUN addgroup -S appgroup && adduser -S appuser -G appgroup --no-create-home
USER appuser

# Copy published files from build stage
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "ONENET.WebAPI.dll"]
