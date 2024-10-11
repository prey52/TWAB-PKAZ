using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace TWAB.Models
{
    public class JobOffersUserViewModel
    {
        public int Id { get; set; }
        public string Status { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public string ExpirationDate { get; set; }
        public string Salary { get; set; }
        public string WorkDimension { get; set; }
        public string ContractType { get; set; }
        public string CompanyName { get; set; }
        public string CompanyLogo { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
    }
}
