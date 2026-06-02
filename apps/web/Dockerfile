```dockerfile
# apps/web/Dockerfile

# --- STAGE 1: Build the React Vite application ---
FROM node:20-alpine AS build
WORKDIR /app

# Sao chép các file quản lý dependency (package.json, package-lock.json) để tận dụng Docker cache
COPY apps/web/package.json apps/web/package-lock.json ./
RUN npm install --frozen-lockfile

# Sao chép toàn bộ mã nguồn của ứng dụng web
COPY apps/web/ .

# Build ứng dụng React Vite
RUN npm run build

# --- STAGE 2: Serve the application with Nginx ---
FROM nginx:alpine AS final

# Tạo một user và group không phải root để chạy Nginx
# Việc chạy Nginx dưới quyền non-root giúp tăng cường bảo mật.
# Mặc định Nginx lắng nghe trên cổng 80, nhưng để chạy non-root, ta sẽ cấu hình lắng nghe trên cổng cao hơn (ví dụ: 8080).
RUN addgroup -g 1000 -S appgroup \
    && adduser -u 1000 -S appuser -G appgroup \
    && mkdir -p /var/cache/nginx /var/run/nginx /var/log/nginx \
    && chown -R appuser:appgroup /var/cache/nginx /var/run/nginx /var/log/nginx

# Sao chép cấu hình Nginx tùy chỉnh.
# File này sẽ cấu hình Nginx lắng nghe trên cổng 8080 và phục vụ các file tĩnh.
COPY apps/web/nginx.conf /etc/nginx/conf.d/default.conf

# Sao chép các file đã build từ stage 'build' vào thư mục phục vụ của Nginx
COPY --from=build /app/dist /usr/share/nginx/html

# Gán quyền sở hữu thư mục HTML cho người dùng 'appuser'
RUN chown -R appuser:appgroup /usr/share/nginx/html

# Chuyển sang chạy container dưới quyền người dùng 'appuser'
USER appuser

# Mở cổng mà Nginx sẽ lắng nghe
EXPOSE 8080

# Lệnh khởi chạy Nginx
CMD ["nginx", "-g", "daemon off;"]
```