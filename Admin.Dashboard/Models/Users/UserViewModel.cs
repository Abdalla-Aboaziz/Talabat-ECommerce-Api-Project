using System.ComponentModel.DataAnnotations;

namespace Admin.Dashboard.Models.Users
{
    public class UserViewModel
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }

        [Display(Name ="Display Name")]
        public string DisplayName { get; set; }
        public IEnumerable<string> Roles { get; set; }
    }
}
