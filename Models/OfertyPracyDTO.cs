namespace TWAB.Models
{
    public class OfertyPracyDTO
    {
        //to samo co model tylko bez ID
        public string IdRekrutera { get; set; }
        public string Status { get; set; }
        public string Tytul { get; set; }
        public string Kategoria { get; set; }
        public string Opis { get; set; }
        public DateTime DataStworzenia { get; set; }
        public DateTime DataPublikacji { get; set; }
        public DateTime DataWaznosci { get; set; }
        public List<OfertyPracyWymaganiaDTO> Wymagania { get; set; } = new List<OfertyPracyWymaganiaDTO>();
        public string Wynagrodzenie { get; set; }
        public string WymiarPracy { get; set; }
        public string RodzajUmowy { get; set; }
        public List<OfertyPracyBenefityDTO> Benefity { get; set; } = new List<OfertyPracyBenefityDTO>();
    }
}
