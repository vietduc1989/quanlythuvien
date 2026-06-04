#!/bin/bash
# scripts/deploy.sh - Script triển khai tích hợp cho dự án Quản lý Thư viện (ONENET)

set -euo pipefail # Thoát ngay lập tức nếu có lệnh lỗi hoặc biến chưa được định nghĩa

PROJECT_NAME="onenet-library"
COMPOSE_FILE="docker-compose.yml"
ENV_FILE=".env"

# Đường dẫn các image trên GHCR (sử dụng biến môi trường hoặc mặc định)
API_IMAGE="${API_IMAGE:-ghcr.io/vietduc1989/quanlythuvien/api:latest}"
WEB_IMAGE="${WEB_IMAGE:-ghcr.io/vietduc1989/quanlythuvien/web:latest}"

echo "=== Bắt đầu triển khai dự án ${PROJECT_NAME} ==="

# 1. Kiểm tra sự tồn tại của file .env
if [ ! -f "${ENV_FILE}" ]; then
  echo "Lỗi: Không tìm thấy file ${ENV_FILE}. Vui lòng sao chép và cấu hình từ .env.example."
  exit 1
fi

# 2. Đăng nhập Docker Registry (nếu có thông tin đăng nhập)
# Mặc định sử dụng GITHUB_TOKEN để đăng nhập GHCR trong CI
if [ -n "${DOCKER_USERNAME:-}" ] && [ -n "${DOCKER_PASSWORD:-}" ]; then
    echo "Đang đăng nhập vào Docker Registry..."
    echo "${DOCKER_PASSWORD}" | docker login -u "${DOCKER_USERNAME}" --password-stdin
    echo "Đăng nhập Registry thành công."
elif [ -n "${GITHUB_TOKEN:-}" ]; then
    echo "Đang đăng nhập vào GitHub Container Registry (GHCR)..."
    echo "${GITHUB_TOKEN}" | docker login ghcr.io -u "${github_actor:-github}" --password-stdin
    echo "Đăng nhập GHCR thành công."
else
    echo "Không tìm thấy thông tin đăng nhập registry. Sử dụng images có sẵn hoặc build tại chỗ."
fi

# 3. Pull các Docker image mới nhất (nếu có sẵn trên Registry)
echo "Đang kiểm tra và tải các Docker image mới nhất..."
docker pull "${API_IMAGE}" || echo "Không thể tải image ${API_IMAGE}, sẽ tự build tại chỗ nếu cần."
docker pull "${WEB_IMAGE}" || echo "Không thể tải image ${WEB_IMAGE}, sẽ tự build tại chỗ nếu cần."

# 4. Dừng các container cũ
echo "Đang dừng các container cũ (nếu có)..."
docker-compose -p "${PROJECT_NAME}" -f "${COMPOSE_FILE}" --env-file "${ENV_FILE}" down --remove-orphans || true

# 5. Khởi động các service mới
echo "Đang khởi động các service bằng docker-compose..."
# Sử dụng biến môi trường để truyền tên image cho docker-compose
export API_IMAGE
export WEB_IMAGE
docker-compose -p "${PROJECT_NAME}" -f "${COMPOSE_FILE}" --env-file "${ENV_FILE}" up -d --build --remove-orphans

# 6. Đợi các service khởi động và kiểm tra healthcheck
echo "Đang kiểm tra trạng thái hoạt động của các service..."
TIMEOUT=120 # 2 phút
ELAPSED=0
HEALTHY_SERVICES=0
REQUIRED_SERVICES=3 # db, api, web

while [ "$HEALTHY_SERVICES" -lt "$REQUIRED_SERVICES" ] && [ "$ELAPSED" -lt "$TIMEOUT" ]; do
    # Đếm số container đạt trạng thái healthy
    HEALTHY_SERVICES=$(docker-compose -p "${PROJECT_NAME}" -f "${COMPOSE_FILE}" ps | grep "healthy" | wc -l)
    echo "Trạng thái service: $HEALTHY_SERVICES/$REQUIRED_SERVICES healthy. Đang chờ... ($ELAPSED/${TIMEOUT}s)"
    
    if [ "$HEALTHY_SERVICES" -lt "$REQUIRED_SERVICES" ]; then
        sleep 5
        ELAPSED=$((ELAPSED + 5))
    fi
done

if [ "$HEALTHY_SERVICES" -eq "$REQUIRED_SERVICES" ]; then
    echo "Tất cả các service đã khởi động thành công và healthy!"
    echo "Web Frontend có thể truy cập tại http://localhost:80"
    echo "API Backend có thể truy cập tại http://localhost:8080"
else
    echo "Triển khai thất bại: Có service không ở trạng thái healthy sau $TIMEOUT giây."
    docker-compose -p "${PROJECT_NAME}" -f "${COMPOSE_FILE}" ps
    docker-compose -p "${PROJECT_NAME}" -f "${COMPOSE_FILE}" logs --tail 50
    exit 1
fi

# 7. Dọn dẹp Docker images rác (dangling images)
echo "Đang dọn dẹp các Docker images dư thừa..."
docker image prune -f

echo "=== Quá trình triển khai dự án ${PROJECT_NAME} hoàn tất ==="
