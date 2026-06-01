# AI Development Rules — quanlythuvien

Tài liệu này định nghĩa các quy tắc bắt buộc dành cho các AI Agent khi tham gia phát triển dự án Quản lý Thư viện.

## 1. Nguyên Tắc Cốt Lõi

- **Tuân thủ Technical Guideline**: Đọc và áp dụng 100% các quy chuẩn trong [Technical_Guideline.md](file:///d:/Project/ONENET.AgentFactory/docs/rules/Technical_Guideline.md).
- **Không sử dụng placeholder**: Mọi đoạn code được sinh ra phải hoạt động hoàn chỉnh, không để lại comment `// TODO` hoặc `// Implement later` đối với các tính năng cốt lõi được yêu cầu.
- **Dependency Rule**: Tuân thủ nghiêm ngặt chiều phụ thuộc:
  `WebAPI -> Application -> Domain` và `Infrastructure -> Application -> Domain`. Không bao giờ import ngược chiều phụ thuộc.

## 2. Quy Trình Phát Triển của Agent

1. **BA Agent (Business Analyst)**: Tổng hợp BRD lưu tại `docs/BA/`.
2. **SA Agent (Solution Architect)**: Thiết kế tài liệu SRS và sơ đồ thực thể lưu tại `docs/SA/`.
3. **Developer Agent**: Phát triển mã nguồn. Luôn thực hiện Unit Test tương ứng.
4. **QA Agent**: Viết các TestCase lưu tại `docs/QA/` và thực hiện kiểm thử.
5. **DevOps Agent**: Cấu hình deployment (Docker, CI/CD).

## 3. Quy Định về Code và Git

- **Pull Request**: Tiêu đề PR phải tuân thủ format: `[BRD|SRS|DEV|TEST|DEVOPS] Tên tính năng`.
- **Độc lập và Clean**: Mỗi file chỉ chứa 1 class/component chính. Độ dài tối đa 300 dòng.
- **Kiểm thử**: Mọi tính năng phát triển ở layer Application và Infrastructure phải có Unit Test đi kèm trong thư mục `tests/`.
