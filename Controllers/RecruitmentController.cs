using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TWAB.Areas.Identity;
using TWAB.Data;
using TWAB.Migrations;
using TWAB.Models;

namespace TWAB.Controllers
{
    public class RecruitmentController : Controller
    {
        private readonly TWABIdentityContext _context;
        private readonly UserManager<DBUser> _userManager;

        public RecruitmentController(TWABIdentityContext context, UserManager<DBUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<bool> IsUserInUserRole(UserManager<DBUser> userManager, ClaimsPrincipal user)
        {
            var appUser = await userManager.GetUserAsync(user);

            if (appUser != null)
            {
                return await userManager.IsInRoleAsync(appUser, "User");
            }

            return false;
        }

        public async Task<bool> IsRecruiterInUserRole(UserManager<DBUser> userManager, ClaimsPrincipal user)
        {
            var appUser = await userManager.GetUserAsync(user);

            if (appUser != null)
            {
                return await userManager.IsInRoleAsync(appUser, "Recruiter");
            }

            return false;
        }

        // GET: Recruitment
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var recruitmentData = await _context.Recruitment.ToListAsync();
            var jobOffers = await _context.JobOffers.ToListAsync();
            var users = await _context.dBUsers.ToListAsync();

            bool isUser = await IsUserInUserRole(_userManager, User);
            bool isRecruiter = await IsRecruiterInUserRole(_userManager, User);

            var recruitmentHistoryOffers = recruitmentData.Select(r => new RecruitmentHistoryViewModel
            {
                Recruitment = r,
                JobOffer = jobOffers.FirstOrDefault(j => j.Id == r.OfferId),
                User = users.FirstOrDefault(u => u.Id == r.UserId)
            }).ToList();

            if (isUser)
            {
                recruitmentHistoryOffers = recruitmentHistoryOffers
            .Where(v => v.User != null && v.User.Id == userId) 
            .ToList();

            }
            if (isRecruiter)
            {
                recruitmentHistoryOffers = recruitmentHistoryOffers
            .Where(v => v.JobOffer != null && v.JobOffer.RecruiterId == userId)
            .ToList();
            }

            return View(recruitmentHistoryOffers);
        }

        [Authorize]
        public async Task<IActionResult> Download(int id)
        {

            var userId = _userManager.GetUserId(User);
            var recruitmentModel = await _context.Recruitment.FindAsync(id);
            var offerId = recruitmentModel.OfferId;
            var jobOffer = await _context.JobOffers.FindAsync(offerId);
            var recruiterId = jobOffer.RecruiterId;

            bool accessToOffer = (recruitmentModel.UserId == userId || recruiterId == userId);

            if (recruitmentModel == null || recruitmentModel.CVfile == null || recruitmentModel.CVfile.Length == 0 || accessToOffer == false)
            {
                return NotFound();
            }

            string fileExtension = ".pdf";
            string contentType = "application/pdf";


            if (recruitmentModel.FileType == ".pdf")
            {
                fileExtension = ".pdf";
                contentType = "application/pdf";
            }
            else if (recruitmentModel.FileType == ".txt")
            {
                fileExtension = ".txt";
                contentType = "text/plain";
            }
            else if (recruitmentModel.FileType == ".doc" || recruitmentModel.FileType == ".docx")
            {
                fileExtension = ".docx";
                contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
            }
            else
            {
                return BadRequest("Unsupported file type.");
            }

            string fileName = $"CV_{recruitmentModel.UserId}{fileExtension}";
            return File(recruitmentModel.CVfile, contentType, fileName);
            
          
        }
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var recruitmentModel = await _context.Recruitment
                .FirstOrDefaultAsync(m => m.Id == id);

            if (recruitmentModel == null)
            {
                return NotFound();
            }

            var jobOffer = await _context.JobOffers
                .FirstOrDefaultAsync(j => j.Id == recruitmentModel.OfferId);

            var user = await _context.dBUsers
                .FirstOrDefaultAsync(u => u.Id == recruitmentModel.UserId);

            // Zabezpieczenie
            var recruiterId = jobOffer.RecruiterId;
            var userId = _userManager.GetUserId(User);

            bool accessToOffer = (recruitmentModel.UserId == userId || recruiterId == userId);

            if (accessToOffer == false)
            {
                return NotFound();
            }
            else
            {

                var viewModel = new RecruitmentHistoryViewModel
                {
                    Recruitment = recruitmentModel,
                    JobOffer = jobOffer,
                    User = user
                };

                return View(viewModel);
            }
        }
        [HttpGet]
        [Authorize]
        public ActionResult LoadCreateApplicationView(int idOgloszenia, string idUsera)
        {
            // Przekazujemy dane do modelu widoku
            var model = new RecruitmentViewModel
            {
                OfferId = idOgloszenia,
                UserId = idUsera
            };

            return PartialView("_Create", model);
        }

        // GET: Recruitment/Create

        // POST: Recruitment/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RecruitmentViewModel recruitmentViewModel)
        {
            if (ModelState.IsValid)
            {
                // Tworzymy instancję RecruitmentModel
                var recruitmentModel = new RecruitmentModel
                {
                    OfferId = recruitmentViewModel.OfferId,
                    UserId = recruitmentViewModel.UserId,
                    Content = recruitmentViewModel.Content,
                    ApplicationData = DateTime.Now // Ustawiamy aktualną datę aplikacji
                };

                // Przetwarzanie pliku CV
                if (recruitmentViewModel.CVfile != null && recruitmentViewModel.CVfile.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await recruitmentViewModel.CVfile.CopyToAsync(memoryStream);
                        recruitmentModel.CVfile = memoryStream.ToArray(); // Konwertujemy plik do byte[]
                        recruitmentModel.FileType = Path.GetExtension(recruitmentViewModel.CVfile.FileName).ToLower();
                    }
                }

                // Dodajemy model do bazy danych
                _context.Add(recruitmentModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index)); // Przekierowanie do akcji Index po zapisaniu danych
            }

            // Jeśli coś poszło nie tak, zwracamy widok z błędami
            return PartialView("_Create", recruitmentViewModel);
        }


        // GET: Recruitment/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recruitmentModel = await _context.Recruitment
                .FirstOrDefaultAsync(m => m.Id == id);
            if (recruitmentModel == null)
            {
                return NotFound();
            }

            return View(recruitmentModel);
        }

        // POST: Recruitment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var recruitmentModel = await _context.Recruitment.FindAsync(id);
            if (recruitmentModel != null)
            {
                _context.Recruitment.Remove(recruitmentModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RecruitmentModelExists(int id)
        {
            return _context.Recruitment.Any(e => e.Id == id);
        }
    }
}
