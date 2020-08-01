using System.Linq;
using System.Security.Claims;
using ChatApp.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ChatContext _context;

        public HomeController(ChatContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var user = User.FindFirst(ClaimTypes.Email).Value;
            return View(model: user);
        }

        [HttpPost]
        public IActionResult EnterRoom()
        {
            return RedirectToAction("Room", "Chat");
        }
    }
}
