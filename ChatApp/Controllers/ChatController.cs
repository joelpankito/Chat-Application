using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Controllers
{
    public class ChatController : Controller
    {
        public IActionResult Room()
        {
            return View();
        }
    }
}
