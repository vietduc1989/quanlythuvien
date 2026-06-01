# Business Requirements Document (BRD)

| Metadata | Giá trị |
|----------|---------|
| **Mã tính năng** | `QUAN-20260601-140229` |
| **Tên tính năng** | Quản lý mượn trả sách |
| **Dự án** | quanlythuvien |
| **Phiên bản** | 1.0 |
| **Ngày tạo** | 2026-06-01 |
| **Tác giả** | BA Agent (ONENET AgentFactory) |

## Tài liệu liên quan

| Mã tính năng | Loại tài liệu | Trạng thái |
|-------------|---------------|------------|
| `QUAN-20260601-140229` | **BRD** (tài liệu này) | ✅ Hiện tại |
| `QUAN-20260601-140229` | SRS/SAD | ⏳ Chờ BA duyệt |
| `QUAN-20260601-140229` | DEV (mã nguồn) | ⏳ Chờ SRS duyệt |
| `QUAN-20260601-140229` | TEST (test cases) | ⏳ Chờ DEV duyệt |

---

## 1. Mục tiêu

### 1.1 Mục tiêu chính
Cung cấp một tính năng cho phép Thủ thư thực hiện các nghiệp vụ mượn và trả sách cho độc giả một cách chính xác, tuân thủ các quy tắc của thư viện, và hiển thị trạng thái mượn trả rõ ràng.

### 1.2 Mục tiêu phụ
- Đảm bảo việc kiểm tra điều kiện mượn sách (tồn kho, hiệu lực thẻ, giới hạn số lượng) được thực hiện tự động và chính xác.
- Tự động hóa việc tính toán ngày hẹn trả để giảm thiểu sai sót thủ công.
- Cung cấp khả năng theo dõi tình trạng mượn trả sách, đặc biệt là các trường hợp quá hạn.
- Tăng cường hiệu quả quản lý thư viện và nâng cao trải nghiệm cho cả thủ thư và độc giả.

---

## 2. Phạm vi, Giả định và Phụ thuộc

### 2.1 In Scope (Trong phạm vi)
- Tạo mới phiếu mượn sách cho độc giả.
- Kiểm tra điều kiện mượn sách: tồn kho, hiệu lực thẻ độc giả, số lượng sách mượn tối đa.
- Tự động thiết lập ngày hẹn trả sách.
- Ghi nhận việc trả sách và cập nhật tồn kho sách.
- Hiển thị trạng thái "Overdue" (quá hạn) cho các phiếu mượn đã vượt quá ngày hẹn trả.

### 2.2 Out of Scope (Ngoài phạm vi)
- Quản lý thông tin chi tiết về sách (thêm, sửa, xóa sách, phân loại).
- Quản lý thông tin chi tiết về độc giả (thêm, sửa, xóa độc giả, gia hạn thẻ).
- Tính toán và thu phí phạt khi trả sách muộn, làm hỏng hoặc mất sách.
- Chức năng đặt trước sách (reservation).
- Báo cáo thống kê tổng hợp về tình hình mượn trả sách.

### 2.3 Giả định (Assumptions)
- Hệ thống đã có sẵn module Quản lý sách để cung cấp thông tin về tồn kho sách và chi tiết sách.
- Hệ thống đã có sẵn module Quản lý độc giả để cung cấp thông tin về hiệu lực thẻ và chi tiết độc giả.
- Thủ thư có đủ quyền hạn để thực hiện các thao tác mượn và trả sách.
- Ngày hiện tại của hệ thống là chính xác và được sử dụng làm cơ sở để tính toán ngày hẹn trả và xác định trạng thái quá hạn.

### 2.4 Phụ thuộc (Dependencies)
- Module Quản lý sách: Cần API/dịch vụ để kiểm tra tồn kho và cập nhật số lượng tồn kho.
- Module Quản lý độc giả: Cần API/dịch vụ để kiểm tra hiệu lực thẻ độc giả và lấy thông tin độc giả.

---

## 3. Nhóm người dùng (User Personas)

