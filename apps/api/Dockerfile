```dockerfile
# syntax=docker/dockerfile:1.4
# Multi-stage build để tối ưu dung lượng image

# --- BUILD STAGE ---
# Sử dụng .NET SDK 10.0-alpine để build, bản alpine nhỏ gọn
FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
WORKDIR /src

# Copy solution file nếu có ở thư mục gốc, nếu không thì bỏ qua
# COPY Onenet.Report.sln .

# Copy các file dự án .csproj và thư mục packages/common (nếu có) để khôi phục dependencies
# Điều này giúp tận dụng Docker layer caching nếu chỉ thay đổi mã nguồn mà không thay đổi dependencies
COPY apps/api/Onenet.Report.Api.csproj apps/api/
COPY packages/common/Onenet.Common.csproj packages/common/ # Giả định có thư viện chung
# Copy toàn bộ thư mục apps/api và packages/common để đảm bảo các file liên quan được copy đúng
COPY apps/api/ .apps/api/ # Copy vào thư mục apps/api trong container
COPY packages/common/ .packages/common/ # Copy vào thư mục packages/common trong container

# Chuyển vào thư mục dự án API để khôi phục dependencies
WORKDIR /src/apps/api
RUN dotnet restore "Onenet.Report.Api.csproj"

# Copy toàn bộ mã nguồn còn lại
COPY . .

# Chuyển vào thư mục dự án API một lần nữa để publish
WORKDIR /src/apps/api
# Publish ứng dụng, tạo ra output vào thư mục /app/publish
# --no-restore vì đã khôi phục ở bước trước
# /p:UseAppHost=false để tạo ra ứng dụng độc lập không cần host OS .NET runtime
RUN dotnet publish "Onenet.Report.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false --no-restore

# --- RUNTIME STAGE ---
# Sử dụng ASP.NET Runtime 10.0-alpine cho giai đoạn chạy, bản nhỏ gọn và chỉ chứa runtime cần thiết
FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS final
WORKDIR /app
EXPOSE 8080 # Mở cổng 8080, cổng mặc định cho ứng dụng ASP.NET Core trong Docker

# Bắt buộc: Chạy container dưới quyền non-root user để bảo mật
# Tạo một nhóm và người dùng hệ thống 'appuser' không có home directory
RUN addgroup -S appgroup && adduser -S appuser -G appgroup --no-create-home
USER appuser

# Copy output đã publish từ giai đoạn build vào thư mục làm việc của giai đoạn runtime
COPY --from=build /app/publish .

# Định nghĩa điểm vào (entrypoint) của ứng dụng
ENTRYPOINT ["dotnet", "Onenet.Report.Api.dll"]
```