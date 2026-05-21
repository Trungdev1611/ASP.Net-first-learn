using _1.first_learn.database;
using _1.first_learn.dtos;
using _1.first_learn.Extension;
using _1.first_learn.features.stocks;
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



builder.Services
.AddScoped<StockService>()
.AddCustomValidationErrorHandling() // 400: [Required], [Range]... trên DTO
.AddGlobalExceptionHandling()       // 404/400/500: exception từ Service
.AddControllersWithApiResponseWrapper(); // Ok(dto) → filter bọc { success, data }

//set DB cho postgres
// Trong file Program.cs của bạn:
builder.Services.AddDbContext<ApplicationDBContext>(options => 
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

var app = builder.Build();

// =========================================================================
// PHẦN 2: CONFIGURE MIDDLEWARE PIPELINE (Tương đương app.use() trong Express/NestJS)
// =========================================================================

// Bắt exception toàn cục — đặt sớm, trước MapControllers
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

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