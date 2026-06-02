# apps/web/Dockerfile
# Dockerfile cho ứng dụng Frontend React Vite

# Stage 1: Build the React application
FROM node:20-alpine AS builder
WORKDIR /app

# Copy package.json and package-lock.json (or yarn.lock) to leverage Docker layer caching
COPY apps/web/package*.json ./
RUN npm install

# Copy the rest of the application code
COPY apps/web/ .

# Build the React app
RUN npm run build

# Stage 2: Serve the application with Nginx
FROM nginx:stable-alpine AS final

# Create a non-root user for Nginx for enhanced security
# Nginx's default user is 'nginx', which is a non-root user, so we will use it.
# We just need to make sure the directories are accessible.
# The `nginx` user is typically part of the `nginx` group, which is fine.

# Remove default Nginx configuration
RUN rm -rf /etc/nginx/conf.d/*

# Copy custom Nginx configuration
COPY apps/web/nginx/nginx.conf /etc/nginx/conf.d/default.conf

# Copy the built React app from the builder stage
COPY --from=builder /app/dist /usr/share/nginx/html

# Adjust permissions for Nginx to serve the files
RUN chown -R nginx:nginx /usr/share/nginx/html \
    && chmod -R 755 /usr/share/nginx/html

# Expose port 80 for web traffic
EXPOSE 80

# Command to run Nginx
CMD ["nginx", "-g", "daemon off;"]