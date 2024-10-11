namespace TWAB.Models
{
    public class OfferDetailsViewModel
    {
        public int Id { get; set; }
        public string RecruiterId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string ExpirationDate { get; set; }
        public string Salary { get; set; }
        public string WorkDimension { get; set; }
        public string ContractType { get; set; }
        public List<string> Benefits { get; set; } = new List<string>();
        public List<string> Requirements { get; set; } = new List<string>();
        public string CompanyLogo { get; set; }
        public string CompanyName { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
    }
}