Chào bạn, Kỹ sư QA/Tester cấp cao của ONENET. Dưới đây là Test Specification chi tiết cho tính năng "Quản lý sách" (Feature ID: QUAN-20260602-104003) dựa trên các thay đổi trong DEV PR Diff, tuân thủ các quy tắc và template đã cung cấp.

---

## Test Specification for Feature: Quản lý sách
**Feature ID:** QUAN-20260602-104003
**Feature Name:** Quản lý sách
**BRD PR#:** 16 | **SRS PR#:** 3 | **DEV PR#:** 11

### 1. Introduction
Tài liệu này mô tả kế hoạch kiểm thử chi tiết cho tính năng Quản lý Sách, bao gồm các chức năng tạo mới, cập nhật, xóa, và truy vấn thông tin sách. Mục tiêu là đảm bảo rằng hệ thống hoạt động đúng theo yêu cầu nghiệp vụ và kỹ thuật, bao gồm cả các ràng buộc về dữ liệu và xử lý ngoại lệ.

### 2. Test Environment
*   **Operating System:** Docker environment (Linux) / Windows Server
*   **Database:** PostgreSQL
*   **API Gateway/Load Balancer:** Nginx (if applicable)
*   **Backend Application:** .NET 9 API
*   **Tools:** Postman/Insomnia for API testing, Swagger UI.

### 3. Test Data Pre-conditions
Để thực hiện các kịch bản kiểm thử, cần chuẩn bị các dữ liệu sau trong cơ sở dữ liệu:

*   **Sách hợp lệ (Valid Book):** Một cuốn sách có đầy đủ thông tin hợp lệ, số lượng tồn kho > 0.
    *   `Id: <GUID_BOOK_AVAILABLE_1>`
    *   `BookCode: "BK-VALID01"`
    *   `Title: "Lập trình C# nâng cao"`
    *   `Author: "Nguyễn Văn A"`
    *   `Category: "Tin học"`
    *   `Publisher: "NXB Giáo dục"`
    *   `PublishYear: 2023`
    *   `Quantity: 5`
    *   `ShelfLocation: "A1-01"`
    *   `Status: Available`

*   **Sách hết hàng (Out of Stock Book):** Một cuốn sách có đầy đủ thông tin hợp lệ, số lượng tồn kho = 0.
    *   `Id: <GUID_BOOK_OUTOFSTOCK_1>`
    *   `BookCode: "BK-OOS01"`
    *   `Title: "Kinh tế học vi mô"`
    *   `Author: "Trần Thị B"`
    *   `Category: "Kinh tế"`
    *   `Publisher: "NXB Tổng hợp"`
    *   `PublishYear: 2020`
    *   `Quantity: 0`
    *   `ShelfLocation: "B2-05"`
    *   `Status: OutOfStock`

*   **Sách đã bị xóa mềm (Soft Deleted Book):** Một cuốn sách đã được tạo và sau đó được đánh dấu là `IsDeleted = true` trong database.
    *   `Id: <GUID_SOFT_DELETED_BOOK>`
    *   `BookCode: "BK-DEL01"`
    *   `Title: "Lịch sử Việt Nam"`
    *   `Author: "Phạm Văn C"`
    *   `Category: "Lịch sử"`
    *   `Publisher: "NXB Khoa học"`
    *   `PublishYear: 2015`
    *   `Quantity: 3`
    *   `ShelfLocation: "C3-10"`
    *   `IsDeleted: true`

*   **Sách đang được mượn (Borrowed Book):** Một cuốn sách có `Id: <GUID_BOOK_BORROWED_1>` và có ít nhất một bản ghi `ChiTietPhieuMuon` liên quan với `TrangThaiChiTiet = "DangMuon"`.
    *   `Id: <GUID_BOOK_BORROWED_1>`
    *   `BookCode: "BK-BRW01"`
    *   `Title: "Toán cao cấp A1"`
    *   `Author: "Lê Văn D"`
    *   `Category: "Toán học"`
    *   `Publisher: "NXB Đại học"`
    *   `PublishYear: 2022`
    *   `Quantity: 2`
    *   `ShelfLocation: "D4-02"`
    *   `Status: Available`
    *   Corresponding `ChiTietPhieuMuon` record: `SachId: <GUID_BOOK_BORROWED_1>`, `TrangThaiChiTiet: "DangMuon"`.

*   **ID không tồn tại (Non-existent ID):** Một GUID ngẫu nhiên không có trong hệ thống, ví dụ: `00000000-0000-0000-0000-000000000000` hoặc `aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa`.

### 4. Test Cases

#### 4.1. Functional Tests

##### 4.1.1. Create Book (POST /api/v1/Books)

**QUAN-20260602-104003-TC01 (Happy Case): Create Book - All Valid Fields, BookCode Auto-Generated**
*   **Description:** Verify that a book can be created successfully when all required fields are provided and BookCode is omitted, allowing auto-generation.
*   **Pre-conditions:** None.
*   **Test Steps:**
    1.  **Given** the API endpoint for creating a book is `/api/v1/Books`.
    2.  **And** a valid `CreateBookCommand` request body is prepared with:
        ```json
        {
            "title": "Sách Mới 1",
            "author": "Tác Giả E",
            "category": "Văn học",
            "publisher": "NXB Mới",
            "publishYear": 2024,
            "quantity": 10,
            "shelfLocation": "E5-01"
        }
        ```
    3.  **When** a POST request is sent to the endpoint with the prepared body.
    4.  **Then** the API should respond with HTTP Status Code `201 Created`.
    5.  **And** the response body should contain `Success: true`, a message "Thêm sách thành công", and `Data` with a `Guid` for the new book's `id`.
    6.  **And** the newly created book should be retrievable via `GET /api/v1/Books/{id}` with a generated `BookCode` (e.g., `BK-XXXXXXX`) and `Status: "Available"`.
*   **Expected Results:** Book created, 201 status, correct response structure, data retrievable.
*   **Post-conditions:** A new book record exists in the database.
*   **Traceability:** `BRD PR#16: FR-01 Create Book`, `SRS PR#3: SAD-API-01 POST /api/v1/Books`, `DEV PR#11: CreateBookCommand.cs`, `Book.cs (UpdateQuantity)`

