## Test Specification: Báo cáo thống kê thư viện

**Mã tính năng:** QUAN-20260601-1634
**Tên tính năng:** Báo cáo thống kê thư viện
**BRD PR#:** 4
**SRS PR#:** 5
**DEV PR#:** 12
**Phiên bản:** 1.0
**Ngày tạo:** 2024-07-30
**Người tạo:** Kỹ sư QA/Tester cấp cao ONENET

---

### 1. Giới thiệu

Tài liệu này mô tả các kịch bản kiểm thử chi tiết cho tính năng "Báo cáo thống kê thư viện" (QUAN-20260601-1634) trong hệ thống ONENET. Dựa trên `BRD PR# 4`, `SRS PR# 5`, và `DEV PR# 12`, tài liệu này bao gồm các loại kiểm thử Functional, API, Security, Boundary/Validation để đảm bảo chất lượng toàn diện của tính năng.

Lưu ý: Mặc dù tên tính năng là "Báo cáo thống kê thư viện", `DEV PR# 12` chủ yếu tập trung vào các thay đổi kiến trúc hạ tầng và cơ chế xử lý lỗi/phản hồi API, soft-delete, và tích hợp frontend cơ bản. Do đó, các Test Case sẽ tập trung kiểm tra các khía cạnh nền tảng này, vốn là điều kiện tiên quyết và hỗ trợ cho bất kỳ tính năng báo cáo nào được xây dựng sau này.

---

### 2. Môi trường kiểm thử

*   **Hệ điều hành Server:** Linux (Ubuntu 20.04+) hoặc Windows Server 2019+
*   **Cơ sở dữ liệu:** PostgreSQL 13+
*   **Runtime:** .NET 9.0
*   **API Gateway/Load Balancer:** Nginx (nếu có)
*   **Trình duyệt Frontend:** Chrome (phiên bản mới nhất), Firefox (phiên bản mới nhất)
*   **Môi trường:** Development (local hoặc dev server)

---

### 3. Dữ liệu kiểm thử (Test Data Pre-conditions)

Để thực hiện các kịch bản kiểm thử, cần chuẩn bị các dữ liệu sau trong hệ thống:

*   **Users:**
    *   `AdminUser`: ID: `user-admin-001`, Username: `admin`, Password: `password123`, Roles: `Admin` (có toàn quyền truy cập).
    *   `ManagerUser`: ID: `user-manager-002`, Username: `manager`, Password: `password123`, Roles: `Manager` (có quyền xem báo cáo, nhưng không có quyền xóa sách).
    *   `LibrarianUser`: ID: `user-librarian-003`, Username: `librarian`, Password: `password123`, Roles: `Librarian` (có quyền quản lý sách, mượn trả, nhưng không có quyền xem báo cáo hoặc xóa sách).
    *   `ReaderUser`: ID: `user-reader-004`, Username: `reader`, Password: `password123`, Roles: `Reader` (chỉ có quyền xem thông tin cá nhân và sách).
*   **Books:**
    *   `BookA`: ID: `book-001`, Tên: "Sách A", Tác giả: "Tác giả A", Số lượng: 5, Trạng thái: "Available".
    *   `BookB`: ID: `book-002`, Tên: "Sách B", Tác giả: "Tác giả B", Số lượng: 1, Trạng thái: "Borrowed" (đang được độc giả mượn).
    *   `BookC`: ID: `book-003`, Tên: "Sách C", Tác giả: "Tác giả C", Số lượng: 10, Trạng thái: "Available".
    *   `BookD_Deleted`: ID: `book-004`, Tên: "Sách D (đã xóa)", Tác giả: "Tác giả D", Số lượng: 2, Trạng thái: "Available", `IsDeleted = true`.
*   **Readers:**
    *   `ReaderX`: ID: `reader-001`, Tên: "Độc giả X", Trạng thái: "Active".
    *   `ReaderY`: ID: `reader-002`, Tên: "Độc giả Y", Trạng thái: "Active".
*   **Loans:**
    *   `Loan1`: ID: `loan-001`, `SachId`: `book-002`, `DocGiaId`: `reader-001`, Ngày mượn: 7 ngày trước, Ngày hẹn trả: 2 ngày trước, Trạng thái: "DangMuon" (quá hạn).
    *   `Loan2`: ID: `loan-002`, `SachId`: `book-001`, `DocGiaId`: `reader-002`, Ngày mượn: 3 ngày trước, Ngày hẹn trả: 4 ngày tới, Trạng thái: "DangMuon" (chưa quá hạn).
    *   `Loan3`: ID: `loan-003`, `SachId`: `book-003`, `DocGiaId`: `reader-001`, Ngày mượn: 10 ngày trước, Ngày trả thực tế: 5 ngày trước, Trạng thái: "DaTra".

---

### 4. Test Cases

#### 4.1. Functional Test Cases (QUAN-20260601-1634-TCXX)

