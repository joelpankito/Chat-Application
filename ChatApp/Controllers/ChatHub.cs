using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using ChattingApp.Data;
using ChattingApp.Data.Entities;
using ChattingApp.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace ChattingApp.Controllers
{
    public class ChatHub : Hub
    {
        private readonly ChatContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly List<string> _users;

        public ChatHub(ChatContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _users = new List<string>();
        }

        public async Task SendMessage(string message, string id)
        {
            var identity = _httpContextAccessor.HttpContext.User;

            // Get the claims values
            var ID = identity.Claims.Where(c => c.Type == "ID")
                               .Select(c => c.Value).SingleOrDefault();
            var name = identity.Claims.Where(c => c.Type == ClaimTypes.Name)
                   .Select(c => c.Value).SingleOrDefault();
            var msg = new Message() { UserId = ID, Text = message, Timestamp = DateTime.Now };
            _context.Messages.Add(msg);
            _context.SaveChanges();
            var messages = _context.Messages
                                   .Include(x => x.User)
                                   .Where(x => x.Id == msg.Id)
                                   .Select(message =>
                                        new MessageViewModel()
                                        {
                                            Id = message.Id,
                                            Message = message.Text,
                                            User = message.User.Email,
                                            DateTimeString = message.Timestamp.ToString("MM ddd, yyyy hh:mm")
                                        }).ToList();
            var messagesVM = new MessagesViewModel
            {
                Messages = messages,
                LastMessageId = messages.Last().Id
            };

            await Clients.All.SendAsync("ReceiveMessage", messagesVM);
        }

        public async Task LoadMessages()
        {
            var messages = _context.Messages
                                   .Include(x => x.User)
                                   .Select(message =>
                                        new MessageViewModel()
                                        {
                                            Id = message.Id,
                                            Message = message.Text,
                                            User = message.User.Email,
                                            DateTimeString = message.Timestamp.ToString("MM ddd, yyyy hh:mm")
                                        }).ToList();

            var messagesVM = new MessagesViewModel
            {
                Messages = messages,
                LastMessageId = messages.Last().Id
            };

            await Clients.All.SendAsync("ReceiveMessage", messagesVM);
        }

        public async Task LoadUsers(string id)
        {
            var user = _context.Users.Where(x => x.Id == id).Select(x => new UserViewModel()
            {
                Id = x.Id,
                Name = x.Name
            });

            //await Clients.User.
            //_users.Add(user);
            var UsersVM = new UsersViewModel() { Users = user };

            await Clients.All.SendAsync("UserStatus", UsersVM, "active");
        }

        public override Task OnConnectedAsync()
        {
            _users.Add(Context.ConnectionId);
            return base.OnConnectedAsync();
        }
        public override Task OnDisconnectedAsync(Exception exception)
        {
            _users.Remove(Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }
    }
}
