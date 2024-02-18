using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SeminarHub.Data;
using SeminarHub.Data.Common;
using SeminarHub.Data.Models;
using SeminarHub.Models;
using System;
using System.Globalization;
using System.Security.Claims;

namespace SeminarHub.Controllers
{
    public class SeminarController : Controller
    {
        private readonly SeminarHubDbContext dbContext;

        public SeminarController(SeminarHubDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IActionResult> All()
        {
            var allSeminars = await dbContext.Seminars
                .AsNoTracking()
                .Select(s => new SeminarAllViewModel(
                    s.Id,
                    s.Topic,
                    s.Lecturer,
                    s.Category.Name,
                    s.DateAndTime,
                    s.Organizer.UserName
                    ))
                .ToListAsync();
            
            return View(allSeminars);
        }

        [HttpPost]
        public async Task<IActionResult> Join(int id)
        {
            var currentSeminar = await dbContext.Seminars
                .Where(s => s.Id == id)
                .Include(s => s.SeminarsParticipants)
                .FirstOrDefaultAsync();

            if (currentSeminar == null)
            {
                return BadRequest();
            }

            var currentUserId = GetUserId();

            if (!currentSeminar.SeminarsParticipants.Any(p => p.ParticipantId == currentUserId))
            {
                currentSeminar.SeminarsParticipants.Add(new Data.Models.SeminarParticipant()
                {
                    SeminarId = currentSeminar.Id,
                    ParticipantId = currentUserId,
                });

                await dbContext.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Joined));
        }

        [HttpGet]
        public async Task<IActionResult> Joined()
        {
            var currentUserId = GetUserId();

            var currentUserSeminars = await dbContext.SeminarParticipants
                .Where(sp => sp.ParticipantId == currentUserId)
                .AsNoTracking()
                .Select(sp => new SeminarAllViewModel(
                sp.Seminar.Id,
                sp.Seminar.Topic,
                sp.Seminar.Lecturer,
                sp.Seminar.Category.Name,
                sp.Seminar.DateAndTime,
                sp.Seminar.Organizer.UserName
                ))
                .ToListAsync();

            return View(currentUserSeminars);
        }

        public async Task<IActionResult> Leave(int id)
        {
            var currentSeminar = await dbContext.Seminars
                .Where(s => s.Id == id)
                .Include(s => s.SeminarsParticipants)
                .FirstOrDefaultAsync();

            if (currentSeminar == null)
            {
                return BadRequest();
            }

            var currentUserId = GetUserId();

            var seminarParticipant = currentSeminar.SeminarsParticipants
                .FirstOrDefault(sp => sp.ParticipantId == currentUserId);

            if (seminarParticipant == null)
            {
                return BadRequest();
            }

            currentSeminar.SeminarsParticipants.Remove(seminarParticipant);
            await dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(All));
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var model = new SeminarAddViewModel();
            model.Categories = await GetCategories();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(SeminarAddViewModel seminarForm)
        {
            if (seminarForm == null)
            {
                return BadRequest();
            }

            DateTime dateTime = DateTime.Now;

            if (!DateTime.TryParseExact(seminarForm.DateAndTime, 
                Data.Common.DataConstants.DateFormat,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out dateTime))
            {
                ModelState
                    .AddModelError(nameof(seminarForm.DateAndTime), $"Invalid date! Format must be: {DataConstants.DateFormat}");
            }

            if (!ModelState.IsValid)
            {
                List<string> errorMessages = new List<string>();

                foreach (var entry in ModelState)
                {
                    foreach (var error in entry.Value.Errors)
                    {
                        errorMessages.Add(error.ErrorMessage);
                    }
                }

                seminarForm.Categories = await GetCategories();
                return View(seminarForm);
            }

            var entity = new Seminar()
            {
                DateAndTime = dateTime,
                Topic = seminarForm.Topic,
                Lecturer = seminarForm.Lecturer,
                Details = seminarForm.Details,
                OrganizerId = GetUserId(),
                CategoryId = seminarForm.CategoryId,
                Duration = seminarForm.Duration,
               
            };

            await dbContext.AddAsync(entity);
            await dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(All));

        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var searchedSeminar = await dbContext.Seminars
                .FindAsync(id);

            if (searchedSeminar == null)
            {
                return BadRequest();
            }

            if (searchedSeminar.OrganizerId != GetUserId())
            {
                return Unauthorized();
            }