| TC ID                           | Mục tiêu kiểm thử                                      | Tham chiếu | Mức độ ưu tiên | Loại kiểm thử |
| :------------------------------ | :----------------------------------------------------- | :--------- | :------------ | :------------ |
| QUAN-20260601-1634-TC01         | Đảm bảo endpoint HealthCheck trả về trạng thái khỏe mạnh. | AC1, SRS-ARCH | Cao           | Happy Path    |
| QUAN-20260601-1634-TC02         | Đảm bảo endpoint HealthCheck/error trả về lỗi hệ thống qua GlobalExceptionMiddleware. | AC1, AC3, SRS-ARCH | Cao           | Negative      |
| QUAN-20260601-1634-TC03         | Xác minh chức năng xóa mềm (soft delete) cho sách.      | AC5, BR-SD | Cao           | Happy Path    |
| QUAN-20260601-1634-TC04         | Xác minh cơ chế soft delete khi sách đang được mượn.    | AC5, BR-SD | Trung         | Edge Case     |
| QUAN-20260601-1634-TC05         | Xác minh API GetReadersList trả về dữ liệu đúng định dạng `Result`. | AC4, SRS-ARCH | Cao           | Happy Path    |
| QUAN-20260601-1634-TC06         | Xác minh API GetLoans trả về dữ liệu đúng định dạng `Result`. | AC4, SRS-ARCH | Cao           | Happy Path    |
| QUAN-20260601-1634-TC07         | Xác minh CORS được áp dụng đúng cho frontend.           | AC7, SRS-ARCH | Cao           | Configuration |
| QUAN-20260601-1634-TC08         | Xác minh frontend có thể kết nối và hiển thị trạng thái HealthCheck. | AC9, FR-FE | Cao           | Integration   |
| QUAN-20260601-1634-TC09         | Xác minh frontend có thể gọi endpoint lỗi và xử lý thông báo. | AC9, FR-FE | Cao           | Integration   |

---

**QUAN-20260601-1634-TC01: Kiểm tra trạng thái API HealthCheck**

*   **Mục tiêu:** Đảm bảo endpoint `/api/v1/HealthCheck` trả về trạng thái thành công và thông báo "API is healthy."
*   **Tham chiếu:** BRD PR#4 (Tổng quan hệ thống), SRS PR#5 (API Stability), AC1
*   **Mức độ ưu tiên:** Cao
*   **Loại kiểm thử:** Happy Path

*   **Test Steps:**
    1.  **Given** API backend đang chạy bình thường
    2.  **When** Gửi một yêu cầu GET đến `/api/v1/HealthCheck`
    3.  **Then** API trả về status code `200 OK`
    4.  **And** Nội dung phản hồi có `Success: true`
    5.  **And** Nội dung phản hồi có `Data: "API is healthy."`
    6.  **And** Nội dung phản hồi có `Message: null`
    7.  **And** Log hệ thống ghi nhận yêu cầu HealthCheck với cấp độ `Information`.

---

**QUAN-20260601-1634-TC02: Kiểm tra xử lý lỗi hệ thống qua HealthCheck/error**

*   **Mục tiêu:** Đảm bảo khi một lỗi không mong muốn xảy ra, `GlobalExceptionMiddleware` bắt được và trả về phản hồi chuẩn hóa `ApiResponse` với status `500 Internal Server Error`.
*   **Tham chiếu:** BRD PR#4 (Ổn định hệ thống), SRS PR#5 (Xử lý lỗi), AC1, AC2, AC3
*   **Mức độ ưu tiên:** Cao
*   **Loại kiểm thử:** Negative

*   **Test Steps:**
    1.  **Given** API backend đang chạy bình thường
    2.  **When** Gửi một yêu cầu GET đến `/api/v1/HealthCheck/error`
    3.  **Then** API trả về status code `500 Internal Server Error`
    4.  **And** Nội dung phản hồi có `Success: false`
    5.  **And** Nội dung phản hồi có `Message: "Đã xảy ra lỗi hệ thống không mong muốn."`
    6.  **And** Log hệ thống ghi nhận lỗi với cấp độ `Error` và chi tiết exception.

---

**QUAN-20260601-1634-TC03: Xác minh chức năng xóa mềm (soft delete) cho sách**

*   **Mục tiêu:** Đảm bảo khi xóa một cuốn sách, sách không bị xóa vật lý mà được đánh dấu `IsDeleted = true`.
*   **Tham chiếu:** BRD PR#4 (Tính toàn vẹn dữ liệu), SRS PR#5 (Quản lý dữ liệu), AC5, BR-SD (Business Rule: Soft Delete)
*   **Test Data Pre-conditions:** `BookC` (ID: `book-003`)
*   **Mức độ ưu tiên:** Cao
*   **Loại kiểm thử:** Happy Path

*   **Test Steps:**
    1.  **Given** Có một cuốn sách `BookC` (ID: `book-003`) với `IsDeleted = false` trong cơ sở dữ liệu.
    2.  **When** Người dùng `AdminUser` (có quyền xóa) gửi yêu cầu DELETE đến `/api/v1/Books/{book-003}`.
    3.  **Then** API trả về status code `200 OK` hoặc `204 No Content` (tùy theo thiết kế API Delete).
    4.  **And** Truy vấn trực tiếp cơ sở dữ liệu, sách `book-003` vẫn tồn tại.
    5.  **And** Trường `IsDeleted` của `book-003` được cập nhật thành `true`.
    6.  **And** Trường `UpdatedBy` được cập nhật thành ID của `AdminUser`.
    7.  **And** Trường `UpdatedAt` được cập nhật thời gian hiện tại.

---

**QUAN-20260601-1634-TC04: Xác minh cơ chế soft delete khi sách đang được mượn**

*   **Mục tiêu:** Đảm bảo sách đang được mượn vẫn được xử lý soft-delete, và rule nghiệp vụ "Không cho phép xóa sách nếu đang có độc giả mượn" được diễn giải là *không cho phép xóa vật lý* (nhưng vẫn có thể soft delete nếu có yêu cầu nghiệp vụ khác). Dựa trên PR Diff, đoạn comment về rule này đã bị xóa nhưng kiểm tra `isBorrowed` vẫn còn, ngụ ý logic kiểm tra vẫn tồn tại.
*   **Tham chiếu:** BRD PR#4 (Quy trình nghiệp vụ, Tính toàn vẹn dữ liệu), SRS PR#5 (Xử lý sách), AC5, BR-SD
*   **Test Data Pre-conditions:** `BookB` (ID: `book-002`) đang có `Loan1` và `Loan2` ở trạng thái "DangMuon".
*   **Mức độ ưu tiên:** Trung bình
*   **Loại kiểm thử:** Edge Case

