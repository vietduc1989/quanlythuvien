```markdown
## Tóm tắt trạng thái dự án: Quản lý độc giả (QUAN-20260601-104445)

**Dự án:** `quanlythuvien`
**Mã tính năng:** `QUAN-20260601-104445`
**Tên tính năng:** Quản lý độc giả

**Module:**
*   Module chính tập trung vào việc quản lý thông tin độc giả trong hệ thống thư viện, bao gồm các chức năng tạo mới, xem, cập nhật và xóa độc giả. Module này cũng tự động sinh mã độc giả duy nhất.

**Requirements:**
*   **Chức năng (FR):** Cung cấp các thao tác CRUD (tạo, xem, sửa, xóa) thông tin độc giả và tự động sinh mã độc giả duy nhất.
*   **Nghiệp vụ (BR):**
    *   Mã độc giả và Số điện thoại của độc giả phải là duy nhất.
    *   Định nghĩa ràng buộc: một độc giả chỉ được mượn tối đa 5 cuốn sách; độc giả có thẻ hết hạn không được tạo phiếu mượn mới (các ràng buộc này sẽ được thực thi tại module Quản lý mượn/trả sách).
*   **Phi chức năng (NFR):**
    *   **Hiệu suất:** Phản hồi thao tác CRUD < 2 giây; tải danh sách độc giả (1000 độc giả) < 3 giây.
    *   **Bảo mật:** Phân quyền truy cập (Thủ thư/Quản trị viên), bảo vệ dữ liệu cá nhân, chống XSS/SQL Injection.
    *   **Sẵn sàng:** Uptime tối thiểu 99.5% trong giờ làm việc.
*   **Phạm vi (In Scope):** Tạo/Xem/Sửa/Xóa thông tin độc giả với các trường cơ bản (Họ tên, SĐT, Email, Địa chỉ, Ngày sinh, Ngày đăng ký/hết hạn, Trạng thái), tự động sinh mã, kiểm tra duy nhất SĐT.
*   **Ngoài phạm vi (Out of Scope):** Tìm kiếm nâng cao, Import/Export, Quản lý phiếu mượn/trả (là module riêng), in thẻ, gửi thông báo tự động.

**Constraints:**
*   Không thể xóa độc giả nếu họ đang có sách mượn.
*   Số điện thoại của mỗi độc giả phải duy nhất trong toàn hệ thống.

**Progress:**
*   **BRD:** Đã hoàn thành (✅ Hiện tại).
*   **SRS/SAD:** Đang chờ BA duyệt (⏳).
*   **DEV (mã nguồn):** Chờ SRS duyệt (⏳).
*   **TEST (test cases):** Chờ DEV duyệt (⏳).

**Known Issues:**
*   Không có vấn đề nổi bật nào được xác định rõ ràng trong tài liệu BRD này.
```