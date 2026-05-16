Quá hợp lý bạn ơi! Lưu lại thành một file Markdown (`.md`) cẩn thận như thế này là chuẩn phong cách làm việc của một Developer thực thụ. Khi lên máy công ty hoặc đổi máy mới, bạn chỉ cần mở file này ra, copy-paste lệnh và cấu hình là chạy ngay, không sợ thiếu sót gì.

Tôi đã tối ưu và viết lại quy trình này một cách sạch sẽ, tường minh nhất theo chuẩn Markdown để bạn tiện copy trực tiếp vào file `setupvscode-addins-gemini.md` nhé:

---

```markdown
# Hướng dẫn Setup VS Code Phát triển Revit 2024 Add-in (C#)

Tài liệu hướng dẫn từng bước cấu hình VS Code/Cursor để thay thế Visual Studio trong việc lập trình, build và debug Add-in dành cho **Revit 2024** (Môi trường .NET Framework 4.8).

---

## 1. Các phần mềm cần cài đặt trước (Prerequisites)

Trước khi cấu hình trong VS Code, máy tính công ty cần được cài đặt các thành phần sau:
* **Autodesk Revit 2024** (Dĩ nhiên rồi).
* **VS Code** hoặc **Cursor**.
* **.NET SDK (6.0 / 8.0 trở lên):** Để sử dụng các câu lệnh `dotnet build`, `dotnet new` trong Terminal.
* **.NET Framework 4.8 Developer Pack:** 
  * *Bắt buộc phải là bản Developer Pack (không phải bản Runtime).*
  * Link tải chính thức từ Microsoft: [Download .NET Framework 4.8 Developer Pack](https://dotnet.microsoft.com/download/dotnet-framework/net48)

---

## 2. Các Extension cần cài trên VS Code

Mở VS Code, vào mục Extensions (`Ctrl + Shift + X`) và cài đặt các extension sau:

| Tên Extension | ID | Mục đích |
|---|---|---|
| **C#** | `ms-dotnettools.csharp` | Cung cấp IntelliSense (gợi ý code) và hỗ trợ bộ Debugger **`clr`** (bắt buộc cho Revit 2024). |
| **C# Dev Kit** | `ms-dotnettools.csdevkit` | Hiển thị cấu trúc thư mục dạng Solution Explorer giống Visual Studio (Tùy chọn - Lưu ý vấn đề bản quyền nếu dùng trong doanh nghiệp lớn). |

---

## 3. Cấu trúc thư mục Dự án (Project)

Tạo một thư mục trống cho dự án (ví dụ: `MyRevit2024Addin`). Cấu trúc các file cần có sẽ như sau:

```text
MyRevit2024Addin/
├── .vscode/
│   ├── launch.json
│   └── tasks.json
├── MyRevit2024Addin.csproj
├── MyRevit2024Addin.addin
└── HelloCommand.cs

```

---

## 4. Chi tiết Nội dung các File Cấu hình

Hãy tạo các file tương ứng và copy toàn bộ nội dung cấu hình chuẩn dưới đây:

### 4.1. File `MyRevit2024Addin.csproj`

File này dùng cấu trúc SDK-style mới nhưng đích đến (Target) vẫn build ra `.NET Framework 4.8`. Khi build thành công, nó sẽ tự động copy file sản phẩm vào thư mục Addins của Revit.

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

  <!-- Đường dẫn mặc định của Revit 2024 -->
  <PropertyGroup>
    <RevitPath>C:\Program Files\Autodesk\Revit 2024</RevitPath>
    <RevitAddinsPath>C:\ProgramData\Autodesk\Revit\Addins\2024</RevitAddinsPath>
  </PropertyGroup>

  <!-- Nhúng thư viện Revit API (Không copy vào thư mục output khi build) -->
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

  <!-- Tự động deploy sang thư mục Revit Addins sau khi build xong ở chế độ Debug -->
  <Target AfterTargets="Build" Condition="'$(Configuration)' == 'Debug'" Name="DeployAddin">
    <Copy DestinationFolder="$(RevitAddinsPath)" SourceFiles="$(TargetPath)"/>
    <Copy DestinationFolder="$(RevitAddinsPath)" SourceFiles="$(ProjectDir)$(ProjectName).addin"/>
  </Target>
</Project>

```

### 4.2. File `MyRevit2024Addin.addin`