*   **Test Steps:**
    1.  **Given** Có một cuốn sách `BookB` (ID: `book-002`) với `IsDeleted = false` và có ít nhất một phiếu mượn đang ở trạng thái "DangMuon".
    2.  **When** Người dùng `AdminUser` (có quyền xóa) gửi yêu cầu DELETE đến `/api/v1/Books/{book-002}`.
    3.  **Then** API trả về status code `200 OK` hoặc `204 No Content`.
    4.  **And** Truy vấn trực tiếp cơ sở dữ liệu, sách `book-002` vẫn tồn tại.
    5.  **And** Trường `IsDeleted` của `book-002` được cập nhật thành `true`.
    6.  **And** Trường `UpdatedBy` được cập nhật thành ID của `AdminUser`.
    7.  **And** Trường `UpdatedAt` được cập nhật thời gian hiện tại.
    *   **Lưu ý kiểm tra:** Đoạn code `isBorrowed = await _context.Set<ChiTietPhieuMuon>().AnyAsync(ld => ld.SachId == request.Id && ld.TrangThaiChiTiet == "DangMuon", cancellationToken);` vẫn còn. Nếu sau này có nghiệp vụ chặn xóa (kể cả soft delete) sách đang mượn, thì bước 3 phải là `400 Bad Request` hoặc `409 Conflict`. Hiện tại, dựa vào việc comment bị xóa và hành vi `EntityState.Deleted` chuyển thành `Modified` để soft delete, giả định soft delete vẫn được phép. Cần làm rõ với BA/Dev nếu cần.

---

**QUAN-20260601-1634-TC05: Xác minh API GetReadersList trả về dữ liệu đúng định dạng `Result`**

*   **Mục tiêu:** Đảm bảo các truy vấn danh sách độc giả sử dụng `ONENET.Domain.Common.Result<PaginatedList<ReaderListItemDto>>` để trả về kết quả.
*   **Tham chiếu:** BRD PR#4 (Cấu trúc dữ liệu), SRS PR#5 (Truy vấn dữ liệu), AC4, SRS-ARCH (Domain Result Pattern)
*   **Mức độ ưu tiên:** Cao
*   **Loại kiểm thử:** Happy Path

*   **Test Steps:**
    1.  **Given** Có ít nhất một độc giả đang hoạt động trong hệ thống.
    2.  **When** Gửi yêu cầu GET đến `/api/v1/Readers?PageNumber=1&PageSize=10`
    3.  **Then** API trả về status code `200 OK`.
    4.  **And** Nội dung phản hồi là JSON có cấu trúc `ApiResponse` chuẩn.
    5.  **And** `ApiResponse.Success` là `true`.
    6.  **And** `ApiResponse.Data` chứa một object với các trường `IsSuccess: true` và `Value` (chứa `PaginatedList<ReaderListItemDto>`).
    7.  **And** `PaginatedList<ReaderListItemDto>` chứa `TotalCount`, `PageNumber`, `PageSize`, `TotalPages`, `HasPreviousPage`, `HasNextPage` và `Items` là danh sách độc giả.

---

**QUAN-20260601-1634-TC06: Xác minh API GetLoans trả về dữ liệu đúng định dạng `Result`**

*   **Mục tiêu:** Đảm bảo các truy vấn danh sách phiếu mượn sử dụng `ONENET.Domain.Common.Result<PaginatedList<LoanDto>>` để trả về kết quả.
*   **Tham chiếu:** BRD PR#4 (Cấu trúc dữ liệu), SRS PR#5 (Truy vấn dữ liệu), AC4, SRS-ARCH (Domain Result Pattern)
*   **Mức độ ưu tiên:** Cao
*   **Loại kiểm thử:** Happy Path

*   **Test Steps:**
    1.  **Given** Có ít nhất một phiếu mượn đang hoạt động hoặc đã trả trong hệ thống.
    2.  **When** Gửi yêu cầu GET đến `/api/v1/Loans?PageNumber=1&PageSize=10`
    3.  **Then** API trả về status code `200 OK`.
    4.  **And** Nội dung phản hồi là JSON có cấu trúc `ApiResponse` chuẩn.
    5.  **And** `ApiResponse.Success` là `true`.
    6.  **And** `ApiResponse.Data` chứa một object với các trường `IsSuccess: true` và `Value` (chứa `PaginatedList<LoanDto>`).
    7.  **And** `PaginatedList<LoanDto>` chứa `TotalCount`, `PageNumber`, `PageSize`, `TotalPages`, `HasPreviousPage`, `HasNextPage` và `Items` là danh sách phiếu mượn.

---

**QUAN-20260601-1634-TC07: Xác minh CORS được áp dụng đúng cho frontend**

*   **Mục tiêu:** Đảm bảo API backend chấp nhận các yêu cầu từ `http://localhost:5173` (URL của frontend trong `web/vite.config.ts`).
*   **Tham chiếu:** BRD PR#4 (Khả năng tương thích), SRS PR#5 (Bảo mật API), AC7, SRS-ARCH (CORS Configuration)
*   **Mức độ ưu tiên:** Cao
*   **Loại kiểm thử:** Configuration

