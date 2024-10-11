namespace TWAB.Areas.Identity.Data
{
    public class ComanyLocation
    {
        public int Id { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string Number { get; set; }
        public string ZipCode {  get; set; }

        // Klucz obcy do DBUser
        public string DbuserID { get; set; }
        public DBUser Dbuser { get; set; }
    }
}