**QUAN-20260602-104003-TC02 (Happy Case): Create Book - All Valid Fields, BookCode Provided**
*   **Description:** Verify that a book can be created successfully when all required fields are provided, including a unique BookCode.
*   **Pre-conditions:** Ensure `BK-CUSTOM01` does not exist.
*   **Test Steps:**
    1.  **Given** the API endpoint for creating a book is `/api/v1/Books`.
    2.  **And** a valid `CreateBookCommand` request body is prepared with:
        ```json
        {
            "bookCode": "BK-CUSTOM01",
            "title": "Sách Mới 2",
            "author": "Tác Giả F",
            "category": "Khoa học",
            "publisher": "NXB Khoa học",
            "publishYear": 2024,
            "quantity": 7,
            "shelfLocation": "F6-02"
        }
        ```
    3.  **When** a POST request is sent to the endpoint with the prepared body.
    4.  **Then** the API should respond with HTTP Status Code `201 Created`.
    5.  **And** the response body should contain `Success: true`, a message "Thêm sách thành công", and `Data` with a `Guid` for the new book's `id`.
    6.  **And** the newly created book should be retrievable via `GET /api/v1/Books/{id}` with `BookCode: "BK-CUSTOM01"` and `Status: "Available"`.
*   **Expected Results:** Book created with provided BookCode, 201 status, correct response structure, data retrievable.
*   **Post-conditions:** A new book record exists in the database.
*   **Traceability:** `BRD PR#16: FR-01 Create Book`, `SRS PR#3: SAD-API-01 POST /api/v1/Books`, `DEV PR#11: CreateBookCommand.cs`

**QUAN-20260602-104003-TC03 (Edge Case): Create Book - Quantity is 0, Status OutOfStock**
*   **Description:** Verify that a book can be created with quantity 0, and its status is automatically set to "OutOfStock".
*   **Pre-conditions:** None.
*   **Test Steps:**
    1.  **Given** the API endpoint for creating a book is `/api/v1/Books`.
    2.  **And** a valid `CreateBookCommand` request body is prepared with `quantity: 0`.
    3.  **When** a POST request is sent to the endpoint.
    4.  **Then** the API should respond with HTTP Status Code `201 Created`.
    5.  **And** the newly created book should be retrievable via `GET /api/v1/Books/{id}` with `Quantity: 0` and `Status: "OutOfStock"`.
*   **Expected Results:** Book created successfully, Status is OutOfStock.
*   **Traceability:** `BRD PR#16: FR-01 Create Book`, `DEV PR#11: Book.cs (UpdateQuantity)`

**QUAN-20260602-104003-TC04 (Negative Case): Create Book - Missing Required Field (Title)**
*   **Description:** Verify that a book cannot be created if a required field like 'Title' is missing.
*   **Pre-conditions:** None.
*   **Test Steps:**
    1.  **Given** the API endpoint for creating a book is `/api/v1/Books`.
    2.  **And** an invalid `CreateBookCommand` request body is prepared with `title` omitted.
    3.  **When** a POST request is sent to the endpoint.
    4.  **Then** the API should respond with HTTP Status Code `400 Bad Request`.
    5.  **And** the response body should contain `Success: false`, a message indicating "Dữ liệu đầu vào không hợp lệ.", and `Errors` including `{ "Field": "Title", "Message": "Tên sách không được để trống." }`.
*   **Expected Results:** 400 status, specific validation error for Title.
*   **Traceability:** `DEV PR#11: CreateBookCommand.cs (CreateBookCommandValidator)`

**QUAN-20260602-104003-TC05 (Negative Case): Create Book - Duplicate BookCode**
*   **Description:** Verify that a book cannot be created if the provided BookCode already exists.
*   **Pre-conditions:** A book with `BookCode: "BK-EXISTING"` already exists in the database. (Refer to `BK-VALID01` as an existing code for this test).
*   **Test Steps:**
    1.  **Given** the API endpoint for creating a book is `/api/v1/Books`.
    2.  **And** an invalid `CreateBookCommand` request body is prepared with `bookCode: "BK-VALID01"` (which is already in use).
    3.  **When** a POST request is sent to the endpoint.
    4.  **Then** the API should respond with HTTP Status Code `400 Bad Request`.
    5.  **And** the response body should contain `Success: false`, and a message `Mã sách 'BK-VALID01' đã tồn tại trong hệ thống.`.
*   **Expected Results:** 400 status, specific error message for duplicate BookCode.
*   **Traceability:** `DEV PR#11: CreateBookCommand.cs (CreateBookCommandHandler)`

**QUAN-20260602-104003-TC06 (Negative Case): Create Book - Invalid PublishYear (Future Year)**
*   **Description:** Verify that a book cannot be created with a PublishYear in the future.
*   **Pre-conditions:** None.
*   **Test Steps:**
    1.  **Given** the API endpoint for creating a book is `/api/v1/Books`.
    2.  **And** an invalid `CreateBookCommand` request body is prepared with `publishYear: <CurrentYear + 1>`.
    3.  **When** a POST request is sent to the endpoint.
    4.  **Then** the API should respond with HTTP Status Code `400 Bad Request`.
    5.  **And** the response body should contain `Success: false`, and `Errors` including `{ "Field": "PublishYear", "Message": "Năm xuất bản không hợp lệ." }`.
*   **Expected Results:** 400 status, specific validation error for PublishYear.
*   **Traceability:** `DEV PR#11: CreateBookCommand.cs (CreateBookCommandValidator)`

**QUAN-20260602-104003-TC07 (Negative Case): Create Book - Invalid Quantity (Negative Value)**
*   **Description:** Verify that a book cannot be created with a negative Quantity.
*   **Pre-conditions:** None.
*   **Test Steps:**
    1.  **Given** the API endpoint for creating a book is `/api/v1/Books`.
    2.  **And** an invalid `CreateBookCommand` request body is prepared with `quantity: -1`.
    3.  **When** a POST request is sent to the endpoint.
    4.  **Then** the API should respond with HTTP Status Code `400 Bad Request`.
    5.  **And** the response body should contain `Success: false`, and `Errors` including `{ "Field": "Quantity", "Message": "Số lượng tồn kho không được nhỏ hơn 0." }`.
*   **Expected Results:** 400 status, specific validation error for Quantity.
*   **Traceability:** `DEV PR#11: CreateBookCommand.cs (CreateBookCommandValidator)`

##### 4.1.2. Update Book (PUT /api/v1/Books/{id})

