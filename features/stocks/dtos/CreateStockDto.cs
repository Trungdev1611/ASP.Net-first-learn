using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace _1.first_learn.features.stocks.dtos;

public class CreateStockDto
{
    [Required(ErrorMessage = "Mã cổ phiếu không được để trống")]
    [MinLength(1, ErrorMessage = "Mã cổ phiếu phải có ít nhất 1 ký tự")]
    [MaxLength(5, ErrorMessage = "Mã cổ phiếu không được vượt quá 5 ký tự")]
    public string Symbol { get; set; } = string.Empty;

    [Required(ErrorMessage = "Tên công ty không được để trống")]
    [MaxLength(50, ErrorMessage = "Tên công ty tối đa 50 ký tự")]
    public string CompanyName { get; set; } = string.Empty;

    [Required]
    public decimal Purchase { get; set; }
    [Required]
    public decimal Divdent { get; set; }
    [Required(ErrorMessage = "Vui lòng nhập tên ngành công nghiệp")]
    public string Industry { get; set; } = string.Empty;

    [Required]
    [Range(1, long.MaxValue, ErrorMessage = "Vốn hóa thị trường phải lớn hơn 0")]
    public long Marketcap { get; set; }
}