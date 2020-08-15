using System.Collections.Generic;

namespace ChattingApp.ViewModels
{
    public class MessagesViewModel
    {
        public IEnumerable<MessageViewModel> Messages { get; set; }
        public int LastMessageId { get; set; }
        public string SendMsgStatus { get; set; }
    }
}