*   **Test Steps:**
    1.  **Given** Frontend đang chạy tại `http://localhost:5173`.
    2.  **When** Frontend gửi một yêu cầu API (ví dụ: HealthCheck) đến backend.
    3.  **Then** Yêu cầu được xử lý thành công.
    4.  **And** Không có lỗi CORS nào xuất hiện trong console của trình duyệt.
    5.  **When** Gửi một yêu cầu API từ một origin *không được phép* (ví dụ: `http://attacker.com`) bằng cURL hoặc Postman với header `Origin: http://attacker.com`.
    6.  **Then** API trả về lỗi CORS (thường là không phản hồi hoặc phản hồi bị trình duyệt chặn).

---

**QUAN-20260601-1634-TC08: Xác minh frontend có thể kết nối và hiển thị trạng thái HealthCheck**

*   **Mục tiêu:** Đảm bảo ứng dụng frontend (web) có thể gọi endpoint HealthCheck của backend và hiển thị trạng thái một cách chính xác.
*   **Tham chiếu:** BRD PR#4 (Khả năng sử dụng), SRS PR#5 (Tích hợp frontend), AC9, FR-FE
*   **Mức độ ưu tiên:** Cao
*   **Loại kiểm thử:** Integration

*   **Test Steps:**
    1.  **Given** Backend API đang chạy và frontend `http://localhost:5173` đang hoạt động.
    2.  **When** Truy cập ứng dụng frontend qua trình duyệt.
    3.  **Then** Text "Backend Health: API is healthy." được hiển thị trên giao diện người dùng.
    4.  **And** Không có lỗi nào trong console của trình duyệt liên quan đến việc gọi API HealthCheck.

---

**QUAN-20260601-1634-TC09: Xác minh frontend có thể gọi endpoint lỗi và xử lý thông báo**

*   **Mục tiêu:** Đảm bảo frontend có thể gửi yêu cầu đến endpoint gây lỗi và hiển thị thông báo lỗi từ backend.
*   **Tham chiếu:** BRD PR#4 (Xử lý lỗi), SRS PR#5 (Tích hợp frontend), AC9, FR-FE
*   **Mức độ ưu tiên:** Cao
*   **Loại kiểm thử:** Integration

*   **Test Steps:**
    1.  **Given** Backend API đang chạy và frontend `http://localhost:5173` đang hoạt động.
    2.  **When** Truy cập ứng dụng frontend qua trình duyệt.
    3.  **And** Nhấn vào nút "Trigger Test Error on API".
    4.  **Then** Text "Last API Error: API Error: Đã xảy ra lỗi hệ thống không mong muốn." (hoặc tương tự) được hiển thị trên giao diện người dùng.
    5.  **And** Có một lỗi được ghi nhận trong console của trình duyệt (ví dụ: `API Error: 500 {message: "..."}`).

---

#### 4.2. API Test Cases (QUAN-20260601-1634-APIXX)

| TC ID                           | Mục tiêu kiểm thử                                      | Tham chiếu | Mức độ ưu tiên | Loại kiểm thử |
| :------------------------------ | :----------------------------------------------------- | :--------- | :------------ | :------------ |
| QUAN-20260601-1634-API01        | Xác minh `GlobalExceptionMiddleware` xử lý `ValidationException`. | AC2, AC3, SRS-ARCH | Cao           | Negative      |
| QUAN-20260601-1634-API02        | Xác minh `GlobalExceptionMiddleware` xử lý `NotFoundException`. | AC2, AC3, SRS-ARCH | Cao           | Negative      |
| QUAN-20260601-1634-API03        | Xác minh `GlobalExceptionMiddleware` xử lý `ForbiddenException`. | AC2, AC3, SRS-ARCH | Cao           | Negative      |
| QUAN-20260601-1634-API04        | Xác minh `GlobalExceptionMiddleware` xử lý `UnauthorizedAccessException`. | AC2, AC3, SRS-ARCH | Cao           | Negative      |
| QUAN-20260601-1634-API05        | Kiểm tra API trả về dữ liệu với cấu trúc `ApiResponse` chuẩn. | AC2, SRS-ARCH | Cao           | Happy Path    |

---

**QUAN-20260601-1634-API01: Xử lý `ValidationException` bởi Global Exception Middleware**

*   **Mục tiêu:** Đảm bảo `GlobalExceptionMiddleware` bắt và định dạng `ValidationException` thành `ApiResponse` với status `400 Bad Request`.
*   **Tham chiếu:** BRD PR#4 (Tính ổn định), SRS PR#5 (Xử lý lỗi), AC2, AC3
*   **Mức độ ưu tiên:** Cao
*   **Loại kiểm thử:** Negative

*   **Test Steps:**
    1.  **Given** API backend đang chạy.
    2.  **When** Gửi yêu cầu tới một API endpoint (ví dụ: tạo sách) với dữ liệu không hợp lệ (ví dụ: `Title` rỗng, hoặc `PageNumber` < 1 trong GetReadersListQuery).
        *   Payload ví dụ (tạo sách, nếu có endpoint): `{"Title": "", "Author": "Test Author"}`
        *   Query ví dụ (GetReadersList): `/api/v1/Readers?PageNumber=0&PageSize=10`
    3.  **Then** API trả về status code `400 Bad Request`.
    4.  **And** Nội dung phản hồi là JSON có cấu trúc `ApiResponse` chuẩn.
    5.  **And** `ApiResponse.Success` là `false`.
    6.  **And** `ApiResponse.Message` là "Dữ liệu đầu vào không hợp lệ."
    7.  **And** `ApiResponse.Errors` chứa danh sách các `ApiError` với `Field` và `Message` tương ứng với lỗi validation.

