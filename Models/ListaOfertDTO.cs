namespace TWAB.Models
{
    public class ListaOfertDTO
    {
        public int Id { get; set; }
        public string IdRektutera { get; set; }
        public string Tytul { get; set; }
        public string Kategoria { get; set; }
        public string Status { get; set; }
        public DateTime DataWaznosci { get; set; }
        public string Wynagrodzenie { get; set; }
        public string WymiarPracy { get; set; }
        public string RodzajUmowy { get; set; }
    }
}