using Microsoft.AspNetCore.Identity;

namespace PROJECT_SEM3.Models
{
    public class Users : IdentityUser
    {
        public string FullName  { get; set; }
    }
}
