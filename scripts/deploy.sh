```bash
#!/bin/bash
# scripts/deploy.sh - Script triển khai cho môi trường production/staging
# Kịch bản này giả định bạn đã có một máy chủ với Docker và Docker Compose đã được cài đặt.
# Và các biến môi trường cần thiết (ví dụ: DOCKER_USERNAME, DOCKER_PASSWORD, các biến trong .env)
# đã được thiết lập trên máy chủ hoặc được truyền vào script.

set -euo pipefail # Thoát ngay lập tức nếu có lệnh fail, hoặc biến chưa được định nghĩa

# --- Cấu hình ---
PROJECT_NAME="onenet-report" # Tên dự án Docker Compose
DOCKER_USERNAME="${DOCKER_USERNAME:-}" # Biến môi trường cho Docker username
DOCKER_PASSWORD="${DOCKER_PASSWORD:-}" # Biến môi trường cho Docker password
# Đường dẫn đến file docker-compose.yml và .env trên máy chủ triển khai
COMPOSE_FILE="docker-compose.yml"
ENV_FILE=".env"

# Tên và tag của các Docker Image đã được build và push từ CI/CD
API_IMAGE="onenet/report-api:latest"
WEB_IMAGE="onenet/report-web:latest"

echo "--- Bắt đầu triển khai cho dự án ${PROJECT_NAME} ---"

# --- 1. Đăng nhập Docker Registry ---
if [ -n "$DOCKER_USERNAME" ] && [ -n "$DOCKER_PASSWORD" ]; then
    echo "Đang đăng nhập vào Docker Registry..."
    echo "${DOCKER_PASSWORD}" | docker login -u "${DOCKER_USERNAME}" --password-stdin
    echo "Đăng nhập Docker thành công."
else
    echo "Cảnh báo: Không tìm thấy DOCKER_USERNAME hoặc DOCKER_PASSWORD. Bỏ qua bước đăng nhập Docker."
    echo "Đảm bảo rằng các image đã được pull trước hoặc có sẵn trên host."
fi

# --- 2. Kiểm tra và Pull các Docker Image mới nhất ---
echo "Đang pull các Docker images mới nhất..."
docker pull "${API_IMAGE}"
docker pull "${WEB_IMAGE}"
echo "Các images đã được pull thành công."

# --- 3. Dừng và xóa các dịch vụ cũ (nếu có) ---
echo "Đang dừng và xóa các dịch vụ cũ..."
# Sử dụng '|| true' để script không bị lỗi nếu không có dịch vụ nào đang chạy
docker-compose -p "${PROJECT_NAME}" -f "${COMPOSE_FILE}" --env-file "${ENV_FILE}" down || true
echo "Các dịch vụ cũ đã được dừng."

# --- 4. Khởi động các dịch vụ mới ---
echo "Đang khởi động các dịch vụ mới bằng docker-compose..."
docker-compose -p "${PROJECT_NAME}" -f "${COMPOSE_FILE}" --env-file "${ENV_FILE}" up -d --remove-orphans
# -d: chạy ở chế độ detached (nền)
# --remove-orphans: xóa các container không còn được định nghĩa trong compose file
echo "Triển khai hoàn tất."

# --- Tùy chọn: Dọn dẹp các Docker images cũ không còn được sử dụng ---
echo "Đang dọn dẹp các Docker images không còn được sử dụng..."
docker image prune -f
echo "Dọn dẹp hoàn tất."

echo "--- Quá trình triển khai dự án ${PROJECT_NAME} đã hoàn thành ---"
```