using Microsoft.AspNetCore.Mvc;

namespace TWAB.Models
{
    public class RecruitmentViewModel
    {
        public int Id { get; set; }
        public int OfferId { get; set; }
        public string UserId { get; set; }
        public string Content { get; set; }
        public DateTime? ApplicationData { get; set; }
        public IFormFile CVfile { get; set; }


    }
}
