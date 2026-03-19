using System.ComponentModel.DataAnnotations;

namespace Admin.Dashboard.Models.Roles
{
    public class RoleViewModel
    {
        [Required(ErrorMessage ="Role Name is Reqired. ")]
        [StringLength(256,ErrorMessage ="Role Name size can't be more than 256 chars")]
        public string Name { get; set; }
    }
}