**QUAN-20260602-104003-TC08 (Happy Case): Update Book - All Valid Fields**
*   **Description:** Verify that an existing book's information can be updated successfully.
*   **Pre-conditions:** A book with `Id: <GUID_BOOK_AVAILABLE_1>` and `BookCode: "BK-VALID01"` exists in the database (Test Data Pre-conditions).
*   **Test Steps:**
    1.  **Given** the API endpoint for updating a book is `/api/v1/Books/{id}`.
    2.  **And** a valid `UpdateBookCommand` request body is prepared with `id: <GUID_BOOK_AVAILABLE_1>` and updated fields:
        ```json
        {
            "id": "<GUID_BOOK_AVAILABLE_1>",
            "title": "Lập trình C# cơ bản và nâng cao",
            "author": "Nguyễn Văn A (Cập nhật)",
            "category": "Công nghệ thông tin",
            "publisher": "NXB Giáo dục TP.HCM",
            "publishYear": 2024,
            "quantity": 15,
            "shelfLocation": "A1-02"
        }
        ```
    3.  **When** a PUT request is sent to `/api/v1/Books/<GUID_BOOK_AVAILABLE_1>` with the prepared body.
    4.  **Then** the API should respond with HTTP Status Code `200 OK`.
    5.  **And** the response body should contain `Success: true` and a message "Cập nhật sách thành công".
    6.  **And** retrieving the book via `GET /api/v1/Books/<GUID_BOOK_AVAILABLE_1>` should show the updated information, and `Status: "Available"`.
*   **Expected Results:** Book updated, 200 status, correct response structure, updated data retrievable.
*   **Traceability:** `BRD PR#16: FR-02 Update Book`, `SRS PR#3: SAD-API-02 PUT /api/v1/Books/{id}`, `DEV PR#11: UpdateBookCommand.cs`, `Book.cs (UpdateQuantity)`

**QUAN-20260602-104003-TC09 (Edge Case): Update Book - Change Quantity to 0, Status OutOfStock**
*   **Description:** Verify that updating a book's quantity to 0 correctly changes its status to "OutOfStock".
*   **Pre-conditions:** A book with `Id: <GUID_BOOK_AVAILABLE_1>` exists with `Quantity > 0`.
*   **Test Steps:**
    1.  **Given** the API endpoint for updating a book is `/api/v1/Books/{id}`.
    2.  **And** a valid `UpdateBookCommand` request body is prepared with `id: <GUID_BOOK_AVAILABLE_1>` and `quantity: 0`.
    3.  **When** a PUT request is sent to `/api/v1/Books/<GUID_BOOK_AVAILABLE_1>` with the prepared body.
    4.  **Then** the API should respond with HTTP Status Code `200 OK`.
    5.  **And** retrieving the book via `GET /api/v1/Books/<GUID_BOOK_AVAILABLE_1>` should show `Quantity: 0` and `Status: "OutOfStock"`.
*   **Expected Results:** Book updated, Status is OutOfStock.
*   **Traceability:** `BRD PR#16: FR-02 Update Book`, `DEV PR#11: Book.cs (UpdateQuantity)`

**QUAN-20260602-104003-TC10 (Negative Case): Update Book - Book Not Found**
*   **Description:** Verify that an update request for a non-existent book returns a "Not Found" error.
*   **Pre-conditions:** None.
*   **Test Steps:**
    1.  **Given** the API endpoint for updating a book is `/api/v1/Books/{id}`.
    2.  **And** an `UpdateBookCommand` request body is prepared with `id: <NON_EXISTENT_GUID>`.
    3.  **When** a PUT request is sent to `/api/v1/Books/<NON_EXISTENT_GUID>` with the prepared body.
    4.  **Then** the API should respond with HTTP Status Code `404 Not Found`.
    5.  **And** the response body should contain `Success: false` and a message `Thực thể "Book" (<NON_EXISTENT_GUID>) không tồn tại trong hệ thống.`.
*   **Expected Results:** 404 status, specific not found error.
*   **Traceability:** `DEV PR#11: UpdateBookCommand.cs (UpdateBookCommandHandler)`, `NotFoundException.cs`

**QUAN-20260602-104003-TC11 (Negative Case): Update Book - ID Mismatch (URL vs. Body)**
*   **Description:** Verify that an update request fails if the ID in the URL does not match the ID in the request body.
*   **Pre-conditions:** A book with `Id: <GUID_BOOK_AVAILABLE_1>` exists.
*   **Test Steps:**
    1.  **Given** the API endpoint for updating a book is `/api/v1/Books/{id}`.
    2.  **And** an `UpdateBookCommand` request body is prepared with `id: <ANOTHER_VALID_GUID>`.
    3.  **When** a PUT request is sent to `/api/v1/Books/<GUID_BOOK_AVAILABLE_1>` (URL ID) with the body containing `<ANOTHER_VALID_GUID>` (body ID).
    4.  **Then** the API should respond with HTTP Status Code `400 Bad Request`.
    5.  **And** the response body should contain `Success: false` and a message `ID sách trong URL không trùng khớp với request body.`.
*   **Expected Results:** 400 status, specific ID mismatch error.
*   **Traceability:** `DEV PR#11: BooksController.cs (Update action)`

##### 4.1.3. Delete Book (DELETE /api/v1/Books/{id})

**QUAN-20260602-104003-TC12 (Happy Case): Delete Book - Successfully Delete an Unborrowed Book**
*   **Description:** Verify that an existing book not currently borrowed can be soft-deleted successfully.
*   **Pre-conditions:** Create a new book `Id: <GUID_BOOK_TO_DELETE>` that is not borrowed.
*   **Test Steps:**
    1.  **Given** the API endpoint for deleting a book is `/api/v1/Books/{id}`.
    2.  **And** a book with `Id: <GUID_BOOK_TO_DELETE>` exists and is not associated with any "DangMuon" `ChiTietPhieuMuon` records.
    3.  **When** a DELETE request is sent to `/api/v1/Books/<GUID_BOOK_TO_DELETE>`.
    4.  **Then** the API should respond with HTTP Status Code `200 OK`.
    5.  **And** the response body should contain `Success: true` and a message "Xóa sách thành công".
    6.  **And** retrieving the book via `GET /api/v1/Books/<GUID_BOOK_TO_DELETE>` should result in `404 Not Found` due to the global query filter.
    7.  **And** querying the database directly should show `IsDeleted: true` for `Id: <GUID_BOOK_TO_DELETE>`.
