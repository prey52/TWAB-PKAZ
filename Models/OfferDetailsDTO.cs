namespace TWAB.Models
{
    public class OfferDetailsDTO
    {
        public int Id { get; set; }
        public string RecruiterId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string Salary { get; set; }
        public string WorkDimension { get; set; }
        public string ContractType {  get; set; }
        public List<string> Benefits {  get; set; }
        public List<string> Requitemens {  get; set; }
    }
}
