using Microsoft.AspNetCore.Identity;

namespace ChattingApp.Data.Entities
{
    public class User : IdentityUser
    {
        public string Name { get; set; }
    }
}
