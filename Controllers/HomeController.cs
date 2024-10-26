using Azure;
using TWAB.Areas.Identity;
using TWAB.Areas.Identity.Data;
using TWAB.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NuGet.Protocol;
using System;
using System.Diagnostics;
using System.Reflection;
using TWAB.Data;
using Microsoft.EntityFrameworkCore;
using TWAB.Database;

namespace TWAB.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<DBUser> _userManager;
        private readonly TWABIdentityContext _dbContext;

        public HomeController(ILogger<HomeController> logger, UserManager<DBUser> userManager, TWABIdentityContext context)
        {
            _logger = logger;
            _userManager = userManager;
            _dbContext = context;
        }

        public IActionResult Index()
        {
            return RedirectToAction();
        }

        public async Task<IActionResult> AddAdvertisement()
        {
            var recruiter = await _userManager.GetUserAsync(User);
            var localizationFilled = _dbContext.ComanyLocations.FirstOrDefault(x => x.DbuserID == recruiter.Id);

            if (localizationFilled == null)
            {
                return Redirect("/Identity/Account/Manage/Lokalizacja");
            }
            else
            {
                return View("/Views/OfertyPracy/AddAdvertisement.cshtml");
            }

        }

       public async Task<IActionResult> Send(JobOfferDTO jobOfferDto)
        {
            jobOfferDto.Status = "Oczekuj¹ca";
            jobOfferDto.CreateDate = DateTime.Now;
            jobOfferDto.PublicationDate = DateTime.Now; //do zmiany

            var jobOffer = new JobOfferModel
            {
                RecruiterId = jobOfferDto.RecruiterId,
                Status = jobOfferDto.Status,
                Title = jobOfferDto.Title,
                Category = jobOfferDto.Category,
                Description = jobOfferDto.Description,
                CreateDate = jobOfferDto.CreateDate,
                PublicationDate = jobOfferDto.PublicationDate,
                ExpirationDate = jobOfferDto.ExpirationDate,
                Salary = jobOfferDto.Salary,
                WorkDimension = jobOfferDto.WorkDimension,
                ContractType = jobOfferDto.ContractType,
                Benefits = jobOfferDto.Benefits.Select(b => new JobOfferBenefits { Description = b.Name }).ToList(),
                Requirements = jobOfferDto.Requirements.Select(r => new JobOfferRequirements { Description = r.Name }).ToList()
            };

            //Identity sam ogarnie zapis do odpowiednich tabel <3
            _dbContext.JobOffers.Add(jobOffer);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction("AdvertisementsList");
        }

        
        public async Task<IActionResult> AdvertisementsList(FilterDTO filter)
        {
            var jobOffers = await _dbContext.JobOffers.Select(x => new OffersList
            {
                Id = x.Id,
                RecruiterId = x.RecruiterId,
                Title = x.Title,
                Category = x.Category,
                Status = x.Status,
                ExpirationDate = x.ExpirationDate,
                Salary = x.Salary,
                WorkDimension = x.WorkDimension,
                ContractType = x.ContractType
            }).ToListAsync();

            List<JobOffersUserViewModel> result = new List<JobOffersUserViewModel>();

            bool isFilterFilled = false;
            if (filter.Category == "Kategoria") filter.Category = null;
            if (filter.WorkingDimension == "Wymiar pracy") filter.WorkingDimension = null;
            if (filter.ContractType == "Rodzaj umowy") filter.ContractType = null;

            if (filter.IsAnyPropertyFilled())
            {
                isFilterFilled = true;
            }

            if (isFilterFilled == false)
            {
                foreach (var item in jobOffers)
                {
                    var user = await _userManager.FindByIdAsync(item.RecruiterId);

                    var lokalizacja = _dbContext.ComanyLocations.FirstOrDefault(x => x.DbuserID == user.Id);

                    JobOffersUserViewModel tmp = new JobOffersUserViewModel()
                    {
                        Id = item.Id,
                        Status = item.Status,
                        Title = item.Title,
                        Category = item.Category,
                        ExpirationDate = item.ExpirationDate.ToShortDateString(),
                        Salary = item.Salary,
                        WorkDimension = item.WorkDimension,
                        ContractType = item.ContractType,
                        CompanyName = user.CompanyName,
                        CompanyLogo = Convert.ToBase64String(user.CompanyLogo),
                        Province = lokalizacja.Province,
                        City = lokalizacja.City
                    };
                    result.Add(tmp);
                }
            }
            else
            {
                foreach (var item in jobOffers)
                {
                    var user = await _userManager.FindByIdAsync(item.RecruiterId);
                    var lokalizacja = _dbContext.ComanyLocations.FirstOrDefault(x => x.DbuserID == user.Id);

                    if ((filter.Title == null || item.Title.Contains(filter.Title)) &&
                    (filter.Category == null || item.Category == filter.Category) &&
                    (filter.WorkingDimension == null || item.WorkDimension == filter.WorkingDimension) &&
                    (filter.ContractType == null || item.ContractType == filter.ContractType) &&
                    (filter.City == null || lokalizacja.City == filter.City))
                    {
                        JobOffersUserViewModel tmp = new JobOffersUserViewModel()
                        {
                            Id = item.Id,
                            Status = item.Status,
                            Title = item.Title,
                            Category = item.Category,
                            ExpirationDate = item.ExpirationDate.ToShortDateString(),
                            Salary = item.Salary,
                            WorkDimension = item.WorkDimension,
                            ContractType = item.ContractType,
                            CompanyName = user.CompanyName,
                            CompanyLogo = Convert.ToBase64String(user.CompanyLogo),
                            Province = lokalizacja.Province,
                            City = lokalizacja.City
                        };
                        result.Add(tmp);
                    }
                }
            }


            return View("/Views/OfertyPracy/UserAdvertisementsList.cshtml", result);
        }

        public async Task<IActionResult> Advertisement(int id)
        {
            var result1 = await _dbContext.JobOffers.FindAsync(id);
            var result2 = await _dbContext.Benefits.Where(x => x.JobOfferId == result1.Id).ToListAsync();
            var result3 = await _dbContext.Requirements.Where(x => x.JobOfferId == result1.Id).ToListAsync();

            List<string> benefity = new List<string>();
            foreach (var item in result2)
            {
                benefity.Add(item.Description);
            }

            List<string> wymagania = new List<string>();
            foreach (var item in result3)
            {
                wymagania.Add(item.Description);
            }

            var user = await _userManager.FindByIdAsync(result1.RecruiterId);
            var lokalizacja = _dbContext.ComanyLocations.FirstOrDefault(x => x.DbuserID == user.Id);

            OfferDetailsViewModel result = new OfferDetailsViewModel()
            {
                Id = result1.Id,
                RecruiterId = user.Id,
                Title = result1.Title,
                Description = result1.Description,
                Category = result1.Category,
                ExpirationDate = result1.ExpirationDate.ToShortDateString(),
                Salary = result1.Salary,
                WorkDimension = result1.WorkDimension,
                ContractType = result1.ContractType,
                CompanyLogo = Convert.ToBase64String(user.CompanyLogo),
                CompanyName = user.CompanyName,
                Province = lokalizacja.Province,
                City = lokalizacja.City,
            };
            foreach (var item in wymagania)
            {
                result.Requirements.Add(item);
            }

            foreach (var item in benefity)
            {
                result.Benefits.Add(item);
            }

            return View("/Views/OfertyPracy/UserAdvertisementDetails.cshtml", result);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
