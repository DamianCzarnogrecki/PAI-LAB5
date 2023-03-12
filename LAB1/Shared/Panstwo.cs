using System.ComponentModel.DataAnnotations;

namespace LAB1.Shared
{
    public class Panstwo
    {
        [Required(ErrorMessage = "UZUPEŁNIENIE NUMERU IDENTYFIKACYJNEGO JEST KONIECZNE")]
        public int ID { get; set; }
        [Required(ErrorMessage = "UZUPEŁNIENIE NAZWY POWSZECHNEJ JEST KONIECZNE")]
        public string NazwaPowszechna { get; set; }
        [Required(ErrorMessage = "UZUPEŁNIENIE NAZWY OFICJALNEJ JEST KONIECZNE")]
        public string NazwaOficjalna { get; set; }
        [Required(ErrorMessage = "UZUPEŁNIENIE KODU JEST KONIECZNE")]
        public string KodISO3166 { get; set; }
        public TypRzadu? TypRzadu { get; set; }
        public int TypRzaduID { get; set; }
        [Required(ErrorMessage = "UZUPEŁNIENIE LICZBY OBYWATELI JEST KONIECZNE")]
        public int LiczbaObywateli { get; set; }
        [Required(ErrorMessage = "UZUPEŁNIENIE HDI JEST KONIECZNE")]
        public double HDI { get; set; }
        [Required(ErrorMessage = "UZUPEŁNIENIE ADRESU FLAGI JEST KONIECZNE")]
        public string URLflagi { get; set; }
    }
}