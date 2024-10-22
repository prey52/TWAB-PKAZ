using TWAB.Areas.Identity; 
using TWAB.Models;

namespace TWAB.Models

{
    public class RecruitmentHistoryViewModel
    {
        public RecruitmentModel Recruitment { get; set; }
        public JobOfferModel JobOffer { get; set; }
        public DBUser User { get; set; }
    }
}