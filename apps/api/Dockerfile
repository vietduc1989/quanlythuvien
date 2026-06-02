#
# apps/api/Dockerfile - .NET 10 Backend API for Book Lending Management
#
# Sử dụng Multi-stage builds để tối ưu dung lượng image và tăng cường bảo mật.
# Chạy container dưới quyền non-root user.
#
# Ghi chú: "dotnet:10.0" là phiên bản giả định dựa trên yêu cầu ".NET 10" từ Technical Guideline.
# Khi .NET 10 được phát hành chính thức, các base image này sẽ khả dụng.
# Hiện tại, nếu .NET 10 chưa có, có thể thay thế bằng phiên bản LTS gần nhất như "8.0".
#

# --- Stage 1: Build application ---
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy và restore các file dự án để tận dụng Docker cache
# Copy .csproj và restore dependencies trước khi copy toàn bộ code
COPY ["apps/api/*.csproj", "apps/api/"]
RUN dotnet restore "apps/api/apps/api.csproj"

# Copy toàn bộ mã nguồn của API
COPY . .
WORKDIR /src/apps/api
RUN dotnet build "apps/api.csproj" -c Release -o /app/build

# Publish ứng dụng
RUN dotnet publish "apps/api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# --- Stage 2: Create a minimal runtime image ---
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# Tạo non-root user và group để tăng cường bảo mật
# UID/GID 10001 là một giá trị phổ biến cho non-root user
RUN addgroup --system --gid 10001 appgroup && \
    adduser --system --uid 10001 --ingroup appgroup appuser

# Copy ứng dụng đã publish từ stage build
COPY --from=build /app/publish .

# Đảm bảo quyền sở hữu của thư mục ứng dụng thuộc về non-root user
RUN chown -R appuser:appgroup /app

# Chuyển đổi sang non-root user
USER appuser

# Expose port mà ứng dụng sẽ lắng nghe
# Mặc định ASP.NET Core lắng nghe trên 8080 khi chạy non-root user
EXPOSE 8080

# Điểm vào để chạy ứng dụng
ENTRYPOINT ["dotnet", "apps/api.dll"]