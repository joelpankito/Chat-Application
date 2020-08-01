using System.ComponentModel.DataAnnotations.Schema;

namespace ChattingApp.Data.Entities
{
    public class Message : BaseEntity
    {
        public string Text { get; set; }
        [ForeignKey(nameof(User))]
        public string UserId { get; set; }
        public User User { get; set; }
    }
}
