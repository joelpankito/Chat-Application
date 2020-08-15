using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ChatApp.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;

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

        
        public IActionResult OnPostUpload(IFormFile files)
        {
            long size = files.Length;
            var fileName = Path.GetFileName(files.FileName);

            //Assigning Unique Filename (Guid)
            var myUniqueFileName = Convert.ToString(Guid.NewGuid());

            //Getting file Extension
            var fileExtension = Path.GetExtension(fileName);

            // concatenating  FileName + FileExtension
            var newFileName = String.Concat(myUniqueFileName, fileExtension);
            
            // full path to file in temp location
            var filePath = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images")).Root + $@"\{newFileName}"; //we are using Temp file name just for the example. Add your own file path.


            using (FileStream fs = System.IO.File.Create(filePath))
            {
                files.CopyTo(fs);
                fs.Flush();
            }


            // process uploaded files
            // Don't rely on or trust the FileName property without validation.

            return Ok(new { count = files.Length, size, filePath });
        }
        
    }
}