---

**QUAN-20260601-1634-API02: Xử lý `NotFoundException` bởi Global Exception Middleware**

*   **Mục tiêu:** Đảm bảo `GlobalExceptionMiddleware` bắt và định dạng `NotFoundException` thành `ApiResponse` với status `404 Not Found`.
*   **Tham chiếu:** BRD PR#4 (Tính ổn định), SRS PR#5 (Xử lý lỗi), AC2, AC3
*   **Mức độ ưu tiên:** Cao
*   **Loại kiểm thử:** Negative

*   **Test Steps:**
    1.  **Given** API backend đang chạy.
    2.  **When** Gửi yêu cầu GET, PUT, hoặc DELETE tới một tài nguyên không tồn tại (ví dụ: sách với ID không có trong DB): `/api/v1/Books/{non-existent-book-id}`.
    3.  **Then** API trả về status code `404 Not Found`.
    4.  **And** Nội dung phản hồi là JSON có cấu trúc `ApiResponse` chuẩn.
    5.  **And** `ApiResponse.Success` là `false`.
    6.  **And** `ApiResponse.Message` là "Entity 'Book' (ID: non-existent-book-id) was not found." (hoặc tương tự theo format của NotFoundException).

---

**QUAN-20260601-1634-API03: Xử lý `ForbiddenException` bởi Global Exception Middleware**

*   **Mục tiêu:** Đảm bảo `GlobalExceptionMiddleware` bắt và định dạng `ForbiddenException` thành `ApiResponse` với status `403 Forbidden`.
*   **Tham chiếu:** BRD PR#4 (Kiểm soát truy cập), SRS PR#5 (Bảo mật), AC2, AC3
*   **Test Data Pre-conditions:** `LibrarianUser` (không có quyền xóa sách hoặc truy cập báo cáo).
*   **Mức độ ưu tiên:** Cao
*   **Loại kiểm thử:** Negative

*   **Test Steps:**
    1.  **Given** API backend đang chạy và người dùng `LibrarianUser` đã xác thực (có token).
    2.  **When** `LibrarianUser` gửi yêu cầu DELETE đến `/api/v1/Books/{book-001}` (một hành động mà `LibrarianUser` không được phép thực hiện, giả sử xóa sách là quyền `Admin` hoặc `Manager` cao hơn, hoặc một endpoint báo cáo bị hạn chế truy cập).
    3.  **Then** API trả về status code `403 Forbidden`.
    4.  **And** Nội dung phản hồi là JSON có cấu trúc `ApiResponse` chuẩn.
    5.  **And** `ApiResponse.Success` là `false`.
    6.  **And** `ApiResponse.Message` là "You are not authorized to perform this action." (hoặc thông báo tùy chỉnh nếu `ForbiddenException` được khởi tạo với message cụ thể).

---

**QUAN-20260601-1634-API04: Xử lý `UnauthorizedAccessException` bởi Global Exception Middleware**

*   **Mục tiêu:** Đảm bảo `GlobalExceptionMiddleware` bắt và định dạng `UnauthorizedAccessException` thành `ApiResponse` với status `401 Unauthorized`.
*   **Tham chiếu:** BRD PR#4 (Xác thực), SRS PR#5 (Bảo mật), AC2, AC3
*   **Mức độ ưu tiên:** Cao
*   **Loại kiểm thử:** Negative

*   **Test Steps:**
    1.  **Given** API backend đang chạy.
    2.  **When** Gửi yêu cầu đến một endpoint yêu cầu xác thực (ví dụ: bất kỳ endpoint ngoài HealthCheck) mà không có token JWT hoặc với token không hợp lệ/hết hạn.
    3.  **Then** API trả về status code `401 Unauthorized`.
    4.  **And** Nội dung phản hồi là JSON có cấu trúc `ApiResponse` chuẩn.
    5.  **And** `ApiResponse.Success` là `false`.
    6.  **And** `ApiResponse.Message` là thông báo lỗi xác thực (ví dụ: "Unauthorized" hoặc thông báo từ `UnauthorizedAccessException`).

---

**QUAN-20260601-1634-API05: Kiểm tra API trả về dữ liệu với cấu trúc `ApiResponse` chuẩn**

*   **Mục tiêu:** Đảm bảo tất cả các endpoint API trả về phản hồi theo định dạng `ApiResponse` mới, bao gồm các trường `Success`, `Data`, `Message`, `Errors`.
*   **Tham chiếu:** BRD PR#4 (Tính nhất quán), SRS PR#5 (Thiết kế API), AC2
*   **Mức độ ưu tiên:** Cao
*   **Loại kiểm thử:** Happy Path

*   **Test Steps:**
    1.  **Given** API backend đang chạy và người dùng `AdminUser` đã xác thực.
    2.  **When** Gửi yêu cầu GET thành công đến `/api/v1/Readers`.
    3.  **Then** API trả về status code `200 OK`.
    4.  **And** Nội dung phản hồi là JSON có cấu trúc `{"Success": true, "Data": {...}, "Message": null, "Errors": null}`.
    5.  **When** Gửi yêu cầu GET thành công đến `/api/v1/Loans`.
    6.  **Then** API trả về status code `200 OK`.
    7.  **And** Nội dung phản hồi là JSON có cấu trúc `{"Success": true, "Data": {...}, "Message": null, "Errors": null}`.
    8.  **When** Gửi yêu cầu DELETE thành công đến `/api/v1/Books/{book-001}` (và sách đã được soft delete).
    9.  **Then** API trả về status code `200 OK` hoặc `204 No Content`.
    10. **And** Nếu có body, nó tuân thủ cấu trúc `ApiResponse`.

