# Kết nối Revit 2024 (BIM) với VS Code — thay Visual Studio

Hướng dẫn phát triển và debug **Revit API add-in (C#)** trên **Revit 2024** bằng **VS Code**.  
Workflow tương tự trên **Cursor** (cùng thư mục `.vscode/`).

| Thông số máy bạn | Giá trị |
|------------------|---------|
| Revit | **2024** |
| Framework add-in | **.NET Framework 4.8** (`net48`) |
| Debugger VS Code | `"type": "clr"` (không dùng `coreclr`) |

> Plugin chạy **trong `Revit.exe`**. VS Code **không** F5 để mở Revit — bạn **mở Revit trước**, rồi **Attach** debugger.

---

## 1. Tổng quan luồng làm việc

```text
[VS Code]                    [Windows]                              [Revit 2024]
    |                            |                                       |
    |-- dotnet build (x64) ---->| copy DLL + .addin → Addins\2024\     |
    |                            |                                       |
    |                            |<------------- mở Revit.exe -----------|
    |                            |              (load add-in)          |
    |-- F5 Attach (clr) -------->| gắn debugger vào Revit.exe          |
    |                            |                                       |
    |<-- breakpoint khi chạy lệnh add-in trong Revit -------------------|
```

| Bước | Việc cần làm |
|------|----------------|
| 1 | Build project → `.dll` (net48, x64) |
| 2 | Đặt `.addin` + `.dll` vào thư mục Addins **2024** |
| 3 | Mở Revit 2024 |
| 4 | VS Code: **Attach to Revit** (`clr`) |
| 5 | Trong Revit: chạy lệnh add-in → breakpoint |

---

## 2. Phần mềm cần có

| Thành phần | Ghi chú |
|------------|---------|
| **Autodesk Revit 2024** | Đã cài |
| **VS Code** + extension C# / C# Dev Kit | Đã cài |
| **.NET Framework 4.8 Developer Pack** | Bắt buộc để build `net48` — [tải từ Microsoft](https://dotnet.microsoft.com/download/dotnet-framework/net48) |
| **.NET SDK** (6/8…) | Dùng lệnh `dotnet build` cho project SDK-style `net48` |

Build **bắt buộc** **x64**:

```bash
dotnet build -c Debug -p:Platform=x64
```

Output thường nằm tại: `bin\x64\Debug\net48\` hoặc `bin\Debug\net48\` (tùy cấu hình `.csproj`).

---

## 3. Đường dẫn cố định — Revit 2024

| Mục đích | Đường dẫn |
|----------|-----------|
| Cài đặt Revit | `C:\Program Files\Autodesk\Revit 2024\` |
| RevitAPI | `C:\Program Files\Autodesk\Revit 2024\RevitAPI.dll` |
| RevitAPIUI | `C:\Program Files\Autodesk\Revit 2024\RevitAPIUI.dll` |
| Add-in (khuyên dùng khi dev) | `C:\ProgramData\Autodesk\Revit\Addins\2024\` |
| Add-in user | `%APPDATA%\Autodesk\Revit\Addins\2024\` |
| Journal (log lỗi) | `%LOCALAPPDATA%\Autodesk\Revit\Autodesk Revit 2024\Journals\` |

Nếu Revit cài ổ khác, chỉnh `RevitPath` trong `.csproj` cho đúng.

---

## 4. Extension VS Code

| Extension | ID | Mục đích |
|-----------|-----|----------|
| **C#** | `ms-dotnettools.csharp` | Build, IntelliSense, debug **clr** |
| **C# Dev Kit** | `ms-dotnettools.csdevkit` | Solution explorer (tùy chọn) |
| **.NET Install Tool** | `ms-dotnettools.vscode-dotnet-runtime` | Runtime gợi ý |

Revit 2024 dùng **.NET Framework** → trong `launch.json` phải là **`"type": "clr"`**, không phải `coreclr` (`coreclr` chỉ cho Revit 2025+ / .NET 8).

---

## 5. Tạo project add-in (Revit 2024)

### Bước 1 — Tạo thư mục và mở VS Code

```bash
mkdir MyRevit2024Addin
cd MyRevit2024Addin
code .
```

### Bước 2 — File `MyRevit2024Addin.csproj`

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net48</TargetFramework>
    <PlatformTarget>x64</PlatformTarget>
    <LangVersion>latest</LangVersion>
    <Nullable>enable</Nullable>
    <Configurations>Debug;Release</Configurations>
    <Platforms>x64</Platforms>
  </PropertyGroup>

  <PropertyGroup>
    <RevitPath>C:\Program Files\Autodesk\Revit 2024</RevitPath>
    <RevitAddinsPath>C:\ProgramData\Autodesk\Revit\Addins\2024</RevitAddinsPath>
  </PropertyGroup>

  <ItemGroup>
    <Reference Include="RevitAPI">
      <HintPath>$(RevitPath)\RevitAPI.dll</HintPath>
      <Private>false</Private>
    </Reference>
    <Reference Include="RevitAPIUI">
      <HintPath>$(RevitPath)\RevitAPIUI.dll</HintPath>
      <Private>false</Private>
    </Reference>
  </ItemGroup>

  <Target Name="DeployAddin" AfterTargets="Build" Condition="'$(Configuration)' == 'Debug'">
    <Copy SourceFiles="$(TargetPath)" DestinationFolder="$(RevitAddinsPath)" />
    <Copy SourceFiles="MyRevit2024Addin.addin" DestinationFolder="$(RevitAddinsPath)" />
  </Target>
</Project>
```

### Bước 3 — Lệnh mẫu `HelloCommand.cs`

```csharp
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace MyRevit2024Addin;

[Transaction(TransactionMode.Manual)]
public class HelloCommand : IExternalCommand
{
    public Result Execute(
        ExternalCommandData commandData,
        ref string message,
        ElementSet elements)
    {
        TaskDialog.Show("VS Code", "Hello from Revit 2024 add-in!");
        return Result.Succeeded;
    }
}
```

### Bước 4 — File `MyRevit2024Addin.addin`

Sau build lần đầu, sửa `Assembly` trỏ đúng DLL (hoặc dùng path cố định trong Addins):

```xml
<?xml version="1.0" encoding="utf-8" standalone="no"?>
<RevitAddIns>
  <AddIn Type="Command">
    <Name>Hello VS Code</Name>
    <Assembly>C:\ProgramData\Autodesk\Revit\Addins\2024\MyRevit2024Addin.dll</Assembly>
    <AddInId>b2c3d4e5-f6a7-8901-bcde-f12345678901</AddInId>
    <FullClassName>MyRevit2024Addin.HelloCommand</FullClassName>
    <Text>Hello</Text>
    <Description>Test add-in Revit 2024 + VS Code</Description>
    <VendorId>DEV</VendorId>
    <VendorDescription>Local dev</VendorDescription>
  </AddIn>
</RevitAddIns>
```

Đổi `AddInId` thành GUID mới nếu trùng add-in khác: PowerShell → `[guid]::NewGuid()`.

> Template Autodesk `revit-cursor-plugin-debug` hướng **Revit 2025+ / .NET 8** — **không** dùng trực tiếp cho 2024; tự tạo project `net48` như trên.

---

## 6. Cấu hình VS Code

### `.vscode/launch.json` (Revit 2024)

```json
{
  "version": "0.2.0",
  "configurations": [
    {
      "name": "Attach to Revit 2024",
      "type": "clr",
      "request": "attach",
      "processName": "Revit.exe",
      "preLaunchTask": "build-revit-addin"
    }
  ]
}
```

### `.vscode/tasks.json`

```json
{
  "version": "2.0.0",
  "tasks": [
    {
      "label": "build-revit-addin",
      "command": "dotnet",
      "type": "process",
      "args": ["build", "-c", "Debug", "-p:Platform=x64"],
      "group": { "kind": "build", "isDefault": true },
      "problemMatcher": "$msCompile"
    }
  ]
}
```

---

## 7. Connect & debug — từng bước (Revit 2024)

1. **Build**
   ```bash
   dotnet build -c Debug -p:Platform=x64
   ```
2. Kiểm tra `MyRevit2024Addin.dll` và `.addin` trong  
   `C:\ProgramData\Autodesk\Revit\Addins\2024\`
3. **Mở Revit 2024** (file `.rvt` bất kỳ hoặc trống).
4. VS Code: breakpoint trong `Execute` → **F5** → **Attach to Revit 2024** → chọn `Revit.exe`.
5. Revit: tab **Add-Ins** → chạy lệnh **Hello** (tên trong `.addin`).

**Sau khi sửa code:** Shift+F5 → build lại → **đóng và mở lại Revit 2024** (DLL bị lock) → F5 attach lại.

---

## 8. Xử lý lỗi (Revit 2024)

| Triệu chứng | Cách xử lý |
|-------------|------------|
| Không attach được | Revit đang chạy; `type` phải là **`clr`**; thử VS Code **Run as administrator** |
| Breakpoint rỗng | Build Debug; path `.addin` trùng DLL vừa build; attach sau khi Revit mở xong |
| Load add-in lỗi | Đọc Journal mới nhất; kiểm tra `FullClassName`, GUID `AddInId` |
| Could not load assembly | Project phải **net48** + **x64**; không copy `RevitAPI.dll` vào output (`Private=false`) |
| Dùng `coreclr` by mistake | Chỉ dành Revit 2025+ — 2024 phải **`clr`** |
| Build net48 lỗi | Cài **.NET Framework 4.8 Developer Pack** |

---

## 9. Khi nâng cấp lên Revit 2025+

| | Revit 2024 | Revit 2025+ |
|---|------------|-------------|
| Framework | .NET Framework **4.8** | **.NET 8** |
| `launch.json` | `"type": "clr"` | `"type": "coreclr"` |
| Addins folder | `Addins\2024` | `Addins\2025` |

Add-in .NET 8 **không** chạy trên Revit 2024 — cần project riêng hoặc migrate khi đổi phiên bản Revit.

---

## 10. pyRevit (Python)

Nếu dùng **pyRevit** thay C#: extension **pyRevit with VSCode**, không cần `.addin` C#. Doc này cho **Revit API C# / net48**.

---

## 11. Tài liệu tham khảo

| Nguồn | Link |
|-------|------|
| Revit API Guide 2024 | https://help.autodesk.com/view/RVT/2024/ENU/?guid=Revit_API_Revit_API_Developers_Guide_html |
| Diễn đàn Revit API | https://forums.autodesk.com/t5/revit-api-forum/bd-p/160 |
| Debug VS Code | https://forums.autodesk.com/t5/revit-api-forum/debug-vscode/td-p/11492664 |
| .NET 8 migration (2025+) | https://forums.autodesk.com/t5/revit-api-forum/net-8-migration/td-p/13301211 |

---

## 12. Checklist Revit 2024

- [ ] Đã cài **.NET Framework 4.8 Developer Pack**
- [ ] Project `net48`, `PlatformTarget` / build **x64**
- [ ] Reference `RevitAPI.dll` từ `Revit 2024\`
- [ ] `.addin` + `.dll` trong `C:\ProgramData\Autodesk\Revit\Addins\2024\`
- [ ] `launch.json`: **`"type": "clr"`**
- [ ] Mở Revit 2024 → F5 Attach → chạy lệnh add-in
- [ ] Lỗi: xem **Journal** trong `Autodesk Revit 2024\Journals\`

**Connect** = đăng ký `.addin` + **Attach** debugger `clr` vào `Revit.exe` — không có kết nối riêng giữa VS Code và Revit.
