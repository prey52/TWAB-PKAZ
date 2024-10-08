namespace TWAB.Areas.Identity.Data
{
    public class LokalizacjaFirmy
    {
        public int Id { get; set; }
        public string Wojewodztwo { get; set; }
        public string Miasto { get; set; }
        public string Ulica { get; set; }
        public string NrLokalu { get; set; }
        public string KodPocztowy {  get; set; }

        // Klucz obcy do DBUser
        public string DbuserID { get; set; }
        public DBUser Dbuser { get; set; }
    }
}
