using System.ComponentModel.DataAnnotations;

namespace LAB1.Shared
{
    public class UzytkownikWprowadzony
    {
        [Required(ErrorMessage = "WPROWADŹ LOGIN")]
        public string Login { get; set; } = "";
        [Required(ErrorMessage = "WPROWADŹ HASŁO")]
        public string Haslo { get; set; } = "";
    }
}