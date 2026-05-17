# Cheat sheet: lệnh `dotnet` & snippet C# (VS Code)

Tài liệu tham khảo nhanh khi làm việc với .NET — từ tạo project, EF migration, đến snippet gõ nhanh class/property trong editor.

> **Phân biệt:** `dotnet ...` chạy trong **terminal**. `prop`, `ctor`… là **snippet** trong **VS Code / Visual Studio** (extension C#), không phải lệnh terminal.

---

## 1. Kiểm tra môi trường

| Lệnh | Mô tả |
|------|--------|
| `dotnet --version` | Phiên bản SDK đang dùng |
| `dotnet --list-sdks` | Danh sách SDK đã cài |
| `dotnet --list-runtimes` | Runtime đã cài |
| `dotnet --info` | Thông tin đầy đủ SDK + runtime |

---

## 2. Tạo project mới (`dotnet new`)

Xem template có sẵn:

```bash
dotnet new list
dotnet new list web
```

### Web API / ASP.NET Core

| Lệnh | Mô tả |
|------|--------|
| `dotnet new webapi -n TenProject` | Web API (minimal, có sẵn OpenAPI) |
| `dotnet new web -n TenProject` | Web app cơ bản |
| `dotnet new mvc -n TenProject` | ASP.NET Core MVC |
| `dotnet new blazor -n TenProject` | Blazor |

Ví dụ (giống project hiện tại):

```bash
cd d:\IT\25.asp_net
dotnet new webapi -n MyApi -f net10.0
cd MyApi
```

### Class library / console / test

| Lệnh | Mô tả |
|------|--------|
| `dotnet new classlib -n TenProject` | Thư viện class (service, domain…) |
| `dotnet new console -n TenProject` | App console |
| `dotnet new xunit -n TenProject.Tests` | Project test xUnit |
| `dotnet new nunit -n TenProject.Tests` | Project test NUnit |

### Tùy chọn hay dùng

| Flag | Ý nghĩa |
|------|---------|
| `-n`, `--name` | Tên project |
| `-o`, `--output` | Thư mục output |
| `-f`, `--framework` | `net10.0`, `net8.0`, `net48`… |
| `--no-restore` | Tạo project, chưa restore package |

```bash
dotnet new webapi -n ShopApi -f net10.0 -o ./src/ShopApi
```

---

## 3. Solution (nhiều project trong một repo)

| Lệnh | Mô tả |
|------|--------|
| `dotnet new sln -n TenSolution` | Tạo file `.sln` |
| `dotnet sln add ./src/MyApi/MyApi.csproj` | Thêm project vào solution |
| `dotnet sln remove ./src/MyApi/MyApi.csproj` | Gỡ project khỏi solution |
| `dotnet sln list` | Liệt kê project trong solution |

```bash
dotnet new sln -n MyApp
dotnet new webapi -n MyApp.Api -o src/MyApp.Api
dotnet new classlib -n MyApp.Core -o src/MyApp.Core
dotnet sln add src/MyApp.Api/MyApp.Api.csproj
dotnet sln add src/MyApp.Core/MyApp.Core.csproj
```

---

## 4. Restore / build / chạy

| Lệnh | Mô tả |
|------|--------|
| `dotnet restore` | Tải package NuGet (thường tự chạy khi build) |
| `dotnet build` | Biên dịch project |
| `dotnet build -c Release` | Build bản Release |
| `dotnet run` | Build + chạy |
| `dotnet run --launch-profile https` | Chạy profile HTTPS (xem `launchSettings.json`) |
| `dotnet watch` | Hot reload khi sửa file |
| `dotnet watch --launch-profile https` | Watch + profile HTTPS |
| `dotnet clean` | Xóa output trong `bin/`, `obj/` |
| `dotnet publish -c Release -o ./publish` | Đóng gói deploy |

**Output DLL (project Web API):**

```text
bin\Debug\net10.0\TenProject.dll
bin\Release\net10.0\TenProject.dll
```

---

## 5. Package NuGet

| Lệnh | Mô tả |
|------|--------|
| `dotnet add package TenPackage` | Thêm package vào project hiện tại |
| `dotnet add package TenPackage --version 8.0.0` | Chỉ định version |
| `dotnet remove package TenPackage` | Gỡ package |
| `dotnet list package` | Liệt kê package đã cài |
| `dotnet list package --outdated` | Package có bản mới |

Ví dụ thường dùng:

```bash
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Swashbuckle.AspNetCore
```

Thêm reference project khác:

```bash
dotnet add reference ../MyApp.Core/MyApp.Core.csproj
```

---

## 6. Entity Framework Core — migration & database

Cài **tool EF** (một lần trên máy):

```bash
dotnet tool install --global dotnet-ef
dotnet ef --version
```

Nếu đã cài, cập nhật:

```bash
dotnet tool update --global dotnet-ef
```

### Migration

| Lệnh | Mô tả |
|------|--------|
| `dotnet ef migrations add TenMigration` | Tạo migration mới |
| `dotnet ef migrations list` | Danh sách migration |
| `dotnet ef migrations remove` | Xóa migration cuối (chưa apply) |
| `dotnet ef database update` | Apply migration lên DB |
| `dotnet ef database update TenMigration` | Update tới migration cụ thể |
| `dotnet ef database drop` | Xóa database (cẩn thận) |

Chỉ định project / startup (khi solution nhiều project):

```bash
dotnet ef migrations add InitialCreate --project ./src/Infrastructure --startup-project ./src/Api
dotnet ef database update --project ./src/Infrastructure --startup-project ./src/Api
```

### DbContext & scaffold

| Lệnh | Mô tả |
|------|--------|
| `dotnet ef dbcontext scaffold "ConnectionString" Microsoft.EntityFrameworkCore.SqlServer -o Models` | Tạo model từ DB có sẵn |
| `dotnet ef dbcontext info` | Thông tin DbContext |

> Cần package: `Microsoft.EntityFrameworkCore.Design` (và provider SQL Server / PostgreSQL…).

---

## 7. User Secrets (config local, không commit)

| Lệnh | Mô tả |
|------|--------|
| `dotnet user-secrets init` | Bật user secrets cho project |
| `dotnet user-secrets set "ConnectionStrings:Default" "Server=..."` | Ghi secret |
| `dotnet user-secrets list` | Xem secrets |
| `dotnet user-secrets remove "Key"` | Xóa một key |
| `dotnet user-secrets clear` | Xóa hết |

---

## 8. HTTPS dev certificate

| Lệnh | Mô tả |
|------|--------|
| `dotnet dev-certs https --trust` | Tạo + tin cậy cert local HTTPS |
| `dotnet dev-certs https --check` | Kiểm tra cert |
| `dotnet dev-certs https --clean` | Xóa cert dev |

---

## 9. Tool global / local

| Lệnh | Mô tả |
|------|--------|
| `dotnet tool install -g dotnet-ef` | Cài tool global |
| `dotnet tool list -g` | Liệt kê tool global |
| `dotnet tool uninstall -g dotnet-ef` | Gỡ tool |
| `dotnet new tool-manifest` | Tạo manifest tool trong repo |
| `dotnet tool install dotnet-ef` | Cài tool local cho repo |

---

## 10. Test

| Lệnh | Mô tả |
|------|--------|
| `dotnet test` | Chạy toàn bộ test trong solution/project |
| `dotnet test --filter "FullyQualifiedName~MyClass"` | Lọc test |
| `dotnet test --logger "console;verbosity=detailed"` | Log chi tiết |

---

## 11. Khác

| Lệnh | Mô tả |
|------|--------|
| `dotnet format` | Format code (cần tool/style) |
| `dotnet workload list` | Liệt kê workload |
| `dotnet nuget locals all --clear` | Xóa cache NuGet (khi lỗi restore lạ) |

---

## 12. Snippet C# trong VS Code (không phải lệnh `dotnet`)

Cần extension **C#** / **C# Dev Kit**. Trong file `.cs`, gõ snippet rồi **Tab** hoặc **Enter**.

### Property & field

| Gõ | Sinh ra (ý tưởng) |
|----|-------------------|
| `prop` | `public int MyProperty { get; set; }` |
| `propfull` | Property + private backing field |
| `propg` | Property chỉ có `get` |
| `propn` | Property nullable |
| `field` | Private field |

### Class, ctor, method

| Gõ | Sinh ra (ý tưởng) |
|----|-------------------|
| `class` | Class skeleton |
| `ctor` | Constructor |
| `dtor` | Finalizer (hiếm dùng) |
| `iface` / `inter` | Interface |
| `enum` | Enum |
| `struct` | Struct |
| `record` | Record type |
| `method` | Method stub |
| `fm` | Main method (console) |

### Vòng lặp & điều kiện

| Gõ | Sinh ra |
|----|---------|
| `for` | Vòng `for` |
| `foreach` | Vòng `foreach` |
| `while` | Vòng `while` |
| `do` | Vòng `do-while` |
| `if` | `if` |
| `else` | `else` |
| `switch` | `switch` |
| `try` | `try-catch` |
| `tryf` | `try-finally` |

### Khác

| Gõ | Sinh ra |
|----|---------|
| `cw` | `Console.WriteLine()` |
| `using` | `using` directive |
| `namespace` | Namespace block |
| `throw` | `throw` |
| `await` | `await` (trong async method) |

### Snippet trong Visual Studio (tham khảo)

| Gõ | Tương tự |
|----|----------|
| `prop` | Auto-property |
| `propfull` | Property full |
| `ctor` | Constructor (trong class đã có field) |
| `mbox` | MessageBox (WinForms/WPF) |

---

## 13. Quy trình thường gặp (copy nhanh)

### Tạo Web API mới từ đầu

```bash
dotnet new webapi -n MyApi -f net10.0
cd MyApi
dotnet restore
dotnet build
dotnet watch --launch-profile https
```

### Thêm EF + migration SQL Server

```bash
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet tool install --global dotnet-ef

dotnet ef migrations add InitialCreate
dotnet ef database update
```

### Chạy test trước khi push

```bash
dotnet build -c Release
dotnet test
```

---

## 14. Liên kết tài liệu trong repo

| File | Nội dung |
|------|----------|
| [project.md](./project.md) | Cấu trúc thư mục project .NET |
| [equipvalen.md](./equipvalen.md) | So sánh NestJS ↔ .NET |
| [revit-vscode.md](./revit-vscode.md) | Revit 2024 + VS Code (add-in `net48`) |

---

## 15. Tham khảo chính thức

- [.NET CLI](https://learn.microsoft.com/dotnet/core/tools/)
- [`dotnet new` templates](https://learn.microsoft.com/dotnet/core/tools/dotnet-new)
- [EF Core tools](https://learn.microsoft.com/ef/core/cli/dotnet)
- [C# snippets VS Code](https://code.visualstudio.com/docs/csharp/refactoring)