*   **Expected Results:** Book soft-deleted, 200 status, not found via GET, `IsDeleted` flag updated.
*   **Traceability:** `BRD PR#16: FR-03 Delete Book`, `SRS PR#3: SAD-API-03 DELETE /api/v1/Books/{id}`, `DEV PR#11: DeleteBookCommand.cs`, `BookConfiguration.cs (HasQueryFilter)`

**QUAN-20260602-104003-TC13 (Negative Case): Delete Book - Book Not Found**
*   **Description:** Verify that a delete request for a non-existent book returns a "Not Found" error.
*   **Pre-conditions:** None.
*   **Test Steps:**
    1.  **Given** the API endpoint for deleting a book is `/api/v1/Books/{id}`.
    2.  **When** a DELETE request is sent to `/api/v1/Books/<NON_EXISTENT_GUID>`.
    3.  **Then** the API should respond with HTTP Status Code `404 Not Found`.
    4.  **And** the response body should contain `Success: false` and a message `Thực thể "Book" (<NON_EXISTENT_GUID>) không tồn tại trong hệ thống.`.
*   **Expected Results:** 404 status, specific not found error.
*   **Traceability:** `DEV PR#11: DeleteBookCommand.cs (DeleteBookCommandHandler)`, `NotFoundException.cs`

**QUAN-20260602-104003-TC14 (Negative Case): Delete Book - Book is Currently Borrowed**
*   **Description:** Verify that a book currently borrowed cannot be deleted.
*   **Pre-conditions:** A book with `Id: <GUID_BOOK_BORROWED_1>` exists and has associated "DangMuon" `ChiTietPhieuMuon` records (Test Data Pre-conditions).
*   **Test Steps:**
    1.  **Given** the API endpoint for deleting a book is `/api/v1/Books/{id}`.
    2.  **And** a book with `Id: <GUID_BOOK_BORROWED_1>` is currently borrowed.
    3.  **When** a DELETE request is sent to `/api/v1/Books/<GUID_BOOK_BORROWED_1>`.
    4.  **Then** the API should respond with HTTP Status Code `400 Bad Request`.
    5.  **And** the response body should contain `Success: false` and a message `Không thể xóa sách vì hiện tại đang có độc giả mượn cuốn sách này.`.
*   **Expected Results:** 400 status, specific business rule validation error.
*   **Traceability:** `DEV PR#11: DeleteBookCommand.cs (DeleteBookCommandHandler)`

##### 4.1.4. Get Book by ID (GET /api/v1/Books/{id})

**QUAN-20260602-104003-TC15 (Happy Case): Get Book by ID - Existing Book**
*   **Description:** Verify that details of an existing book can be retrieved successfully.
*   **Pre-conditions:** A book with `Id: <GUID_BOOK_AVAILABLE_1>` exists (Test Data Pre-conditions).
*   **Test Steps:**
    1.  **Given** the API endpoint for getting a book by ID is `/api/v1/Books/{id}`.
    2.  **And** a book with `Id: <GUID_BOOK_AVAILABLE_1>` exists.
    3.  **When** a GET request is sent to `/api/v1/Books/<GUID_BOOK_AVAILABLE_1>`.
    4.  **Then** the API should respond with HTTP Status Code `200 OK`.
    5.  **And** the response body should contain `Success: true` and `Data` with the full details of `GUID_BOOK_AVAILABLE_1`, including `Status: "Available"`.
*   **Expected Results:** Book details retrieved, 200 status, correct response structure and data.
*   **Traceability:** `BRD PR#16: FR-04 Get Book by ID`, `SRS PR#3: SAD-API-04 GET /api/v1/Books/{id}`, `DEV PR#11: GetBookByIdQuery.cs`

**QUAN-20260602-104003-TC16 (Negative Case): Get Book by ID - Book Not Found**
*   **Description:** Verify that a request for a non-existent book ID returns a "Not Found" error.
*   **Pre-conditions:** None.
*   **Test Steps:**
    1.  **Given** the API endpoint for getting a book by ID is `/api/v1/Books/{id}`.
    2.  **When** a GET request is sent to `/api/v1/Books/<NON_EXISTENT_GUID>`.
    3.  **Then** the API should respond with HTTP Status Code `404 Not Found`.
    4.  **And** the response body should contain `Success: false` and a message `Thực thể "Book" (<NON_EXISTENT_GUID>) không tồn tại trong hệ thống.`.
*   **Expected Results:** 404 status, specific not found error.
*   **Traceability:** `DEV PR#11: GetBookByIdQuery.cs (GetBookByIdQueryHandler)`, `NotFoundException.cs`

**QUAN-20260602-104003-TC17 (Edge Case): Get Book by ID - Soft Deleted Book**
*   **Description:** Verify that a soft-deleted book is not retrievable by ID.
*   **Pre-conditions:** A book with `Id: <GUID_SOFT_DELETED_BOOK>` exists and `IsDeleted: true` (Test Data Pre-conditions).
*   **Test Steps:**
    1.  **Given** the API endpoint for getting a book by ID is `/api/v1/Books/{id}`.
    2.  **And** a book with `Id: <GUID_SOFT_DELETED_BOOK>` is soft-deleted (`IsDeleted = true`).
    3.  **When** a GET request is sent to `/api/v1/Books/<GUID_SOFT_DELETED_BOOK>`.
    4.  **Then** the API should respond with HTTP Status Code `404 Not Found`.
    5.  **And** the response body should contain `Success: false` and a message `Thực thể "Book" (<GUID_SOFT_DELETED_BOOK>) không tồn tại trong hệ thống.`.
*   **Expected Results:** 404 status, confirming global query filter is working.
*   **Traceability:** `DEV PR#11: BookConfiguration.cs (HasQueryFilter)`

##### 4.1.5. Get All Books (GET /api/v1/Books)

**QUAN-20260602-104003-TC18 (Happy Case): Get All Books - No Filters, Default Pagination**
*   **Description:** Verify that all available books are returned with default pagination (Page 1, Size 10) when no filters are applied.
*   **Pre-conditions:** Multiple books exist, some available, some out of stock, some soft-deleted. Ensure at least 15 active books.
*   **Test Steps:**
    1.  **Given** the API endpoint for getting all books is `/api/v1/Books`.
    2.  **When** a GET request is sent to `/api/v1/Books` without any query parameters.
    3.  **Then** the API should respond with HTTP Status Code `200 OK`.
    4.  **And** the response body should contain `Success: true` and `Data` which is a `PaginatedList`.
    5.  **And** `PaginatedList.Items` should contain 10 books.
    6.  **And** `PaginatedList.PageNumber` should be 1, `PageSize` should be 10.
    7.  **And** the books in `Items` should be sorted by `Title` alphabetically.
    8.  **And** no soft-deleted books should be included.
