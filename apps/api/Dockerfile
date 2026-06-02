```dockerfile
# apps/api/Dockerfile

# --- STAGE 1: Build and Publish the .NET API ---
# Sử dụng .NET SDK 10.0 để build ứng dụng.
# Giả định .NET 10.0 sẽ được phát hành, hiện tại có thể dùng 8.0/9.0 cho mục đích phát triển.
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Sao chép file csproj và khôi phục các dependency
# Điều này giúp tận dụng cache của Docker layer khi các dependency không thay đổi
COPY ["apps/api/ReaderManagement.Api.csproj", "apps/api/"]
RUN dotnet restore "apps/api/ReaderManagement.Api.csproj"

# Sao chép toàn bộ mã nguồn của ứng dụng API
COPY apps/api/ .

# Build và publish ứng dụng ra thư mục /app/publish
WORKDIR /src/apps/api
RUN dotnet publish "ReaderManagement.Api.csproj" -c Release -o /app/publish --no-restore

# --- STAGE 2: Runtime Image ---
# Sử dụng ASP.NET runtime 10.0 để chạy ứng dụng đã publish.
# Đây là image nhỏ gọn hơn nhiều so với SDK image.
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# Mở cổng mà API sẽ lắng nghe (mặc định cho ASP.NET Core trong Docker là 80, nhưng 8080 là an toàn hơn khi chạy non-root)
EXPOSE 8080

# Sao chép các file đã publish từ stage 'build' vào image runtime
COPY --from=build /app/publish .

# Chạy container dưới quyền người dùng 'app' không phải root
# Người dùng 'app' được cung cấp sẵn trong các base image của Microsoft và là một best practice bảo mật.
USER app

# Định nghĩa lệnh khởi chạy ứng dụng API
ENTRYPOINT ["dotnet", "ReaderManagement.Api.dll"]
```