---

#### 4.3. Security Test Cases (QUAN-20260601-1634-SECXX)

| TC ID                           | Mục tiêu kiểm thử                                      | Tham chiếu | Mức độ ưu tiên | Loại kiểm thử |
| :------------------------------ | :----------------------------------------------------- | :--------- | :------------ | :------------ |
| QUAN-20260601-1634-SEC01        | Xác minh chỉ người dùng được phép mới có thể xem báo cáo (nếu có endpoint báo cáo). | AC3, BRD-AC | Cao           | Authorization |
| QUAN-20260601-1634-SEC02        | Xác minh API từ chối truy cập nếu không có JWT hoặc JWT không hợp lệ. | AC3, SRS-SEC | Cao           | Authentication |

---

**QUAN-20260601-1634-SEC01: Xác minh quyền truy cập báo cáo (Giả định)**

*   **Mục tiêu:** Đảm bảo rằng chỉ những người dùng có vai trò được phép (ví dụ: `Admin`, `Manager`) mới có thể truy cập các endpoint liên quan đến báo cáo thống kê. (Giả định sẽ có các endpoint báo cáo trong tương lai, hiện tại dùng DELETE Book làm ví dụ cho `ForbiddenException`).
*   **Tham chiếu:** BRD PR#4 (Quyền truy cập), SRS PR#5 (Bảo mật), AC3
*   **Test Data Pre-conditions:** `AdminUser`, `LibrarianUser`
*   **Mức độ ưu tiên:** Cao
*   **Loại kiểm thử:** Authorization

*   **Test Steps:**
    1.  **Given** Người dùng `AdminUser` đã đăng nhập và có token JWT hợp lệ.
    2.  **When** `AdminUser` gửi yêu cầu đến một endpoint yêu cầu quyền cao (ví dụ: DELETE `/api/v1/Books/{book-001}`).
    3.  **Then** API xử lý thành công (nếu Admin có quyền xóa sách).
    4.  **Given** Người dùng `LibrarianUser` đã đăng nhập và có token JWT hợp lệ.
    5.  **When** `LibrarianUser` gửi yêu cầu đến cùng endpoint DELETE `/api/v1/Books/{book-001}`.
    6.  **Then** API trả về status code `403 Forbidden`.
    7.  **And** Nội dung phản hồi có `Success: false` và `Message: "You are not authorized to perform this action."`.

---

**QUAN-20260601-1634-SEC02: Xác minh API từ chối truy cập nếu không có JWT hoặc JWT không hợp lệ**

*   **Mục tiêu:** Đảm bảo các endpoint được bảo vệ yêu cầu JWT hợp lệ và từ chối các yêu cầu không có JWT hoặc JWT không hợp lệ.
*   **Tham chiếu:** BRD PR#4 (Xác thực), SRS PR#5 (Bảo mật API), AC3
*   **Mức độ ưu tiên:** Cao
*   **Loại kiểm thử:** Authentication

*   **Test Steps:**
    1.  **Given** Một endpoint API yêu cầu xác thực (ví dụ: `GET /api/v1/Readers`).
    2.  **When** Gửi yêu cầu GET đến endpoint đó mà không đính kèm header `Authorization`.
    3.  **Then** API trả về status code `401 Unauthorized`.
    4.  **And** Nội dung phản hồi là `ApiResponse` với `Success: false` và `Message` lỗi xác thực.
    5.  **When** Gửi yêu cầu GET đến endpoint đó với header `Authorization: Bearer invalid_jwt_token`.
    6.  **Then** API trả về status code `401 Unauthorized`.
    7.  **And** Nội dung phản hồi là `ApiResponse` với `Success: false` và `Message` lỗi xác thực.

---

#### 4.4. Boundary/Validation Test Cases (QUAN-20260601-1634-BVXX)

| TC ID                           | Mục tiêu kiểm thử                                      | Tham chiếu | Mức độ ưu tiên | Loại kiểm thử |
| :------------------------------ | :----------------------------------------------------- | :--------- | :------------ | :------------ |
| QUAN-20260601-1634-BV01         | Kiểm tra phân trang với `PageNumber` nhỏ nhất (1).     | AC4, BRD-P | Trung         | Boundary      |
| QUAN-20260601-1634-BV02         | Kiểm tra phân trang với `PageSize` nhỏ nhất (1).      | AC4, BRD-P | Trung         | Boundary      |
| QUAN-20260601-1634-BV03         | Kiểm tra phân trang với `PageNumber` lớn hơn tổng số trang. | AC4, BRD-P | Trung         | Boundary      |

---

**QUAN-20260601-1634-BV01: Kiểm tra phân trang với `PageNumber` nhỏ nhất (1)**

*   **Mục tiêu:** Đảm bảo API phân trang hoạt động chính xác khi `PageNumber` là 1.
*   **Tham chiếu:** BRD PR#4 (Phân trang), SRS PR#5 (Truy vấn dữ liệu), AC4
*   **Mức độ ưu tiên:** Trung bình
*   **Loại kiểm thử:** Boundary

*   **Test Steps:**
    1.  **Given** Có đủ dữ liệu để tạo nhiều trang (ví dụ: nhiều hơn 10 cuốn sách hoặc độc giả).
    2.  **When** Gửi yêu cầu GET đến `/api/v1/Readers?PageNumber=1&PageSize=5`.
    3.  **Then** API trả về status code `200 OK`.
    4.  **And** `ApiResponse.Data.Value.PageNumber` là `1`.
    5.  **And** `ApiResponse.Data.Value.Items` chứa 5 phần tử đầu tiên của danh sách độc giả.
    6.  **And** `ApiResponse.Data.Value.HasPreviousPage` là `false`.

