using System;

namespace BusinessLayer.Model.Models
{
    public class ResultInfo
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