| # | Persona | Vai trò | Quyền hạn chính | Mục tiêu sử dụng |
|---|---------|-------|-----------------|------------------|
| 1 | Thủ thư | Quản lý thư viện | Tạo/Ghi nhận phiếu mượn, Ghi nhận trả sách, Xem danh sách mượn trả | Thực hiện các nghiệp vụ mượn trả sách hàng ngày một cách hiệu quả, tuân thủ quy định. |

---

## 4. User Flow nghiệp vụ (Mermaid Diagram)

> Sơ đồ luồng nghiệp vụ chi tiết. Mọi luồng chính (Happy Path) và luồng rẽ nhánh (Edge Cases) cần được thể hiện.

```mermaid
flowchart TD
    subgraph Mượn Sách
        StartBorrow([Thủ thư bắt đầu mượn sách]) --> A[Chọn Độc giả & Chọn Sách cần mượn]
        A --> B{Sách có còn tồn kho không?}
        B -- Không --> ErrorStock[Thông báo lỗi: Sách không đủ tồn kho]
        B -- Có --> C{Thẻ độc giả còn hiệu lực không?}
        C -- Không --> ErrorCard[Thông báo lỗi: Thẻ độc giả hết hiệu lực]
        C -- Có --> D{Tổng số sách mượn <= 5 cuốn?}
        D -- Không --> ErrorLimit["Thông báo lỗi: Đã mượn quá số lượng cho phép (tối đa 5 cuốn)"]
        D -- Có --> E[Tạo phiếu mượn mới]
        E --> F[Đặt Ngày hẹn trả = Ngày mượn + 14 ngày]
        F --> G[Cập nhật tồn kho sách: Giảm 1 cho mỗi cuốn]
        G --> SuccessBorrow([Mượn sách thành công])
        ErrorStock --> EndBorrow([Kết thúc])
        ErrorCard --> EndBorrow
        ErrorLimit --> EndBorrow
        SuccessBorrow --> EndBorrow
    end

    subgraph Trả Sách
        StartReturn([Thủ thư bắt đầu trả sách]) --> H[Tìm kiếm phiếu mượn/Sách cần trả]
        H --> I[Xác nhận sách đã được trả]
        I --> J[Cập nhật tồn kho sách: Tăng 1 cho mỗi cuốn]
        J --> K[Đánh dấu phiếu mượn là "Đã trả"]
        K --> L{Kiểm tra: Ngày trả thực tế > Ngày hẹn trả?}
        L -- Có --> M["Hiển thị trạng thái 'Overdue'"]
        L -- Không --> N["Không hiển thị 'Overdue'"]
        M --> SuccessReturn([Trả sách thành công])
        N --> SuccessReturn
    end
```

---

## 5. Functional Requirements (FR)

> Chuyển đổi yêu cầu thành định dạng User Story: `As a [persona], I want to [action] so that [benefit]`

| Mã FR | Tên FR | User Story | Độ ưu tiên |
|--------|-----|------------|-----------|
| `QUAN-20260601-140229-FR01` | Mượn sách cho độc giả | As a Thủ thư, I want to mượn sách cho độc giả so that độc giả có thể đọc sách và hệ thống quản lý được tình trạng sách. | Must |
| `QUAN-20260601-140229-FR02` | Ghi nhận trả sách | As a Thủ thư, I want to ghi nhận sách đã trả từ độc giả so that tồn kho sách được cập nhật và hệ thống biết sách nào đã được trả. | Must |
| `QUAN-20260601-140229-FR03` | Hiển thị trạng thái quá hạn | As a Thủ thư, I want the system to hiển thị trạng thái "Overdue" khi phiếu mượn quá hạn so that tôi có thể dễ dàng nhận biết và xử lý các trường hợp trả sách muộn. | Must |

---

## 6. Non-functional Requirements (NFR)

### 6.1 Performance
- Thời gian phản hồi của các thao tác mượn sách và trả sách phải dưới 2 giây đối với các giao dịch đơn lẻ.
- Hệ thống phải có khả năng xử lý đồng thời 50 yêu cầu mượn/trả sách mà không làm giảm đáng kể hiệu suất.

