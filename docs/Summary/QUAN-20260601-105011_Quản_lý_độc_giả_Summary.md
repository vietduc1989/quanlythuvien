**Tóm tắt trạng thái dự án: Quản lý độc giả (`QUAN-20260601-105011`)**

**1. Module:**
Tính năng "Quản lý độc giả" là module cốt lõi trong hệ thống thư viện ONENET, hoạt động như một dịch vụ backend độc lập. Module này cung cấp dữ liệu nền tảng và tương tác với các module khác qua API. Giao diện người dùng sẽ được phát triển bằng React + Mantine UI.

**2. Yêu cầu (Requirements):**
*   **Chức năng:**
    *   **CRUD độc giả:** Tạo mới (tự động sinh mã độc giả `DG[YYYYMMDD][SequentialNumber]`, kiểm tra SĐT duy nhất), xem (danh sách có tìm kiếm, lọc, phân trang; chi tiết theo ID), cập nhật thông tin độc giả.
    *   **Phạm vi loại trừ:** Không hỗ trợ xóa vĩnh viễn độc giả, tự động gia hạn thẻ, tích hợp hệ thống thanh toán/hội viên bên ngoài, hoặc quản lý hình ảnh đại diện.
*   **Phi chức năng:**
    *   **Xác thực & Phân quyền (AuthN/AuthZ):** Tất cả API yêu cầu Bearer Token (JWT). Quyền `Librarian` cho tạo/cập nhật, `Librarian` hoặc `Viewer` cho xem danh sách/chi tiết.
    *   **Hiệu suất:** Thời gian phản hồi CUD/GetById dưới 1 giây, Get List dưới 3 giây (90% percentile). Yêu cầu indexing trên `ReaderCode`, `PhoneNumber`, `FullName`, `Status`, `ExpiryDate`.
    *   **Kiến trúc:** Clean Architecture, CQRS + MediatR trên .NET 10, ASP.NET Web API. Database PostgreSQL.

**3. Ràng buộc (Constraints):**
*   **Dữ liệu:** `ReaderCode` và `PhoneNumber` phải là duy nhất. `ExpiryDate` phải >= `RegistrationDate`. Các trường bắt buộc và validation chặt chẽ.
*   **Công nghệ:** Backend: .NET 10, ASP.NET Web API, EF Core, PostgreSQL. Frontend: React, Mantine UI.
*   **Kiến trúc DB:** Entity `Reader` (Id: UUID PK, ReaderCode: UK, PhoneNumber: UK, Status: enum). Sử dụng các trường audit `CreatedAt/By`, `LastModifiedAt/By`.

**4. Tiến độ (Progress):**
*   BRD (`QUAN-20260601-105011`): ✅ Đã duyệt.
*   SRS/SAD (Tài liệu này): ✅ Hiện tại (Đã duyệt về mặt tài liệu).
*   DEV (Mã nguồn): ⏳ Chờ SRS duyệt (Đang chờ phát triển).
*   TEST (Test cases): ⏳ Chờ DEV duyệt (Đang chờ kiểm thử).

**5. Vấn đề đã biết & Giảm thiểu (Known Issues & Mitigations):**
*   **Rủi ro cập nhật đồng thời (Concurrency Issue):** Cao. Giảm thiểu bằng cơ chế Optimistic Concurrency (kiểm tra `LastModifiedAt` trên server, trả về `409 Conflict` nếu có xung đột) và cột `xmin` của PostgreSQL.
*   **Rủi ro trùng lặp dữ liệu (SĐT):** Trung bình. Giảm thiểu bằng ràng buộc `UNIQUE INDEX` ở tầng Database bên cạnh kiểm tra ở tầng ứng dụng.
*   **Rủi ro giảm hiệu suất khi dữ liệu lớn:** Trung bình. Giảm thiểu bằng indexing các trường tìm kiếm/lọc, phân trang bắt buộc, và tối ưu hóa truy vấn (ví dụ: `AsNoTracking()`).
*   **Rủi ro truy cập trái phép:** Cao. Giảm thiểu bằng kiểm tra phân quyền chặt chẽ (`[Authorize(Roles="Librarian")]`) ở API Controller hoặc MediatR pipeline.