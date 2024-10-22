using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;


namespace TWAB.Models
{
    public class RecruitmentViewModel
    {
        public int Id { get; set; }
        public int OfferId { get; set; }
        public string UserId { get; set; }
        [Required(ErrorMessage = "Treść jest wymagana.")]
        public string Content { get; set; }
        public DateTime? ApplicationData { get; set; }
        [Required(ErrorMessage = "Plik CV jest wymagany.")]
        public IFormFile CVfile { get; set; }


    }
}
