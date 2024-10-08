using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace TWAB.Models
{
    public class OfertyPracyUserViewModel
    {
        public int Id { get; set; }
        public string Status { get; set; }
        public string Tytuł { get; set; }
        public string Kategoria { get; set; }
        public string DataWaznosci { get; set; }
        public string Wynagrodzenie { get; set; }
        public string WymiarPracy { get; set; }
        public string RodzajUmowy { get; set; }
        public string NazwaFirmy { get; set; }
        public string LogoFirmy { get; set; }
        public string Wojewodztwo { get; set; }
        public string Miasto { get; set; }
    }
}
