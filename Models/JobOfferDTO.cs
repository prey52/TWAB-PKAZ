namespace TWAB.Models
{
    public class JobOfferDTO
    {
        //to samo co model tylko bez ID
        public string RecruiterId { get; set; }
        public string Status { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime PublicationDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public List<JobOfferRequirementsDTO> Requirements { get; set; } = new List<JobOfferRequirementsDTO>();
        public string Salary { get; set; }
        public string WorkDimension { get; set; }
        public string ContractType { get; set; }
        public List<JobOfferBenefitsDTO> Benefits { get; set; } = new List<JobOfferBenefitsDTO>();
    }
}
