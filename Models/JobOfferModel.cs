using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using TWAB.Database;

namespace TWAB.Models
{
    public class JobOfferModel
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string RecruiterId { get; set; }
        public string Status { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime PublicationDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        [AllowNull]
        public ICollection<JobOfferRequirements> Requirements { get; set; } = new List<JobOfferRequirements>();
        public string Salary { get; set; }
        public string WorkDimension { get; set; }
        public string ContractType { get; set; }
        [AllowNull]
        public ICollection<JobOfferBenefits> Benefits { get; set; } = new List<JobOfferBenefits>();
    }
}
