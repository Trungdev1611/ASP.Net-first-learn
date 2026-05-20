using _1.first_learn.database;
using _1.first_learn.dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection; // 1. Đảm bảo có dòng import này để dùng swagger
var builder = WebApplication.CreateBuilder(args);

// =========================================================================
// PHẦN 1: REGISTER SERVICES (Tương đương khai báo Providers trong NestJS Module)
// =========================================================================

// Giúp .NET hiểu và quét được các Endpoint dạng Minimal API để làm tài liệu
builder.Services.AddEndpointsApiExplorer();

// Kích hoạt bộ sinh tài liệu Swagger
builder.Services.AddSwaggerGen();

//update controller
builder.Services.AddControllers(); // Đăng ký các controller để .NET có thể tìm thấy và


//set DB cho postgres
// Trong file Program.cs của bạn:
builder.Services.AddDbContext<ApplicationDBContext>(options => 
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

var app = builder.Build();

app.MapControllers();
// =========================================================================
// PHẦN 2: CONFIGURE MIDDLEWARE PIPELINE (Tương đương app.use() trong Express/NestJS)
// =========================================================================

// Chỉ bật tài liệu API khi đang phát triển (Development) để tránh lộ API ở Production
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();   // Tạo ra endpoint chứa file json đặc tả (mặc định: /swagger/v1/swagger.json)
    app.UseSwaggerUI(); // Bật giao diện UI trực quan đồ họa (mặc định: /swagger)
}

// Tự động chuyển hướng các request HTTP thường sang HTTPS bảo mật
app.UseHttpsRedirection();

// =========================================================================
// PHẦN 3: MAP ENDPOINTS (Tương đương app.get() hay app.post() trong NodeJS)
// =========================================================================


List<GameDto> games = [
    new GameDto(1, "test", "Fighting",19.99M, new DateOnly(1992,7,15)),
    new GameDto(1, "test", "Fighting",19.99M, new DateOnly(1992,7,15))
];

app.MapGet("/", () =>games)
   .WithTags("Home")        // Gom endpoint này vào nhóm lớn tên là "Home" trên giao diện Swagger
   .WithName("GetHomeInfo") // Đặt tên định danh duy nhất cho endpoint này (đã thêm dấu chấm phẩy chuẩn)
    .WithSummary("Hàm lấy thông tin trang chủ");
// Kích hoạt server lắng nghe các cổng cấu hình
app.Run();