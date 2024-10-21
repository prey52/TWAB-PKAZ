using Microsoft.AspNetCore.Mvc;

namespace TWAB.Models
{
    public class UserAdvertisementDetailsViewModel
    {
        public RecruitmentModel RecruitmentModel { get; set; }
        public string PartialViewName { get; set; } // Opcjonalnie, jeśli chcesz dynamicznie załadować widok
    }

}