### 6.2 Security
- Chỉ người dùng có vai trò Thủ thư và được cấp quyền mới có thể thực hiện các thao tác mượn và trả sách.
- Dữ liệu liên quan đến phiếu mượn, thông tin độc giả và thông tin sách phải được bảo mật, chống truy cập trái phép.

### 6.3 Availability
- Hệ thống quản lý mượn trả sách phải có thời gian hoạt động (uptime) tối thiểu 99.5% trong giờ làm việc (8:00 - 17:00, thứ 2 đến thứ 7).

---

## 7. Business Rules (BR)

| Mã BR | Quy tắc | Mô tả & Ví dụ |
|--------|---------|---------------|
| `QUAN-20260601-140229-BR01` | Kiểm tra tồn kho sách khi mượn | **Mô tả:** Hệ thống chỉ cho phép mượn sách khi sách còn tồn kho (số lượng tồn kho > 0). <br> **VD:** <br> *Đầu vào:* Sách "Lập trình C#" có tồn kho = 0. Độc giả yêu cầu mượn. <br> *Kết quả:* Hệ thống báo lỗi "Sách không đủ tồn kho", không cho phép mượn. <br> *Đầu vào:* Sách "Giải tích 1" có tồn kho = 2. Độc giả yêu cầu mượn. <br> *Kết quả:* Hệ thống cho phép mượn. |
| `QUAN-20260601-140229-BR02` | Kiểm tra hiệu lực thẻ độc giả khi mượn | **Mô tả:** Hệ thống chỉ cho phép mượn sách khi thẻ độc giả được chọn còn hiệu lực. <br> **VD:** <br> *Đầu vào:* Độc giả "Nguyễn A", thẻ hết hạn ngày 2026-05-30. Ngày mượn: 2026-06-01. <br> *Kết quả:* Hệ thống báo lỗi "Thẻ độc giả đã hết hiệu lực", không cho phép mượn. <br> *Đầu vào:* Độc giả "Trần B", thẻ còn hạn đến 2027-12-31. Ngày mượn: 2026-06-01. <br> *Kết quả:* Hệ thống cho phép tiếp tục. |
| `QUAN-20260601-140229-BR03` | Giới hạn số lượng sách mượn | **Mô tả:** Mỗi độc giả chỉ được mượn tối đa 5 cuốn sách tại một thời điểm. <br> **VD:** <br> *Đầu vào:* Độc giả đang mượn 4 cuốn. Yêu cầu mượn thêm 2 cuốn. <br> *Kết quả:* Hệ thống báo lỗi "Độc giả đã mượn tối đa 5 cuốn, không thể mượn thêm". <br> *Đầu vào:* Độc giả đang mượn 3 cuốn. Yêu cầu mượn thêm 1 cuốn. <br> *Kết quả:* Hệ thống cho phép mượn, tổng số sách đang mượn là 4. |
| `QUAN-20260601-140229-BR04` | Quy tắc tính ngày hẹn trả mặc định | **Mô tả:** Ngày hẹn trả sách sẽ được mặc định bằng Ngày mượn cộng thêm 14 ngày. <br> **VD:** <br> *Đầu vào:* Ngày mượn sách là 2026-06-01. <br> *Kết quả:* Ngày hẹn trả mặc định là 2026-06-15. |
| `QUAN-20260601-140229-BR05` | Cập nhật tồn kho khi trả sách | **Mô tả:** Khi một cuốn sách được trả, số lượng tồn kho của cuốn sách đó trong hệ thống sẽ tự động tăng lên 1. <br> **VD:** <br> *Đầu vào:* Sách "Vật lý đại cương" có tồn kho = 0. Độc giả trả sách "Vật lý đại cương". <br> *Kết quả:* Tồn kho sách "Vật lý đại cương" tăng lên 1 (tồn kho = 1). |
| `QUAN-20260601-140229-BR06` | Hiển thị trạng thái "Overdue" | **Mô tả:** Nếu ngày trả thực tế của sách muộn hơn ngày hẹn trả, hệ thống sẽ hiển thị trạng thái "Overdue" cho phiếu mượn tương ứng. <br> **VD:** <br> *Đầu vào:* Phiếu mượn có Ngày hẹn trả là 2026-06-15. Ngày trả thực tế là 2026-06-16. <br> *Kết quả:* Hệ thống hiển thị chữ "Overdue" trên phiếu mượn hoặc trong danh sách sách đang mượn của độc giả. |

