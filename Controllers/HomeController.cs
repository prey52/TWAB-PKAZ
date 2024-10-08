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

namespace TWAB.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HttpClient _httpClient;
        private readonly UserManager<DBUser> _userManager;
        private readonly TWABIdentityContext _dbContext;

        public HomeController(ILogger<HomeController> logger, HttpClient httpClient, UserManager<DBUser> userManager, TWABIdentityContext context)
        {
            _logger = logger;
            _httpClient = httpClient;
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
            var url = "https://localhost:7029/api/OfertyPracy";

            jobOfferDto.Status = "Oczekuj¹ca";
            jobOfferDto.DataStworzenia = DateTime.Now;
            jobOfferDto.DataPublikacji = DateTime.Now; //do zmiany

            await _httpClient.PostAsJsonAsync(url, jobOfferDto);

            return RedirectToAction("ListaOgloszen");
        }

        public async Task<IActionResult> ListaOgloszen(FiltrDTO filtr)
        {
            var url = "https://localhost:7029/api/OfertyPracy";
            var jobOffers = await _httpClient.GetFromJsonAsync<List<ListaOfertDTO>>(url);

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
            var url = $"https://localhost:7029/api/OfertyPracy/{id}";
            SzczegolyOfertyDTO oferta = await _httpClient.GetFromJsonAsync<SzczegolyOfertyDTO>(url);
            var user = await _userManager.FindByIdAsync(oferta.IdRekrutera);
            var lokalizacja = _dbContext.LokalizacjeFirm.FirstOrDefault(x => x.DbuserID == user.Id);

            SzczegolyOfertyViewModel result = new SzczegolyOfertyViewModel()
            {
                Id = oferta.Id,
                IdRekrutera = oferta.IdRekrutera,
                Tytul = oferta.Tytul,
                Opis = oferta.Opis,
                Kategoria = oferta.Kategoria,
                DataWaznosci = oferta.DataWaznosci.ToShortDateString(),
                Wynagrodzenie = oferta.Wynagrodzenie,
                WymiarPracy = oferta.WymiarPracy,
                RodzajUmowy = oferta.RodzajUmowy,
                LogoFirmy = Convert.ToBase64String(user.CompanyLogo),
                NazwaFirmy = user.CompanyName,
                Wojewodztwo = lokalizacja.Wojewodztwo,
                Miasto = lokalizacja.Miasto,
            };
            foreach (var item in oferta.Wymagania)
            {
                result.Wymagania.Add(item);
            }

            foreach (var item in oferta.Benefity)
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