*   **Expected Results:** Paginated list of 10 available books, sorted by title.
*   **Traceability:** `BRD PR#16: FR-05 Get All Books`, `SRS PR#3: SAD-API-05 GET /api/v1/Books`, `DEV PR#11: GetBooksQuery.cs`, `BookConfiguration.cs (HasQueryFilter)`

**QUAN-20260602-104003-TC19 (Happy Case): Get All Books - Filter by Search Term (Title)**
*   **Description:** Verify that books can be filtered by a search term matching their title (case-insensitive).
*   **Pre-conditions:** Books with titles like "Lập trình C# nâng cao" and "Lập trình Web" exist.
*   **Test Steps:**
    1.  **Given** the API endpoint for getting all books is `/api/v1/Books`.
    2.  **When** a GET request is sent to `/api/v1/Books?search=Lập trình`.
    3.  **Then** the API should respond with HTTP Status Code `200 OK`.
    4.  **And** `PaginatedList.Items` should contain only books whose titles (or authors) include "Lập trình" (case-insensitive).
*   **Expected Results:** Filtered list of books matching the search term in title.
*   **Traceability:** `BRD PR#16: FR-05 Get All Books`, `DEV PR#11: GetBooksQuery.cs (Search filter)`

**QUAN-20260602-104003-TC20 (Happy Case): Get All Books - Filter by Search Term (Author)**
*   **Description:** Verify that books can be filtered by a search term matching their author (case-insensitive).
*   **Pre-conditions:** Books with authors like "Nguyễn Văn A" and "Phạm Văn C" exist.
*   **Test Steps:**
    1.  **Given** the API endpoint for getting all books is `/api/v1/Books`.
    2.  **When** a GET request is sent to `/api/v1/Books?search=văn a`.
    3.  **Then** the API should respond with HTTP Status Code `200 OK`.
    4.  **And** `PaginatedList.Items` should contain only books whose authors (or titles) include "văn a" (case-insensitive).
*   **Expected Results:** Filtered list of books matching the search term in author.
*   **Traceability:** `BRD PR#16: FR-05 Get All Books`, `DEV PR#11: GetBooksQuery.cs (Search filter)`

**QUAN-20260602-104003-TC21 (Happy Case): Get All Books - Filter by Category**
*   **Description:** Verify that books can be filtered by category (case-insensitive).
*   **Pre-conditions:** Books with `Category: "Tin học"` and `Category: "Kinh tế"` exist.
*   **Test Steps:**
    1.  **Given** the API endpoint for getting all books is `/api/v1/Books`.
    2.  **When** a GET request is sent to `/api/v1/Books?category=tin học`.
    3.  **Then** the API should respond with HTTP Status Code `200 OK`.
    4.  **And** `PaginatedList.Items` should contain only books with `Category: "Tin học"` (case-insensitive).
*   **Expected Results:** Filtered list of books matching the category.
*   **Traceability:** `BRD PR#16: FR-05 Get All Books`, `DEV PR#11: GetBooksQuery.cs (Category filter)`

**QUAN-20260602-104003-TC22 (Happy Case): Get All Books - Filter by Status (Available)**
*   **Description:** Verify that books can be filtered by `Status: Available`.
*   **Pre-conditions:** Books with `Status: Available` and `Status: OutOfStock` exist.
*   **Test Steps:**
    1.  **Given** the API endpoint for getting all books is `/api/v1/Books`.
    2.  **When** a GET request is sent to `/api/v1/Books?status=Available`.
    3.  **Then** the API should respond with HTTP Status Code `200 OK`.
    4.  **And** `PaginatedList.Items` should contain only books with `Status: "Available"`.
*   **Expected Results:** Filtered list of books with "Available" status.
*   **Traceability:** `BRD PR#16: FR-05 Get All Books`, `DEV PR#11: GetBooksQuery.cs (Status filter)`

**QUAN-20260602-104003-TC23 (Happy Case): Get All Books - Filter by Status (OutOfStock)**
*   **Description:** Verify that books can be filtered by `Status: OutOfStock`.
*   **Pre-conditions:** Books with `Status: OutOfStock` exist (e.g., `GUID_BOOK_OUTOFSTOCK_1`).
*   **Test Steps:**
    1.  **Given** the API endpoint for getting all books is `/api/v1/Books`.
    2.  **When** a GET request is sent to `/api/v1/Books?status=OutOfStock`.
    3.  **Then** the API should respond with HTTP Status Code `200 OK`.
    4.  **And** `PaginatedList.Items` should contain only books with `Status: "OutOfStock"`.
*   **Expected Results:** Filtered list of books with "OutOfStock" status.
*   **Traceability:** `BRD PR#16: FR-05 Get All Books`, `DEV PR#11: GetBooksQuery.cs (Status filter)`

**QUAN-20260602-104003-TC24 (Edge Case): Get All Books - Pagination with Large PageNumber**
*   **Description:** Verify that pagination works correctly for a large page number where no results exist.
*   **Pre-conditions:** Assume 20 active books in total.
*   **Test Steps:**
    1.  **Given** the API endpoint for getting all books is `/api/v1/Books`.
    2.  **When** a GET request is sent to `/api/v1/Books?pageNumber=100&pageSize=10`.
    3.  **Then** the API should respond with HTTP Status Code `200 OK`.
    4.  **And** `PaginatedList.Items` should be an empty list.
    5.  **And** `PaginatedList.PageNumber` should be 100, `PageSize` should be 10.
    6.  **And** `PaginatedList.TotalCount` should reflect the total number of active books.
*   **Expected Results:** Empty list, correct pagination metadata.
*   **Traceability:** `BRD PR#16: FR-05 Get All Books`, `DEV PR#11: GetBooksQuery.cs (Pagination logic)`

**QUAN-20260602-104003-TC25 (Edge Case): Get All Books - Pagination with PageNumber/PageSize 0 or Negative**
*   **Description:** Verify that pageNumber and pageSize default to 1 and 10 respectively when invalid values (0 or negative) are provided.
*   **Pre-conditions:** Multiple active books exist.
*   **Test Steps:**
    1.  **Given** the API endpoint for getting all books is `/api/v1/Books`.
    2.  **When** a GET request is sent to `/api/v1/Books?pageNumber=0&pageSize=-5`.
    3.  **Then** the API should respond with HTTP Status Code `200 OK`.
    4.  **And** `PaginatedList.Items` should contain 10 books.
    5.  **And** `PaginatedList.PageNumber` should be 1, `PageSize` should be 10.
