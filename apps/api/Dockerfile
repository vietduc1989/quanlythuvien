FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ONENET.slnx", "."]
COPY ["src/ONENET.Domain/ONENET.Domain.csproj", "src/ONENET.Domain/"]
COPY ["src/ONENET.Application/ONENET.Application.csproj", "src/ONENET.Application/"]
COPY ["src/ONENET.Infrastructure/ONENET.Infrastructure.csproj", "src/ONENET.Infrastructure/"]
COPY ["src/ONENET.WebAPI/ONENET.WebAPI.csproj", "src/ONENET.WebAPI/"]
RUN dotnet restore

COPY . .
WORKDIR "/src/src/ONENET.WebAPI"
RUN dotnet build -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ONENET.WebAPI.dll"]
