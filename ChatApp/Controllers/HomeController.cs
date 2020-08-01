using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ChatApp.Models;
using ChattingApp.Data;
using Microsoft.AspNetCore.Authorization;
using ChattingApp.Data.Entities;

namespace ChatApp.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ChatContext _context;

        public HomeController(ILogger<HomeController> logger, ChatContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

       
        [HttpPost]
        public IActionResult Enter(string name)
        {
            var User = _context.Users.Where(user => user.Name == name);
            //if (!User.Any()) 
            //{ 
            //    var user = new User() { Name = name, Timestamp = DateTime.Now };
            //    _context.Users.Add(user);
            //    _context.SaveChanges();
            //    HttpContext.Session.SetString("UserName", name);
            //    HttpContext.Session.SetInt32("UserId", user.Id);
            //}
            //else
            //{
            //    var user = _context.Users.Where(user => user.Name == name).FirstOrDefault();

            //    HttpContext.Session.SetString("UserName", name);
            //    HttpContext.Session.SetInt32("UserId", user.Id);
            //}

            return RedirectToAction("Index", "Chat");
        }
       
        [HttpPost]
        public IActionResult EnterRoom(string name)
        {
            var user = _context.Users.FirstOrDefault(user => user.Name == name);
            if (user == null)
            {
                user = new User() { Name = name };
                _context.Users.Add(user);
                _context.SaveChanges();

            }

            HttpContext.Response.Cookies.Append("userName", user.Name);
            HttpContext.Response.Cookies.Append("userId", user.Id.ToString());
            return RedirectToAction("Room", "Chat");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
