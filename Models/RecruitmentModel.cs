using Microsoft.AspNetCore.Mvc;

namespace TWAB.Models
{
    public class RecruitmentModel
    {
        public int Id { get; set; }
        public int OfferId { get; set; }
        public string UserId { get; set; }
        public string Content { get; set; }
        public DateTime ApplicationData { get; set; }
        public byte[] CVfile { get; set; }
        public string FileType { get; set; }


    }
}
