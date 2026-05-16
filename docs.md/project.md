# Cấu trúc dự án .NET 10 Web API (Minimal APIs)

Sơ đồ cấu trúc và giải thích các file/thư mục được sinh ra sau khi chạy lệnh `dotnet new webapi`.

---

## 1. Sơ đồ cấu trúc thư mục

```text
1.first_learn/
├── Properties/
│   └── launchSettings.json       # Cấu hình cổng (port) và môi trường khi chạy dự án
├── docs.md/
│   └── project.md                  # Tài liệu dự án (file này)
├── bin/                            # File đã biên dịch — không sửa, Git thường bỏ qua
├── obj/                            # File tạm của trình biên dịch — không sửa, Git thường bỏ qua
├── appsettings.json                # Cấu hình hệ thống (Database, JWT, Log...)
├── appsettings.Development.json    # Cấu hình riêng môi trường Development (máy cá nhân)
├── Program.cs                      # Khởi tạo server, middleware, định nghĩa API
├── 1.first_learn.csproj            # Phiên bản .NET và các gói NuGet
└── 1.first_learn.http              # Gửi thử request API từ trong IDE (tùy chọn)
```

---

## 2. Giải thích chi tiết từng thành phần

### Program.cs (quan trọng nhất)

File chạy chính của ứng dụng. Toàn bộ cấu hình và các endpoint API tập trung tại đây. Code thường chia làm 3 bước:

1. **Khởi tạo Builder** — `var builder = WebApplication.CreateBuilder(args);`  
   Đăng ký các dịch vụ (Dependency Injection).

2. **Build ứng dụng** — `var app = builder.Build();`  
   Tạo đối tượng ứng dụng.

3. **Cấu hình endpoint / middleware** — ví dụ `app.MapGet("/weatherforecast", ...)`.  
   Khi frontend gọi URL này, code tại đây xử lý và trả về dữ liệu (thường là JSON).

### appsettings.json và appsettings.Development.json

Nơi lưu thông số cấu hình hệ thống (tương tự file `.env` ở nền tảng khác), định dạng JSON.

| File | Mục đích |
|------|----------|
| `appsettings.json` | Cấu hình chung: connection string, log, JWT... |
| `appsettings.Development.json` | Chỉ dùng khi chạy local (`Development`). Trên server production, file này không được ưu tiên. |

### Properties / launchSettings.json

Quyết định cách app khởi chạy trên máy local:

- **Cổng (port)** — ví dụ trong dự án này:
  - Profile `http`: chỉ `http://localhost:5091`
  - Profile `https`: `https://localhost:7074` và `http://localhost:5091`
- **Tự mở trình duyệt** khi `dotnet run` (`launchBrowser`)
- **Biến môi trường** — mặc định `ASPNETCORE_ENVIRONMENT=Development`

> Muốn dùng HTTPS (`https://localhost:7074`), chạy:  
> `dotnet watch --launch-profile https`

### 1.first_learn.csproj

File cấu hình dự án (XML), chứa:

- Phiên bản .NET: `<TargetFramework>net10.0</TargetFramework>`
- Danh sách gói NuGet (Entity Framework, OpenAPI, Redis...)

### Thư mục `bin/` và `obj/`

Do .NET tự sinh khi biên dịch:

| Thư mục | Nội dung |
|---------|----------|
| `obj/` | File tạm phục vụ quá trình build |
| `bin/` | File chạy cuối cùng (`.dll`, `.exe`) |

**Lưu ý:** Không sửa code trong hai thư mục này. Git thường ignore chúng khi push lên remote.

---

## 3. Các lệnh terminal cơ bản

| Lệnh | Mô tả |
|------|--------|
| `dotnet build` | Kiểm tra lỗi cú pháp và biên dịch dự án |
| `dotnet run` | Khởi động server Web API |
| `dotnet run --launch-profile https` | Chạy cả HTTP và HTTPS (cổng 5091 và 7074) |
| `dotnet watch` | Hot-reload: tự build lại khi lưu code (`Ctrl + S`) |
| `dotnet watch --launch-profile https` | Hot-reload kèm profile HTTPS |