---

**QUAN-20260601-1634-BV02: Kiểm tra phân trang với `PageSize` nhỏ nhất (1)**

*   **Mục tiêu:** Đảm bảo API phân trang hoạt động chính xác khi `PageSize` là 1.
*   **Tham chiếu:** BRD PR#4 (Phân trang), SRS PR#5 (Truy vấn dữ liệu), AC4
*   **Mức độ ưu tiên:** Trung bình
*   **Loại kiểm thử:** Boundary

*   **Test Steps:**
    1.  **Given** Có đủ dữ liệu để tạo nhiều trang.
    2.  **When** Gửi yêu cầu GET đến `/api/v1/Readers?PageNumber=2&PageSize=1`.
    3.  **Then** API trả về status code `200 OK`.
    4.  **And** `ApiResponse.Data.Value.PageNumber` là `2`.
    5.  **And** `ApiResponse.Data.Value.Items` chứa đúng 1 phần tử (là phần tử thứ 2 trong danh sách tổng).
    6.  **And** `ApiResponse.Data.Value.HasPreviousPage` là `true` (nếu có trang trước đó).

---

**QUAN-20260601-1634-BV03: Kiểm tra phân trang với `PageNumber` lớn hơn tổng số trang**

*   **Mục tiêu:** Đảm bảo API phân trang xử lý đúng khi yêu cầu một trang không tồn tại.
*   **Tham chiếu:** BRD PR#4 (Phân trang), SRS PR#5 (Truy vấn dữ liệu), AC4
*   **Mức độ ưu tiên:** Trung bình
*   **Loại kiểm thử:** Boundary

*   **Test Steps:**
    1.  **Given** Có tổng cộng 2 trang dữ liệu độc giả (với `PageSize` = 5).
    2.  **When** Gửi yêu cầu GET đến `/api/v1/Readers?PageNumber=100&PageSize=5` (một `PageNumber` lớn hơn nhiều so với tổng số trang).
    3.  **Then** API trả về status code `200 OK`.
    4.  **And** `ApiResponse.Data.Value.PageNumber` là `100`.
    5.  **And** `ApiResponse.Data.Value.Items` là một mảng rỗng `[]`.
    6.  **And** `ApiResponse.Data.Value.HasNextPage` là `false`.

---

### 5. Ma trận truy vết (Traceability Matrix)

Ma trận này ánh xạ các Test Case tới các Yêu cầu Nghiệp vụ (BRD), Yêu cầu Hệ thống (SRS), và các Tiêu chí Chấp nhận (AC) đã được xác định.

