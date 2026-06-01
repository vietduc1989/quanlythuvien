# Coding Standards — quanlythuvien

Tài liệu này quy định các tiêu chuẩn viết code (C# và TypeScript/React) áp dụng cho dự án Quản lý Thư viện.

## 1. Tiêu chuẩn viết code C# (.NET 10)

### 1.1 Quy tắc đặt tên
- **Class, Interface, Struct, Enum**: Dùng `PascalCase`. Ví dụ: `Book`, `IBookRepository`, `BookStatus`.
- **Method**: Dùng `PascalCase`. Ví dụ: `GetBookByIdAsync()`.
- **Property**: Dùng `PascalCase`. Ví dụ: `Title`, `IsAvailable`.
- **Private Field**: Dùng `_camelCase` (gạch dưới). Ví dụ: `_bookRepository`, `_logger`.
- **Local Variable, Parameter**: Dùng `camelCase`. Ví dụ: `bookDto`, `totalCount`.
- **Constant**: Dùng `UPPER_SNAKE_CASE`. Ví dụ: `MAX_BORROW_DAYS`.
- **Async Method**: Bắt buộc có hậu tố `Async`. Ví dụ: `CreateBookAsync()`.
- **Boolean Property/Variable**: Sử dụng tiền tố `Is`, `Has`, `Can`. Ví dụ: `IsDeleted`, `HasStock`.

### 1.2 Cấu trúc file và độ dài
- Mỗi file chỉ chứa **1 class chính**.
- Tối đa **300 dòng/file**, **30 dòng/method**, **4 parameter/method**.
- Tránh lồng nhau quá sâu (Deep nesting) -> sử dụng guard clauses để return sớm.

---

## 2. Tiêu chuẩn viết code TypeScript & React

### 2.1 Quy tắc đặt tên
- **Component**: Dùng `PascalCase` và lưu file dưới dạng `.tsx`. Ví dụ: `BookListPage.tsx`, `BookCard.tsx`.
- **Hook**: Dùng `camelCase`, bắt đầu bằng `use`. Ví dụ: `useBooks.ts`.
- **Function**: Dùng `camelCase`. Ví dụ: `handleSearch()`, `formatDate()`.
- **Type, Interface**: Dùng `PascalCase`. Ví dụ: `BookDto`, `IBorrowRequest`.
- **Constant**: Dùng `UPPER_SNAKE_CASE`. Ví dụ: `API_VERSION`.

### 2.2 Quy tắc viết Component
- Sử dụng Functional Component với React Hooks.
- Không viết logic quá phức tạp trong Component UI, hãy tách sang Custom Hooks hoặc Helper functions.
- Sử dụng Mantine UI làm thư viện giao diện chính.

---

## 3. Best Practices chung
- **Result Pattern**: Sử dụng class `Result<T>` để trả về kết quả xử lý nghiệp vụ thay vì ném Exception vô điều kiện.
- **CQRS**: Các thay đổi dữ liệu dùng `Command`, truy vấn dữ liệu dùng `Query` thông qua MediatR.
- **FluentValidation**: Toàn bộ dữ liệu đầu vào phải được Validate qua FluentValidation ở lớp Application.