---

## 8. Integration Requirements

- **Module Quản lý Sách:** Cần tích hợp với module Quản lý Sách để truy vấn thông tin tồn kho sách và cập nhật tồn kho khi mượn/trả.
- **Module Quản lý Độc giả:** Cần tích hợp với module Quản lý Độc giả để truy vấn trạng thái hiệu lực thẻ độc giả và thông tin độc giả.

---

## 9. Audit Trail

- Hệ thống phải ghi lại nhật ký (log) cho các thao tác quan trọng sau:
    - **Tạo phiếu mượn:** Ghi lại thời gian, người thực hiện (Thủ thư), độc giả, danh sách sách mượn, ngày hẹn trả.
    - **Ghi nhận trả sách:** Ghi lại thời gian, người thực hiện (Thủ thư), độc giả, danh sách sách đã trả, tình trạng quá hạn (nếu có).
    - Các lần chỉnh sửa thông tin phiếu mượn (nếu có trong các phase sau).

---

## 10. KPI & Metrics theo dõi

- **Tỷ lệ mượn/trả thành công:** Tổng số giao dịch mượn/trả thành công trên tổng số yêu cầu mượn/trả.
- **Số lượng sách đang được mượn:** Tổng số sách hiện đang trong trạng thái được mượn.
- **Số lượng phiếu mượn quá hạn:** Tổng số phiếu mượn mà ngày trả thực tế đã vượt quá ngày hẹn trả (hoặc ngày hiện tại đã vượt quá ngày hẹn trả đối với sách chưa được trả).
- **Thời gian trung bình xử lý:** Thời gian trung bình để hoàn thành một giao dịch mượn sách hoặc trả sách.

---

## 11. Acceptance Criteria (Tiêu chí chấp nhận - BDD)

> Viết tiêu chí chấp nhận cho từng FR theo chuẩn BDD (Given / When / Then).

### Luồng chính (Happy Path)

| Mã FR | Scenario | Given (Đầu vào/Điều kiện) | When (Hành động) | Then (Kết quả mong đợi) |
|-------|----------|---------------------------|------------------|-------------------------|
| `QUAN-20260601-140229-FR01` | Mượn sách thành công | Thủ thư đã đăng nhập, có quyền. Sách "A" có tồn kho = 1. Thẻ độc giả "ĐG001" còn hiệu lực. "ĐG001" đang mượn 2 cuốn. Ngày hiện tại là 2026-06-01. | Thủ thư chọn "ĐG001", chọn sách "A" và xác nhận mượn. | <ul><li>Phiếu mượn mới được tạo cho "ĐG001" với sách "A".</li><li>Ngày hẹn trả là 2026-06-15 (2026-06-01 + 14 ngày).</li><li>Tồn kho sách "A" giảm xuống 0.</li><li>Hệ thống hiển thị thông báo "Mượn sách thành công".</li></ul> |
| `QUAN-20260601-140229-FR02` | Trả sách thành công (không quá hạn) | Thủ thư đã đăng nhập, có quyền. Có phiếu mượn sách "B" của "ĐG002" với ngày hẹn trả là 2026-06-15. Ngày trả thực tế là 2026-06-14. Tồn kho sách "B" = 0. | Thủ thư tìm và chọn phiếu mượn sách "B" của "ĐG002" và xác nhận trả. | <ul><li>Phiếu mượn sách "B" được đánh dấu là "Đã trả".</li><li>Tồn kho sách "B" tăng lên 1.</li><li>Hệ thống hiển thị thông báo "Trả sách thành công".</li><li>Hệ thống không hiển thị trạng thái "Overdue" cho phiếu mượn này.</li></ul> |
| `QUAN-20260601-140229-FR03` | Trả sách thành công (có quá hạn) | Thủ thư đã đăng nhập, có quyền. Có phiếu mượn sách "C" của "ĐG003" với ngày hẹn trả là 2026-06-10. Ngày trả thực tế là 2026-06-12. Tồn kho sách "C" = 0. | Thủ thư tìm và chọn phiếu mượn sách "C" của "ĐG003" và xác nhận trả. | <ul><li>Phiếu mượn sách "C" được đánh dấu là "Đã trả".</li><li>Tồn kho sách "C" tăng lên 1.</li><li>Hệ thống hiển thị thông báo "Trả sách thành công" kèm cảnh báo "Phiếu mượn quá hạn".</li><li>Trạng thái "Overdue" được hiển thị cho phiếu mượn này.</li></ul> |