            var seminarToEdit = new SeminarAddViewModel()
            {
                Topic = searchedSeminar.Topic,
                Lecturer = searchedSeminar.Lecturer,
                Details = searchedSeminar.Details,
                DateAndTime = searchedSeminar.DateAndTime.ToString(Data.Common.DataConstants.DateFormat),
                Duration = searchedSeminar.Duration,
                CategoryId = searchedSeminar.CategoryId,
            };

            seminarToEdit.Categories = await GetCategories();

            return View(seminarToEdit);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(SeminarAddViewModel seminarForm, int id)
        {
            var seminarToEdit = await dbContext.Seminars
                .FindAsync(id);

            if (seminarToEdit == null)
            {
                return BadRequest();
            }

            if (seminarToEdit.OrganizerId != GetUserId())
            {
                return Unauthorized();
            }

            DateTime dateTime = DateTime.Now;

            if (!DateTime.TryParseExact(seminarForm.DateAndTime,
                Data.Common.DataConstants.DateFormat,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out dateTime))
            {
                ModelState
                    .AddModelError(nameof(seminarForm.DateAndTime), $"Invalid date! Format must be: {DataConstants.DateFormat}");
            }

            if (!ModelState.IsValid)
            {
                List<string> errorMessages = new List<string>();

                foreach (var entry in ModelState)
                {
                    foreach (var error in entry.Value.Errors)
                    {
                        errorMessages.Add(error.ErrorMessage);
                    }
                }

                seminarForm.Categories = await GetCategories();
                return View(seminarForm);
            }

            seminarToEdit.Topic = seminarForm.Topic;
            seminarToEdit.Lecturer = seminarForm.Lecturer;
            seminarToEdit.Details = seminarForm.Details;
            seminarToEdit.DateAndTime = dateTime;
            seminarToEdit.Duration = seminarForm.Duration;
            seminarToEdit.CategoryId = seminarForm.CategoryId;

            await dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(All));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var currentSeminar = await dbContext.Seminars
                .Where(s => s.Id == id)
                .Include(s => s.SeminarsParticipants)
                .FirstOrDefaultAsync();

            var seminarParticipants = await dbContext.SeminarParticipants
                .Where(sp => sp.SeminarId == id)
                .ToListAsync();

            if (currentSeminar == null)
            {
                return BadRequest();
            }

            if (currentSeminar.OrganizerId != GetUserId())
            {
                return Unauthorized();
            }

            var seminarToDelete = new DeleteSeminarViewModel()
            {
                Id = currentSeminar.Id,
                Topic = currentSeminar.Topic,
                DateAndTime = currentSeminar.DateAndTime
            };

            return View(seminarToDelete);

        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var currentSeminar = await dbContext.Seminars
                .Where(s => s.Id == id)
                .Include(s => s.SeminarsParticipants)
                .FirstOrDefaultAsync();

            var seminarParticipants = await dbContext.SeminarParticipants
                .Where(sp => sp.SeminarId == id)
                .ToListAsync();

            if (currentSeminar == null)
            {
                return BadRequest();
            }

            if (currentSeminar.OrganizerId != GetUserId())
            {
                return Unauthorized();
            }

            if (seminarParticipants != null && seminarParticipants.Any())
            {
                dbContext.SeminarParticipants.RemoveRange(seminarParticipants);
            }

            dbContext.Seminars.Remove(currentSeminar);
            await dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(All));
        }


        public async Task<IActionResult> Details(int id)
        {
            var searchedSeminar = await dbContext.Seminars
                .Where(s => s.Id == id)
                .AsNoTracking()
                .Select(s => new SeminarDetailsViewModel()
                {
                    Id = s.Id,
                    Topic = s.Topic,
                    Lecturer = s.Lecturer,
                    Category = s.Category.Name,
                    DateAndTime = s.DateAndTime.ToString(Data.Common.DataConstants.DateFormat),
                    Duration = s.Duration,
                    Details = s.Details,
                    Organizer = s.Organizer.UserName
                })
                .FirstOrDefaultAsync();

            if (searchedSeminar == null)
            {
                return BadRequest();
            }

            return View(searchedSeminar);

        }

        private string GetUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
        }

        private async Task<IEnumerable<CategoryViewModel>> GetCategories()
        {
            return await dbContext.Categories
                .AsNoTracking()
                .Select(t => new CategoryViewModel
                {
                    Id = t.Id,
                    Name = t.Name
                })
                .ToListAsync();
        }
    }
}