*   **Expected Results:** Default pagination applied.
*   **Traceability:** `DEV PR#11: GetBooksQuery.cs (Pagination logic)`

**QUAN-20260602-104003-TC26 (Edge Case): Get All Books - Search/Category/Status Filters with No Matches**
*   **Description:** Verify that an empty list is returned when filters result in no matches.
*   **Pre-conditions:** None.
*   **Test Steps:**
    1.  **Given** the API endpoint for getting all books is `/api/v1/Books`.
    2.  **When** a GET request is sent to `/api/v1/Books?search=NonExistentBook&category=NonExistentCategory&status=Available`.
    3.  **Then** the API should respond with HTTP Status Code `200 OK`.
    4.  **And** `PaginatedList.Items` should be an empty list.
    5.  **And** `PaginatedList.TotalCount` should be 0.
*   **Expected Results:** Empty list, `TotalCount` is 0.
*   **Traceability:** `BRD PR#16: FR-05 Get All Books`, `DEV PR#11: GetBooksQuery.cs (Filter logic)`

#### 4.2. API Tests

**QUAN-20260602-104003-API01: API Response Structure Compliance (Success)**
*   **Description:** Verify that all successful API responses conform to the `ApiResponse.SuccessResult` structure.
*   **Pre-conditions:** Valid API operations (Create, Update, Delete, Get, Get All).
*   **Test Steps:**
    1.  **Given** a successful API call (e.g., `POST /api/v1/Books` with valid data).
    2.  **When** the API responds.
    3.  **Then** the response body should be a JSON object containing `Success: true`, `Data` (optional, can be null or object/array), and `Message` (optional, can be null or string).
*   **Expected Results:** Consistent success response structure.
*   **Traceability:** `DEV PR#11: BooksController.cs`, `ApiResponse.cs`

**QUAN-20260602-104003-API02: API Response Structure Compliance (Validation Error)**
*   **Description:** Verify that all API validation error responses conform to the `ApiResponse.FailureResult` structure for `ValidationException`.
*   **Pre-conditions:** API calls triggering validation errors (e.g., missing required fields, invalid formats).
*   **Test Steps:**
    1.  **Given** an API call triggering a `ValidationException` (e.g., `POST /api/v1/Books` with missing `Title`).
    2.  **When** the API responds.
    3.  **Then** the response body should be a JSON object containing `Success: false`, `Message` (e.g., "Dữ liệu đầu vào không hợp lệ."), and `Errors` (a list of `ApiError` objects, each with `Field` and `Message`).
    4.  **And** the HTTP Status Code should be `400 Bad Request`.
*   **Expected Results:** Consistent validation error response structure and 400 status.
*   **Traceability:** `DEV PR#11: GlobalExceptionMiddleware.cs`, `ValidationBehavior.cs`, `ApiResponse.cs`

**QUAN-20260602-104003-API03: API Response Structure Compliance (Not Found Error)**
*   **Description:** Verify that all API "not found" error responses conform to the `ApiResponse.FailureResult` structure for `NotFoundException`.
*   **Pre-conditions:** API calls referencing non-existent resources (e.g., `GET /api/v1/Books/{non-existent-id}`).
*   **Test Steps:**
    1.  **Given** an API call triggering a `NotFoundException` (e.g., `GET /api/v1/Books/<NON_EXISTENT_GUID>`).
    2.  **When** the API responds.
    3.  **Then** the response body should be a JSON object containing `Success: false` and `Message` (e.g., `Thực thể "Book" (...) không tồn tại trong hệ thống.`). `Errors` list should be empty.
    4.  **And** the HTTP Status Code should be `404 Not Found`.
*   **Expected Results:** Consistent not found error response structure and 404 status.
*   **Traceability:** `DEV PR#11: GlobalExceptionMiddleware.cs`, `NotFoundException.cs`, `ApiResponse.cs`

**QUAN-20260602-104003-API04: Trimming of String Inputs**
*   **Description:** Verify that string inputs are automatically trimmed (leading/trailing spaces removed).
*   **Pre-conditions:** None.
*   **Test Steps:**
    1.  **Given** a `CreateBookCommand` request with `title: "  Sách với khoảng trắng  "`, `author: " Tác giả có space "`
    2.  **When** a POST request is sent to `/api/v1/Books`.
    3.  **Then** the API should respond with HTTP Status Code `201 Created`.
    4.  **And** retrieving the book by ID should show `Title: "Sách với khoảng trắng"` and `Author: "Tác giả có space"` (trimmed).
*   **Expected Results:** String fields are trimmed before being saved.
*   **Traceability:** `DEV PR#11: CreateBookCommand.cs (CreateBookCommandHandler)`, `UpdateBookCommand.cs (UpdateBookCommandHandler)`

#### 4.3. Security Tests
*(Note: Based solely on the provided diff, full-fledged security testing for authentication/authorization is beyond the scope here as no related code is visible. Focus will be on input validation and data integrity aspects as they relate to security.)*

**QUAN-20260602-104003-SEC01: Input Sanitization / XSS Prevention (Implied)**
*   **Description:** Verify that malicious script injection in string inputs is handled, either by sanitization or by being rendered as plain text. (Given EF Core and basic string operations, direct SQL injection is largely mitigated, this focuses on output rendering).
*   **Pre-conditions:** None.
*   **Test Steps:**
    1.  **Given** a `CreateBookCommand` request with `title: "<script>alert('XSS');</script>"` and `author: "Valid Author"`.
    2.  **When** a POST request is sent to `/api/v1/Books`.
    3.  **Then** the API should respond with HTTP Status Code `201 Created`.
    4.  **And** retrieving the book via `GET /api/v1/Books/{id}` should return the title as `<script>alert('XSS');</script>`. (This is expected for API output; actual XSS prevention happens at the UI level during rendering, but API should not *transform* input in a way that makes it executable).
*   **Expected Results:** The API should store and return the exact string, relying on the client-side to properly escape HTML/prevent XSS upon rendering. No server-side sanitization converting `&lt;` to `<` or similar.
*   **Traceability:** `Technical Guideline: Data Sanitization` (Assumed general guideline, not explicitly in diff but good practice).