| TC ID                         | Loại TC             | Mục tiêu kiểm thử                                             | Covered BRD (PR#4) | Covered SRS (PR#5) | Covered AC/BR | DEV PR Diff Coverage (PR#12)                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                      |
| :---------------------------- | :------------------ | :------------------------------------------------------------ | :----------------- | :----------------- | :------------ | :------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| QUAN-20260601-1634-TC01       | Functional          | Đảm bảo endpoint HealthCheck trả về trạng thái khỏe mạnh.      | Tổng quan hệ thống | API Stability      | AC1           | `HealthCheckController.cs` (NEW), `ApiResponse.cs` (NEW), `GlobalExceptionMiddleware.cs` (NEW), `web/App.tsx` (calls HealthCheck)                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                    |
| QUAN-20260601-1634-TC02       | Functional          | Đảm bảo endpoint HealthCheck/error trả về lỗi hệ thống qua GlobalExceptionMiddleware. | Ổn định hệ thống   | Xử lý lỗi          | AC1, AC2, AC3 | `HealthCheckController.cs` (NEW), `GlobalExceptionMiddleware.cs` (NEW), `ApiResponse.cs` (NEW), `web/App.tsx` (calls HealthCheck/error)                                                                                                                                                                                                                                                                                                                                                                                                                                                                                         |
| QUAN-20260601-1634-TC03       | Functional          | Xác minh chức năng xóa mềm (soft delete) cho sách.           | Tính toàn vẹn dữ liệu | Quản lý dữ liệu    | AC5, BR-SD    | `AppDbContext.cs` (Added `EntityState.Deleted` handler), `BaseEntity.cs` (Added `IsDeleted`)                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                   |
| QUAN-20260601-1634-TC04       | Functional (Edge)   | Xác minh cơ chế soft delete khi sách đang được mượn.         | Quy trình nghiệp vụ | Xử lý sách         | AC5, BR-SD    | `AppDbContext.cs` (Added `EntityState.Deleted` handler), `DeleteBookCommand.cs` (Removed comment, `isBorrowed` check remains)                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                   |
| QUAN-20260601-1634-TC05       | Functional          | Xác minh API GetReadersList trả về dữ liệu đúng định dạng `Result`. | Cấu trúc dữ liệu   | Truy vấn dữ liệu   | AC4           | `GetReadersListQuery.cs` (Changed return type), `GetReadersListQueryHandler.cs` (Changed return type and `Result` usage)                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                           |
| QUAN-20260601-1634-TC06       | Functional          | Xác minh API GetLoans trả về dữ liệu đúng định dạng `Result`. | Cấu trúc dữ liệu   | Truy vấn dữ liệu   | AC4           | `GetLoansQuery.cs` (Changed return type), `GetLoansQueryHandler.cs` (Changed return type and `Result` usage)                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                           |
| QUAN-20260601-1634-TC07       | Functional (Config) | Xác minh CORS được áp dụng đúng cho frontend.                | Khả năng tương thích | Bảo mật API        | AC7           | `Program.cs` (Added CORS configuration `AllowSpecificOrigin`)                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                       |
| QUAN-20260601-1634-TC08       | Functional (Integr.)| Xác minh frontend có thể kết nối và hiển thị trạng thái HealthCheck. | Khả năng sử dụng   | Tích hợp frontend   | AC9, FR-FE    | `web/App.tsx` (useEffect for health check), `web/apiClient.ts` (axios setup), `web/vite.config.ts` (API_BASE_URL)                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                      |
| QUAN-20260601-1634-TC09       | Functional (Integr.)| Xác minh frontend có thể gọi endpoint lỗi và xử lý thông báo. | Xử lý lỗi          | Tích hợp frontend   | AC9, FR-FE    | `web/App.tsx` (triggerError function), `web/apiClient.ts` (response interceptor for error handling)                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                    |
| QUAN-20260601-1634-API01       | API (Negative)      | Xác minh `GlobalExceptionMiddleware` xử lý `ValidationException`. | Tính ổn định       | Xử lý lỗi          | AC2, AC3      | `GlobalExceptionMiddleware.cs` (Added handler for `ONENET.Application.Common.Exceptions.ValidationException`), `ValidationException.cs` (Modified)                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                       |
| QUAN-20260601-1634-API02       | API (Negative)      | Xác minh `GlobalExceptionMiddleware` xử lý `NotFoundException`. | Tính ổn định       | Xử lý lỗi          | AC2, AC3      | `GlobalExceptionMiddleware.cs` (Handler for `NotFoundException`), `NotFoundException.cs` (Added comment)                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             |
| QUAN-20260601-1634-API03       | API (Negative)      | Xác minh `GlobalExceptionMiddleware` xử lý `ForbiddenException`. | Kiểm soát truy cập | Bảo mật            | AC2, AC3      | `GlobalExceptionMiddleware.cs` (Added handler for `ForbiddenException`), `ForbiddenException.cs` (NEW)                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                 |
| QUAN-20260601-1634-API04       | API (Negative)      | Xác minh `GlobalExceptionMiddleware` xử lý `UnauthorizedAccessException`. | Xác thực          | Bảo mật            | AC2, AC3      | `GlobalExceptionMiddleware.cs` (Added handler for `UnauthorizedAccessException`)                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                      |
| QUAN-20260601-1634-API05       | API (Happy)         | Kiểm tra API trả về dữ liệu với cấu trúc `ApiResponse` chuẩn. | Tính nhất quán     | Thiết kế API       | AC2           | `ApiResponse.cs` (NEW), `GlobalExceptionMiddleware.cs` (Uses ApiResponse), `GetReadersListQueryHandler.cs`, `GetLoansQueryHandler.cs` (Implicitly use ApiResponse through controllers)                                                                                                                                                                                                                                                                                                                                                                                                                                                                                  |
| QUAN-20260601-1634-SEC01       | Security            | Xác minh chỉ người dùng được phép mới có thể xem báo cáo (nếu có endpoint báo cáo). | Quyền truy cập     | Bảo mật            | AC3           | `ForbiddenException.cs` (NEW), `GlobalExceptionMiddleware.cs` (Handler for `ForbiddenException`) - kiểm tra dựa trên cơ chế ném exception này.                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                    |
| QUAN-20260601-1634-SEC02       | Security            | Xác minh API từ chối truy cập nếu không có JWT hoặc JWT không hợp lệ. | Xác thực          | Bảo mật API        | AC3           | `GlobalExceptionMiddleware.cs` (Handler for `UnauthorizedAccessException`), `Program.cs` (JWT Auth configuration)                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        |
| QUAN-20260601-1634-BV01       | Boundary            | Kiểm tra phân trang với `PageNumber` nhỏ nhất (1).           | Phân trang         | Truy vấn dữ liệu   | AC4           | `GetReadersListQueryHandler.cs`, `GetLoansQueryHandler.cs` (Phân trang logic), `ONENET.Domain/Common/Result.cs` (new Result class for consistency)                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                    |
| QUAN-20260601-1634-BV02       | Boundary            | Kiểm tra phân trang với `PageSize` nhỏ nhất (1).            | Phân trang         | Truy vấn dữ liệu   | AC4           | `GetReadersListQueryHandler.cs`, `GetLoansQueryHandler.cs` (Phân trang logic), `ONENET.Domain/Common/Result.cs`                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                       |
| QUAN-20260601-1634-BV03       | Boundary            | Kiểm tra phân trang với `PageNumber` lớn hơn tổng số trang.  | Phân trang         | Truy vấn dữ liệu   | AC4           | `GetReadersListQueryHandler.cs`, `GetLoansQueryHandler.cs` (Phân trang logic, trả về list rỗng), `ONENET.Domain/Common/Result.cs`                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                    |

**Giải thích các mã tham chiếu:**
*   **BRD PR#4 / SRS PR#5**: Tài liệu yêu cầu nghiệp vụ / đặc tả hệ thống (giả định tồn tại).
*   **AC1-AC9**: Acceptance Criteria được suy luận từ tên tính năng và phân tích DEV PR Diff.
*   **BR-SD**: Business Rule - Soft Delete (được suy luận từ `AppDbContext.cs`).
*   **FR-FE**: Functional Requirement - Front End (yêu cầu chức năng liên quan đến frontend).
*   **SRS-ARCH**: SRS - Kiến trúc (yêu cầu liên quan đến cấu trúc hệ thống, xử lý lỗi, v.v.).
*   **SRS-SEC**: SRS - Bảo mật (yêu cầu liên quan đến bảo mật).

---