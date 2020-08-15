using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ChatApp.Data;
using ChattingApp.Data.Entities;
using ChattingApp.ViewModels;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace ChattingApp.Controllers
{
    public class ChatHub : Hub
    {
        private readonly ChatContext _context;
        private readonly static List<string> _users = new List<string>();

        public ChatHub(ChatContext context)
        {
            _context = context;
        }

        public async Task SendMessage(string message)
        {
            var user = _context.Users.FirstOrDefault(e => e.Email == Context.User.FindFirst(ClaimTypes.Email).Value);
            var msg = new Message { UserId = user.Id, Text = message, Timestamp = DateTime.Now };
            _context.Messages.Add(msg);
            _context.SaveChanges();

            var messages = _context.Messages
                                   .Include(x => x.User)
                                   .Where(x => x.Id == msg.Id)
                                   .Select(message =>
                                        new MessageViewModel
                                        {
                                            Id = message.Id,
                                            Message = message.Text,
                                            User = message.User.Email,
                                            DateTimeString = message.Timestamp.ToString("MM ddd, yyyy hh:mm")
                                        }).ToList();
            var senderMessagesVM = new MessagesViewModel
            {
                Messages = messages,
                LastMessageId = messages.Count == 0 ? 0 : messages.Last().Id,
                SendMsgStatus = "sent"
            };

            var messagesVM = new MessagesViewModel
            {
                Messages = messages,
                LastMessageId = messages.Count == 0 ? 0 : messages.Last().Id,
                SendMsgStatus = "receved"
            };

            await Clients.Caller.SendAsync("ReceiveMessage", senderMessagesVM);
            await Clients.Others.SendAsync("ReceiveMessage", messagesVM);

        }

        public async Task LoadMessages()
        {
            var messages = _context.Messages
                                   .Include(x => x.User)
                                   .Select(message =>
                                        new MessageViewModel
                                        {
                                            Id = message.Id,
                                            Message = message.Text,
                                            User = message.User.Email,
                                            DateTimeString = message.Timestamp.ToString("MM ddd, yyyy hh:mm"),
                                            LoadMsgStatus = Status(message.User.Email)
                                        }).ToList();


            var messagesVM = new MessagesViewModel
            {
                Messages = messages,
                LastMessageId = messages.Count == 0 ? 0 : messages.Last().Id
            };

            await Clients.Caller.SendAsync("ReceiveMessage", messagesVM);
            
        }

        public async Task LoadUsers()
        {
            await Clients.All.SendAsync("DisplayUsers", _users);
        }

        public override async Task OnConnectedAsync()
        {
            var user = Context.User.FindFirst(ClaimTypes.Email).Value;
            _users.Add(user);
            
            await base.OnConnectedAsync();
        }
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var user = Context.User.FindFirst(ClaimTypes.Email).Value;
            _users.Remove(user);

            await LoadUsers();

            await base.OnDisconnectedAsync(exception);
        }

        private string Status(string user)
        {
            if (user == Context.User.FindFirst(ClaimTypes.Email).Value)
            {
                return "sent";
            }
            else
            {
                return "recived";
            }
            
            
        }


    }
}
