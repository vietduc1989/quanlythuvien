#
# apps/web/Dockerfile - React Vite Frontend for Book Lending Management
#
# Sử dụng Multi-stage builds để tối ưu dung lượng image.
# Chạy Nginx dưới quyền non-root user (Nginx mặc định chạy với user 'nginx').
#

# --- Stage 1: Build the React application ---
FROM node:18-alpine AS build
WORKDIR /app

# Tạo non-root user cho quá trình build để tăng cường bảo mật
RUN addgroup --system --gid 10001 nodegroup && \
    adduser --system --uid 10001 --ingroup nodegroup nodeuser

# Chuyển đổi sang non-root user cho quá trình build
USER nodeuser

# Copy package.json và package-lock.json để tận dụng Docker cache
COPY ["apps/web/package.json", "apps/web/package-lock.json", "./"]

# Cài đặt dependencies
RUN npm ci

# Copy toàn bộ mã nguồn của frontend
COPY apps/web/. .

# Build ứng dụng React
# Thêm biến môi trường để trỏ tới API backend, sẽ được thay thế khi deploy
ARG VITE_API_BASE_URL=/api
RUN npm run build -- --emptyOutDir --base=/

# --- Stage 2: Serve with Nginx ---
FROM nginx:stable-alpine AS final
WORKDIR /usr/share/nginx/html

# Copy cấu hình Nginx tùy chỉnh
COPY scripts/nginx.conf /etc/nginx/conf.d/default.conf

# Copy output đã build từ stage build
COPY --from=build /app/dist .

# Nginx mặc định chạy dưới user 'nginx' (non-root) trong các image alpine.
# Không cần tạo user mới hoặc thay đổi user.
# Tuy nhiên, cần đảm bảo quyền truy cập file cho Nginx.
# Trong Alpine, user 'nginx' có uid 101, gid 101.
# Chown files sang user/group 'nginx' để đảm bảo Nginx có thể đọc chúng.
RUN chown -R nginx:nginx /usr/share/nginx/html && \
    chmod -R 755 /usr/share/nginx/html && \
    chown -R nginx:nginx /var/cache/nginx && \
    chown -R nginx:nginx /var/run && \
    chown -R nginx:nginx /var/log/nginx

USER nginx

# Expose cổng mặc định của Nginx
EXPOSE 80

# Chạy Nginx
CMD ["nginx", "-g", "daemon off;"]