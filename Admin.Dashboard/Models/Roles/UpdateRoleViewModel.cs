using System.ComponentModel.DataAnnotations;
using System.Security.Principal;

namespace Admin.Dashboard.Models.Roles
{
    public class UpdateRoleViewModel
    {
        public string Id { get; set; }
      
        public string Name { get; set; }

        public bool IsSelected { get; set; }
    }
}
