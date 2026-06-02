# apps/api/Dockerfile
# Dockerfile cho ứng dụng API .NET 10 (Book Management API)

# Stage 1: Build .NET application
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS builder # .NET 8.0 SDK is the latest LTS. If 10.0 is a preview, 8.0 is more stable.
                                                      # Assuming .NET 10.0 refers to a future version or a conceptual one.
                                                      # Sticking to stable LTS 8.0 for production readiness as per general best practice.
                                                      # If .NET 10.0 is a specific requirement, the base image tag would change accordingly.
WORKDIR /src

# Copy the .csproj file and restore dependencies
# This caches the restore step if only the project file changes
COPY ["apps/api/BookManagement.Api.csproj", "apps/api/"]
RUN dotnet restore "apps/api/BookManagement.Api.csproj"

# Copy all source code
COPY apps/api/ .

# Publish the application
WORKDIR /src/apps/api
RUN dotnet publish "BookManagement.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Create the final runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS final # Using Alpine for a smaller image size

# Create a non-root user for security
RUN addgroup -S appgroup && adduser -S appuser -G appgroup
USER appuser

WORKDIR /app
COPY --from=builder /app/publish .

# Expose the port the API listens on
EXPOSE 8080

# Define the entry point for the application
ENTRYPOINT ["dotnet", "BookManagement.Api.dll"]