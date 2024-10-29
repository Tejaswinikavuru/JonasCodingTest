using System;

namespace DataAccessLayer.Model.Models
{
    public class DbResultInfo
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
