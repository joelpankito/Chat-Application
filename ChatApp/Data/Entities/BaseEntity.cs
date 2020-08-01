using System;

namespace ChattingApp.Data.Entities
{
    public class BaseEntity
    {
        public int Id { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
