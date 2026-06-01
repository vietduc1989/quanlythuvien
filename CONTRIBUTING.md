# Contributing Guidelines — quanlythuvien

Tài liệu này hướng dẫn cách thức đóng góp mã nguồn và quy trình làm việc trên Git/GitHub cho dự án Quản lý Thư viện.

## 1. Quy trình làm việc (Git Workflow)

Chúng tôi sử dụng mô hình **Git Flow** đơn giản hóa:
- **main/master**: Nhánh chứa mã nguồn ổn định nhất đang chạy production.
- **dev**: Nhánh chính dùng để tích hợp mã nguồn từ các tính năng đang phát triển.
- **feature/*, bugfix/*, docs/*, hotfix/***: Các nhánh phát triển độc lập cho từng task.

### Các bước đóng góp:
1. Từ nhánh `dev`, kéo mã nguồn mới nhất: `git checkout dev && git pull`.
2. Tạo nhánh mới tương ứng với tính năng cần làm: `git checkout -b feature/quan-ly-sach`.
3. Tiến hành phát triển, viết Unit Test và kiểm tra kỹ chất lượng code.
4. Tạo Pull Request (PR) đẩy code về nhánh `dev`.
5. Chờ Reviewer phê duyệt và merge PR.

---

## 2. Quy ước đặt tên nhánh và PR

### Nhánh (Branch):
- `{loại nhánh}/{tên-tính-năng}`
- Ví dụ: `feature/book-management`, `bugfix/fix-borrow-logic`, `docs/update-readme`.

### Pull Request (PR):
- Tiêu đề PR phải tuân theo format: `[BRD|SRS|DEV|TEST|DEVOPS] Tên tính năng`
- Ví dụ:
  - `[BRD] Quản lý sách`
  - `[SRS] Quản lý mượn trả`
  - `[DEV] Xây dựng API và giao diện Quản lý sách`
  - `[TEST] Thêm testcase cho tính năng trả sách`
  - `[DEVOPS] Cấu hình Docker Compose cho PostgreSQL và API`

---

## 3. Quy tắc tích hợp liên tục (CI)
- Mỗi PR được đẩy lên sẽ kích hoạt quy trình CI để build dự án và chạy Unit Tests.
- PR chỉ được merge khi:
  - Vượt qua vòng build và test tự động 100%.
  - Được phê duyệt bởi ít nhất 1 Tech Lead / Reviewer.
  - Không bị conflict với nhánh `dev`.
