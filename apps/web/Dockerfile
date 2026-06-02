```dockerfile
# syntax=docker/dockerfile:1.4
# Multi-stage build để tối ưu dung lượng image

# --- BUILD STAGE ---
# Sử dụng Node.js 20-alpine để build ứng dụng React Vite, bản alpine nhỏ gọn
FROM node:20-alpine AS build
WORKDIR /app

# Copy package.json và lock file (giả định dùng pnpm) để cài đặt dependencies
# Điều này giúp tận dụng Docker layer caching
COPY apps/web/package.json apps/web/pnpm-lock.yaml ./
RUN pnpm install --frozen-lockfile # Cài đặt dependencies, --frozen-lockfile để đảm bảo tính nhất quán

# Copy toàn bộ mã nguồn ứng dụng web
COPY apps/web/ .

# Build ứng dụng React Vite
RUN pnpm run build

# --- RUNTIME STAGE ---
# Sử dụng Nginx alpine để phục vụ các file tĩnh, bản nhỏ gọn
FROM nginx:alpine AS final
# Nginx base image mặc định chạy với user 'nginx' (non-root) cho các worker process,
# đáp ứng yêu cầu bảo mật. Không cần tạo hoặc switch user mới.

# Xóa cấu hình Nginx mặc định
RUN rm /etc/nginx/conf.d/default.conf

# Copy cấu hình Nginx tùy chỉnh từ dự án
# (Giả định file nginx.conf nằm trong apps/web/)
COPY apps/web/nginx.conf /etc/nginx/conf.d/default.conf

# Copy các file tĩnh đã build từ giai đoạn build vào thư mục phục vụ của Nginx
COPY --from=build /app/dist /usr/share/nginx/html

EXPOSE 80 # Nginx lắng nghe trên cổng 80

# Command mặc định của Nginx là đủ để chạy
CMD ["nginx", "-g", "daemon off;"]
```