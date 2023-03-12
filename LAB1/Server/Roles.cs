using System.ComponentModel.DataAnnotations;

namespace LAB1.Shared
{
    public class Roles
    {
        [Key]
        public int RoleId { get; set; }
        public string? RoleName { get; set; }
    }
}