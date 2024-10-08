namespace TWAB.Models
{
    public class SzczegolyOfertyDTO
    {
        public int Id { get; set; }
        public string IdRekrutera { get; set; }
        public string Tytul { get; set; }
        public string Opis { get; set; }
        public string Kategoria { get; set; }
        public DateTime DataWaznosci { get; set; }
        public string Wynagrodzenie { get; set; }
        public string WymiarPracy { get; set; }
        public string RodzajUmowy {  get; set; }
        public List<string> Benefity {  get; set; }
        public List<string> Wymagania {  get; set; }
    }
}
