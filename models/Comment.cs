using System.Text.Json.Serialization;

namespace _1.first_learn.models;

public class Comment
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedOn { get; set; } = DateTime.Now;

    public int? StockId { get; set; }

    // Quan hệ ngược về Stock — khi serialize Comment đừng trả stock (tránh null / vòng lặp)
    [JsonIgnore]
    public Stock? Stock { get; set; }
}