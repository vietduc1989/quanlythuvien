# QUAN-20260601-105011
# Sử dụng base image của SDK .NET 10 để build ứng dụng
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy các file dự án và khôi phục các dependency
# Copy solution file và các file csproj trước để tối ưu hóa caching layer của Docker
COPY ["apps/api/src/ONENET.WebAPI/ONENET.WebAPI.csproj", "apps/api/src/ONENET.WebAPI/"]
COPY ["apps/api/src/ONENET.Application/ONENET.Application.csproj", "apps/api/src/ONENET.Application/"]
COPY ["apps/api/src/ONENET.Infrastructure/ONENET.Infrastructure.csproj", "apps/api/src/ONENET.Infrastructure/"]
COPY ["apps/api/src/ONENET.Domain/ONENET.Domain.csproj", "apps/api/src/ONENET.Domain/"]

# Khôi phục dependencies cho WebAPI, điều này cũng sẽ khôi phục cho các dự án phụ thuộc
RUN dotnet restore "apps/api/src/ONENET.WebAPI/ONENET.WebAPI.csproj"

# Copy toàn bộ mã nguồn
COPY . .

# Build ứng dụng
WORKDIR "/src/apps/api/src/ONENET.WebAPI"
RUN dotnet build "ONENET.WebAPI.csproj" -c Release -o /app/build

# Publish ứng dụng
FROM build AS publish
RUN dotnet publish "ONENET.WebAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Tạo final image
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Copy files đã publish từ stage 'publish'
COPY --from=publish /app/publish .

# Định nghĩa điểm khởi đầu của ứng dụng
ENTRYPOINT ["dotnet", "ONENET.WebAPI.dll"]