namespace TWAB.Models
{
    public class SzczegolyOfertyViewModel
    {
        public int Id { get; set; }
        public string IdRekrutera { get; set; }
        public string Tytul { get; set; }
        public string Opis { get; set; }
        public string Kategoria { get; set; }
        public string DataWaznosci { get; set; }
        public string Wynagrodzenie { get; set; }
        public string WymiarPracy { get; set; }
        public string RodzajUmowy { get; set; }
        public List<string> Benefity { get; set; } = new List<string>();
        public List<string> Wymagania { get; set; } = new List<string>();
        public string LogoFirmy { get; set; }
        public string NazwaFirmy { get; set; }
        public string Wojewodztwo { get; set; }
        public string Miasto { get; set; }
    }
}