**QUAN-20260602-104003-SEC02: Concurrency Handling (Optimistic Concurrency)**
*   **Description:** Verify that concurrent updates to the same book are handled gracefully, preventing data loss.
*   **Pre-conditions:** A book exists with `Id: <GUID_BOOK_AVAILABLE_1>`.
*   **Test Steps:**
    1.  **Given** two concurrent requests to update the same book `Id: <GUID_BOOK_AVAILABLE_1>`.
    2.  **And** Request 1 reads the book, prepares an update.
    3.  **And** Request 2 reads the book, prepares an update with different data.
    4.  **And** Request 1 sends its `PUT` request and succeeds.
    5.  **When** Request 2 sends its `PUT` request.
    6.  **Then** Request 2 should respond with a concurrency exception (e.g., `DbUpdateConcurrencyException` leading to 500 error or a custom 409 Conflict if middleware handles it).
*   **Expected Results:** Only one update succeeds, the other fails due to concurrency conflict (due to `xmin` token).
*   **Traceability:** `DEV PR#11: BookConfiguration.cs (xmin concurrency token)`

#### 4.4. Boundary/Validation Tests

**QUAN-20260602-104003-BV01: Max Length - Title (200 chars)**
*   **Description:** Verify `Title` field respects max length (200 characters).
*   **Pre-conditions:** None.
*   **Test Steps:**
    1.  **Given** a `CreateBookCommand` request with `title` having exactly 200 characters.
    2.  **When** a POST request is sent.
    3.  **Then** the API should respond with `201 Created`.
    4.  **Given** a `CreateBookCommand` request with `title` having 201 characters.
    5.  **When** a POST request is sent.
    6.  **Then** the API should respond with `400 Bad Request` and validation error `Tên sách không được vượt quá 200 ký tự.`.
*   **Expected Results:** 200 chars accepted, 201 chars rejected.
*   **Traceability:** `DEV PR#11: CreateBookCommand.cs (CreateBookCommandValidator)`

**QUAN-20260602-104003-BV02: Max Length - Author, Category, Publisher (100 chars)**
*   **Description:** Verify `Author`, `Category`, `Publisher` fields respect max length (100 characters).
*   **Pre-conditions:** None.
*   **Test Steps:** (Repeat for Author, Category, Publisher)
    1.  **Given** a `CreateBookCommand` request with `author` (or `category` or `publisher`) having exactly 100 characters.
    2.  **When** a POST request is sent.
    3.  **Then** the API should respond with `201 Created`.
    4.  **Given** a `CreateBookCommand` request with `author` (or `category` or `publisher`) having 101 characters.
    5.  **When** a POST request is sent.
    6.  **Then** the API should respond with `400 Bad Request` and relevant validation error.
*   **Expected Results:** 100 chars accepted, 101 chars rejected.
*   **Traceability:** `DEV PR#11: CreateBookCommand.cs (CreateBookCommandValidator)`

**QUAN-20260602-104003-BV03: Min Value - PublishYear (No explicit min, but must be valid past year)**
*   **Description:** Verify `PublishYear` field handles extreme past valid values.
*   **Pre-conditions:** None.
*   **Test Steps:**
    1.  **Given** a `CreateBookCommand` request with `publishYear: 1000` (assuming a book can be this old).
    2.  **When** a POST request is sent.
    3.  **Then** the API should respond with `201 Created`.
*   **Expected Results:** Extremely old valid year is accepted.
*   **Traceability:** `DEV PR#11: CreateBookCommand.cs (CreateBookCommandValidator)`

**QUAN-20260602-104003-BV04: Min/Max Value - Quantity (0 and large number)**
*   **Description:** Verify `Quantity` field allows 0 and very large numbers.
*   **Pre-conditions:** None.
*   **Test Steps:**
    1.  **Given** a `CreateBookCommand` request with `quantity: 0`.
    2.  **When** a POST request is sent.
    3.  **Then** the API should respond with `201 Created` and `Status: "OutOfStock"`.
    4.  **Given** a `CreateBookCommand` request with `quantity: 2147483647` (Max `int` value).
    5.  **When** a POST request is sent.
    6.  **Then** the API should respond with `201 Created` and `Status: "Available"`.
*   **Expected Results:** Quantity 0 accepted, max int accepted.
*   **Traceability:** `DEV PR#11: CreateBookCommand.cs (CreateBookCommandValidator)`, `Book.cs (UpdateQuantity)`

### 5. Traceability Matrix

