using System.ComponentModel.DataAnnotations;

namespace LAB1.Server
{
    public class User
    {
        [Key]
        public int? UserId { get; set; }
        public string? Login { get; set; }
        public byte[]? PasswordHash { get; set; }
        public byte[]? PasswordSalt { get; set; }
        public DateTime? AddTime { get; set; }
    }
}