# BRD-01 – Quản lý sách

## 1. Thông tin tài liệu

| Thuộc tính | Giá trị |
|---|---:|
| Module | Book Management |
| Mã tài liệu | BRD-01 |
| Phiên bản | 1.0 |
| Loại | Business Requirements Document |

## 2. Mục tiêu nghiệp vụ

Cho phép thư viện quản lý toàn bộ danh mục sách hiện có, theo dõi số lượng tồn kho và trạng thái sẵn sàng cho mượn.

## 3. User Story

**Là thủ thư**, tôi muốn quản lý thông tin sách để dễ dàng theo dõi, tìm kiếm và phục vụ việc mượn/trả.

## 4. Chức năng chính

- Thêm sách mới
- Cập nhật sách
- Xóa sách
- Tra cứu sách

## 5. Thông tin dữ liệu

- Mã sách
- Tên sách
- Tác giả
- Thể loại
- Nhà xuất bản
- Năm xuất bản
- Số lượng
- Vị trí kệ
- Trạng thái

## 6. Business Rules

- Mỗi sách phải có mã duy nhất
- Số lượng tồn kho không được nhỏ hơn 0
- Không cho phép xóa sách nếu đang có độc giả mượn
- Nếu số lượng = 0 thì trạng thái hiển thị “Hết sách”

## 7. Acceptance Criteria

- Có thể tạo mới sách
- Có thể chỉnh sửa sách
- Có thể xóa sách
- Có thể tìm kiếm sách
- Hiển thị tồn kho chính xác
