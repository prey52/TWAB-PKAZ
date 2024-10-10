using TWAB.Database;
using TWAB.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using TWAB.Data;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TWAB.Controllers
{
    public class JobOffersController : Controller
    {
        private readonly TWABIdentityContext _dbcontext;
        public JobOffersController(TWABIdentityContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task<IEnumerable<ListaOfertDTO>> Get()
        {
            //List<OfertyPracyModel> list = await _dbcontext.OfertyPracy.ToListAsync();
            var list = await _dbcontext.OfertyPracy.Select(x => new ListaOfertDTO
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
            return list;
        }

        public async Task<SzczegolyOfertyDTO> Get(int id)
        {
            var result1 = await _dbcontext.OfertyPracy.FindAsync(id);
            var result2 = await _dbcontext.Benefity.Where(x => x.OfertaPracyId == result1.Id).ToListAsync();
            var result3 = await _dbcontext.Wymagania.Where(x => x.OfertaPracyId == result1.Id).ToListAsync();

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


            SzczegolyOfertyDTO DTO = new SzczegolyOfertyDTO()
            {
                Id = result1.Id,
                IdRekrutera = result1.IdRekrutera,
                Tytul = result1.Tytul,
                Opis = result1.Opis,
                Kategoria = result1.Kategoria,
                DataWaznosci = result1.DataWaznosci,
                Wynagrodzenie = result1.Wynagrodzenie,
                WymiarPracy = result1.WymiarPracy,
                RodzajUmowy = result1.RodzajUmowy,
                Benefity = benefity,
                Wymagania = wymagania
            };


            return DTO;
        }

        public async Task<IActionResult> Post([FromBody] OfertyPracyDTO jobOfferDto)
        {
            try
            {
                if (jobOfferDto == null)
                {
                    return BadRequest("Model jest null");
                }

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
                _dbcontext.OfertyPracy.Add(jobOffer);
                await _dbcontext.SaveChangesAsync();

                return Ok(jobOffer);
            }
            catch (Exception ex)
            {
                // Logowanie wyjątku
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Wystąpił błąd podczas zapisywania danych");
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            var offer = await _dbcontext.OfertyPracy.FindAsync(id);

            if (offer == null)
            {
                return NotFound("Oferta nie została znaleziona");
            }

            var benefity = await _dbcontext.Benefity.Where(x => x.OfertaPracyId == id).ToListAsync();
            _dbcontext.Benefity.RemoveRange(benefity);

            var wymagania = await _dbcontext.Wymagania.Where(x => x.OfertaPracyId == id).ToListAsync();
            _dbcontext.Wymagania.RemoveRange(wymagania);

            _dbcontext.OfertyPracy.Remove(offer);

            await _dbcontext.SaveChangesAsync();

            return Ok("Usunięto pomyślnie");
        }
    }
}