### Luồng lỗi (Unhappy Path / Edge Cases)

| Mã FR | Scenario | Given (Đầu vào/Điều kiện) | When (Hành động) | Then (Kết quả mong đợi) |
|-------|----------|---------------------------|------------------|-------------------------|
| `QUAN-20260601-140229-FR01` | Sách không đủ tồn kho | Thủ thư đã đăng nhập, có quyền. Sách "X" có tồn kho = 0. Thẻ độc giả "ĐG004" còn hiệu lực. "ĐG004" đang mượn < 5 cuốn. | Thủ thư chọn "ĐG004", chọn sách "X" và xác nhận mượn. | <ul><li>Hệ thống hiển thị thông báo lỗi "Sách không đủ tồn kho".</li><li>Không có phiếu mượn nào được tạo.</li><li>Tồn kho của sách "X" không thay đổi.</li></ul> |
| `QUAN-20260601-140229-FR01` | Thẻ độc giả hết hiệu lực | Thủ thư đã đăng nhập, có quyền. Sách "Y" có tồn kho = 1. Thẻ độc giả "ĐG005" đã hết hiệu lực. "ĐG005" đang mượn < 5 cuốn. | Thủ thư chọn "ĐG005", chọn sách "Y" và xác nhận mượn. | <ul><li>Hệ thống hiển thị thông báo lỗi "Thẻ độc giả đã hết hiệu lực".</li><li>Không có phiếu mượn nào được tạo.</li><li>Tồn kho của sách "Y" không thay đổi.</li></ul> |
| `QUAN-20260601-140229-FR01` | Vượt quá giới hạn số lượng mượn | Thủ thư đã đăng nhập, có quyền. Sách "Z" có tồn kho = 1. Thẻ độc giả "ĐG006" còn hiệu lực. "ĐG006" đang mượn 5 cuốn. | Thủ thư chọn "ĐG006", chọn sách "Z" và xác nhận mượn. | <ul><li>Hệ thống hiển thị thông báo lỗi "Độc giả đã mượn tối đa 5 cuốn, không thể mượn thêm".</li><li>Không có phiếu mượn nào được tạo.</li><li>Tồn kho của sách "Z" không thay đổi.</li></ul> |
| `QUAN-20260601-140229-FR03` | Xem trạng thái quá hạn (trước khi trả) | Thủ thư đã đăng nhập, có quyền. Có phiếu mượn sách "D" của "ĐG007" với ngày hẹn trả là 2026-06-05. Ngày hiện tại là 2026-06-07. Sách "D" chưa được trả. | Thủ thư xem danh sách các sách đang được "ĐG007" mượn hoặc xem chi tiết phiếu mượn sách "D". | <ul><li>Hệ thống hiển thị rõ ràng trạng thái "Overdue" cho phiếu mượn sách "D".</li></ul> |

---

_Mã tính năng `QUAN-20260601-140229` — Tài liệu BRD được sinh tự động bởi ONENET AgentFactory._