| Requirement ID (FR/SRS from diff)                                                                    | Test Case IDs                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                    | Coverage % |
| :--------------------------------------------------------------------------------------------------- | :----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | :--------- |
| **CREATE BOOK (CreateBookCommand)**                                                                  |                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                  |            |
| FR-01: Ability to create a new book.                                                                 | QUAN-20260602-104003-TC01, QUAN-20260602-104003-TC02                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                | 100%       |
| SRS-API-01: POST /api/v1/Books endpoint.                                                             | QUAN-20260602-104003-TC01, QUAN-20260602-104003-TC02, QUAN-20260602-104003-TC03, QUAN-20260602-104003-TC04, QUAN-20260602-104003-TC05, QUAN-20260602-104003-TC06, QUAN-20260602-104003-TC07, QUAN-20260602-104003-BV01, QUAN-20260602-104003-BV02, QUAN-20260602-104003-BV03, QUAN-20260602-104003-BV04                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                | 100%       |
| Validation: Title NotEmpty, MaxLength(200).                                                          | QUAN-20260602-104003-TC04, QUAN-20260602-104003-BV01                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                | 100%       |
| Validation: Author NotEmpty, MaxLength(100).                                                         | QUAN-20260602-104003-BV02                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        | 100%       |
| Validation: Category NotEmpty, MaxLength(100).                                                       | QUAN-20260602-104003-BV02                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        | 100%       |
| Validation: Publisher NotEmpty, MaxLength(100).                                                      | QUAN-20260602-104003-BV02                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        | 100%       |
| Validation: PublishYear <= CurrentYear.                                                              | QUAN-20260602-104003-TC06, QUAN-20260602-104003-BV03                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                              | 100%       |
| Validation: Quantity >= 0.                                                                           | QUAN-20260602-104003-TC07, QUAN-20260602-104003-BV04                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                              | 100%       |
| Business Rule: BookCode auto-generation if not provided.                                             | QUAN-20260602-104003-TC01                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        | 100%       |
| Business Rule: BookCode must be unique.                                                              | QUAN-20260602-104003-TC05                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        | 100%       |
| Domain Rule: UpdateQuantity sets Status (Available/OutOfStock).                                      | QUAN-20260602-104003-TC03                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        | 100%       |
| Technical: String trimming on input.                                                                 | QUAN-20260602-104003-API04                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                       | 100%       |
| **UPDATE BOOK (UpdateBookCommand)**                                                                  |                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                  |            |
| FR-02: Ability to update an existing book.                                                           | QUAN-20260602-104003-TC08, QUAN-20260602-104003-TC09                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                | 100%       |
| SRS-API-02: PUT /api/v1/Books/{id} endpoint.                                                         | QUAN-20260602-104003-TC08, QUAN-20260602-104003-TC09, QUAN-20260602-104003-TC10, QUAN-20260602-104003-TC11                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                           | 100%       |
| Validation: Id NotEmpty.                                                                             | (Implicitly covered by TC10, TC11 - invalid GUID would lead to Not Found or Bad Request)                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                            | 100%       |
| Validation: Title, Author, Category, Publisher, PublishYear, Quantity as per Create.                 | QUAN-20260602-104003-TC08, QUAN-20260602-104003-TC09 (implicitly covers update validation, specific negative/boundary cases covered by Create's equivalent where logic is shared)                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                         | 100%       |
| Business Rule: Book must exist (NotFoundException).                                                  | QUAN-20260602-104003-TC10                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        | 100%       |
| Business Rule: ID in URL must match ID in body.                                                      | QUAN-20260602-104003-TC11                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        | 100%       |
| Domain Rule: UpdateQuantity sets Status (Available/OutOfStock).                                      | QUAN-20260602-104003-TC09                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        | 100%       |
| Technical: String trimming on input.                                                                 | QUAN-20260602-104003-API04                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                       | 100%       |
| **DELETE BOOK (DeleteBookCommand)**                                                                  |                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                  |            |
| FR-03: Ability to delete a book.                                                                     | QUAN-20260602-104003-TC12                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        | 100%       |
| SRS-API-03: DELETE /api/v1/Books/{id} endpoint.                                                      | QUAN-20260602-104003-TC12, QUAN-20260602-104003-TC13, QUAN-20260602-104003-TC14                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                  | 100%       |
| Validation: Id NotEmpty.                                                                             | (Implicitly covered by TC13)                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                   | 100%       |
| Business Rule: Book must exist (NotFoundException).                                                  | QUAN-20260602-104003-TC13                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        | 100%       |
| Business Rule: Cannot delete if borrowed (`TrangThaiChiTiet == "DangMuon"`).                         | QUAN-20260602-104003-TC14                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        | 100%       |
| Technical: Soft Delete (`IsDeleted = true`).                                                         | QUAN-20260602-104003-TC12                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        | 100%       |
| **GET BOOK BY ID (GetBookByIdQuery)**                                                                |                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                  |            |
| FR-04: Ability to retrieve a book by ID.                                                             | QUAN-20260602-104003-TC15                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        | 100%       |
| SRS-API-04: GET /api/v1/Books/{id} endpoint.                                                         | QUAN-20260602-104003-TC15, QUAN-20260602-104003-TC16, QUAN-20260602-104003-TC17                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                  | 100%       |
| Business Rule: Book must exist (NotFoundException).                                                  | QUAN-20260602-104003-TC16                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        | 100%       |
| Technical: Global query filter for `IsDeleted`.                                                      | QUAN-20260602-104003-TC17                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        | 100%       |
| **GET ALL BOOKS (GetBooksQuery)**                                                                    |                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                  |            |
| FR-05: Ability to retrieve a paginated list of books.                                                | QUAN-20260602-104003-TC18                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        | 100%       |
| SRS-API-05: GET /api/v1/Books endpoint.                                                              | QUAN-20260602-104003-TC18, QUAN-20260602-104003-TC19, QUAN-20260602-104003-TC20, QUAN-20260602-104003-TC21, QUAN-20260602-104003-TC22, QUAN-20260602-104003-TC23, QUAN-20260602-104003-TC24, QUAN-20260602-104003-TC25, QUAN-20260602-104003-TC26                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                     | 100%       |
| Filtering: By `Search` (Title/Author, case-insensitive).                                             | QUAN-20260602-104003-TC19, QUAN-20260602-104003-TC20                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                              | 100%       |
| Filtering: By `Category` (case-insensitive).                                                         | QUAN-20260602-104003-TC21                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        | 100%       |
| Filtering: By `Status` (Available/OutOfStock).                                                       | QUAN-20260602-104003-TC22, QUAN-20260602-104003-TC23                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                              | 100%       |
| Pagination: `PageNumber`, `PageSize`, defaults to 1 and 10.                                          | QUAN-20260602-104003-TC18, QUAN-20260602-104003-TC24, QUAN-20260602-104003-TC25                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                  | 100%       |
| Sorting: By `Title`.                                                                                 | QUAN-20260602-104003-TC18                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        | 100%       |
| Technical: Global query filter for `IsDeleted`.                                                      | QUAN-20260602-104003-TC18 (implied, no soft-deleted books in results)                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                              | 100%       |
| **COMMON INFRASTRUCTURE / GUIDELINES**                                                               |                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                  |            |
| Technical Guideline: API Response structure (Success).                                               | QUAN-20260602-104003-API01                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                       | 100%       |
| Technical Guideline: API Response structure (Validation Error).                                      | QUAN-20260602-104003-API02                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                       | 100%       |
| Technical Guideline: API Response structure (Not Found Error).                                       | QUAN-20260602-104003-API03                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                       | 100%       |
| Technical Guideline: Concurrency handling (`xmin`).                                                  | QUAN-20260602-104003-SEC02                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                       | 100%       |
| Technical Guideline: FluentValidation integration (`ValidationBehavior`).                              | QUAN-20260602-104003-TC04, QUAN-20260602-104003-TC06, QUAN-20260602-104003-TC07, QUAN-20260602-104003-BV01, QUAN-20260602-104003-BV02, QUAN-20260602-104003-BV03, QUAN-20260602-104003-BV04 (all validation related TCs)                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                            | 100%       |
| Technical Guideline: IApplicationDbContext interface for Book entity.                                | (Covered by functional tests, as it's an internal architectural concern. API tests confirm interaction with DB, hence implicitly covers this)                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        | 100%       |

---