Tấm "thẻ bài" để khai báo thông tin Add-in với Revit.
*(Mẹo tạo GUID nhanh bằng PowerShell: `[guid]::NewGuid().Guid` và dán vào thẻ AddInId)*

```xml
<?xml version="1.0" encoding="utf-8" standalone="no"?>
<RevitAddIns>
  <AddIn Type="Command">
    <Name>Hello VS Code</Name>
    <Assembly>C:\ProgramData\Autodesk\Revit\Addins\2024\MyRevit2024Addin.dll</Assembly>
    <AddInId>b2c3d4e5-f6a7-8901-bcde-f12345678901</AddInId>
    <FullClassName>MyRevit2024Addin.HelloCommand</FullClassName>
    <Text>Hello From Gemini</Text>
    <Description>Test add-in Revit 2024 + VS Code</Description>
    <VendorId>DEV</VendorId>
    <VendorDescription>Local Dev Environment</VendorDescription>
  </AddIn>
</RevitAddIns>

```

### 4.3. File code mẫu `HelloCommand.cs`

Đoạn code C# đơn giản tạo ra một hộp thoại thông báo trong Revit.

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
        // Hiển thị một hộp thoại thông báo trong Revit
        TaskDialog.Show("VS Code Debugger", "Xin chào! Bạn đã setup thành công Revit 2024 Add-in trên VS Code!");
        return Result.Succeeded;
    }
}

```

### 4.4. File `.vscode/tasks.json`

Định nghĩa hành động build dự án bằng lệnh `dotnet`.

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
  }
}

```

### 4.5. File `.vscode/launch.json`

Cấu hình Debugger đính kèm (`attach`) vào tiến trình phần mềm Revit. **Lưu ý: type phải là `clr**`, không dùng `coreclr` (chỉ dành cho Revit 2025 trở lên).

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

---

## 5. Quy trình Phát triển và Debug (Workflow chuẩn)

Mỗi lần code hoặc kiểm tra lỗi, hãy tuân thủ nghiêm ngặt 5 bước sau:

* **Bước 1: Biên dịch code (Build)**
Mở Terminal của VS Code gõ: `dotnet build`. Hệ thống sẽ tự biên dịch và copy file `.dll` cùng `.addin` vào thư mục của Revit.
* **Bước 2: Mở phần mềm Revit**
Bật Revit 2024 lên (Mở một dự án trống hoặc một file `.rvt` bất kỳ). Nếu có hộp thoại bảo mật xuất hiện ➡️ Chọn **Always Load**.
* **Bước 3: Đặt Breakpoint và Đính kèm Debugger**
Quay lại VS Code, click chuột đặt dấu chấm đỏ (Breakpoint) ở dòng muốn kiểm tra trong file `HelloCommand.cs`. Nhấn nút **F5** (hoặc chọn mục Run & Debug -> Nhấn nút Play xanh **Attach to Revit 2024**).
* **Bước 4: Chạy lệnh trên Revit**
Trên thanh Menu của Revit ➡️ Vào tab **Add-Ins** ➡️ Bấm nút **Hello From Gemini**. Ngay lập tức Revit sẽ đóng băng và VS Code sẽ sáng lên tại dòng code bạn đặt Breakpoint.

---

## 6. Quy tắc Bắt buộc khi SỬA CODE (Hot-reload hạn chế)

Vì Revit 2024 chạy trên nền tảng cũ `.NET Framework 4.8`, khi phần mềm Revit đã chạy, nó sẽ **KHÓA CHẶT** file `.dll` của bạn trong bộ nhớ, bạn không thể build đè file mới lên được. Muốn sửa code, hãy làm theo chu kỳ:

1. **Ngắt Debug:** Nhấn cụm phím `Shift + F5` trong VS Code.
2. **Tắt hẳn phần mềm Revit.**
3. **Sửa code** trong VS Code/Cursor theo ý muốn.
4. **Chạy lại lệnh:** `dotnet build` trong Terminal để cập nhật code mới.
5. **Mở lại Revit** ➡️ Nhấn **F5** để Attach lại từ đầu.

```

***

File cấu hình này mình đã lược bỏ tối đa những thứ rườm rà của các template tự động, giữ lại đúng cấu trúc chuẩn thô sơ nhất để bạn dễ kiểm soát, đảm bảo mang lên máy công ty làm phát ăn ngay! Bạn copy vào file `.md` của mình nhé.

```