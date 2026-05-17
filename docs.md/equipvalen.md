# So sánh cấu trúc dự án: Node.js / NestJS ↔ .NET (C#)

Đọc theo từng mục: **bên trái = Node**, **bên phải = .NET**. Mỗi mục có phần *Giải thích* ngắn bên dưới.

---

## Bản đồ nhanh (nhìn một lần)

```text
                    NODE / NESTJS              │              .NET (C#)
───────────────────────────────────────────────┼──────────────────────────────────
Khai báo dự án     package.json                │  *.csproj
Gom nhiều project  pnpm-workspace / Lerna       │  *.sln / .slnx
Điểm vào app       main.ts / index.js          │  Program.cs
Biến môi trường    .env + .env.development     │  appsettings.json + Development.json
Chạy local         nodemon / package.json      │  launchSettings.json
Hot reload         nest --watch / nodemon      │  dotnet watch
Thư mục build      node_modules, dist          │  obj, bin  → .gitignore
```

---

## 1. Quản lý dependencies & thông tin dự án

| Node.js / NestJS | .NET (C#) |
|:-----------------|:----------|
| `package.json` | `Tên_Dự_Án.csproj` |

*Giải thích:* Khai báo thư viện cài thêm (**npm** vs **NuGet**) và phiên bản runtime (**Node v20** vs **.NET 10**).

---

## 2. Gom nhiều project (workspace / solution)

| Node.js / NestJS | .NET (C#) |
|:-----------------|:----------|
| `pnpm-workspace.yaml` hoặc **Lerna** | `Tên_Dự_Án.sln` hoặc `.slnx` |

*Giải thích:* Một repo chứa nhiều package/project (Web API, Core, Test…) chạy và build cùng nhau.

---

## 3. File chạy chính (entry point)

| Node.js / NestJS | .NET (C#) |
|:-----------------|:----------|
| `main.ts` (NestJS) hoặc `index.js` | `Program.cs` |

*Giải thích:* Khởi tạo app, cấu hình middleware (CORS, request), đăng ký services, bật server.

---

## 4. Cấu hình môi trường

| Node.js / NestJS | .NET (C#) |
|:-----------------|:----------|
| `.env` | `appsettings.json` |
| `.env.development` | `appsettings.Development.json` |

*Giải thích:* Connection string DB, JWT secret, port, key bí mật…

### `appsettings.Development.json` là gì?

File cấu hình **chỉ dùng khi dev trên máy local** — tương đương `.env.development` bên Node.

```text
appsettings.json                    ← cấu hình chung (mọi môi trường)
        +
appsettings.Development.json        ← ghi đè / bổ sung khi Development
        =
        cấu hình thực tế lúc dotnet run / dotnet watch
```

**Khi nào file này được load?**

Khi biến `ASPNETCORE_ENVIRONMENT=Development`. Trong project `1.first_learn`, `Properties/launchSettings.json` đặt sẵn giá trị này khi chạy profile `http` hoặc `https`.

| So sánh | Node | .NET |
|---------|------|------|
| File config chung | `.env` | `appsettings.json` |
| File config dev | `.env.development` | `appsettings.Development.json` |
| Biến chọn môi trường | `NODE_ENV=development` | `ASPNETCORE_ENVIRONMENT=Development` |

**Thường đặt gì trong `appsettings.Development.json`?**

| Mục đích | Ví dụ |
|----------|--------|
| Log chi tiết hơn | `"Default": "Debug"` |
| Database máy local | `"ConnectionStrings": { "Default": "..." }` |
| Bật Swagger / OpenAPI | chỉ khi dev |
| Key test (không phải production) | JWT dev, API sandbox |

**Production:** dùng `appsettings.Production.json` hoặc biến môi trường trên server — file `Development` **không** được merge khi `ASPNETCORE_ENVIRONMENT=Production`.

**Trong project hiện tại:** hai file gần giống nhau (chủ yếu `Logging`). Sau này thêm config dev vào `appsettings.Development.json`, giữ `appsettings.json` cho phần chung.

**Git / bảo mật**

- Commit `appsettings.Development.json` nếu chỉ có cấu hình dev an toàn (log, port…).
- **Không** commit password / API key production — dùng User Secrets local hoặc biến môi trường server.
- `appsettings.*.local.json` (nếu tạo) = cấu hình cá nhân, thường nằm trong `.gitignore`.

---

## 5. Cấu hình chạy local (dev)

| Node.js / NestJS | .NET (C#) |
|:-----------------|:----------|
| `nodemon.json` hoặc script trong `package.json` | `Properties/launchSettings.json` |

*Giải thích:* Chọn cổng, HTTP/HTTPS, biến môi trường khi dev trên máy cá nhân.

---

## 6. Hot reload (sửa code → tự chạy lại)

| Node.js / NestJS | .NET (C#) |
|:-----------------|:----------|
| `nest start --watch` hoặc `nodemon` | `dotnet watch` |

*Giải thích:* Theo dõi file thay đổi, build/run lại — không cần tắt terminal mỗi lần sửa.

---

## 7. Thư mục tạm / output sau build

| Node.js / NestJS | .NET (C#) |
|:-----------------|:----------|
| `node_modules/` · `dist/` | `obj/` · `bin/` |

*Giải thích:* Chứa package tải về và file sau compile. **Không commit** — thêm vào `.gitignore`.

---

## Gợi ý khi đọc trong VS Code

1. Mở **Preview** (`Ctrl + Shift + V`) — bảng 2 cột rộng hơn, dễ so sánh.
2. Muốn thu gọn: chỉ xem khối **Bản đồ nhanh** ở đầu.
3. Muốn chi tiết từng mục: cuộn xuống mục 1 → 7.
