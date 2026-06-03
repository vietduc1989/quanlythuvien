**Memory Summarizer Agent - Tóm tắt dự án**

---

**Project:** `quanlythuvien`
**Module:** Quản lý sách (`QUAN-20260602-104003`)

**Mục tiêu chính:**
Cung cấp hệ thống hiệu quả cho Thủ thư quản lý thông tin sách (thêm, xem, chỉnh sửa, xóa) nhằm đảm bảo dữ liệu chính xác và cập nhật, đồng thời đặt nền tảng cho các module mượn/trả và quản lý kho.

**Trạng thái tài liệu & Tiến độ:**
- BRD (`QUAN-20260602-104003`): ✅ Hiện tại.
- SRS/SAD: ⏳ Chờ BA duyệt.
- DEV (mã nguồn): ⏳ Chờ SRS duyệt.
- TEST (test cases): ⏳ Chờ DEV duyệt.

**Phạm vi (In Scope):**
- Tạo mới, xem danh sách/chi tiết, chỉnh sửa, và xóa thông tin sách.
- Tìm kiếm và lọc sách theo nhiều tiêu chí.

**Yêu cầu chức năng (Functional Requirements - FRs):**
Hệ thống phải hỗ trợ Thủ thư các tác vụ quản lý sách cơ bản bao gồm:
1.  **FR01:** Tạo mới sách.
2.  **FR02:** Xem danh sách sách.
3.  **FR03:** Xem chi tiết sách.
4.  **FR04:** Chỉnh sửa sách.
5.  **FR05:** Xóa sách.
6.  **FR06:** Tìm kiếm và lọc sách (theo tên, tác giả, ISBN, thể loại).

**Yêu cầu phi chức năng (Non-functional Requirements - NFRs):**
-   **Hiệu năng:** Tải danh sách sách (<2s cho 1000 đầu sách), CRUD (<1s). Hỗ trợ 20 Thủ thư đồng thời.
-   **Bảo mật:** Chỉ Thủ thư có quyền mới được truy cập và thao tác; xác thực vai trò; mã hóa dữ liệu nhạy cảm (khi truyền/lưu trữ); kiểm soát truy cập theo trường.
-   **Khả dụng:** Uptime tối thiểu 99.5% trong giờ làm việc (T2-T6, 8:00-17:00).

**Ràng buộc & Quy tắc nghiệp vụ (Constraints & Business Rules - BRs):**
-   **BR01:** Tên sách là bắt buộc.
-   **BR02:** Mã ISBN phải là duy nhất.
-   **BR03:** Không thể xóa sách nếu có bản sao đang được mượn.
-   **BR04:** Số lượng tổng (bản sao) phải là số nguyên dương.

**Phụ thuộc (Dependencies):**
-   Module Quản lý Người dùng và Phân quyền (để xác thực và cấp quyền Thủ thư).
-   Module Quản lý Danh mục (để quản lý Tác giả, Nhà xuất bản, Thể loại).

**Audit Trail:**
-   Hệ thống phải ghi lại lịch sử thao tác thêm, sửa, xóa sách (người thực hiện, thời gian, loại thao tác, chi tiết thay đổi, mã sách bị ảnh hưởng).

**Vấn đề đã biết (Known Issues):**
-   Không có vấn đề đã biết được báo cáo rõ ràng trong tài liệu này. Tuy nhiên, các tiêu chí chấp nhận đã bao gồm chi tiết các kịch bản lỗi (ví dụ: ISBN trùng, thiếu trường bắt buộc, xóa sách đang mượn) mà hệ thống cần xử lý và thông báo lỗi tương ứng.

---