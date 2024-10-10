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
            return View();
        }

        public async Task<IActionResult> DodajOgloszenie()
        {
            var recruiter = await _userManager.GetUserAsync(User);
            var localizationFilled = _dbContext.LokalizacjeFirm.FirstOrDefault(x => x.DbuserID == recruiter.Id);

            if (localizationFilled == null)
            {
                return Redirect("/Identity/Account/Manage/Lokalizacja");
            }
            else
            {
                return View("/Views/OfertyPracy/DodajOgloszenie.cshtml");
            }

        }

       public async Task<IActionResult> Wyslij(OfertyPracyDTO jobOfferDto)
        {
            jobOfferDto.Status = "Oczekuj¹ca";
            jobOfferDto.DataStworzenia = DateTime.Now;
            jobOfferDto.DataPublikacji = DateTime.Now; //do zmiany

            var jobOffer = new OfertyPracyModel
            {
                IdRekrutera = jobOfferDto.IdRekrutera,
                Status = jobOfferDto.Status,
                Tytul = jobOfferDto.Tytul,
                Kategoria = jobOfferDto.Kategoria,
                Opis = jobOfferDto.Opis,
                DataStworzenia = jobOfferDto.DataStworzenia,
                DataPublikacji = jobOfferDto.DataPublikacji,
                DataWaznosci = jobOfferDto.DataWaznosci,
                Wynagrodzenie = jobOfferDto.Wynagrodzenie,
                WymiarPracy = jobOfferDto.WymiarPracy,
                RodzajUmowy = jobOfferDto.RodzajUmowy,
                Benefity = jobOfferDto.Benefity.Select(b => new OfertyPracyBenefity { Opis = b.Nazwa }).ToList(),
                Wymagania = jobOfferDto.Wymagania.Select(r => new OfertyPracyWymagania { Opis = r.Nazwa }).ToList()
            };

            //Identity sam ogarnie zapis do odpowiednich tabel <3
            _dbContext.OfertyPracy.Add(jobOffer);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction("ListaOgloszen");
        }

        
        public async Task<IActionResult> ListaOgloszen(FiltrDTO filtr)
        {
            var jobOffers = await _dbContext.OfertyPracy.Select(x => new ListaOfertDTO
            {
                Id = x.Id,
                IdRektutera = x.IdRekrutera,
                Tytul = x.Tytul,
                Kategoria = x.Kategoria,
                Status = x.Status,
                DataWaznosci = x.DataWaznosci,
                Wynagrodzenie = x.Wynagrodzenie,
                WymiarPracy = x.WymiarPracy,
                RodzajUmowy = x.RodzajUmowy
            }).ToListAsync();

            List<OfertyPracyUserViewModel> result = new List<OfertyPracyUserViewModel>();

            bool isFiltrFilled = false;
            if (filtr.Kategoria == "Kategoria") filtr.Kategoria = null;
            if (filtr.WymiarPracy == "Wymiar pracy") filtr.WymiarPracy = null;
            if (filtr.RodzajUmowy == "Rodzaj umowy") filtr.RodzajUmowy = null;

            PropertyInfo[] properties = filtr.GetType().GetProperties();
            foreach (PropertyInfo property in properties)
            {
                // SprawdŸ czy w³aœciwoœæ nie jest null ani nie jest pustym ci¹giem znaków
                if (property.GetValue(filtr) != null && !string.IsNullOrWhiteSpace(property.GetValue(filtr).ToString()))
                {
                    // Jeœli chocia¿ jedna w³aœciwoœæ nie jest pusta, zwróæ false
                    isFiltrFilled = true;
                }
            }

            if (isFiltrFilled == false)
            {
                foreach (var item in jobOffers)
                {
                    var user = await _userManager.FindByIdAsync(item.IdRektutera);

                    var lokalizacja = _dbContext.LokalizacjeFirm.FirstOrDefault(x => x.DbuserID == user.Id);

                    OfertyPracyUserViewModel tmp = new OfertyPracyUserViewModel()
                    {
                        Id = item.Id,
                        Status = item.Status,
                        Tytu³ = item.Tytul,
                        Kategoria = item.Kategoria,
                        DataWaznosci = item.DataWaznosci.ToShortDateString(),
                        Wynagrodzenie = item.Wynagrodzenie,
                        WymiarPracy = item.WymiarPracy,
                        RodzajUmowy = item.RodzajUmowy,
                        NazwaFirmy = user.CompanyName,
                        LogoFirmy = Convert.ToBase64String(user.CompanyLogo),
                        Wojewodztwo = lokalizacja.Wojewodztwo,
                        Miasto = lokalizacja.Miasto
                    };
                    result.Add(tmp);
                }
            }
            else
            {
                foreach (var item in jobOffers)
                {
                    var user = await _userManager.FindByIdAsync(item.IdRektutera);
                    var lokalizacja = _dbContext.LokalizacjeFirm.FirstOrDefault(x => x.DbuserID == user.Id);

                    if ((filtr.Tytul == null || item.Tytul == filtr.Tytul) &&
                    (filtr.Kategoria == null || item.Kategoria == filtr.Kategoria) &&
                    (filtr.WymiarPracy == null || item.WymiarPracy == filtr.WymiarPracy) &&
                    (filtr.RodzajUmowy == null || item.RodzajUmowy == filtr.RodzajUmowy) &&
                    (filtr.Miasto == null || lokalizacja.Miasto == filtr.Miasto))
                    {
                        OfertyPracyUserViewModel tmp = new OfertyPracyUserViewModel()
                        {
                            Id = item.Id,
                            Status = item.Status,
                            Tytu³ = item.Tytul,
                            Kategoria = item.Kategoria,
                            DataWaznosci = item.DataWaznosci.ToShortDateString(),
                            Wynagrodzenie = item.Wynagrodzenie,
                            WymiarPracy = item.WymiarPracy,
                            RodzajUmowy = item.RodzajUmowy,
                            NazwaFirmy = user.CompanyName,
                            LogoFirmy = Convert.ToBase64String(user.CompanyLogo),
                            Wojewodztwo = lokalizacja.Wojewodztwo,
                            Miasto = lokalizacja.Miasto
                        };
                        result.Add(tmp);
                    }
                }
            }


            return View("/Views/OfertyPracy/UserListaOgloszen.cshtml", result);
        }
        
        public async Task<IActionResult> Ogloszenie(int id)
        {
            var result1 = await _dbContext.OfertyPracy.FindAsync(id);
            var result2 = await _dbContext.Benefity.Where(x => x.OfertaPracyId == result1.Id).ToListAsync();
            var result3 = await _dbContext.Wymagania.Where(x => x.OfertaPracyId == result1.Id).ToListAsync();

            List<string> benefity = new List<string>();
            foreach (var item in result2)
            {
                benefity.Add(item.Opis);
            }

            List<string> wymagania = new List<string>();
            foreach (var item in result3)
            {
                wymagania.Add(item.Opis);
            }

            var user = await _userManager.FindByIdAsync(result1.IdRekrutera);
            var lokalizacja = _dbContext.LokalizacjeFirm.FirstOrDefault(x => x.DbuserID == user.Id);

            SzczegolyOfertyViewModel result = new SzczegolyOfertyViewModel()
            {
                Id = result1.Id,
                IdRekrutera = user.Id,
                Tytul = result1.Tytul,
                Opis = result1.Opis,
                Kategoria = result1.Kategoria,
                DataWaznosci = result1.DataWaznosci.ToShortDateString(),
                Wynagrodzenie = result1.Wynagrodzenie,
                WymiarPracy = result1.WymiarPracy,
                RodzajUmowy = result1.RodzajUmowy,
                LogoFirmy = Convert.ToBase64String(user.CompanyLogo),
                NazwaFirmy = user.CompanyName,
                Wojewodztwo = lokalizacja.Wojewodztwo,
                Miasto = lokalizacja.Miasto,
            };
            foreach (var item in wymagania)
            {
                result.Wymagania.Add(item);
            }

            foreach (var item in benefity)
            {
                result.Benefity.Add(item);
            }

            return View("/Views/OfertyPracy/UserSzczegolyOferty.cshtml", result);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
