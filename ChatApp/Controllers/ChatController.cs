using System;
using System.Collections.Generic;
using System.Linq;
using ChattingApp.Data;
using ChattingApp.Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChattingApp.Controllers
{
    [Route("[controller]/[action]")]
    public class ChatController : Controller
    {
        private readonly ChatContext _context;

        public  ChatController(ChatContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            List<User> lis = new List<User>();
            List<string> lisName = new List<string>();
            var messages = _context.Messages.Include(x => x.User).ToList();
            using (var db = _context)
            {
                foreach (var item in db.Users) 
                {
                    if (!lisName.Contains(item.Name))
                    {
                        lisName.Add(item.Name);
                        lis.Add(item);
                    }

                }
                ViewBag.allUsers = lis;
            }
            ViewBag.currentUser = HttpContext.Session.GetString("UserName");
            return View(messages);
        }

        public IActionResult Room()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Send(string message)
        {
            var mess = new Message() { UserId = HttpContext.Session.GetString("UserId"), Text = message, Timestamp = DateTime.Now };
            _context.Messages.Add(mess);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        

    }
}