namespace TWAB.Models
{
    public class OffersList
    {
        public int Id { get; set; }
        public string RecruiterId { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public string Status { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string Salary { get; set; }
        public string WorkDimension { get; set; }
        public string ContractType { get; set; }
    }
}