#
# scripts/deploy.sh - Script triển khai mẫu
#
# Đây là một script mẫu đơn giản để khởi động các service bằng docker-compose.
# Trong môi trường sản phẩm thực tế, bạn sẽ sử dụng các công cụ mạnh mẽ hơn như Kubernetes,
# Docker Swarm, hoặc các dịch vụ cloud (AWS ECS/EKS, Azure AKS, GCP GKE).
#
# Trước khi chạy, đảm bảo đã có file .env với các biến môi trường cần thiết
# và các Docker image đã được build/pull.
#

#!/bin/bash
set -e # Thoát ngay lập tức nếu có lệnh lỗi

echo "Bắt đầu triển khai dự án Quản lý mượn trả sách..."

# Kiểm tra xem file .env có tồn tại không
if [ ! -f .env ]; then
  echo "Lỗi: File .env không tìm thấy. Vui lòng tạo một file .env từ .env.example."
  exit 1
fi

echo "Đang dừng các container cũ (nếu có)..."
docker-compose -f docker-compose.yml down --remove-orphans || true # Cho phép lỗi nếu chưa có container

echo "Đang kéo các Docker image mới nhất từ registry (nếu cần)..."
# Trong môi trường CI/CD thực tế, bạn sẽ kéo image đã build từ GHCR hoặc Docker Hub
# Ví dụ: docker pull ghcr.io/your_org/your_repo/api:latest
#       docker pull ghcr.io/your_org/your_repo/web:latest
# Đối với deploy cục bộ, docker-compose build sẽ build lại từ Dockerfile
# Hoặc bạn có thể override image trong docker-compose.yml để dùng image từ registry

echo "Đang khởi tạo hoặc cập nhật các service với docker-compose..."
# -d: Chạy container ở chế độ detached (nền)
# --build: Buộc xây dựng lại image nếu có thay đổi trong Dockerfile hoặc context (hoặc bỏ đi để dùng image có sẵn)
# --force-recreate: Tái tạo các container ngay cả khi cấu hình không thay đổi
docker-compose -f docker-compose.yml up -d --build --force-recreate

echo "Đợi các service khởi động và kiểm tra trạng thái..."
# Đợi cho tất cả các service đạt trạng thái healthy
docker-compose -f docker-compose.yml ps
docker-compose -f docker-compose.yml logs -f --tail 20

echo "Kiểm tra healthcheck của các service..."
# Lặp lại kiểm tra healthcheck cho đến khi tất cả service healthy hoặc hết thời gian
TIMEOUT=120 # 2 phút
ELAPSED=0
HEALTHY_SERVICES=0
REQUIRED_SERVICES=3 # db, api, web

while [ "$HEALTHY_SERVICES" -lt "$REQUIRED_SERVICES" ] && [ "$ELAPSED" -lt "$TIMEOUT" ]; do
    HEALTHY_SERVICES=$(docker-compose -f docker-compose.yml ps | grep "healthy" | wc -l)
    echo "Trạng thái service: $HEALTHY_SERVICES/$REQUIRED_SERVICES healthy. Đang chờ... ($ELAPSED/${TIMEOUT}s)"
    if [ "$HEALTHY_SERVICES" -lt "$REQUIRED_SERVICES" ]; then
        sleep 5
        ELAPSED=$((ELAPSED + 5))
    fi
done

if [ "$HEALTHY_SERVICES" -eq "$REQUIRED_SERVICES" ]; then
    echo "Tất cả các service đã khởi động thành công và healthy!"
    echo "API có thể truy cập tại http://localhost:8080"
    echo "Web Frontend có thể truy cập tại http://localhost:3000"
else
    echo "Triển khai thất bại: Không phải tất cả các service đều healthy sau $TIMEOUT giây."
    exit 1
fi

echo "Triển khai hoàn tất."